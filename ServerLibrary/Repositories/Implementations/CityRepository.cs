using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository(EmpDbContext empDbContext) : IGenericRepositoryInterface<City>
    {
         public async Task<GenralResponse> DeleteById(int id)
        {
            var dep = await empDbContext.Cities.FindAsync(id);
            if (dep is null) return NotFound();

            empDbContext.Cities.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<City>> GetAll()
        => await empDbContext.Cities.ToListAsync();
        
        public async Task<City> GetById(int id)
        => await empDbContext.Cities.FindAsync(id);

        public async Task<GenralResponse> Insert(City item)
        {
            if(!await CheckName(item.Name!)) return new GenralResponse(false, " City already exists");
            empDbContext.Cities.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GenralResponse> Update(City item)
        {
            var dep = await empDbContext.Cities.FindAsync(item.Id);
            if (dep is null) return NotFound();
            dep.Name = item.Name;
            await Commit();
            return Success();
        }

        private static GenralResponse NotFound() => new(false, "Sorry City not found");

        private static GenralResponse Success() => new(true, "Process completed");

        private async Task Commit() => await empDbContext.SaveChangesAsync(); 

        private async Task<bool> CheckName(string name)
        {
            var items = await empDbContext.Departments.FirstOrDefaultAsync(x => x.Name!.ToLower().Equals(name.ToLower()));
            return items is null;
        }
    }
}