using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsToolkit.Models;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    /// <summary>
    /// Interação lógica para SistemasPage.xaml
    /// </summary>
    public partial class SistemasPage : Page
    {
        public SistemasPage()
        {
            InitializeComponent();
        }

        private void BotaoInformacoesSistema_Click(object sender, RoutedEventArgs e)
        {
            SistemaInfo info = SistemaService.ObterInformacoes();
            PainelFerramentas.Visibility = Visibility.Collapsed;
            PainelInformacoesSistema.Visibility = Visibility.Visible;
            DataContext = info;
        }

        private void BotaoVoltar_Click(object sender, RoutedEventArgs e)
        {
            PainelInformacoesSistema.Visibility = Visibility.Collapsed;
            PainelFerramentas.Visibility = Visibility.Visible;
        }

        private void BotaoAtualizar_Click(object sender, RoutedEventArgs e)
        {
            DataContext = SistemaService.ObterInformacoes();
        }

        private void BotaoReiniciarExplorer_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                "Deseja reiniciar o Windows Explorer?\n\n" +
                "A barra de tarefas e a área de trabalho desaparecerão por alguns segundos.",
                "Reiniciar Explorer",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso = SistemaService.ReiniciarExplorer(out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "Sucesso" : "Erro",
                MessageBoxButton.OK,
                sucesso ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private void BotaoExecutarSfc_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                "Deseja executar a verificação de arquivos do Windows?\n\n" +
                "O processo pode demorar vários minutos e não deve ser interrompido.",
                "Executar SFC",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso = SistemaService.ExecutarSfc(out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "SFC iniciado" : "Erro",
                MessageBoxButton.OK,
                sucesso ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private void BotaoExecutarDism_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                "Deseja executar o reparo da imagem do Windows?\n\n" +
                "O processo pode demorar vários minutos e pode parecer parado em algumas porcentagens.",
                "Executar DISM",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso = SistemaService.ExecutarDism(out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "DISM iniciado" : "Erro",
                MessageBoxButton.OK,
                sucesso ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        private void BotaoInformacoesDiscos_Click(object sender, RoutedEventArgs e)
        {
            PainelFerramentas.Visibility = Visibility.Collapsed;
            PainelInformacoesSistema.Visibility = Visibility.Collapsed;
            PainelDiscos.Visibility = Visibility.Visible;

            ListaDiscos.ItemsSource = SistemaService.ObterDiscos();
        }

        private void BotaoAtualizarDiscos_Click(object sender, RoutedEventArgs e)
        {
            ListaDiscos.ItemsSource = SistemaService.ObterDiscos();
        }

        private void BotaoVoltarDiscos_Click(object sender, RoutedEventArgs e)
        {
            PainelDiscos.Visibility = Visibility.Collapsed;
            PainelFerramentas.Visibility = Visibility.Visible;
        }

        // --- MÉTODOS DO TERMINAL MAS OFFICE ---

        private async void BtnAtivarOffice_Click(object sender, RoutedEventArgs e)
        {
            // Transiciona a interface para a tela de log
            PainelFerramentas.Visibility = Visibility.Collapsed;
            PainelTerminalMas.Visibility = Visibility.Visible;

            TxtConsoleMas.Clear();
            TxtConsoleMas.AppendText("Iniciando ativação do Office (Ohook)...\n\n");

            // Executa a ativação de forma silenciosa e transmite as linhas de log para a UI
            await SistemaService.ExecutarMasComandoSilenciosoAsync("/Ohook", texto =>
            {
                Dispatcher.Invoke(() =>
                {
                    TxtConsoleMas.AppendText(texto);
                    TxtConsoleMas.ScrollToEnd();
                });
            });

            TxtConsoleMas.AppendText("\n--- Processo finalizado ---");
        }

        private void BotaoVoltarMas_Click(object sender, RoutedEventArgs e)
        {
            PainelTerminalMas.Visibility = Visibility.Collapsed;
            PainelFerramentas.Visibility = Visibility.Visible;
        }
    }
}