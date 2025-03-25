namespace zenra_finance_back.Models
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
        public string? ErrorDetails { get; set; } // Optional: Include more details in case of an error

        public Response() { }

        // Convenience method for success
        public static Response<T> Success(T result, string message = "")
        {
            return new Response<T> { IsSuccess = true, Message = message, Result = result };
        }

        // Convenience method for failure
        public static Response<T> Failure(string message, string? errorDetails = null)
        {
            return new Response<T> { IsSuccess = false, Message = message, ErrorDetails = errorDetails };
        }
    }
}
