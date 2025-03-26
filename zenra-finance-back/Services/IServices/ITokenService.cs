namespace zenra_finance_back.Services.IServices
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}