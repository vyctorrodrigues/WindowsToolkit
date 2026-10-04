using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WindowsToolkit.Services;

public class LicencaService
{
    public async Task<string> ObterStatusOfficeAsync()
    {
        StringBuilder resultado = new();

        resultado.AppendLine("DIAGNÓSTICO DO MICROSOFT OFFICE");
        resultado.AppendLine(new string('─', 45));
        resultado.AppendLine();

        InformacaoOffice? office = DetectarOfficeInstalado();

        if (office is null)
        {
            resultado.AppendLine("❌ Microsoft Office não encontrado.");
            resultado.AppendLine();
            resultado.AppendLine(
                "O Office pode não estar instalado ou pode ser uma versão " +
                "que não foi identificada pelo Registro do Windows.");

            return resultado.ToString();
        }

        resultado.AppendLine("✅ Microsoft Office encontrado.");
        resultado.AppendLine($"Produto: {office.NomeProduto}");
        resultado.AppendLine($"Versão: {office.Versao}");
        resultado.AppendLine($"Arquitetura: {office.Arquitetura}");
        resultado.AppendLine($"Caminho: {office.CaminhoInstalacao}");
        resultado.AppendLine();

        resultado.AppendLine("APLICATIVOS");
        resultado.AppendLine(new string('─', 45));

        resultado.AppendLine(
            File.Exists(Path.Combine(office.CaminhoInstalacao, "WINWORD.EXE"))
                ? "✅ Microsoft Word instalado"
                : "❌ Microsoft Word não encontrado");

        resultado.AppendLine(
            File.Exists(Path.Combine(office.CaminhoInstalacao, "EXCEL.EXE"))
                ? "✅ Microsoft Excel instalado"
                : "❌ Microsoft Excel não encontrado");

        resultado.AppendLine(
            File.Exists(Path.Combine(office.CaminhoInstalacao, "OUTLOOK.EXE"))
                ? "✅ Microsoft Outlook instalado"
                : "❌ Microsoft Outlook não encontrado");

        resultado.AppendLine();
        resultado.AppendLine("LICENÇA");
        resultado.AppendLine(new string('─', 45));

        string statusLicenca =
            await ObterStatusLicencaPeloOsppAsync(office.CaminhoInstalacao);

        resultado.AppendLine(statusLicenca);

        resultado.AppendLine();
        resultado.AppendLine("Observação:");
        resultado.AppendLine(
            "Exchange é um serviço de e-mail configurado no Outlook, " +
            "não um tipo de licença do Office.");

        return resultado.ToString();
    }

    private InformacaoOffice? DetectarOfficeInstalado()
    {
        InformacaoOffice? clickToRun = DetectarClickToRun();

        if (clickToRun is not null)
            return clickToRun;

        return DetectarOfficePeloCaminho();
    }

    private InformacaoOffice? DetectarClickToRun()
    {
        const string caminhoRegistro =
            @"SOFTWARE\Microsoft\Office\ClickToRun\Configuration";

        RegistryView[] visualizacoes =
        [
            RegistryView.Registry64,
            RegistryView.Registry32
        ];

        foreach (RegistryView visualizacao in visualizacoes)
        {
            try
            {
                using RegistryKey chaveBase =
                    RegistryKey.OpenBaseKey(
                        RegistryHive.LocalMachine,
                        visualizacao);

                using RegistryKey? chave =
                    chaveBase.OpenSubKey(caminhoRegistro);

                if (chave is null)
                    continue;

                string? caminho =
                    chave.GetValue("InstallationPath")?.ToString();

                string? versao =
                    chave.GetValue("VersionToReport")?.ToString();

                string? produto =
                    chave.GetValue("ProductReleaseIds")?.ToString();

                string? plataforma =
                    chave.GetValue("Platform")?.ToString();

                if (string.IsNullOrWhiteSpace(caminho))
                    continue;

                string caminhoOffice =
                    LocalizarPastaOffice(caminho);

                return new InformacaoOffice
                {
                    NomeProduto = TraduzirProduto(produto),
                    Versao = TraduzirVersao(versao),
                    Arquitetura = TraduzirArquitetura(plataforma),
                    CaminhoInstalacao = caminhoOffice
                };
            }
            catch
            {
                // Continua tentando outra visualização do Registro.
            }
        }

        return null;
    }

    private InformacaoOffice? DetectarOfficePeloCaminho()
    {
        string[] caminhosPossiveis =
        [
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                "Microsoft Office",
                "root",
                "Office16"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFilesX86),
                "Microsoft Office",
                "root",
                "Office16"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                "Microsoft Office",
                "Office16"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFilesX86),
                "Microsoft Office",
                "Office16")
        ];

        foreach (string caminho in caminhosPossiveis)
        {
            if (!Directory.Exists(caminho))
                continue;

            bool possuiAplicativo =
                File.Exists(Path.Combine(caminho, "WINWORD.EXE")) ||
                File.Exists(Path.Combine(caminho, "EXCEL.EXE")) ||
                File.Exists(Path.Combine(caminho, "OUTLOOK.EXE"));

            if (!possuiAplicativo)
                continue;

            bool programa32Bits =
                caminho.Contains(
                    "Program Files (x86)",
                    StringComparison.OrdinalIgnoreCase);

            return new InformacaoOffice
            {
                NomeProduto = "Microsoft Office",
                Versao = "Office 2016, 2019, 2021, 2024 ou Microsoft 365",
                Arquitetura = programa32Bits ? "32 bits" : "64 bits",
                CaminhoInstalacao = caminho
            };
        }

        return null;
    }

    private async Task<string> ObterStatusLicencaPeloOsppAsync(
        string caminhoOffice)
    {
        string? caminhoOspp = LocalizarOspp(caminhoOffice);

        if (caminhoOspp is null)
        {
            return
                "⚠️ O arquivo OSPP.VBS não foi encontrado.\n" +
                "Não foi possível consultar a licença por esse método.\n" +
                "Isso pode acontecer em algumas instalações do Microsoft 365.";
        }

        try
        {
            ProcessStartInfo inicio = new()
            {
                FileName = "cscript.exe",
                Arguments = $"//Nologo \"{caminhoOspp}\" /dstatus",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process processo = new()
            {
                StartInfo = inicio
            };

            processo.Start();

            Task<string> leituraSaida =
                processo.StandardOutput.ReadToEndAsync();

            Task<string> leituraErro =
                processo.StandardError.ReadToEndAsync();

            await processo.WaitForExitAsync();

            string saida = await leituraSaida;
            string erro = await leituraErro;

            if (!string.IsNullOrWhiteSpace(erro))
            {
                return
                    "❌ Erro ao consultar a licença:\n" +
                    erro.Trim();
            }

            return InterpretarResultadoLicenca(saida);
        }
        catch (Exception ex)
        {
            return
                "❌ Não foi possível consultar a licença.\n" +
                $"Detalhes: {ex.Message}";
        }
    }

    private string InterpretarResultadoLicenca(string saida)
    {
        if (string.IsNullOrWhiteSpace(saida))
            return "⚠️ Nenhuma informação de licença foi retornada.";

        string textoMaiusculo = saida.ToUpperInvariant();

        if (textoMaiusculo.Contains("LICENSE STATUS:  ---LICENSED---") ||
            textoMaiusculo.Contains("LICENSE STATUS: LICENSED"))
        {
            return
                "✅ Office licenciado.\n\n" +
                ExtrairInformacoesPrincipais(saida);
        }

        if (textoMaiusculo.Contains("NOTIFICATIONS"))
        {
            return
                "⚠️ Office em modo de notificação.\n" +
                "A licença pode estar ausente, expirada ou com problema.\n\n" +
                ExtrairInformacoesPrincipais(saida);
        }

        if (textoMaiusculo.Contains("UNLICENSED"))
        {
            return
                "❌ Office não licenciado.\n\n" +
                ExtrairInformacoesPrincipais(saida);
        }

        if (textoMaiusculo.Contains("GRACE"))
        {
            return
                "⚠️ Office em período de avaliação ou tolerância.\n\n" +
                ExtrairInformacoesPrincipais(saida);
        }

        return
            "⚠️ Não foi possível interpretar automaticamente o status.\n\n" +
            saida.Trim();
    }

    private string ExtrairInformacoesPrincipais(string saida)
    {
        string[] linhas = saida.Split(
            ['\r', '\n'],
            StringSplitOptions.RemoveEmptyEntries);

        string[] termosImportantes =
        [
            "LICENSE NAME:",
            "LICENSE DESCRIPTION:",
            "LICENSE STATUS:",
            "REMAINING GRACE:",
            "Last 5 characters"
        ];

        IEnumerable<string> linhasImportantes =
            linhas
                .Select(linha => linha.Trim())
                .Where(linha =>
                    termosImportantes.Any(termo =>
                        linha.Contains(
                            termo,
                            StringComparison.OrdinalIgnoreCase)));

        string texto = string.Join(
            Environment.NewLine,
            linhasImportantes);

        return string.IsNullOrWhiteSpace(texto)
            ? saida.Trim()
            : texto;
    }

    private string? LocalizarOspp(string caminhoOffice)
    {
        string[] caminhosPossiveis =
        [
            Path.Combine(caminhoOffice, "OSPP.VBS"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                "Microsoft Office",
                "root",
                "Office16",
                "OSPP.VBS"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFilesX86),
                "Microsoft Office",
                "root",
                "Office16",
                "OSPP.VBS"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                "Microsoft Office",
                "Office16",
                "OSPP.VBS"),

            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFilesX86),
                "Microsoft Office",
                "Office16",
                "OSPP.VBS")
        ];

        return caminhosPossiveis.FirstOrDefault(File.Exists);
    }

    private string LocalizarPastaOffice(string caminho)
    {
        string[] possibilidades =
        [
            Path.Combine(caminho, "root", "Office16"),
            Path.Combine(caminho, "Office16"),
            caminho
        ];

        return possibilidades.FirstOrDefault(Directory.Exists)
               ?? caminho;
    }

    private string TraduzirProduto(string? produto)
    {
        if (string.IsNullOrWhiteSpace(produto))
            return "Microsoft Office";

        if (produto.Contains(
                "O365",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Microsoft 365";
        }

        if (produto.Contains(
                "2024",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Microsoft Office 2024";
        }

        if (produto.Contains(
                "2021",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Microsoft Office 2021";
        }

        if (produto.Contains(
                "2019",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Microsoft Office 2019";
        }

        return produto.Replace(",", ", ");
    }

    private string TraduzirVersao(string? versao)
    {
        if (string.IsNullOrWhiteSpace(versao))
            return "Não identificada";

        return versao;
    }

    private string TraduzirArquitetura(string? plataforma)
    {
        return plataforma?.ToLowerInvariant() switch
        {
            "x64" => "64 bits",
            "x86" => "32 bits",
            _ => "Não identificada"
        };
    }

    private sealed class InformacaoOffice
    {
        public string NomeProduto { get; init; } = "Microsoft Office";

        public string Versao { get; init; } = "Não identificada";

        public string Arquitetura { get; init; } = "Não identificada";

        public string CaminhoInstalacao { get; init; } = string.Empty;
    }
}