using System;
using System.Windows;
using System.Windows.Threading;
using WindowsToolkit.Views;

namespace WindowsToolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
   
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            IniciarControleInatividade();

            FramePrincipal.Navigate(new LimpezaPage());
        }
        private void BotaoLimpeza_Click(
            object sender, 
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new LimpezaPage());
        }
        private void BotaoRede_Click(
            object sender,
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new RedePage());
        }

        private void BotaoImpressoras_Click(
            object sender,
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new ImpressorasPage());
        }

        private void BotaoSistemas_Click(
            object sender,
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new SistemasPage());
        }

        private void BotaoSobre_Click(
            object sender,
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new SobrePage());
        }

        private void BotaoEscolherCor_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelecionarTemaWindow janela = new()
            {
                Owner = this
            };

            janela.ShowDialog();
        }

        private void AlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            AlterarSenhaWindow janela = new()
            {
                Owner = this
            };

            janela.ShowDialog();
        }

        private readonly DispatcherTimer _timerInatividade = new();

        private int _segundosRestantes = 60;

        private void IniciarControleInatividade()
        {
            _segundosRestantes = 60;

            _timerInatividade.Interval = TimeSpan.FromSeconds(1);
            _timerInatividade.Tick += TimerInatividade_Tick;
            _timerInatividade.Start();

            PreviewMouseDown += ReiniciarTimer;
            PreviewKeyDown += ReiniciarTimer;
        }

        private void ReiniciarTimer(object? sender, EventArgs e)
        {
            _segundosRestantes = 60;

            TempoInatividadeText.Text =
                $"Logout em {_segundosRestantes}s";
        }

        private void TimerInatividade_Tick(
            object? sender,
            EventArgs e)
        {
            _segundosRestantes--;

            TempoInatividadeText.Text =
                $"Logout em {_segundosRestantes}s";

            if (_segundosRestantes > 0)
                return;

            _timerInatividade.Stop();

            LoginWindow login = new();
            login.Show();

            Close();
        }

        private void BotaoOffice_Click(
            object sender,
            RoutedEventArgs e)
        {
            FramePrincipal.Navigate(new OfficePage());
        }
    }

}