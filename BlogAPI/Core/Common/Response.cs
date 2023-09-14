namespace BlogAPI.Core.Common
{
    public class Response
    {
        public bool IsSucceed { get; set; }
        public string? Message { get; set; }
    }

    public class AuthServiceResponse : Response
    {
    }
}
