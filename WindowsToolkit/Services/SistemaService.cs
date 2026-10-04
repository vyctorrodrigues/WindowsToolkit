using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WindowsToolkit.Models;

namespace WindowsToolkit.Services
{
    public static class SistemaService
    {
        private static Process? _processoMas;

        public static SistemaInfo ObterInformacoes()
        {
            SistemaInfo info = new();

            info.NomeComputador = Environment.MachineName;
            info.Arquitetura = Environment.Is64BitOperatingSystem
                ? "64 bits"
                : "32 bits";

            using ManagementObjectSearcher pesquisaSistema =
                new("SELECT Caption, Version, BuildNumber, LastBootUpTime FROM Win32_OperatingSystem");

            ManagementObject? sistema =
                pesquisaSistema.Get()
                    .Cast<ManagementObject>()
                    .FirstOrDefault();

            if (sistema != null)
            {
                info.SistemaOperacional =
                    sistema["Caption"]?.ToString() ?? "Desconhecido";

                info.Versao =
                    sistema["Version"]?.ToString() ?? "Desconhecida";

                info.Build =
                    sistema["BuildNumber"]?.ToString() ?? "Desconhecida";

                string? ultimoBoot =
                    sistema["LastBootUpTime"]?.ToString();

                if (!string.IsNullOrWhiteSpace(ultimoBoot))
                {
                    DateTime dataBoot =
                        ManagementDateTimeConverter.ToDateTime(ultimoBoot);

                    TimeSpan tempoLigado =
                        DateTime.Now - dataBoot;

                    info.TempoLigado =
                        $"{(int)tempoLigado.TotalDays} dias, " +
                        $"{tempoLigado.Hours} horas e " +
                        $"{tempoLigado.Minutes} minutos";
                }
            }

            using ManagementObjectSearcher pesquisaCpu =
                new("SELECT Name FROM Win32_Processor");

            ManagementObject? cpu =
                pesquisaCpu.Get()
                    .Cast<ManagementObject>()
                    .FirstOrDefault();

            info.Processador =
                cpu?["Name"]?.ToString()?.Trim()
                ?? "Desconhecido";

            using ManagementObjectSearcher pesquisaMemoria =
                new("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

            ManagementObject? computador =
                pesquisaMemoria.Get()
                    .Cast<ManagementObject>()
                    .FirstOrDefault();

            if (computador?["TotalPhysicalMemory"] != null)
            {
                ulong bytes =
                    Convert.ToUInt64(
                        computador["TotalPhysicalMemory"]);

                double gigabytes =
                    bytes / 1024d / 1024d / 1024d;

                info.MemoriaRam =
                    $"{gigabytes:F1} GB";
            }
            else
            {
                info.MemoriaRam = "Desconhecida";
            }

            return info;
        }

        public static bool ReiniciarExplorer(out string mensagem)
        {
            try
            {
                Process[] processos =
                    Process.GetProcessesByName("explorer");

                foreach (Process processo in processos)
                {
                    using (processo)
                    {
                        processo.Kill();
                        processo.WaitForExit(5000);
                    }
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    UseShellExecute = true
                });

                mensagem = "Explorer reiniciado com sucesso.";
                return true;
            }
            catch (Exception ex)
            {
                mensagem =
                    $"Não foi possível reiniciar o Explorer.\n\n{ex.Message}";

                return false;
            }
        }

        public static bool ExecutarSfc(out string mensagem)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k sfc /scannow",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                mensagem = "A verificação do sistema foi iniciada.";
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                mensagem = ex.NativeErrorCode == 1223
                    ? "A execução foi cancelada pelo usuário."
                    : $"Não foi possível iniciar o SFC.\n\n{ex.Message}";

                return false;
            }
            catch (Exception ex)
            {
                mensagem = $"Não foi possível iniciar o SFC.\n\n{ex.Message}";
                return false;
            }
        }

        public static bool ExecutarDism(out string mensagem)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k DISM /Online /Cleanup-Image /RestoreHealth",
                    UseShellExecute = true,
                    Verb = "runas"
                });

                mensagem = "O reparo da imagem do Windows foi iniciado.";
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                mensagem = ex.NativeErrorCode == 1223
                    ? "A execução foi cancelada pelo usuário."
                    : $"Não foi possível iniciar o DISM.\n\n{ex.Message}";

                return false;
            }
            catch (Exception ex)
            {
                mensagem = $"Não foi possível iniciar o DISM.\n\n{ex.Message}";
                return false;
            }
        }

        public static List<DiscoInfo> ObterDiscos()
        {
            List<DiscoInfo> discos = new();

            foreach (DriveInfo unidade in DriveInfo.GetDrives())
            {
                if (!unidade.IsReady)
                    continue;

                long total = unidade.TotalSize;
                long livre = unidade.AvailableFreeSpace;
                long usado = total - livre;

                double percentualUso =
                    total > 0
                        ? usado * 100d / total
                        : 0;

                discos.Add(new DiscoInfo
                {
                    Unidade = unidade.Name,
                    SistemaArquivos = unidade.DriveFormat,
                    EspacoTotal = FormatarBytes(total),
                    EspacoLivre = FormatarBytes(livre),
                    EspacoUsado = FormatarBytes(usado),
                    PercentualUso = $"{percentualUso:F1}%"
                });
            }

            return discos;
        }

        private static string FormatarBytes(long bytes)
        {
            double gigabytes =
                bytes / 1024d / 1024d / 1024d;

            return $"{gigabytes:F1} GB";
        }

        public static void ExecutarMasOffice()
        {
            string psCommand = "irm https://get.activated.win | iex";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(psi);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Trata o caso em que o usuário recusa a elevação de Admin (UAC)
            }
        }

        public static async Task ExecutarMasComandoSilenciosoAsync(string parametroMas, Action<string> noReceberTexto)
        {
            // O parâmetro instrui o repositório a rodar direto e fechar sem abrir o menu .cmd
            string psCommand = "$ProgressPreference = 'SilentlyContinue'; " +
                               "[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12; " +
                               $"iex ((irm https://get.activated.win) + ' {parametroMas}')";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psCommand}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            _processoMas = new Process { StartInfo = psi };

            _processoMas.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    noReceberTexto(e.Data + Environment.NewLine);
            };

            _processoMas.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    noReceberTexto("[ERRO] " + e.Data + Environment.NewLine);
            };

            try
            {
                _processoMas.Start();
                _processoMas.BeginOutputReadLine();
                _processoMas.BeginErrorReadLine();

                await _processoMas.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                noReceberTexto($"Erro ao executar a rotina: {ex.Message}" + Environment.NewLine);
            }
        }
    }
}