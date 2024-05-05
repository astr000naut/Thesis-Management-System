namespace TMS.Worker.Core.Response

{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; } = null;
        public string ErrorCode { get; set; } = null;
        public string ErrorDetail { get; set; } = null;

    }
}
