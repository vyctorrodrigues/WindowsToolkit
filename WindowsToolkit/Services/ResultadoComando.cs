using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsToolkit.Services
{
    public class ResultadoComando
    {
        public bool Sucesso { get; set; }

        public string Saida { get; set; } = string.Empty;

        public string Erro { get; set; } = string.Empty;

        public int CodigoSaida { get; set; }
    }
}
