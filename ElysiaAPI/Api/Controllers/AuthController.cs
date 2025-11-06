using Microsoft.AspNetCore.Mvc;
using ElysiaAPI.Infrastructure.Context;
using ElysiaAPI.Application.Services;
using Microsoft.AspNetCore.Identity;
using ElysiaAPI.Domain.Entity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;


namespace ElysiaAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Usuário não encontrado.");

            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(user, user.SenhaHash, request.Senha);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Senha incorreta.");

            var expiresAt = DateTime.UtcNow.AddMinutes(60);
            var token = _tokenService.GenerateToken(user, expiresAt);

            return Ok(new { token, expiresAt });
        }
    }

    public record LoginRequest(string Email, string Senha);
}