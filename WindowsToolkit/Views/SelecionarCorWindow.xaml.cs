using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsToolkit.Views
{
    public partial class SelecionarCorWindow : Window
    {
        public SelecionarCorWindow()
        {
            InitializeComponent();
        }

        private void BotaoCor_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button botao)
                return;

            string codigoCor =
                botao.Tag?.ToString() ?? "#00BCD4";

            Color cor =
                (Color)ColorConverter.ConvertFromString(codigoCor);

            Application.Current.Resources["CorDestaque"] =
                new SolidColorBrush(cor);

            DialogResult = true;
            Close();
        }

        private void BotaoCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}