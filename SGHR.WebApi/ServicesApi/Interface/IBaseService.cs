using SGHR.WebApi.Models;

namespace SGHR.WebApi.ServicesApi.Interface
{
    public interface IBaseService<TDtoSave, TDtoUpdate, TDtoRemove>
    {
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<OperationResult> Update(TDtoUpdate dto);
        Task<OperationResult> Save(TDtoSave dto);
        Task<OperationResult> Remove(TDtoRemove dto);
    }
}
