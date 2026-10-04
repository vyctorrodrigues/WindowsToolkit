using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsToolkit.Models
{
    public class DiscoInfo
    {
        public string Unidade { get; set; } = "";
        public string SistemaArquivos { get; set; } = "";
        public string EspacoTotal { get; set; } = "";
        public string EspacoLivre { get; set; } = "";
        public string EspacoUsado { get; set; } = "";
        public string PercentualUso { get; set; } = "";
    }
}
