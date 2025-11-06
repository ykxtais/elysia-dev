using ElysiaAPI.Domain.Entity; 

namespace ElysiaAPI.Tests;

public class VagaTests
{
    [Fact]
    public void NovaVaga_DeveComecarLivre_E_GuardarLocalizacao()
    {
        var vaga = new Vaga(numero: 1, patio: "  A  ");

        Assert.Equal("Livre", vaga.Status); 
        Assert.Equal(1, vaga.Numero);
        Assert.Equal("A", vaga.Patio);     
    }

    [Fact]
    public void AtualizarLocalizacao_DeveAlterarNumeroEPatio_ComTrim()
    {
        var vaga = new Vaga(1, "A");

        vaga.AtualizarLocalizacao(numero: 12, patio: "  B-10  ");

        Assert.Equal(12, vaga.Numero);
        Assert.Equal("B-10", vaga.Patio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ctor_ComNumeroInvalido_DeveLancar(int numeroInvalido)
    {
        Assert.Throws<ArgumentException>(() => new Vaga(numeroInvalido, "A"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Ctor_ComPatioInvalido_DeveLancar(string? patioInvalido)
    {
        Assert.Throws<ArgumentException>(() => new Vaga(1, patioInvalido!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AtualizarLocalizacao_ComNumeroInvalido_DeveLancar(int numeroInvalido)
    {
        var vaga = new Vaga(1, "A");
        Assert.Throws<ArgumentException>(() => vaga.AtualizarLocalizacao(numeroInvalido, "B"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void AtualizarLocalizacao_ComPatioInvalido_DeveLancar(string? patioInvalido)
    {
        var vaga = new Vaga(1, "A");
        Assert.Throws<ArgumentException>(() => vaga.AtualizarLocalizacao(2, patioInvalido!));
    }
}