using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Constants = ServerLibrary.Helpers.Constants;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly IOptions<JwtSection> _config;
        private readonly EmpDbContext _empDbContext;

        public UserAccountRepository(IOptions<JwtSection> config, EmpDbContext empDbContext)
        {
            _config = config;
            _empDbContext = empDbContext;
        }

        // Creating User
        public async Task<GenralResponse> CreateAsync(Register user)
        {
            //Check If User Existed
            if (user is null) return new GenralResponse(false, "Model is empty");

            //Check User By Email If Alreday Registered
            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GenralResponse(false, "User Already registered");

            //Save User
            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                FullName = user.FullName,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            //Check Create and Assign Role
            var checkAdminRole = await _empDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GenralResponse(true, "Account Created");
            }

            //Check Create and Assign Role As User If Admin Already Exits
            var checkUserRole = await _empDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            return new GenralResponse(true, "Acount Created");
        }

        //Sign In User
        public async Task<LoginResponse> SignInAsync(Login user)
        {
            //Checking user value is null or not
            if (user is null) return new LoginResponse(false, "Model is empty");

            //Checking user by email if exists or not
            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");

            //Verify User Encrypted Password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password not Valid");

            //Checking user Role
            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "User rolenot found");

            //Checking user Role Name
            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getUserRole is null) return new LoginResponse(false, "User rolenot found");

            //Generating Jwt tokens on Login Successfull
            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateRefreshToken();
            return new LoginResponse(true, "Login Succesfully", jwtToken, refreshToken);
        }

        // Assining Refresh Token From Db To A User
        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");
            var findToken = await _empDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Refresh Token is Required!");

            //get current user details if existed
            var user = await _empDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refersh token could not be generated because user not found");

            var userRole = await FindUserRole(user.Id);
            if (userRole == null) return new LoginResponse(false, "user role not found");
            var roleName = await FindRoleName(userRole.RoleId);
            if (roleName == null) return new LoginResponse(false, "user role name not found");
            string jwtToken = GenerateToken(user, roleName.Name);
            string refreshToken = GenerateRefreshToken();

            //updating refresh token if user is valid or logged in
            var updateRefreshToken = await _empDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user has not signed in");

            updateRefreshToken.Token = refreshToken;
            await _empDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }

        //Generate Jwt tokens Method
        private string GenerateToken(ApplicationUser user, string? role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!)
            };
            var token = new JwtSecurityToken(
                issuer: _config.Value.Issuer,
                audience: _config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Generate Refresh Token
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // Find User by Email Method
        private async Task<ApplicationUser?> FindUserByEmail(string email) =>
            await _empDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Email!.ToLower()!.Equals(email!.ToLower()));

        // Find User role Method
        private async Task<UserRole?> FindUserRole(int userId) => await _empDbContext.UserRoles.FirstOrDefaultAsync(_ => _.UserId == userId);

        // Find User role name Method
        private async Task<SystemRole?> FindRoleName(int roleId) => await _empDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);

        //Add User To Databse Method
        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = _empDbContext.Add(model!);
            await _empDbContext.SaveChangesAsync();
            return (T)result.Entity;

        }
    }
}
