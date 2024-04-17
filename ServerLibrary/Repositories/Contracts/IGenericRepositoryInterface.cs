using BaseLibrary.Responses;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IGenericRepositoryInterface<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<GenralResponse> Insert(T item);
        Task<GenralResponse> Update(T item);
        Task<GenralResponse> DeleteById(int id);
    }
}