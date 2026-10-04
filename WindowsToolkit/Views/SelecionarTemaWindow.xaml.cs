using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    public partial class SelecionarTemaWindow : Window
    {
        public SelecionarTemaWindow()
        {
            InitializeComponent();
        }

        private void BotaoTema_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            string tema =
                botao.Tag?.ToString() ?? "Escuro";

            TemaService.AplicarTema(tema);
        }

        private void BotaoFechar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}