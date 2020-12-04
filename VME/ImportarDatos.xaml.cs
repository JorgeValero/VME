using BarcodeLib.BarcodeReader;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;

namespace VME
{
    /// <summary>
    /// Interaction logic for ImportarDatos.xaml
    /// </summary>
    public partial class ImportarDatos : Window
    {
        KinectSensor miKinect;
        ConfiguracionInicial configuracionInicial;
        DispatcherTimer contador = new DispatcherTimer();
        public ImportarDatos(ConfiguracionInicial n)
        {
            InitializeComponent();
            configuracionInicial = n;
            contador.Interval = TimeSpan.FromMilliseconds(50);
            contador.Tick += contador_Tick;
            contador.Start();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            this.Title = RecursosLocalizables.StringResources.importarDatos;
            label.Content = RecursosLocalizables.StringResources.informacionImportarDatos;
        }


        private void contador_Tick(object sender, EventArgs e)
        {
            Bitmap bmpOut = null;

            using (MemoryStream ms = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imagen1.Source));
                encoder.Save(ms);

                using (Bitmap bmp = new Bitmap(ms))
                {
                    bmpOut = new Bitmap(bmp);
                }

                bmpOut.RotateFlip(RotateFlipType.Rotate180FlipY);
            }
            String[] resultados  = BarcodeReader.read(bmpOut, BarcodeReader.QRCODE);

            try
            {
                if (resultados[0].IndexOf("1111") != -1)
                    {
                    resultados[0] = resultados[0].Replace("1111", "");
                    
                    String nombre = funcionCortar(resultados[0], "[N]");
                    String apellidos = funcionCortar(resultados[0], "[A]");
                    String edad = funcionCortar(resultados[0], "[AN]");
                    String peso = funcionCortar(resultados[0], "[P]");
                    String pruebaReflejosAciertos = funcionCortar(resultados[0], "[PRA]");
                    String pruebaReflejosFallos = funcionCortar(resultados[0], "[PRF]");
                    String daltonico = funcionCortar(resultados[0], "[D]");
                    String movilidad = funcionCortar(resultados[0], "[M]");

                    Properties.Settings.Default.nombre = nombre;
                    Properties.Settings.Default.apellidos = apellidos;
                    Properties.Settings.Default.añoNacimiento = edad;
                    Properties.Settings.Default.peso = peso;
                    Properties.Settings.Default.pruebaReflejosAciertos = Int32.Parse(pruebaReflejosAciertos);
                    Properties.Settings.Default.pruebaReflejosFallos = Int32.Parse(pruebaReflejosFallos);
                    Properties.Settings.Default.Daltonico = Convert.ToBoolean(daltonico);
                    Properties.Settings.Default.tipoMovilidad = Int32.Parse(movilidad);


                    configuracionInicial.NombreBienvenida.Text = nombre;
                    configuracionInicial.ApellidosBienvenida.Text = apellidos;
                    configuracionInicial.NacimientoBienvenida.Text = edad;
                    configuracionInicial.PesoBienvenida.Text = peso;

                    if (Convert.ToBoolean(daltonico))
                    {
                        configuracionInicial.escribirCegueraBienvenida.Text = "1";
                        configuracionInicial.escribirCeguera2Bienvenida.Text = "1";
                    }
                    else {
                        configuracionInicial.escribirCegueraBienvenida.Text = "6";
                        configuracionInicial.escribirCeguera2Bienvenida.Text = "42";
                    }


                    if (movilidad == "4")
                    {
                        configuracionInicial.movilidad2Bienvenida.IsChecked = true;
                        Properties.Settings.Default.tipoMovilidad = 4;
                    }
                    else if (movilidad == "3")
                    {
                        configuracionInicial.movilidad3Bienvenida.IsChecked = true;
                        Properties.Settings.Default.tipoMovilidad = 3;
                    }
                    else if (movilidad == "2")
                    {
                        configuracionInicial.movilidad4Bienvenida.IsChecked = true;
                        Properties.Settings.Default.tipoMovilidad = 2;
                    }
                    else if (movilidad == "1")
                    {
                        configuracionInicial.movilidad5Bienvenida.IsChecked = true;
                        Properties.Settings.Default.tipoMovilidad = 1;
                    }


                    this.Close();
                }

            }
            catch (System.NullReferenceException) {
            }

        }

        public String funcionCortar(String resultado, String palabra) {

            String[] cortador = resultado.Split(new string[] { palabra }, StringSplitOptions.None);
            return cortador[1];

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {

                // Escogemos el primer sensor kinect que tengamos conectado. Puede haber más de un kinect conectado
                miKinect = KinectSensor.KinectSensors[0];
                // Habilitamos la cámara elegiendo el formato de imagen.
                miKinect.ColorStream.Enable();
                // Arrancamos Kinect.
                miKinect.Start();
                // Nos suscribimos al método
                miKinect.AllFramesReady += KinectAllFramesReady;


            }
        }

        void KinectAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //Obtenemos el frame de imagen de la camara
            using (var colorFrame = e.OpenColorImageFrame())
            {
                // Si este es null no continuamos
                if (colorFrame == null) return;
                // Creamos un array de bytes del tamaño de los pixel del frame.
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                //Copiamos los pixel del frame a nuestro array de bytes.
                colorFrame.CopyPixelDataTo(pixels);
                // Colocamos los pixel del frame en la imagen del xml
                int stride = colorFrame.Width * 4;
                imagen1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                //imageKinect es el objeto imagen colocado en el xaml
            }
        }

        /*
         *  Private Function funcionCortarRTB1(palabra As String) As String
        Dim cortador() As String
        cortador = Split(RichTextBox1.Text, palabra)
        Return cortador(1)
    End Function



String textExport = "[N]" + Properties.Settings.Default.nombre + "[N]" +
                "[A]" + Properties.Settings.Default.apellidos + "[A]" +
                "[AN]" + Properties.Settings.Default.añoNacimiento + "[AN]" +
                "[P]" + Properties.Settings.Default.peso + "[P]" +
                "[PRA]" + Properties.Settings.Default.pruebaReflejosAciertos + "[PRA]" +
                "[PRF]" + Properties.Settings.Default.pruebaReflejosFallos + "[PRF]" +
                "[D]" + Properties.Settings.Default.Daltonico + "[D]" +
                "[M]" + Properties.Settings.Default.tipoMovilidad + "[M]";
         * */


    }
}
