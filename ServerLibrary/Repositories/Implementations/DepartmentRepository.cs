using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(EmpDbContext empDbContext) : IGenericRepositoryInterface<Department>
    {
        public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.Departments.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll()
        => await empDbContext.Departments.ToListAsync();
        
        public async Task<Department> GetById(int id)
        => await empDbContext.Departments.FindAsync(id);

        public async Task<GenralResponse> Insert(Department item)
        {
            if(!await CheckName(item.Name!)) return new GenralResponse(false, " General Department already exists");
            empDbContext.Departments.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(Department item)
        {
            var dep = await empDbContext.Departments.FindAsync(item.Id);
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
            var items = await empDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}