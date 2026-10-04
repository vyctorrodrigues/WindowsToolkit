
using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    /// <summary>
    /// Interação lógica para RedePage.xam
    /// </summary>
    public partial class RedePage : Page
    {
        public RedePage()
        {
            InitializeComponent();
        }

        private async void BotaoLimparDns_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                ResultadoComando resultado = await Task.Run(() =>
                    RedeService.LimparDns()
                );

                if (resultado.Sucesso)
                {
                    MessageBox.Show(
                        "Cache DNS limpo com sucesso.\n\n" +
                        "O cache de resolução de nomes foi apagado.",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Não foi possivel limpar o cache DNS. \n\n" +
                        $"Código: {resultado.CodigoSaida}\n" +
                        $"Erro: {resultado.Erro}",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro inesperado: {ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void BotaoRenovarConexao_Click(
                object sender,
                RoutedEventArgs e)
        {
            try
            {
                ResultadoComando resultado = await Task.Run(() =>
                    RedeService.RenovarConexao()
                );

                if (resultado.Sucesso)
                {
                    MessageBox.Show(
                        "Conexão de rede renovada com sucesso.\n\n" +
                        "O endereço IP foi liberado e renovado.",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"Não foi possível renovar a conexão.\n\n" +
                        $"Código: {resultado.CodigoSaida}\n" +
                        $"Erro: {resultado.Erro}",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro inesperado: {ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void BotaoReiniciarWinsock_Click(
                object sender,
                RoutedEventArgs e)
        {
            MessageBoxResult resposta = MessageBox.Show(
                "O Winsock será redefinido.\n\n" +
                "Pode ser necessário reiniciar o computador após a conclusão.\n\n" +
                "Deseja continuar?",
                "Windows Toolkit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resposta != MessageBoxResult.Yes)
                return;

            try
            {
                ResultadoComando resultado = await Task.Run(() =>
                    RedeService.ReiniciarWinSock());

                if (resultado.Sucesso)
                {
                    MessageBox.Show(
                        "Winsock redefinido com sucesso.\n\nReinicie o computador para concluir a operação.",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"Não foi possível redefinir o Winsock.\n\n" +
                        $"Código: {resultado.CodigoSaida}\n" +
                        $"Erro: {resultado.Erro}",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro inesperado: {ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void BotaoInformacoesRede_Click(
          object sender,
          RoutedEventArgs e)
        {
            PainelFerramentas.Visibility = Visibility.Collapsed;
            PainelInformacoesRede.Visibility = Visibility.Visible;

            await CarregarInformacoesRede();
        }

        private void BotaoVoltar_Click(
            object sender,
            RoutedEventArgs e)
        {
            PainelInformacoesRede.Visibility = Visibility.Collapsed;
            PainelFerramentas.Visibility = Visibility.Visible;
        }

        private async void BotaoAtualizarInformacoes_Click(
            object sender,
            RoutedEventArgs e)
        {
            await CarregarInformacoesRede();
        }

        private async Task CarregarInformacoesRede()
        {
            ListaAdaptadoresRede.ItemsSource = null;

            try
            {
                var adaptadores = await Task.Run(() =>
                    RedeService.ObterAdaptadoresRede());

                ListaAdaptadoresRede.ItemsSource = adaptadores;

                if (adaptadores.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhum adaptador de rede foi encontrado.",
                        "Windows Toolkit",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível obter as informações da rede.\n\n{ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private static void ExibirErro(
            ResultadoComando resultado)
        {
            MessageBox.Show(
                "Não foi possível concluir a operação.\n\n" +
                $"Código de saída: {resultado.CodigoSaida}\n" +
                $"Erro: {resultado.Erro}",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Error);


        }
    }
}
