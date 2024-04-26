using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class TownRepository (EmpDbContext empDbContext) : IGenericRepositoryInterface<Town>
    {
         public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.Towns.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.Towns.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Town>> GetAll()
        => await empDbContext.Towns.ToListAsync();
        
        public async Task<Town> GetById(int id)
        => await empDbContext.Towns.FindAsync(id);

        public async Task<GenralResponse> Insert(Town item)
        {
            if(!await CheckName(item.Name!)) return new GenralResponse(false, " Town already exists");
            empDbContext.Towns.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(Town item)
        {
            var dep = await empDbContext.Towns.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GenralResponse NotFound() => new(false, "Sorry Town not found");

        private static GenralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await empDbContext.SaveChangesAsync(); 

        private async Task<bool> CheckName(string name)
        {
            var items = await empDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}