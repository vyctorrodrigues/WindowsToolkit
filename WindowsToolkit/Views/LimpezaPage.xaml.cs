using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsToolkit.Services;
using WindowsToolkit.Views;

namespace WindowsToolkit.Views
{
    /// <summary>
    /// Interação lógica para LimpezaPage.xam
    /// </summary>
    public partial class LimpezaPage : Page
    {
        public LimpezaPage()
        {
            InitializeComponent();
        }

        private void BotaoLimparTemp_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                LimpezaService.LimparTemp();

                MessageBox.Show(
                    "Arquivos temporários foram removidos da pasta",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro durante a limpeza: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void BotaoLixeira_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resposta = MessageBox.Show(
                "Deseja esvaziar a lixeira? Os arquivos não poderão ser recuperados.",
                "Confirmar limpeza",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resposta != MessageBoxResult.Yes)
            {
                return;
            }


            bool sucesso = LimpezaService.EsvaziarLixeira();


            MessageBox.Show(
                sucesso
                    ? "Lixeira esvaziada com sucesso."
                    : "Erro ao esvaziar a lixeira.",
                "Windows Toolkit",
                MessageBoxButton.OK,
                sucesso ? MessageBoxImage.Information : MessageBoxImage.Error
             );
        }

        private void BotaoLimparExecutar_Click(
            object sender, RoutedEventArgs e)
        {
            try
            {
                int itensRemovidos = LimpezaService.LimparHistoricoExecutar();

                MessageBox.Show(
                    $"{itensRemovidos} entradas do histórico do Executar foram removidas.",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao limpar o histórico: {ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void BotaoLimpezaCompleta_Click(
            object sender, RoutedEventArgs e)
        {
            MessageBoxResult resposta = MessageBox.Show(
                "A limpeza completa vai remover arquivos temporários, " +
                "Limpar o histórico do Executar e esvaziar a lixeira.\n\n" +
                "Deseja continuar?",
                "Confirmar limpeza completa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resposta != MessageBoxResult.Yes)
            {
                return;
            }

            BotaoLimpezaCompleta.IsEnabled = false;

            BarraProgressoLimpeza.Visibility = Visibility.Visible;
            BarraProgressoLimpeza.Value = 0;

            TextoStatusLimpeza.Text = "Iniciando limpeza completa...";

            try
            {
                //Etapa 1: Limpar arquivos temporários
                TextoStatusLimpeza.Text = "Limpando arquivos temporários...";
                BarraProgressoLimpeza.Value = 10;

                await Task.Run(() =>
                {
                    LimpezaService.LimparTemp();
                });

                BarraProgressoLimpeza.Value = 40;

                //Etapa 2: Limpar histórico do Executar

                TextoStatusLimpeza.Text = "Limpando histórico do Executar...";

                int historicosRemovidos = await Task.Run(() =>
                {
                    return LimpezaService.LimparHistoricoExecutar();
                });

                BarraProgressoLimpeza.Value = 70;

                //Etapa 3: Esvaziar a lixeira

                TextoStatusLimpeza.Text = "Esvaziando a lixeira...";

                bool lixeiraEsvaziada = await Task.Run(() =>
                {
                    return LimpezaService.EsvaziarLixeira();
                });

                BarraProgressoLimpeza.Value = 100;
                TextoStatusLimpeza.Text = "Limpeza completa finalizada.";

                string resultadoLixeira = lixeiraEsvaziada
                    ? "Lixeira esvaziada com sucesso."
                    : "Erro ao esvaziar a lixeira.";

                MessageBox.Show(
                    $"Limpeza completa concluída.\n\n" +
                    $"Arquivos temporários: limpeza executada.\n" +
                    $"Historicos removidos: {historicosRemovidos}.\n" +
                    $"{resultadoLixeira}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    lixeiraEsvaziada
                        ? MessageBoxImage.Information
                        : MessageBoxImage.Warning
                );
            }
            catch (Exception ex)
            {
                TextoStatusLimpeza.Text = "Erro durante a limpeza completa.";

                MessageBox.Show(
                    $"Erro durante a limpeza completa: {ex.Message}",
                    "Windows Toolkit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                BotaoLimpezaCompleta.IsEnabled = true;

                await Task.Delay(1500);

                BarraProgressoLimpeza.Value = 0;
                BarraProgressoLimpeza.Visibility = Visibility.Collapsed;
                TextoStatusLimpeza.Text = "";
            }

        }
    }
}
