using System.Windows;
using System.Windows.Media;

namespace WindowsToolkit.Services
{
    public static class TemaService
    {
        public static void AplicarTema(string tema)
        {
            switch (tema)
            {
                case "Azul":
                    AplicarAzul();
                    break;

                case "Roxo":
                    AplicarRoxo();
                    break;

                case "Verde":
                    AplicarVerde();
                    break;

                case "Vermelho":
                    AplicarVermelho();
                    break;

                case "Amarelo":
                    AplicarAmarelo();
                    break;

                default:
                    AplicarEscuro();
                    break;


            }
        }

        private static void AplicarEscuro()
        {
            DefinirCor("CorFundoPrincipal", "#202124");
            DefinirCor("CorMenuLateral", "#2B2B2B");
            DefinirCor("CorPainel", "#3A3A3A");
            DefinirCor("CorBotao", "#3F3F3F");
            DefinirCor("CorBotaoHover", "#505050");
            DefinirCor("CorDestaque", "#00BCD4");
            DefinirCor("CorTextoPrincipal", "#FFFFFF");
            DefinirCor("CorTextoSecundario", "#CCCCCC");
        }

        private static void AplicarAzul()
        {
            DefinirCor("CorFundoPrincipal", "#0F172A");
            DefinirCor("CorMenuLateral", "#111827");
            DefinirCor("CorPainel", "#1E293B");
            DefinirCor("CorBotao", "#334155");
            DefinirCor("CorBotaoHover", "#475569");
            DefinirCor("CorDestaque", "#38BDF8");
            DefinirCor("CorTextoPrincipal", "#FFFFFF");
            DefinirCor("CorTextoSecundario", "#CBD5E1");
        }

        private static void AplicarRoxo()
        {
            DefinirCor("CorFundoPrincipal", "#1C1326");
            DefinirCor("CorMenuLateral", "#251735");
            DefinirCor("CorPainel", "#332044");
            DefinirCor("CorBotao", "#452B5A");
            DefinirCor("CorBotaoHover", "#5B3975");
            DefinirCor("CorDestaque", "#C084FC");
            DefinirCor("CorTextoPrincipal", "#FFFFFF");
            DefinirCor("CorTextoSecundario", "#E9D5FF");
        }

        private static void AplicarVerde()
        {
            DefinirCor("CorFundoPrincipal", "#102018");
            DefinirCor("CorMenuLateral", "#16291F");
            DefinirCor("CorPainel", "#1E3528");
            DefinirCor("CorBotao", "#294836");
            DefinirCor("CorBotaoHover", "#376149");
            DefinirCor("CorDestaque", "#4ADE80");
            DefinirCor("CorTextoPrincipal", "#FFFFFF");
            DefinirCor("CorTextoSecundario", "#D1FAE5");
        }

        private static void AplicarVermelho()
        {
            DefinirCor("CorFundoPrincipal", "#241313");
            DefinirCor("CorMenuLateral", "#321818");
            DefinirCor("CorPainel", "#452020");
            DefinirCor("CorBotao", "#5C2A2A");
            DefinirCor("CorBotaoHover", "#783939");
            DefinirCor("CorDestaque", "#F87171");
            DefinirCor("CorTextoPrincipal", "#FFFFFF");
            DefinirCor("CorTextoSecundario", "#FEE2E2");
        }

        private static void AplicarAmarelo()
        {
            DefinirCor("CorFundoPrincipal", "#FFF7E6");
            DefinirCor("CorMenuLateral", "#FCD9A8");
            DefinirCor("CorPainel", "#FFE5B4");
            DefinirCor("CorBotao", "#FDB87E");
            DefinirCor("CorBotaoHover", "#FC9A4B");
            DefinirCor("CorDestaque", "#FACC15");
            DefinirCor("CorTextoPrincipal", "#111827");
            DefinirCor("CorTextoSecundario", "#4B5563");
        }
        private static void DefinirCor(string chave, string hexadecimal)
        {
            Color cor = (Color)ColorConverter.ConvertFromString(hexadecimal);

            Application.Current.Resources[chave] =
                new SolidColorBrush(cor);
        }
    }
}