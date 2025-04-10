namespace FreelanceManager.IO._shared
{
    public class ErrorDto
    {
        public ErrorDto()
        {
            ErrorType = "error";
            Status = "error";
            Message = "somethingwentwrong";
        }

        public ErrorDto(Exception ex)
        {
            ErrorType = "error";
            Status = "error";
            Message = "somethingwentwrong";
            Exception = ex?.Message ?? "GLOBAL";
        }

        public string ErrorType { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public object ExtraProps { get; set; }
    }
}
