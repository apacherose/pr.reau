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
        Task<T> ExecuteAsync<T>(string operationID, ServiceOperationTypes operationType, Func<long, Task<T>> transactionalOperation, Action<T> nonTransactionOperationOnFirstCall = null);
    }

    public class IdempotentOperationExecutor : IIdempotentOperationExecutor
    {
        private readonly IServiceOperationRepository ServiceOperationRepository;

        public IdempotentOperationExecutor(IServiceOperationRepository serviceOperationRepository)
        {
            ServiceOperationRepository = serviceOperationRepository;
        }

        public async Task<T> ExecuteAsync<T>(string operationID, ServiceOperationTypes operationType, 
                Func<long, Task<T>> transactionalOperation, Action<T> nonTransactionOperationOnFirstCall = null)
        {
            try
            {
                bool firstOperationCall = false;

                var transactResult = await DbContextHelper.TransactionalOperationAsync(async () =>
                {
                    var operation = new ServiceOperation()
                    {
                        OperationID = operationID,
                        ServiceOperationType = operationType
                    };

                    ServiceOperationRepository.Create(operation);

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
                        ServiceOperationRepository.Update(operation);

                        firstOperationCall = true;
                        return res;
                    }
                });

                if (firstOperationCall)
                    nonTransactionOperationOnFirstCall?.Invoke(transactResult);

                return transactResult;
            }
            catch (Exception ex)
            {
                // log ex

                throw;
            }
        }
    }
}
