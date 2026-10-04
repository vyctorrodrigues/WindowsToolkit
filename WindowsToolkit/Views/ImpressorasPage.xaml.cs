using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Models;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    /// <summary>
    /// Interação lógica para ImpressorasPage.xam
    /// </summary>
    public partial class ImpressorasPage : Page
    {
        public ImpressorasPage()
        {
            InitializeComponent();
        }
    

        private void BotaoInformacoesImpressoras_Click(
            object sender,
            RoutedEventArgs e)

        {
            PainelFerramentas.Visibility = Visibility.Collapsed;
            PainelInformacoesImpressoras.Visibility = Visibility.Visible;

            CarregarImpressoras();
        }

        private void BotaoVoltar_Click(
            object sender,
            RoutedEventArgs e)
        {
            PainelInformacoesImpressoras.Visibility = Visibility.Collapsed;
            PainelFerramentas.Visibility = Visibility.Visible;
        }

        private void BotaoAtualizarInformacoes_Click(
            object sender,
            RoutedEventArgs e)
        {
            CarregarImpressoras();
        }

        private void CarregarImpressoras()
        {
            ListaImpressoras.ItemsSource = ImpressoraService.ObterImpressoras();
        }


        private void BotaoReiniciarSpooler_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (SpoolerService.ReiniciarSpooler(out string mensagem))
            {
                    MessageBox.Show(
                        mensagem,
                        "Sucesso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
            }
            else
            {
                    MessageBox.Show(
                        mensagem,
                        "Erro",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
            }
        }

        private void BotaoLimparFila_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult confirmacao = MessageBox.Show(
                "Deseja apagar todos os trabalhos pendentes da fila de impressão?",
                "Limpar fila de impressão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso =
                SpoolerService.LimparFilaImpressao(out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "Sucesso" : "Erro",
                MessageBoxButton.OK,
                sucesso
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Error);
        }

        private void BotaoImprimirTeste_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            string? nomeImpressora = botao.Tag?.ToString();

            if (string.IsNullOrWhiteSpace(nomeImpressora))
            {
                MessageBox.Show(
                    "Não foi possível identificar a impressora.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBoxResult confirmacao = MessageBox.Show(
                $"Deseja imprimir uma página de teste em:\n\n{nomeImpressora}?",
                "Imprimir página de teste",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
                return;

            bool sucesso = ImpressoraService.ImprimirPaginaTeste(
                nomeImpressora,
                out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "Sucesso" : "Erro",
                MessageBoxButton.OK,
                sucesso
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Error);
        }

        private void BotaoDefinirPadrao_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            string? nomeImpressora = botao.Tag?.ToString();

            if (string.IsNullOrWhiteSpace(nomeImpressora))
                return;

            MessageBoxResult resposta =
                MessageBox.Show(
                    $"Deseja definir\n\n{nomeImpressora}\n\ncomo impressora padrão?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (resposta != MessageBoxResult.Yes)
                return;

            bool sucesso =
                ImpressoraService.DefinirComoPadrao(
                    nomeImpressora,
                    out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "Sucesso" : "Erro",
                MessageBoxButton.OK,
                sucesso
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Error);

            if (sucesso)
            {
                CarregarImpressoras();
            }
        }

        private void BotaoCopiarInformacoes_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            if (botao.DataContext is not ImpressoraInfo impressora)
                return;

            string texto =
                $@"Nome: {impressora.Nome}
                Status: {impressora.Status}
                Driver: {impressora.Driver}
                Porta: {impressora.Porta}
                Padrão: {impressora.Padrao}
                Compartilhada: {impressora.Compartilhada}
                Compartilhamento: {impressora.NomeCompartilhamento}
                Localização: {impressora.Localizacao}";

            Clipboard.SetText(texto);

            MessageBox.Show(
                "Informações copiadas para a área de transferência.",
                "Copiado",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BotaoAbrirFila_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            string? nome = botao.Tag?.ToString();

            if (string.IsNullOrWhiteSpace(nome))
                return;

            ImpressoraService.AbrirFilaImpressao(
                nome,
                out string mensagem);

            MessageBox.Show(mensagem);
        }

        private void BotaoMapearLpt_Click(
            object sender,
            RoutedEventArgs e)
        {
            MapearLptWindow janela = new()
            {
                Owner = Window.GetWindow(this)
            };

            janela.ShowDialog();
        }

        private void BotaoAdicionarImpressora_Click(
            object sender,
            RoutedEventArgs e)
        {
            AdicionarImpressoraWindow janela = new()
            {
                Owner = Window.GetWindow(this)
            };

            janela.ShowDialog();

            ListaImpressoras.ItemsSource =
                ImpressoraService.ObterImpressoras();
        }

    }
}
