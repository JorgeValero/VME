using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VME
{
    /// <summary>
    /// Lógica de interacción para AcercaDe.xaml
    /// </summary>
    public partial class AcercaDe : Window
    {
        MainWindow menuPrincipal;
        public AcercaDe(MainWindow mPrincipal)
        {
            InitializeComponent();
            menuPrincipal = mPrincipal;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma();
            AplicarEstilo();

            if (Properties.Settings.Default.tamanoLetra == "P")
                AplicarTamanoLetra(-3);
            else if (Properties.Settings.Default.tamanoLetra == "G")
                AplicarTamanoLetra(3);
            else
                AplicarTamanoLetra(0);
        }

        public void AplicarIdioma()
        {
            cerrar.Content = RecursosLocalizables.StringResources.cerrar;
            Title = RecursosLocalizables.StringResources.acercaDe;
            nameProduct.Content = RecursosLocalizables.StringResources.nombrePrograma + " VME - Virtual Medical Examination";
            version.Content = RecursosLocalizables.StringResources.version + " 0.2 Alfa";
            nameCompany.Content = RecursosLocalizables.StringResources.nombreCompania + " UCLM 2016/2017";
            authors.Content = RecursosLocalizables.StringResources.autores + " Alejandro Moya Moya y Jorge Valero Molina";

        }

        public void AplicarTamanoLetra(int tamano)
        {
            cerrar.FontSize = cerrar.FontSize + tamano;
            nameProduct.FontSize = nameProduct.FontSize + tamano;
            version.FontSize = version.FontSize + tamano;
            authors.FontSize = authors.FontSize + tamano;
            copyright.FontSize = copyright.FontSize + tamano;
            nameCompany.FontSize = nameCompany.FontSize + tamano;
        }

        public void AplicarEstilo()
        {
            //Inicializamos los colores a la interfaz oscura, por si acaso hay un problema en la configuracion y no entra en ningun if, crear tantos colores como sean necesarios
            SolidColorBrush color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
            SolidColorBrush color2 = new SolidColorBrush(Colors.White); //Blanco

            if (Properties.Settings.Default.tipoInterfaz == "Oscuro")
            {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
                color2 = new SolidColorBrush(Colors.White); //Blanco

            }
            else if (Properties.Settings.Default.tipoInterfaz == "Claro")
            {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B7AAAA")); // -- No se que color es --
                color2 = new SolidColorBrush(Colors.Black); //Negro

            }

            this.grid1.Background = color1;
            nameProduct.Foreground = color2;
            version.Foreground = color2;
            authors.Foreground = color2;
            copyright.Foreground = color2;
            nameCompany.Foreground = color2;

        }

        private void aceptarError_Click(object sender, RoutedEventArgs e)
        {
            menuPrincipal.desactivarMicrofono = false;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            menuPrincipal.desactivarMicrofono = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
