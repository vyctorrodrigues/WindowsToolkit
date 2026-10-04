using System.Windows;
using System.Windows.Input;
using WindowsToolkit.Services;

namespace WindowsToolkit.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        SenhaBox.Focus();
    }

    private void Entrar_Click(object sender, RoutedEventArgs e)
    {
        if (LoginService.ValidarSenha(SenhaBox.Password))
        {
            MainWindow tela = new();
            tela.Show();

            Close();
        }
        else
        {
            MessageBox.Show(
                "Senha incorreta.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            SenhaBox.Clear();
            SenhaBox.Focus();
        }
    }

    private void SenhaBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Entrar_Click(sender, e);
        }
    }
}