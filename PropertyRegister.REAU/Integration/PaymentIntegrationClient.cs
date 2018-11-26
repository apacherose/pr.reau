using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Integration
{
    public interface IPaymentIntegrationClient
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentResponse> InitiateServiceApplicationAsync(ServiceRequest serviceRequest);
    }

    internal class PaymentIntegrationClient : IPaymentIntegrationClient
    {
        public Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentResponse> InitiateServiceApplicationAsync(ServiceRequest serviceRequest)
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentRequest
    {
        public string PaymentFor { get; set; }

        /// <summary>
        /// размера на задължението
        /// </summary>
        public decimal ObligationAmount { get; set; }

        /// <summary>
        /// стойността на услугата
        /// </summary>
        public decimal ServiceFee { get; set; }
    }

    public class ServiceRequest
    {
        /// <summary>
        /// стойността на услугата
        /// </summary>
        public decimal ServiceFee { get; set; }
    }

    public class PaymentResponse
    {
        public string PaymentIdentifier { get; set; }
        public bool Successfull { get; set; }
        public string Errors { get; set; }
    }
}
