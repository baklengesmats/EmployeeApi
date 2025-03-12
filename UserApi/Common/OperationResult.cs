namespace UserApi.Common
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Errors { get; set; }
        public int StatusCode { get; set; }
        public static OperationResult Ok()
        {
            return new OperationResult { Success = true };
        }

        public static OperationResult Fail(string errors, int statusCode)
        {
            return new OperationResult { Success = false, Errors = errors, StatusCode = statusCode};
        }
    }

}
