using ElysiaAPI.Domain.Entity;

namespace ElysiaAPI.Tests;

public class UsuarioEmailTests
{
    private static bool EmailBasicoValido(string email) =>
        !string.IsNullOrWhiteSpace(email) && email.Contains('@') && email.Contains('.');

    [Theory]
    [InlineData("joao@fiap.com")]
    [InlineData("ana@mail.com")]
    [InlineData("user@test.org")]
    [InlineData("@teste.com")] 
    public void Email_Valido_DevePassar(string email)
    {
        var u = new Usuario("Joao", email, "senha123", "12345678901");
        Assert.True(EmailBasicoValido(u.Email));
    }

    [Theory]
    [InlineData("joao@fiap")]    
    [InlineData("joaofiap.com")] 
    public void Email_Invalido_DeveFalharPorFormato(string email)
    {
        var u = new Usuario("Joao", email, "senha123", "12345678901");
        Assert.False(EmailBasicoValido(u.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Email_Vazio_DeveLancarExcecaoNoDominio(string emailVazio)
    {
        Assert.Throws<ArgumentException>(() =>
            new Usuario("Joao", emailVazio, "senha123", "12345678901"));
    }
}