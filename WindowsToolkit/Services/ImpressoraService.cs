using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows;
using Microsoft.Win32;
using WindowsToolkit.Models;
using System;
using System.Text;
using System.Collections.Generic;



namespace WindowsToolkit.Services
{
    public static class ImpressoraService
    {
        public static List<ImpressoraInfo> ObterImpressoras()
        {
            List<ImpressoraInfo> impressoras = new();

            ManagementObjectSearcher pesquisa =
                new("SELECT * FROM Win32_Printer");

            foreach (ManagementObject impressora in pesquisa.Get())
            {
                impressoras.Add(new ImpressoraInfo
                {
                    Nome = impressora["Name"]?.ToString() ?? "Desconhecido",
                    Driver = impressora["DriverName"]?.ToString() ?? "Desconhecido",
                    Porta = impressora["PortName"]?.ToString() ?? "Desconhecida",
                    Localizacao = impressora["Location"]?.ToString() ?? "-",
                    NomeCompartilhamento = impressora["ShareName"]?.ToString() ?? "-",
                    Compartilhada = (bool?)impressora["Shared"] == true ? "Sim" : "Não",
                    Padrao = (bool?)impressora["Default"] == true ? "Sim" : "Não",
                    Status = ObterStatusImpressora(
                        impressora["PrinterStatus"],
                        impressora["WorkOffline"])
                });
            }

            return impressoras;
        }

        public static bool ImprimirPaginaTeste(
            string nomeImpressora,
            out string mensagem)
        {
            try
            {
                string nomeEscapado =
                    nomeImpressora.Replace("\\", "\\\\");

                string consulta =
                    $"SELECT * FROM Win32_Printer " +
                    $"WHERE Name = '{nomeEscapado.Replace("'", "''")}'";

                using ManagementObjectSearcher pesquisa = new(consulta);

                ManagementObject? impressora =
                    pesquisa.Get()
                            .Cast<ManagementObject>()
                            .FirstOrDefault();

                if (impressora == null)
                {
                    mensagem = "A impressora não foi encontrada.";
                    return false;
                }

                object resultado = impressora.InvokeMethod(
                    "PrintTestPage",
                    null);

                uint codigo = Convert.ToUInt32(resultado);

                if (codigo == 0)
                {
                    mensagem =
                        $"Página de teste enviada para:\n{nomeImpressora}";

                    return true;
                }

                mensagem =
                    $"O Windows não conseguiu imprimir a página de teste.\n" +
                    $"Código retornado: {codigo}";

                return false;
            }
            catch (Exception ex)
            {
                mensagem =
                    $"Erro ao imprimir a página de teste:\n\n{ex.Message}";

                return false;
            }
        }

        public static bool DefinirComoPadrao(
                string nomeImpressora,
                out string mensagem)
        {
            try
            {
                string consulta =
                    $"SELECT * FROM Win32_Printer WHERE Name='{nomeImpressora.Replace("\\", "\\\\").Replace("'", "''")}'";

                using ManagementObjectSearcher pesquisa = new(consulta);

                ManagementObject? impressora =
                    pesquisa.Get()
                            .Cast<ManagementObject>()
                            .FirstOrDefault();

                if (impressora == null)
                {
                    mensagem = "Impressora não encontrada.";
                    return false;
                }

                uint resultado =
                    (uint)impressora.InvokeMethod("SetDefaultPrinter", null);

                if (resultado == 0)
                {
                    mensagem = $"'{nomeImpressora}' agora é a impressora padrão.";
                    return true;
                }

                mensagem = $"Erro ao definir impressora padrão.\nCódigo: {resultado}";
                return false;
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                return false;
            }
        }

        private static string ObterStatusImpressora(
            object? valorStatus,
            object? valorOffline)
        {
            bool offline = valorOffline is bool estaOffline && estaOffline;

            if (offline)
                return "Offline";

            if (valorStatus == null)
                return "Desconhecido";

            ushort status = Convert.ToUInt16(valorStatus);

            return status switch
            {
                1 => "Outro",
                2 => "Desconhecido",
                3 => "Pronta",
                4 => "Imprimindo",
                5 => "Aquecendo",
                6 => "Interrompida",
                7 => "Offline",
                _ => $"Status {status}"
            };
        }



        public static bool AbrirFilaImpressao(
            string nomeImpressora,
            out string mensagem)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "rundll32.exe",
                    Arguments = $"printui.dll,PrintUIEntry /o /n \"{nomeImpressora}\"",
                    UseShellExecute = true
                });

                mensagem = "Fila de impressão aberta.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                return false;
            }
        }



        public static bool CorrigirImpressaoAlias(out string mensagem)
        {

            try
            {
                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Print",
                    "DnsOnWire",
                    1,
                    RegistryValueKind.DWord);

                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters",
                    "OptionalNames",
                    "aliasname",
                    RegistryValueKind.String);

                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters",
                    "DisableStrictNameChecking",
                    1,
                    RegistryValueKind.DWord);

                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Print\Providers\Client Side Rendering Print Provider",
                    "InactiveGuidPrinterAge",
                    1200,
                    RegistryValueKind.DWord);

                Registry.SetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Print\Providers\Client Side Rendering Print Provider",
                    "InactiveGuidPrinterTrim",
                    3600,
                    RegistryValueKind.DWord);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c net stop spooler && net start spooler",
                    CreateNoWindow = true,
                    UseShellExecute = false
                })?.WaitForExit();

                mensagem = "Configurações aplicadas e Spooler reiniciado com sucesso.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                return false;
            }
        }

        public static bool MapearLpt(
           string portaLpt,
           string caminhoImpressora,
           bool persistente,
           out string mensagem)
        {
            try
            {
                string portaNormalizada =
                    portaLpt.Trim().TrimEnd(':').ToUpperInvariant();

                if (portaNormalizada is not "LPT1"
                    and not "LPT2"
                    and not "LPT3")
                {
                    mensagem = "A porta LPT informada é inválida.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(caminhoImpressora)
                    || !caminhoImpressora.StartsWith(@"\\"))
                {
                    mensagem = "O caminho da impressora compartilhada é inválido.";
                    return false;
                }

                using Process processo = new();

                string opcaoPersistencia =
                    persistente ? "yes" : "no";

                processo.StartInfo = new ProcessStartInfo
                {
                    FileName = "net.exe",
                    Arguments =
                        $"use {portaNormalizada}: \"{caminhoImpressora}\" " +
                        $"/persistent:{opcaoPersistencia}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                processo.Start();

                string saida = processo.StandardOutput.ReadToEnd();
                string erro = processo.StandardError.ReadToEnd();

                processo.WaitForExit();

                if (processo.ExitCode == 0)
                {
                    mensagem =
                        $"A porta {portaNormalizada} foi mapeada para:\n\n" +
                        caminhoImpressora;

                    return true;
                }

                mensagem =
                    $"Não foi possível mapear a porta {portaNormalizada}.\n\n" +
                    (string.IsNullOrWhiteSpace(erro) ? saida : erro);

                return false;
            }
            catch (Exception ex)
            {
                mensagem =
                    $"Ocorreu um erro ao mapear a porta LPT.\n\n{ex.Message}";

                return false;
            }
        }

        public static bool InstalarImpressoraRede(
            string caminhoCompartilhado,
            out string mensagem)
        {
            if (string.IsNullOrWhiteSpace(caminhoCompartilhado) ||
                !caminhoCompartilhado.StartsWith(@"\\"))
            {
                mensagem =
                    "Informe um caminho de rede válido.\n\n" +
                    @"Exemplo: \\SERVIDOR\IMPRESSORA";

                return false;
            }

            string caminhoSeguro =
                caminhoCompartilhado.Replace("'", "''");

            string comando =
                $"Add-Printer -ConnectionName '{caminhoSeguro}'";

            bool sucesso = ExecutarPowerShell(
                comando,
                out string resultado);

            mensagem = sucesso
                ? $"Impressora de rede instalada com sucesso:\n\n{caminhoCompartilhado}"
                : $"Não foi possível instalar a impressora.\n\n{resultado}";

            return sucesso;
        }

        public static bool AdicionarImpressoraLocal(
            string nomeImpressora,
            string nomeDriver,
            string nomePorta,
            out string mensagem)
        {
            if (string.IsNullOrWhiteSpace(nomeImpressora))
            {
                mensagem = "Informe o nome da impressora.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nomeDriver))
            {
                mensagem = "Selecione um driver.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(nomePorta))
            {
                mensagem = "Selecione uma porta.";
                return false;
            }

            string impressoraSegura =
                nomeImpressora.Replace("'", "''");

            string driverSeguro =
                nomeDriver.Replace("'", "''");

            string portaSegura =
                nomePorta.Replace("'", "''");

            string comando =
                "Add-Printer " +
                $"-Name '{impressoraSegura}' " +
                $"-DriverName '{driverSeguro}' " +
                $"-PortName '{portaSegura}'";

            bool sucesso = ExecutarPowerShell(
                comando,
                out string resultado);

            mensagem = sucesso
                ? $"Impressora adicionada com sucesso.\n\n" +
                  $"Nome: {nomeImpressora}\n" +
                  $"Porta: {nomePorta}\n" +
                  $"Driver: {nomeDriver}"
                : $"Não foi possível adicionar a impressora.\n\n{resultado}";

            return sucesso;
        }

        private static bool ExecutarPowerShell(
            string comando,
            out string resultado)
        {
            try
            {
                string comandoCodificado =
                    Convert.ToBase64String(
                        Encoding.Unicode.GetBytes(comando));

                using Process processo = new();

                processo.StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments =
                        "-NoProfile " +
                        "-NonInteractive " +
                        "-ExecutionPolicy Bypass " +
                        $"-EncodedCommand {comandoCodificado}",

                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                processo.Start();

                string saida =
                    processo.StandardOutput.ReadToEnd();

                string erro =
                    processo.StandardError.ReadToEnd();

                processo.WaitForExit();

                resultado = string.IsNullOrWhiteSpace(erro)
                    ? saida.Trim()
                    : erro.Trim();

                if (processo.ExitCode == 0)
                {
                    if (string.IsNullOrWhiteSpace(resultado))
                        resultado = "Comando concluído com sucesso.";

                    return true;
                }

                if (string.IsNullOrWhiteSpace(resultado))
                {
                    resultado =
                        $"O comando terminou com o código {processo.ExitCode}.";
                }

                return false;
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
                return false;
            }
        }

        public static List<string> ObterDriversInstalados()
        {
            List<string> drivers = new();

            using ManagementObjectSearcher pesquisa =
                new("SELECT Name FROM Win32_PrinterDriver");

            foreach (ManagementObject driver in pesquisa.Get())
            {
                string? nome = driver["Name"]?.ToString();

                if (string.IsNullOrWhiteSpace(nome))
                    continue;

                string nomeLimpo = nome.Split(',')[0].Trim();

                if (!drivers.Contains(nomeLimpo))
                    drivers.Add(nomeLimpo);
            }

            drivers.Sort();

            return drivers;
        }

        public static List<string> ObterPortasImpressora()
        {
            List<string> portas = new();

            using ManagementObjectSearcher pesquisa =
                new("SELECT Name FROM Win32_TCPIPPrinterPort");

            foreach (ManagementObject porta in pesquisa.Get())
            {
                string? nome = porta["Name"]?.ToString();

                if (!string.IsNullOrWhiteSpace(nome))
                    portas.Add(nome);
            }

            using ManagementObjectSearcher pesquisaImpressoras =
                new("SELECT PortName FROM Win32_Printer");

            foreach (ManagementObject impressora in pesquisaImpressoras.Get())
            {
                string? nomePorta =
                    impressora["PortName"]?.ToString();

                if (string.IsNullOrWhiteSpace(nomePorta))
                    continue;

                if (!portas.Contains(nomePorta))
                    portas.Add(nomePorta);
            }

            portas.Sort();

            return portas;
        }

    }

}
