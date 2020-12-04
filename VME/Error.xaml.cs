using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
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

using System.Windows.Navigation;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace VME
{
    /// <summary>
    /// Lógica de interacción para Error.xaml
    /// </summary>
    public partial class Error : Window
    {
        SpeechRecognitionEngine speechengine;
        KinectSensor sensor;
        Boolean desactivarMicrofono = true;
        //mensaje -> es el mensaje que queremos mostrar en pantalla
        //tipoMensaje -> si se trata de un error (0), una advertencia (1), una información(2) 
        public Error(String mensaje, int tipoMensaje)
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma(tipoMensaje);

            AplicarEstilo();

            if (Properties.Settings.Default.tamanoLetra == "P")
                AplicarTamanoLetra(-3);
            else if (Properties.Settings.Default.tamanoLetra == "G")
                AplicarTamanoLetra(3);
            else
                AplicarTamanoLetra(0);

            mensajeVentanaError.Text = mensaje;
            if (tipoMensaje == 0)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("Resources/iconoError.png", UriKind.Relative);
                imagenVentanaError.Source = bi3;
                bi3.EndInit();
                SystemSounds.Exclamation.Play();
            }
            else if (tipoMensaje == 1)
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("Resources/iconoAdvertencia.png", UriKind.Relative);
                imagenVentanaError.Source = bi3;
                bi3.EndInit();
                SystemSounds.Exclamation.Play();
            }
            else {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("Resources/iconoInformacion.png", UriKind.Relative);
                imagenVentanaError.Source = bi3;
                bi3.EndInit();
                SystemSounds.Beep.Play();
            } 
        }

        public void AplicarIdioma(int tipoMensaje) {
            aceptarError.Content = RecursosLocalizables.StringResources.aceptar;
                if (tipoMensaje == 0)
                {
                    Title = RecursosLocalizables.StringResources.VMEErrorTitulo;
                }
                else if (tipoMensaje == 1)
                {
                Title = RecursosLocalizables.StringResources.VMESugerenciaTitulo;
                }
                else
                {
                this.Title = RecursosLocalizables.StringResources.VMEInformacionTitulo;
                }

        }

        public void AplicarTamanoLetra(int tamano) {
            mensajeVentanaError.FontSize = mensajeVentanaError.FontSize + tamano;
            aceptarError.FontSize = aceptarError.FontSize + tamano;
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

            this.gridError.Background = color1;
            mensajeVentanaError.Foreground = color2;

        }


        private void aceptarError_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

                            opciones.Add(RecursosLocalizables.StringResources.aceptar, "UNO");
                           
                            //En esta linea "windows ocho" es el valor de opcion y "TRES" es la llave y asi sucesivamente
                            opciones.Add(new SemanticResultValue("windows", "TRES"));
                            opciones.Add(new SemanticResultValue("new windows", "TRES"));

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
        }
        void speechengine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //la variable igualdad sera el porcentaje de igualdad entre la palabra reconocida y el valor de opcion
            //es decir si yo digo "uno" y el valor de opcion es "uno" la igualdad sera mayor al 50 %
            //Si yo digo "jugo" y el valor de opcion es "uno" notaras que el sonido es muy similar pero quizas no mayor al 50 %
            //El valor de porcentaje va de 0.0  a 1.0, ademas notaras que le di un valos de .5 lo cual representa el 50% de igualdad
            const double igualdad = 0.5;

            //Si hay mas del 50% de igualdad con alguna de nuestras opciones
            if (e.Result.Confidence > igualdad )
            {
                Uri src;
                BitmapImage img;
                //haremos un switch para aquellos valores que se componen de unicamente una palabra

                if (e.Result.Text.Equals(RecursosLocalizables.StringResources.aceptar))
                {
                    this.Close();
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

       
        private void button_Click(object sender, RoutedEventArgs e)
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
    }
}
