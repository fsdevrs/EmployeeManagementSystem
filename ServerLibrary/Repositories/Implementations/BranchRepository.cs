using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(EmpDbContext empDbContext) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.Branches.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.Branches.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll()
        => await empDbContext.Branches.ToListAsync();

        public async Task<Branch> GetById(int id)
        => await empDbContext.Branches.FindAsync(id);

        public async Task<GenralResponse> Insert(Branch item)
        {
            if (!await CheckName(item.Name!)) return new GenralResponse(false, "Branch already exists");
            empDbContext.Branches.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(Branch item)
        {
            var dep = await empDbContext.Branches.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GenralResponse NotFound() => new(false, "Sorry Branch not found");

        private static GenralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await empDbContext.SaveChangesAsync(); 

        private async Task<bool> CheckName(string name)
        {
            var items = await empDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}