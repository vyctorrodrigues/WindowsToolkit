using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsToolkit.Models
{
    public class SistemaInfo
    {
        public string NomeComputador { get; set; } = "";
        public string SistemaOperacional { get; set; } = "";
        public string Versao { get; set; } = "";
        public string Build { get; set; } = "";
        public string Arquitetura { get; set; } = "";
        public string Processador { get; set; } = "";
        public string MemoriaRam { get; set; } = "";
        public string TempoLigado { get; set; } = "";
    }
}

