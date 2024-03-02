using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
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

        public Task<LoginResponse> SignInAsync(Login user)
        {
            throw new NotImplementedException();
        }

        // Find User by Email Method
        private async Task<ApplicationUser?> FindUserByEmail(string email) =>
            await _empDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Email!.ToLower()!.Equals(email!.ToLower()));

        //Add User To Databse Method
        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = _empDbContext.Add(model!);
            await _empDbContext.SaveChangesAsync();
            return (T)result.Entity;

        }
    }
}
