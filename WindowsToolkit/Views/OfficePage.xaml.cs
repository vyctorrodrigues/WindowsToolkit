using System.Windows;
using System.Windows.Controls;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views;

public partial class OfficePage : Page
{
    private readonly LicencaService _licencaService = new();

    public OfficePage()
    {
        InitializeComponent();
    }

    private async void VerificarLicenca_Click(
        object sender,
        RoutedEventArgs e)
    {
        VerificarLicencaButton.IsEnabled = false;
        ResultadoOfficeTextBox.Text = "Verificando licença do Office...";

        try
        {
            string resultado =
                await _licencaService.ObterStatusOfficeAsync();

            ResultadoOfficeTextBox.Text = resultado;
        }
        catch (Exception ex)
        {
            ResultadoOfficeTextBox.Text =
                $"Erro ao verificar licença:\n{ex.Message}";
        }
        finally
        {
            VerificarLicencaButton.IsEnabled = true;
        }
    }

    private void BtnAtivarOffice_Click(object sender, RoutedEventArgs e)
    {
        SistemaService.ExecutarMasOffice();
    }

    private void Limpar_Click(
        object sender,
        RoutedEventArgs e)
    {
        ResultadoOfficeTextBox.Clear();
    }
}