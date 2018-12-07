using CNSys.Security;
using PropertyRegister.REAU.Applications.Results;
using Rebus.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Applications.MessageHandlers
{
    public class ApplicationAcceptedResultHandler : IHandleMessages<ApplicationAcceptedResult>
    {
        private readonly IApplicationProcessingService ProcessingService;

        public ApplicationAcceptedResultHandler(IApplicationProcessingService processingService)
        {
            ProcessingService = processingService;
        }

        public async Task Handle(ApplicationAcceptedResult message)
        {
            using (var mti = new ManagedThreadImpersonation(new GenericDataSourceUser("1", Thread.CurrentPrincipal)))
            {
                await ProcessingService.ProcessApplicationAsync(message.ApplicationID);
            }

            Console.WriteLine($"Handled message of {nameof(message)} with value {message.ApplicationNumber}.");
        }
    }

    public class ApplicationProcessedResultHandler : IHandleMessages<ApplicationProcessedResult>
    {
        private readonly IApplicationProcessingService ProcessingService;

        public ApplicationProcessedResultHandler(IApplicationProcessingService processingService)
        {
            ProcessingService = processingService;
        }

        public Task Handle(ApplicationProcessedResult message)
        {
            throw new NotImplementedException("in progress");
        }
    }
}
