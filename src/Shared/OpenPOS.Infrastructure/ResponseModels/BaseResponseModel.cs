namespace OpenPOS.Infrastructure.ResponseModels
{
    public class BaseResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;
    }
}