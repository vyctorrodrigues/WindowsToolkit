using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace WindowsToolkit.Services
{
    public static class LimpezaService
    {

        [DllImport("Shell32.dll")]
        private static extern int SHEmptyRecycleBin(
            IntPtr hwnd,
            string? pszRootPath,
            RecycleFlags dwFlags);

        [Flags]
        public enum RecycleFlags : uint
        {
            SemConfirmacao = 0x00000001,
            SemProgresso = 0x00000002,
            SemSom = 0x00000004
        }

        public static void LimparTemp()
        {
            string pastaTemp = Path.GetTempPath();

            foreach (string arquivo in Directory.GetFiles(pastaTemp))
            {
                try
                {
                    File.Delete(arquivo);
                }
                catch
                {
                    // Handle exceptions if needed
                }
            }

            foreach (string pasta in Directory.GetDirectories(pastaTemp))
            {
                try
                {
                    Directory.Delete(pasta, true);
                }
                catch
                {
                    // Handle exceptions if needed
                }
            }
        }

        public static bool EsvaziarLixeira()
        {
            int resultado = SHEmptyRecycleBin(
                IntPtr.Zero,
                null,
                RecycleFlags.SemConfirmacao |
                RecycleFlags.SemProgresso |
                RecycleFlags.SemSom
            );
            return resultado == 0;
        }

        public static int LimparHistoricoExecutar()
            
        {
            int itensRemovidos = 0;

                string[] caminhosRegistro = {
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU",
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"
                };

            foreach (string caminho in caminhosRegistro)
            {
                using RegistryKey? chave = Registry.CurrentUser.OpenSubKey(
                    caminho,
                    writable: true
                );
            

                if (chave is null)
                {
                    continue;
                }

                foreach (string nomeValor in chave.GetValueNames())
                {
                    if (string.IsNullOrEmpty(nomeValor))
                    {
                        continue;
                    }


                    try
                    {
                        chave.DeleteValue(nomeValor, throwOnMissingValue: false);
                        itensRemovidos++;

                    }
                    catch
                    {
                        // Handle exceptions if needed
                    }
                }


            }

            return itensRemovidos;

        }
    }
}
