using System;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;

namespace WindowsToolkit.Services
{
    public static class SpoolerService
    {
        public static bool ReiniciarSpooler(out string mensagem)
        {
            try
            {
                using ServiceController spooler = new("Spooler");

                spooler.Refresh();

                if (spooler.Status != ServiceControllerStatus.Stopped)
                {
                    spooler.Stop();

                    spooler.WaitForStatus(
                        ServiceControllerStatus.Stopped,
                        TimeSpan.FromSeconds(15));
                }

                spooler.Start();

                spooler.WaitForStatus(
                    ServiceControllerStatus.Running,
                    TimeSpan.FromSeconds(15));

                mensagem = "Serviço de impressão reiniciado com sucesso.";
                return true;
            }
            catch (InvalidOperationException ex)
                when (ex.InnerException is Win32Exception win32Exception)
            {
                mensagem =
                    $"Não foi possível controlar o serviço Spooler.\n\n" +
                    $"Erro do Windows: {win32Exception.Message}\n" +
                    $"Código: {win32Exception.NativeErrorCode}\n\n" +
                    "Execute o WindowsToolkit como administrador.";

                return false;
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                mensagem =
                    "O serviço Spooler demorou mais de 15 segundos para reiniciar.";

                return false;
            }
            catch (Exception ex)
            {
                mensagem =
                    $"Erro ao reiniciar o Spooler:\n\n{ex}";

                return false;
            }
        }

        public static bool LimparFilaImpressao(out string mensagem)
        {
            string pastaFila = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                "System32",
                "spool",
                "PRINTERS");

            int arquivosExcluidos = 0;
            bool spoolerFoiParado = false;

            try
            {
                using ServiceController spooler = new("Spooler");

                spooler.Refresh();

                if (spooler.Status != ServiceControllerStatus.Stopped)
                {
                    spooler.Stop();

                    spooler.WaitForStatus(
                        ServiceControllerStatus.Stopped,
                        TimeSpan.FromSeconds(15));

                    spoolerFoiParado = true;
                }

                if (!Directory.Exists(pastaFila))
                {
                    mensagem = $"A pasta da fila não foi encontrada:\n{pastaFila}";
                    return false;
                }

                foreach (string arquivo in Directory.GetFiles(pastaFila))
                {
                    File.Delete(arquivo);
                    arquivosExcluidos++;
                }

                mensagem = arquivosExcluidos == 0
                    ? "A fila de impressão já estava vazia."
                    : $"Fila limpa com sucesso.\n\nArquivos removidos: {arquivosExcluidos}";

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                mensagem =
                    "Acesso negado ao limpar a fila de impressão.\n\n" +
                    "Execute o WindowsToolkit como administrador.";

                return false;
            }
            catch (System.ServiceProcess.TimeoutException)
            {
                mensagem =
                    "O serviço Spooler demorou mais de 15 segundos para responder.";

                return false;
            }
            catch (Exception ex)
            {
                mensagem = $"Erro ao limpar a fila:\n\n{ex.Message}";
                return false;
            }
            finally
            {
                if (spoolerFoiParado)
                {
                    try
                    {
                        using ServiceController spooler = new("Spooler");

                        spooler.Refresh();

                        if (spooler.Status != ServiceControllerStatus.Running)
                        {
                            spooler.Start();

                            spooler.WaitForStatus(
                                ServiceControllerStatus.Running,
                                TimeSpan.FromSeconds(15));
                        }
                    }
                    catch
                    {
                        // Não substitui a mensagem do erro principal.
                    }
                }
            }
        }
    }
}