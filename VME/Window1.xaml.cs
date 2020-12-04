using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Kinect;

namespace VME
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        KinectSensor miKinect;
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //Evento ejecutado al cerrar
                Closing += Window_Closing;
                // Escogemos el primer sensor kinect que tengamos conectado. Puede haber más de un kinect conectado
                miKinect = KinectSensor.KinectSensors[0];
                // Habilitamos la cámara elegiendo el formato de imagen.
                miKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                // Arrancamos Kinect.
                miKinect.Start();
                // Nos suscribimos al método
                miKinect.AllFramesReady += KinectAllFramesReady;

                estado.Text = "Activado correctamente";

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Al cerrar paramos Kinect
            //if (miKinect != null)
            //{
             //   miKinect.Stop();
            //}
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

        private void button_Click(object sender, RoutedEventArgs e)
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
