using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IGenericServiceInterface<T>
    {
        Task<List<T>> GetAll(string baseUrl);
        Task<T> GetById(int id, string baseUrl);
        Task <GenralResponse> Insert(T item, string baseUrl);
        Task<GenralResponse> Update(T item, string baseUrl);
        Task<GenralResponse> DeleteById(int id, string baseUrl);
    }
}
