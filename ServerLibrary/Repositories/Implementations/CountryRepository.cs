using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository(EmpDbContext empDbContext) : IGenericRepositoryInterface<Country>
    {
         public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.Countries.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.Countries.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Country>> GetAll()
        => await empDbContext.Countries.ToListAsync();
        
        public async Task<Country> GetById(int id)
        => await empDbContext.Countries.FindAsync(id);

        public async Task<GenralResponse> Insert(Country item)
        {
            if(!await CheckName(item.Name!)) return new GenralResponse(false, " Country already exists");
            empDbContext.Countries.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(Country item)
        {
            var dep = await empDbContext.Countries.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GenralResponse NotFound() => new(false, "Sorry Country not found");

        private static GenralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await empDbContext.SaveChangesAsync(); 

        private async Task<bool> CheckName(string name)
        {
            var items = await empDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}