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
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.Win32;
using Microsoft.Speech.Recognition;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;

namespace VME
{
    /// <summary>
    /// Interaction logic for InformacionUsuario.xaml
    /// </summary>
    public partial class InformacionUsuario : Window
    {
        MainWindow menuPrincipal;
        Boolean desactivarMicrofono = true;
        SpeechRecognitionEngine speechengine;
        private KinectSensor sensor;
        public InformacionUsuario(MainWindow mPrincipal)
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


            String textExport = "1111" + "[N]" + Properties.Settings.Default.nombre + "[N]" +
                "[A]" + Properties.Settings.Default.apellidos + "[A]" +
                "[AN]" + Properties.Settings.Default.añoNacimiento + "[AN]" +
                "[P]" + Properties.Settings.Default.peso + "[P]" +
                "[PRA]" + Properties.Settings.Default.pruebaReflejosAciertos + "[PRA]" +
                "[PRF]" + Properties.Settings.Default.pruebaReflejosFallos + "[PRF]" +
                "[D]" + Properties.Settings.Default.Daltonico + "[D]" +
                "[M]" + Properties.Settings.Default.tipoMovilidad + "[M]";
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(textExport, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
            MemoryStream ms = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            var imageTemporal = new Bitmap(ms);
            var image = new Bitmap(imageTemporal, new System.Drawing.Size(new System.Drawing.Point(200, 200)));
            System.Windows.Media.Imaging.BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(image.Width, image.Height));
            codigoQR.Source = b;


        }

        public void AplicarIdioma()
        {
            cerrar.Content = RecursosLocalizables.StringResources.cerrar;
            Title = RecursosLocalizables.StringResources.VMEInformacionUsuarioTitulo;
            nombreUsuario.Content = RecursosLocalizables.StringResources.nombreBienvenida + " " + Properties.Settings.Default.nombre;
            apellidosUsuario.Content = RecursosLocalizables.StringResources.apellidosBienvenida + " " + Properties.Settings.Default.apellidos;
            anoUsuario.Content = RecursosLocalizables.StringResources.anoDeNacimientoBienvenida + " " + Properties.Settings.Default.añoNacimiento;
            pesoUsuario.Content = RecursosLocalizables.StringResources.pesoBienvenida + " " + Properties.Settings.Default.peso;
            labelTitulo.Content = RecursosLocalizables.StringResources.VMEInformacionUsuarioTitulo;
            labelPruebas.Content = RecursosLocalizables.StringResources.labelPruebas;
            pruebaReflejos.Content = RecursosLocalizables.StringResources.VMEPruebaReflejosTitulo + ":";
            pruebaReflejosAciertos.Content = RecursosLocalizables.StringResources.Aciertos + " " + Properties.Settings.Default.pruebaReflejosAciertos;
            pruebaReflejosFallos.Content = RecursosLocalizables.StringResources.Fallos + " " + Properties.Settings.Default.pruebaReflejosFallos;
            exportarDatos.Content = RecursosLocalizables.StringResources.exportarDatos;
        }

        public void AplicarTamanoLetra(int tamano)
        {
            cerrar.FontSize = cerrar.FontSize + tamano;
            nombreUsuario.FontSize = nombreUsuario.FontSize + tamano;
            apellidosUsuario.FontSize = apellidosUsuario.FontSize + tamano;
            anoUsuario.FontSize = anoUsuario.FontSize + tamano;
            pesoUsuario.FontSize = pesoUsuario.FontSize + tamano;
            labelTitulo.FontSize = labelTitulo.FontSize + tamano;
            labelPruebas.FontSize = labelPruebas.FontSize + tamano;
            pruebaReflejos.FontSize = pruebaReflejos.FontSize + tamano;
            pruebaReflejosAciertos.FontSize = pruebaReflejosAciertos.FontSize + tamano;
            pruebaReflejosFallos.FontSize = pruebaReflejosFallos.FontSize + tamano;
            exportarDatos.FontSize = exportarDatos.FontSize + tamano;
        }

        public void AplicarEstilo()
        {
            //Inicializamos los colores a la interfaz oscura, por si acaso hay un problema en la configuracion y no entra en ningun if, crear tantos colores como sean necesarios
            SolidColorBrush color1 = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
            SolidColorBrush color2 = new SolidColorBrush(Colors.White); //Blanco

            if (Properties.Settings.Default.tipoInterfaz == "Oscuro")
            {
                color1 = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
                color2 = new SolidColorBrush(Colors.White); //Blanco

            }
            else if (Properties.Settings.Default.tipoInterfaz == "Claro")
            {
                color1 = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#B7AAAA")); // -- No se que color es --
                color2 = new SolidColorBrush(Colors.Black); //Negro

            }

            this.grid1.Background = color1;
            nombreUsuario.Foreground = color2;
            apellidosUsuario.Foreground = color2;
            anoUsuario.Foreground = color2;
            pesoUsuario.Foreground = color2;
            labelTitulo.Foreground = color2;
            labelPruebas.Foreground = color2;
            pruebaReflejos.Foreground = color2;
            pruebaReflejosAciertos.Foreground = color2;
            pruebaReflejosFallos.Foreground = color2;
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

        private void exportarDatos_Click(object sender, RoutedEventArgs e)
        {
            String textExport = "1111" + "[N]" + Properties.Settings.Default.nombre + "[N]" +
                "[A]" + Properties.Settings.Default.apellidos + "[A]" +
                "[AN]" + Properties.Settings.Default.añoNacimiento + "[AN]" +
                "[P]" + Properties.Settings.Default.peso + "[P]" +
                "[PRA]" + Properties.Settings.Default.pruebaReflejosAciertos + "[PRA]" +
                "[PRF]" + Properties.Settings.Default.pruebaReflejosFallos + "[PRF]" +
                "[D]" + Properties.Settings.Default.Daltonico + "[D]" +
                "[M]" + Properties.Settings.Default.tipoMovilidad + "[M]";
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(textExport, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
            MemoryStream ms = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
            var imageTemporal = new Bitmap(ms);
            var image = new Bitmap(imageTemporal, new System.Drawing.Size(new System.Drawing.Point(600, 600)));
            SaveFileDialog sv = new SaveFileDialog();
            sv.Filter = "Image PNG (*.png)|*.png";
            sv.ShowDialog();
            if (!string.IsNullOrEmpty(sv.FileName))
            {
                image.Save(sv.FileName, ImageFormat.Png);
            }
            image.Dispose();
            
        }

        private void microfono_click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();

                if (desactivarMicrofono == true)
                {
                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Resources/iconoMicrofono.png", UriKind.Relative);
                    bi3.EndInit();
                    imagenMicrofono.Source = bi3;
                    desactivarMicrofono = false;
                    conectaActiva();
                }
                else
                {
                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Resources/iconoMicrofonoDesactivado.png", UriKind.Relative);
                    bi3.EndInit();
                    imagenMicrofono.Source = bi3;
                    desactivarMicrofono = true;
                }
        }

        void conectaActiva()
        {
            //Nos aseguramos que la cuenta de sensores conectados sea de al menos 1
            if (KinectSensor.KinectSensors.Count > 0)
            {
                //Checamos que la variable _sensor sea nula
                if (this.sensor == null)
                {
                    //Asignamos el primer sensor Kinect a nuestra variable
                    this.sensor = KinectSensor.KinectSensors[0];
                }
                if (this.sensor != null)
                {
                    try
                    {
                        //Iniciamos el dispositivo Kinect
                        this.sensor.Start();
                        //Esto es opcional pero ayuda a colocar el dispositivo Kinect a un cierto angulo de inclinacion, desde -27 a 27
                        //   sensor.ElevationAngle = 3;
                        //Informamos que se ha conectado e inicializado correctamente el dispositivo Kinect
                        //  Error err = new VME.Error(RecursosLocalizables.StringResources.KinectDetect, 3);
                        // err.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error");
                    }

                    //Creamos esta variable ri que tratara de encontrar un language pack valido haciendo uso del metodo obtenerLP
                    RecognizerInfo ri = obtenerLP();

                    //Si se encontro el language pack requerido lo asignaremos a nuestra variable speechengine
                    if (ri != null)
                    {
                        this.speechengine = new SpeechRecognitionEngine(ri.Id);
                        //Creamos esta variable opciones la cual almacenara las opciones de palabras o frases que podran ser reconocidas por el dispositivo
                        Choices opciones = new Choices();
                        //Comenzamos a agregar las opciones comenzando por el valor de opcion que tratamos reconocer y una llave que identificara a ese valor
                        //Por ejemplo en esta linea "uno" es el valor de opcion y "UNO" es la llave

                        opciones.Add(RecursosLocalizables.StringResources.cerrar, "UNO");
                        //En esta linea "dos" es el valor de opcion y "DOS" es la llave
                        opciones.Add(RecursosLocalizables.StringResources.exportarDatos, "DOS");
                        //En esta linea "windows ocho" es el valor de opcion y "TRES" es la llave y asi sucesivamente


                        //Esta variable creará todo el conjunto de frases y palabras en base a nuestro lenguaje elegido en la variable ri
                        var grammarb = new GrammarBuilder { Culture = ri.Culture };
                        //Agregamos las opciones de palabras y frases a grammarb
                        grammarb.Append(opciones);
                        //Creamos una variable de tipo Grammar utilizando como parametro a grammarb
                        var grammar = new Grammar(grammarb);
                        //Le decimos a nuestra variable speechengine que cargue a grammar
                        this.speechengine.LoadGrammar(grammar);
                        //mandamos llamar al evento SpeechRecognized el cual se ejecutara cada vez que una palabra sea detectada
                        speechengine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(speechengine_SpeechRecognized);
                        //speechengine inicia la entrada de datos de tipo audio
                        speechengine.SetInputToAudioStream(sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                        speechengine.RecognizeAsync(RecognizeMode.Multiple);

                    }
                }
            }

        }

        void speechengine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //la variable igualdad sera el porcentaje de igualdad entre la palabra reconocida y el valor de opcion
            //es decir si yo digo "uno" y el valor de opcion es "uno" la igualdad sera mayor al 50 %
            //Si yo digo "jugo" y el valor de opcion es "uno" notaras que el sonido es muy similar pero quizas no mayor al 50 %
            //El valor de porcentaje va de 0.0  a 1.0, ademas notaras que le di un valos de .5 lo cual representa el 50% de igualdad
            const double igualdad = 0.5;

            //Si hay mas del 50% de igualdad con alguna de nuestras opciones
            if (e.Result.Confidence > igualdad && desactivarMicrofono == false)
            {
                Uri src;
                BitmapImage img;
                //haremos un switch para aquellos valores que se componen de unicamente una palabra
                if (e.Result.Text.Equals(RecursosLocalizables.StringResources.cerrar))
                {
                    menuPrincipal.desactivarMicrofono = false;
                    this.Close();
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.exportarDatos))
                {
                    exportarDatos_Click(null, null);
                }


            }
        }
        private static RecognizerInfo obtenerLP()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);

                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && Properties.Settings.Default.idioma.Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            MessageBox.Show(RecursosLocalizables.StringResources.ReconocimientoNoRecon);
            return null;
        }
    }
}
