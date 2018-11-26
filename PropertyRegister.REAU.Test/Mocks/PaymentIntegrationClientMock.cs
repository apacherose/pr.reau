using PropertyRegister.REAU.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Test.Mocks
{
    public class PaymentIntegrationClientMock : IPaymentIntegrationClient
    {
        public Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest paymentRequest)
        {
            return Task.FromResult(new PaymentResponse()
            {
                PaymentIdentifier = $"PMT_{Guid.NewGuid()}",
                Successfull = true
            });
        }

        public Task<PaymentResponse> InitiateServiceApplicationAsync(ServiceRequest serviceRequest)
        {
            return Task.FromResult(new PaymentResponse()
            {
                PaymentIdentifier = $"PMT_{Guid.NewGuid()}",
                Successfull = true
            });
        }
    }
}
