

namespace WindowsToolkit.Models
{
    public class AdaptadorRedeInfo
    {
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string EnderecoMac { get; set; } = string.Empty;

        public string Velocidade { get; set; } = string.Empty;

        public List<string> EnderecosIPv4 { get; set; } = new();

        public List<string> EnderecosIPv6 { get; set; } = new();

        public List<string> Mascaras { get; set; } = new();

        public List<string> Gateways { get; set; } = new();

        public List<string> ServidoresDns { get; set; } = new();
        public string Dhcp { get; set; } = string.Empty;


        public string IPv4Texto =>
            EnderecosIPv4.Count > 0
                ? string.Join(", ", EnderecosIPv4)
                : "Não disponível";

        public string IPv6Texto =>
            EnderecosIPv6.Count > 0
                ? string.Join(", ", EnderecosIPv6)
                : "Não disponível";

        public string MascaraTexto =>
            Mascaras.Count > 0
                ? string.Join(", ", Mascaras)
                : "Não disponível";

        public string GatewayTexto =>
            Gateways.Count > 0
                ? string.Join(", ", Gateways)
                : "Não disponível";

        public string DnsTexto =>
            ServidoresDns.Count > 0
                ? string.Join(", ", ServidoresDns)
                : "Não disponível";



    }


}
