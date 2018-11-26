using Newtonsoft.Json;
using PropertyRegister.REAU.Common.Models;
using PropertyRegister.REAU.Common.Persistence;
using PropertyRegister.REAU.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Common
{
    public interface IIdempotentOperationExecutor
    {
        Task<T> ExecuteAsync<T>(string operationID, ServiceOperationTypes operationType, Func<long, Task<T>> transactionalOperation);
    }

    public class IdempotentOperationExecutor : IIdempotentOperationExecutor
    {
        private readonly IServiceOperationEntity ServiceOperationEntity;

        public IdempotentOperationExecutor(IServiceOperationEntity serviceOperationEntity)
        {
            ServiceOperationEntity = serviceOperationEntity;
        }

        public Task<T> ExecuteAsync<T>(string operationID, ServiceOperationTypes operationType, Func<long, Task<T>> transactionalOperation)
        {
            try
            {
                return DbContextHelper.TransactionalOperationAsync(async () =>
                {
                    var operation = new ServiceOperation()
                    {
                        OperationID = operationID,
                        ServiceOperationType = operationType
                    };

                    ServiceOperationEntity.Create(operation);

                    if (operation.IsCompleted)
                    {
                        if (string.IsNullOrEmpty(operation.Result))
                            return default(T);

                        return JsonConvert.DeserializeObject<T>(operation.Result);
                    }
                    else
                    {
                        T res = await transactionalOperation(operation.ServiceOperationID.Value);

                        operation.Result = JsonConvert.SerializeObject(res);
                        operation.IsCompleted = true;
                        ServiceOperationEntity.Update(operation);

                        return res;
                    }
                });                
            }
            catch (Exception ex)
            {
                // log ex

                throw;
            }
        }
    }
}
