using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace WindowsToolkit.Services;

public static class LoginService
{
    private const int Iteracoes = 100_000;
    private const int TamanhoHash = 32;
    private const int TamanhoSalt = 16;

    private static readonly string PastaConfiguracao =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "WindowsToolkit");

    private static readonly string ArquivoSenha =
        Path.Combine(PastaConfiguracao, "senha.json");

    public static bool ValidarSenha(string senha)
    {
        GarantirSenhaInicial();

        SenhaConfig? configuracao = CarregarConfiguracao();

        if (configuracao is null)
            return false;

        byte[] salt = Convert.FromBase64String(configuracao.Salt);
        byte[] hashSalvo = Convert.FromBase64String(configuracao.Hash);

        byte[] hashDigitado = GerarHash(senha, salt);

        return CryptographicOperations.FixedTimeEquals(
            hashDigitado,
            hashSalvo);
    }

    public static bool AlterarSenha(
        string senhaAtual,
        string novaSenha)
    {
        if (!ValidarSenha(senhaAtual))
            return false;

        SalvarNovaSenha(novaSenha);
        return true;
    }

    private static void GarantirSenhaInicial()
    {
        if (!File.Exists(ArquivoSenha))
        {
            // Senha utilizada somente no primeiro acesso.
            SalvarNovaSenha("V@2020dev");
        }
    }

    private static void SalvarNovaSenha(string senha)
    {
        Directory.CreateDirectory(PastaConfiguracao);

        byte[] salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        byte[] hash = GerarHash(senha, salt);

        SenhaConfig configuracao = new()
        {
            Salt = Convert.ToBase64String(salt),
            Hash = Convert.ToBase64String(hash)
        };

        string json = JsonSerializer.Serialize(
            configuracao,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(ArquivoSenha, json);
    }

    private static SenhaConfig? CarregarConfiguracao()
    {
        if (!File.Exists(ArquivoSenha))
            return null;

        try
        {
            string json = File.ReadAllText(ArquivoSenha);

            return JsonSerializer.Deserialize<SenhaConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    private static byte[] GerarHash(
        string senha,
        byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            password: senha,
            salt: salt,
            iterations: Iteracoes,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: TamanhoHash);
    }

    private sealed class SenhaConfig
    {
        public string Salt { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
    }
}