using System.Windows;
using System.Windows.Input;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views;

public partial class AlterarSenhaWindow : Window
{
    public AlterarSenhaWindow()
    {
        InitializeComponent();
        SenhaAtualBox.Focus();
    }

    private void AlterarSenha_Click(
        object sender,
        RoutedEventArgs e)
    {
        string senhaAtual = SenhaAtualBox.Password;
        string novaSenha = NovaSenhaBox.Password;
        string confirmacao = ConfirmarSenhaBox.Password;

        if (string.IsNullOrWhiteSpace(senhaAtual) ||
            string.IsNullOrWhiteSpace(novaSenha) ||
            string.IsNullOrWhiteSpace(confirmacao))
        {
            MessageBox.Show(
                "Preencha todos os campos.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (novaSenha.Length < 6)
        {
            MessageBox.Show(
                "A nova senha deve possuir pelo menos 6 caracteres.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (novaSenha != confirmacao)
        {
            MessageBox.Show(
                "A confirmação não corresponde à nova senha.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            NovaSenhaBox.Clear();
            ConfirmarSenhaBox.Clear();
            NovaSenhaBox.Focus();

            return;
        }

        if (!LoginService.AlterarSenha(
                senhaAtual,
                novaSenha))
        {
            MessageBox.Show(
                "A senha atual está incorreta.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            SenhaAtualBox.Clear();
            SenhaAtualBox.Focus();

            return;
        }

        MessageBox.Show(
            "Senha alterada com sucesso.",
            "Windows Toolkit",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        Close();
    }

    private void ConfirmarSenhaBox_KeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AlterarSenha_Click(sender, e);
        }
    }
}