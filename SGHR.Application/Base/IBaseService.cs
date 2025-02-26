
using SGHR.Domain.Base;

namespace SGHR.Application.Base
{
    public interface IBaseService<TDtoAdd, TDtoupdate, TDtoRemove>
    {
        Task<OperationResult> GetAll();

        Task<OperationResult> GetById(int id);
        Task<OperationResult> Update(TDtoupdate dto);
        Task<OperationResult> Remove(TDtoRemove dto);
        Task<OperationResult> Save (TDtoAdd dto);
    }
}
