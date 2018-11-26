using PropertyRegister.REAU.Applications.Models;
using PropertyRegister.REAU.Domain;
using PropertyRegister.REAU.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Payments
{
    public interface IPaymentManager
    {
        Task<ServicePayment> RequestApplicationPaymentAsync(Application application, decimal? filedAmount);
    }

    public class PaymentManager : IPaymentManager
    {
        private readonly IPaymentIntegrationClient PaymentIntegrationClient;
        private readonly ICollection<ApplicationServiceType> ApplicationTypesCollection;

        public PaymentManager(IPaymentIntegrationClient paymentIntegrationClient, IApplicationServiceTypeCollection applicationTypesCollection)
        {
            PaymentIntegrationClient = paymentIntegrationClient;
            ApplicationTypesCollection = applicationTypesCollection.GetItems();
        }

        public async Task<ServicePayment> RequestApplicationPaymentAsync(Application application, decimal? filedAmount)
        {
            decimal? serviceFee, obligationAmount;
            var serviceType = ApplicationTypesCollection.Single(t => t.ApplicationTypeID == application.ApplicationTypeID);

            if (serviceType.IsFree)
            {
                serviceFee = obligationAmount = 0m;
            }
            else
            {
                if (serviceType.PaymentAmount.HasValue)
                {
                    serviceFee = obligationAmount = serviceType.PaymentAmount.Value;
                }
                else
                {
                    serviceFee = 0m;
                    obligationAmount = filedAmount.Value;
                }
            }

            var paymentResponse = serviceType.PaymentAfterRegistration ?
                await PaymentIntegrationClient.InitiateServiceApplicationAsync(new ServiceRequest { ServiceFee = serviceFee.Value }) :
                await PaymentIntegrationClient.InitiatePaymentAsync(new PaymentRequest()
                {
                    PaymentFor = application.ApplicationIdentifier,
                    ServiceFee = serviceFee.Value,
                    ObligationAmount = obligationAmount.Value
                });

            var applPayment = new ServicePayment()
            {
                ServiceInstanceID = application.ApplicationID,
                ServiceFee = serviceFee.Value,
                ObligationAmount = obligationAmount.Value
            };

            if (paymentResponse.Successfull)
            {
                applPayment.Status = PaymentStatuses.Ordered;
                applPayment.PaymentIdentifier = paymentResponse.PaymentIdentifier;

                if (serviceType.PaymentDeadline.HasValue)
                    applPayment.PaymentDeadline = DateTime.Now.Add(serviceType.PaymentDeadline.Value); // как се определя?
            }
            else
            {
                applPayment.Status = PaymentStatuses.Error;
                applPayment.ErrorDescription = paymentResponse.Errors;
            }

            return applPayment;
        }
    }
}
