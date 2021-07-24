using System;
using OpenPOS.Domain.Models.Dtos;

namespace OpenPOS.Infrastructure.ResponseModels
{
    public class TransactionCreateResponseModel : BaseResponseModel
    {
        public Guid? TransactionId { get; set; }
        public TransactionDto Transaction { get; set; }
    }
}