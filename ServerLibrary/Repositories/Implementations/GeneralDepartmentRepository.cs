using System.Linq.Expressions;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository(EmpDbContext empDbContext) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.GeneralDepartments.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.GeneralDepartments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll()
        => await empDbContext.GeneralDepartments.ToListAsync();
        
        public async Task<GeneralDepartment> GetById(int id)
        => await empDbContext.GeneralDepartments.FindAsync(id);

        public async Task<GenralResponse> Insert(GeneralDepartment item)
        {
            var checkIfNull = await CheckName(item.Name);
            if(checkIfNull) 
            return new GenralResponse(false, "General Department already exists");
            empDbContext.GeneralDepartments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(GeneralDepartment item)
        {
            var dep = await empDbContext.GeneralDepartments.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GenralResponse NotFound() => new(false, "Sorry general department not found");

        private static GenralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await empDbContext.SaveChangesAsync(); 

        private async Task<bool> CheckName(string name)
        {
            var items = await empDbContext.GeneralDepartments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}