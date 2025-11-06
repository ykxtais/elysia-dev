namespace ElysiaAPI.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(Domain.Entity.Usuario user, DateTime expiresAt);
    }
}