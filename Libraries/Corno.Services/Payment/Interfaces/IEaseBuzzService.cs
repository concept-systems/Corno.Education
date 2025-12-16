using Corno.Data.Operation;
using Corno.Services.Corno.Interfaces;
using System.Web.Mvc;

namespace Corno.Services.Payment.Interfaces;

public interface IEaseBuzzService : IBaseService
{
    // Payment API
    string PaymentApiRequest(OperationRequest operationRequest);
    void PaymentApiResponse(FormCollection form);

    // Transaction API
    string TransactionApi(OperationRequest operationRequest);
    string TransactionDateApi(OperationRequest operationRequest);
    string SendPayoutRequest(OperationRequest operationRequest);
}