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
    /// Interaction logic for Opciones.xaml
    /// </summary>
    public partial class Opciones : Window
    {
        MainWindow menuPrincipal;
        public Opciones(MainWindow mPrincipal)
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
            Title = RecursosLocalizables.StringResources.VMEInformacionTitulo;


        }

        public void AplicarTamanoLetra(int tamano)
        {
            cerrar.FontSize = cerrar.FontSize + tamano;
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
            //mensajeVentanaError.Foreground = color2;

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

        private void priIdioma_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }

        private void priTipoInterfaz_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
        }

        private void priTamanoLetra_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 3;
        }

        private void priDatosUsuario_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 4;
        }

        private void priRestablecerDatos_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 7;
        }

        private void movilidad_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 5;
        }

        private void regularKinect_Selected(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 6;
        }

        private void banderaInglesa_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void banderaEspanola_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void letraPequeña_Click(object sender, RoutedEventArgs e)
        {

        }

        private void resetearAplicacion_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.idioma = "es-Es";
            Properties.Settings.Default.configuracionInicial = false;
            Properties.Settings.Default.nombre = "";
            Properties.Settings.Default.apellidos = "";
            Properties.Settings.Default.añoNacimiento = "";
            Properties.Settings.Default.peso = "";
            Properties.Settings.Default.Daltonico = false;
            Properties.Settings.Default.tamanoLetra = "M";
            Properties.Settings.Default.tipoInterfaz = "Oscuro";
            Properties.Settings.Default.tipoMovilidad = 0;
            Properties.Settings.Default.pruebaReflejosAciertos = 0;
            Properties.Settings.Default.pruebaReflejosFallos = 0;
        }
    }

    
}
