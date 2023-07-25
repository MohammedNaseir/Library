namespace library.Web.Services.Email
{
    public interface IEmailBodyBuilder
    {
        string GetEmailBody(string imageUrl, string header, string body, string url, string linkTitle);
    }
}
