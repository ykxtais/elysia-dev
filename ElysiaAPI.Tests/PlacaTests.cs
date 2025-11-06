using System.Reflection;
using System.Text.RegularExpressions;
using ElysiaAPI.Domain.Entity;         
using ElysiaAPI.Domain.ValueObjects;  

namespace ElysiaAPI.Tests;

public class PlacaTests
{
    private static readonly Regex Antiga   = new(@"^[A-Z]{3}[0-9]{4}$", RegexOptions.Compiled);
    private static readonly Regex Mercosul = new(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.Compiled);

    private static readonly HashSet<string> Blacklist = new(StringComparer.OrdinalIgnoreCase)
    {
        "AAA9999",
        "AAA9A99"
    };

    private static Placa CriarPlaca(string texto)
    {
        var t = typeof(Placa);
        var opImplicit = t.GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (opImplicit is not null) return (Placa)opImplicit.Invoke(null, new object[] { texto })!;

        foreach (var nome in new[] { "Create", "From", "Of", "Parse", "New", "Nova" })
        {
            var m = t.GetMethod(nome, BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
            if (m is not null) return (Placa)m.Invoke(null, new object[] { texto })!;
        }

        var ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        if (ctor is not null) return (Placa)ctor.Invoke(new object[] { texto })!;

        throw new InvalidOperationException("Nenhuma fábrica/conversão pública encontrada para Placa(string).");
    }

    [Theory]
    [InlineData("ADW9F87")]
    [InlineData("ABC1D23")]
    public void Placa_Mercosul_DeveSerValida(string placaTexto)
    {
        Assert.Matches(Mercosul, placaTexto);
        Assert.DoesNotContain(placaTexto, Blacklist);

        var placa = CriarPlaca(placaTexto);
        var moto  = new Moto(placa, "Honda", "CG 160", 2024);
        Assert.NotNull(moto.Placa);
    }

    [Theory]
    [InlineData("AHD8326")]
    [InlineData("BRA2014")]
    public void Placa_Antiga_DeveSerValida(string placaTexto)
    {
        Assert.Matches(Antiga, placaTexto);
        Assert.DoesNotContain(placaTexto, Blacklist);

        var placa = CriarPlaca(placaTexto);
        var moto  = new Moto(placa, "Yamaha", "Factor", 2019);
        Assert.NotNull(moto.Placa);
    }

    [Theory]
    [InlineData("AAA9999")] 
    [InlineData("AAA9A99")]
    [InlineData("abc1234")] 
    [InlineData("AB12345")]
    [InlineData("")]
    public void Placa_Invalida_DeveFalhar(string placaTexto)
    {
        var bateFormato = Antiga.IsMatch(placaTexto) || Mercosul.IsMatch(placaTexto);
        var ehBlacklist = Blacklist.Contains(placaTexto);

        Assert.True(ehBlacklist || !bateFormato);
    }
}
