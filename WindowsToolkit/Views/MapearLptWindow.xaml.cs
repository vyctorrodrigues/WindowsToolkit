using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    public partial class MapearLptWindow : Window
    {
        public MapearLptWindow()
        {
            InitializeComponent();
        }

        private void BotaoMapear_Click(
            object sender,
            RoutedEventArgs e)
        {
            string porta = ObterPortaSelecionada();
            string caminho = CampoCaminhoImpressora.Text.Trim();

            if (string.IsNullOrWhiteSpace(caminho))
            {
                MessageBox.Show(
                    "Informe o caminho da impressora compartilhada.",
                    "Campo obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!caminho.StartsWith(@"\\"))
            {
                MessageBox.Show(
                    @"O caminho deve começar com \\." +
                    "\n\nExemplo: \\\\SERVIDOR\\IMPRESSORA",
                    "Caminho inválido",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool sucesso = ImpressoraService.MapearLpt(
                porta,
                caminho,
                true,
                out string mensagem);

            MessageBox.Show(
                mensagem,
                sucesso ? "Mapeamento concluído" : "Erro",
                MessageBoxButton.OK,
                sucesso
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Error);

            if (sucesso)
                Close();
        }

        private string ObterPortaSelecionada()
        {
            if (ComboPorta.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? "LPT1";

            return "LPT1";
        }

        private void BotaoCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}