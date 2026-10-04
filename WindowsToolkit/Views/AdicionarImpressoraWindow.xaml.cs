using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views
{
    public partial class AdicionarImpressoraWindow : Window
    {
        public AdicionarImpressoraWindow()
        {
            InitializeComponent();

            CarregarDadosUsb();
        }

        private void CarregarDadosUsb()
        {
            ComboDrivers.ItemsSource =
                ImpressoraService.ObterDriversInstalados();

            ComboPortas.ItemsSource =
                ImpressoraService.ObterPortasImpressora();

            if (ComboDrivers.Items.Count > 0)
                ComboDrivers.SelectedIndex = 0;

            if (ComboPortas.Items.Count > 0)
                ComboPortas.SelectedIndex = 0;
        }

        private void OpcaoInstalacao_Checked(
            object sender,
            RoutedEventArgs e)
        {
            if (PainelRede == null || PainelUsb == null)
                return;

            bool rede =
                OpcaoRede.IsChecked == true;

            PainelRede.Visibility =
                rede
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            PainelUsb.Visibility =
                rede
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        private void BotaoAdicionar_Click(
         object sender,
         RoutedEventArgs e)
        {
            bool sucesso;
            string mensagem;

            if (OpcaoRede.IsChecked == true)
            {
                string caminho =
                    CampoCaminhoRede.Text.Trim();

                sucesso =
                    ImpressoraService.InstalarImpressoraRede(
                        caminho,
                        out mensagem);

                if (sucesso &&
                    OpcaoMapearLpt.IsChecked == true)
                {
                    string portaLpt =
                        ObterPortaLptSelecionada();

                    bool persistente =
                        OpcaoPersistente.IsChecked == true;

                    bool mapeamentoConcluido =
                        ImpressoraService.MapearLpt(
                            portaLpt,
                            caminho,
                            persistente,
                            out string mensagemLpt);

                    if (!mapeamentoConcluido)
                    {
                        sucesso = false;
                        mensagem =
                            "A impressora foi instalada, mas o mapeamento LPT falhou.\n\n" +
                            mensagemLpt;
                    }
                    else
                    {
                        mensagem +=
                            $"\n\nPorta {portaLpt} mapeada com sucesso.";
                    }
                }
            }
            else
            {
                string nome =
                    CampoNomeImpressora.Text.Trim();

                string driver =
                    ComboDrivers.SelectedItem?.ToString() ?? "";

                string porta =
                    ComboPortas.SelectedItem?.ToString() ?? "";

                sucesso =
                    ImpressoraService.AdicionarImpressoraLocal(
                        nome,
                        driver,
                        porta,
                        out mensagem);
            }

            MessageBox.Show(
                mensagem,
                sucesso ? "Operação concluída" : "Atenção",
                MessageBoxButton.OK,
                sucesso
                    ? MessageBoxImage.Information
                    : MessageBoxImage.Warning);

            if (sucesso)
                Close();
        }
        private string ObterPortaLptSelecionada()
        {
            if (ComboPortaLpt.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? "LPT1";
            }

            return "LPT1";
        }

        private void BotaoCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }

        private void OpcaoMapearLpt_Changed(
            object sender,
            RoutedEventArgs e)
        {
            if (PainelLpt == null)
                return;

            PainelLpt.Visibility =
                OpcaoMapearLpt.IsChecked == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}