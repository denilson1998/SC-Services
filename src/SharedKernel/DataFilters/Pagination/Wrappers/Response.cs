namespace SharedKernel.DataFilters.Pagination.Wrappers
{
    public class Response<T>
    {
        public T Result { get; set; }
        public string[] Errors { get; set; }

        public string Message;

        public Response()
        { }

        public Response(T result)
        {
            Errors = null;
            Result = result;
            Message = string.Empty;
        }

        public Response(T result, string message, string[] errors)
        {
            Result = result;
            Message = message;
            Errors = errors;
        }
    }
}