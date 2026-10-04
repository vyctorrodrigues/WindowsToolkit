using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using WindowsToolkit.Models;
using System.Collections.Generic;
using System.Linq;

namespace WindowsToolkit.Services
{   
    
    public static class RedeService
    {

        static RedeService()
        {
            Encoding.RegisterProvider(
                CodePagesEncodingProvider.Instance);
        }
        public static ResultadoComando LimparDns()
        {
            return ExecutarComando("ipconfig", "/flushdns");
        }

        public static ResultadoComando RenovarConexao()
        {
                ResultadoComando liberar = ExecutarComando(
                    "ipconfig",
                    "/release"
                );
            
            if (!liberar.Sucesso)
            {
                return liberar;
            }

            System.Threading.Thread.Sleep(2000); // Aguarda 2 segundos antes de renovar a conexão

            return ExecutarComando(
                "ipconfig",
                "/renew"
            );
        }

        public static ResultadoComando ReiniciarWinSock()
        {
            return ExecutarComando("netsh", "winsock reset");
        }

        private static ResultadoComando ExecutarComando(
            string programa, 
            string argumentos)

        {
            try
            {
                ProcessStartInfo processoInfo = new ProcessStartInfo
                {
                    FileName = programa,
                    Arguments = argumentos,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.GetEncoding(850),
                    StandardErrorEncoding = Encoding.GetEncoding(850),
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using Process? processo = Process.Start(processoInfo);

                if (processo == null)
                {
                    return new ResultadoComando 
                    { 
                        Sucesso = false,
                        Erro = "Não foi possível iniciar o processo.",
                        CodigoSaida = -1
                    };
                }

                

                string saida = processo.StandardOutput.ReadToEnd();
                string erro = processo.StandardError.ReadToEnd();

                processo.WaitForExit();

                return new ResultadoComando
                {
                    Sucesso = processo.ExitCode == 0,
                    Saida = saida,
                    Erro = erro,
                    CodigoSaida = processo.ExitCode
                };

            } 
            
            catch (Exception ex) 

            {
                return new ResultadoComando { 
                    Sucesso = false,
                    Erro = ex.Message,
                    CodigoSaida = -1
                };
            }
        }

        public static List<AdaptadorRedeInfo> ObterAdaptadoresRede()
        {
            List<AdaptadorRedeInfo> adaptadores = new();

            foreach (NetworkInterface adaptador in
                     NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adaptador.NetworkInterfaceType !=
                        NetworkInterfaceType.Ethernet &&
                    adaptador.NetworkInterfaceType !=
                        NetworkInterfaceType.Wireless80211)
                {
                    continue;
                }

                try
                {
                    IPInterfaceProperties propriedades =
                        adaptador.GetIPProperties();

                    IPv4InterfaceProperties? propriedadesIPv4 = null;

                    try
                    {
                        propriedadesIPv4 =
                            propriedades.GetIPv4Properties();
                    }
                    catch (NetworkInformationException)
                    {
                        // Alguns adaptadores não fornecem propriedades IPv4.
                    }

                    AdaptadorRedeInfo informacoes =
                        new AdaptadorRedeInfo
                        {
                            Nome = adaptador.Name,
                            Descricao = adaptador.Description,

                            Tipo = adaptador.NetworkInterfaceType ==
                                   NetworkInterfaceType.Wireless80211
                                ? "Wi-Fi"
                                : "Ethernet",

                            Status = adaptador.OperationalStatus ==
                                     OperationalStatus.Up
                                ? "Ativo"
                                : "Inativo",

                            Dhcp = propriedadesIPv4 == null
                                ? "Não disponível"
                                : propriedadesIPv4.IsDhcpEnabled
                                    ? "Ativado"
                                    : "Desativado",

                            EnderecoMac = FormatarEnderecoMac(
                                adaptador
                                    .GetPhysicalAddress()
                                    .ToString()),

                            Velocidade = FormatarVelocidade(
                                adaptador.Speed)
                        };

                    foreach (UnicastIPAddressInformation endereco in
                             propriedades.UnicastAddresses)
                    {
                        if (endereco.Address.AddressFamily ==
                            AddressFamily.InterNetwork)
                        {
                            informacoes.EnderecosIPv4.Add(
                                endereco.Address.ToString());

                            try
                            {
                                if (endereco.IPv4Mask != null)
                                {
                                    informacoes.Mascaras.Add(
                                        endereco.IPv4Mask.ToString());
                                }
                            }
                            catch (NetworkInformationException)
                            {
                                // A máscara não foi disponibilizada.
                            }
                        }
                        else if (endereco.Address.AddressFamily ==
                                 AddressFamily.InterNetworkV6)
                        {
                            informacoes.EnderecosIPv6.Add(
                                endereco.Address.ToString());
                        }
                    }

                    try
                    {
                        informacoes.Gateways.AddRange(
                            propriedades.GatewayAddresses
                                .Select(gateway =>
                                    gateway.Address.ToString())
                                .Where(endereco =>
                                    !string.IsNullOrWhiteSpace(
                                        endereco)));
                    }
                    catch (NetworkInformationException)
                    {
                        // Gateway indisponível.
                    }

                    try
                    {
                        informacoes.ServidoresDns.AddRange(
                            propriedades.DnsAddresses
                                .Select(dns => dns.ToString()));
                    }
                    catch (NetworkInformationException)
                    {
                        // DNS indisponível.
                    }

                    adaptadores.Add(informacoes);
                }
                catch (NetworkInformationException)
                {
                    // Ignora somente o adaptador que causou o erro.
                    continue;
                }
            }

            return adaptadores
                .OrderByDescending(a => a.Status == "Ativo")
                .ThenBy(a => a.Tipo == "Ethernet" ? 0 : 1)
                .ToList();
        
        }

        private static string FormatarEnderecoMac(string enderecoMac)
        {
            if (string.IsNullOrWhiteSpace(enderecoMac))
            {
                return "Não disponível";
            }

            return string.Join(
                "-",
                Enumerable.Range(0, enderecoMac.Length / 2)
                    .Select(indice =>
                        enderecoMac.Substring(indice * 2, 2)));
        }

        private static string FormatarVelocidade(long velocidadeBits)
        {
            if (velocidadeBits <= 0)
            {
                return "Não disponível";
            }

            double velocidadeMbps =
                velocidadeBits / 1_000_000d;

            if (velocidadeMbps >= 1000)
            {
                return $"{velocidadeMbps / 1000:0.##} Gbps";
            }

            return $"{velocidadeMbps:0.##} Mbps";
        }
    }
}
