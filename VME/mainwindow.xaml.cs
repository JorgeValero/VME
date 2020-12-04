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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Threading;
using System.Globalization;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace VME
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine speechengine;
        KinectSensor sensor;
        KinectSensorChooser sensorChooser = new KinectSensorChooser();
        int estadoPrueba = 0; //Estado inicial, 1 si se ha seleccionado pruebas físicas, 2 si se ha seleccionado pruebas psicologicas.
        ConfiguracionInicial verVentana=null;
        public Boolean desactivarMicrofono = false;
        Boolean desactivarMicrofonoSiempre = true;
        public MainWindow()
        {
            InitializeComponent();

            // Cogemos el idioma almacenado en la configuracion y la aplicamos
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma();
        }

        // Metodo que cambia el idioma de la ventana
        public void AplicarIdioma()
        {
            fisicoCirculoMenuPrincipal.Label = RecursosLocalizables.StringResources.fisicoCirculoMenuPrincipal;
            psicologicoCirculoMenuPrincipal.Label = RecursosLocalizables.StringResources.psicologicoCirculoMenuPrincipal;

        }

        // Metodo que cambia el tamaño de la letra de la ventana
        public void AplicarTamanoLetra(int tamano) {

        }

        // Metodo que cambia el estilo de la ventana
        public void AplicarEstilo(){
            //Inicializamos los colores a la interfaz oscura, por si acaso hay un problema en la configuracion y no entra en ningun if, crear tantos colores como sean necesarios
            SolidColorBrush color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro

            if (Properties.Settings.Default.tipoInterfaz == "Oscuro")
            {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro

            }
            else if (Properties.Settings.Default.tipoInterfaz == "Claro")
            {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B7AAAA")); // -- No se que color es --

            }

            this.gridMain.Background = color1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Iniciar la Kinect (objeto, evento de cambio e iniciación)
            this.sensorChooser.KinectChanged += SensorChooser_KinectChanged;
            this.sensorChooser.Start();

            // Iniciar la ventana de configuracion inicial si no se ve que se haya realizado antes
            if (Properties.Settings.Default.configuracionInicial == false) {
                this.sensorChooser.Stop();
                verVentana = new ConfiguracionInicial(this);
                verVentana.Show();
                this.Hide();
               
            }

            

        }
        
        public void metodo()
        {
            this.Show();
        }

        void SensorChooser_KinectChanged(object sender, KinectChangedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            if (e.OldSensor == null)
            {
                try
                {
                    e.OldSensor.DepthStream.Disable();
                    e.OldSensor.SkeletonStream.Disable();
                }
                catch (Exception)
                { }
            }

            if (e.NewSensor == null)
                return;
            try
            {
                e.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                e.NewSensor.SkeletonStream.Enable();

                try
                {
                    e.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    e.NewSensor.DepthStream.Range = DepthRange.Near;
                    e.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                }
                catch (InvalidOperationException)
                {
                    e.NewSensor.DepthStream.Range = DepthRange.Default;
                    e.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                }
            }
            catch (InvalidOperationException)
            {}

            ZonaCursor.KinectSensor = e.NewSensor;
        }


        private void fisicoCirculoMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            ScrollPruebasMenuPrincipal.Visibility = Visibility.Visible;
            fisicoCirculoMenuPrincipal.Visibility = Visibility.Hidden;
            psicologicoCirculoMenuPrincipal.Visibility = Visibility.Hidden;
            atrasCirculoMenuPrincipal.Visibility = Visibility.Visible;
            estadoPrueba = 1;
            Prueba1MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba1MenuPrincipalFisica;
            Prueba2MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba2MenuPrincipalFisica;
            Prueba3MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba3MenuPrincipalFisica;
            Prueba4MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba4MenuPrincipalFisica;
            Prueba5MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba5MenuPrincipalFisica;
            OcultarMostrarBotonesPruebas();


        }

        private void psicologicoCirculoMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            ScrollPruebasMenuPrincipal.Visibility = Visibility.Visible;
            psicologicoCirculoMenuPrincipal.Visibility = Visibility.Hidden;
            fisicoCirculoMenuPrincipal.Visibility = Visibility.Hidden;
            atrasCirculoMenuPrincipal.Visibility = Visibility.Visible;
            estadoPrueba = 2;
            Prueba1MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba1MenuPrincipalPsicologica;
            Prueba2MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba2MenuPrincipalPsicologica;
            Prueba3MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba3MenuPrincipalPsicologica;
            Prueba4MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba4MenuPrincipalPsicologica;
            Prueba5MenuPrincipal.Label = RecursosLocalizables.StringResources.Prueba5MenuPrincipalPsicologica;
            OcultarMostrarBotonesPruebas();



        }

        private void OcultarMostrarBotonesPruebas() {
            if (Prueba1MenuPrincipal.Label == "")
            {
                Prueba1MenuPrincipal.Visibility = Visibility.Hidden;
            }
            else
            {
                Prueba1MenuPrincipal.Visibility = Visibility.Visible;
            }
            if (Prueba2MenuPrincipal.Label == "")
            {
                Prueba2MenuPrincipal.Visibility = Visibility.Hidden;
            }
            else
            {
                Prueba2MenuPrincipal.Visibility = Visibility.Visible;
            }
            if (Prueba3MenuPrincipal.Label == "")
            {
                Prueba3MenuPrincipal.Visibility = Visibility.Hidden;
            }
            else
            {
                Prueba3MenuPrincipal.Visibility = Visibility.Visible;
            }
            if (Prueba4MenuPrincipal.Label == "")
            {
                Prueba4MenuPrincipal.Visibility = Visibility.Hidden;
            }
            else
            {
                Prueba4MenuPrincipal.Visibility = Visibility.Visible;
            }
            if (Prueba5MenuPrincipal.Label == "")
            {
                Prueba5MenuPrincipal.Visibility = Visibility.Hidden;
            }
            else
            {
                Prueba5MenuPrincipal.Visibility = Visibility.Visible;
            }
        }

        private void Prueba1MenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (estadoPrueba == 1)
            {
                PruebaReflejos verVentana = new PruebaReflejos(this);
                verVentana.Show();
                desactivarMicrofono = true;
                this.Hide();
            }
            else {

            }
        }

        private void Prueba2MenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (estadoPrueba == 1)
            {

            }
            else
            {

            }
        }

        private void Prueba3MenuPrincipal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Prueba4MenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (estadoPrueba == 1)
            {

            }
            else
            {

            }
        }

        private void Prueba5MenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (estadoPrueba == 1)
            {

            }
            else
            {

            }
        }
        private void botonKinectMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(sensorChooser.Status) != "Conneted")
            {
                //

            }
            else {
                //
            }
            
        }

        private void usuarioMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            InformacionUsuario infoUsuario = new InformacionUsuario(this);
            infoUsuario.Show();
            desactivarMicrofono = true;
        }

        private void configuracionMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            Opciones opciones = new Opciones(this);
            opciones.Show();
            desactivarMicrofono = true;
        }

        private void usuarioMenuPrincipal_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void acercaDeMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            AcercaDe acercaDe = new AcercaDe(this);
            acercaDe.Show();
            desactivarMicrofono = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (verVentana.cerradaAntesDeTiempo != 1)
            {
                if (MessageBox.Show(RecursosLocalizables.StringResources.mensajeSalir, RecursosLocalizables.StringResources.confirmarSalir, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void botonMicrofonoMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            if (desactivarMicrofonoSiempre == true)
            {
                bi3.BeginInit();
                bi3.UriSource = new Uri("Resources/iconoMicrofono.png", UriKind.Relative);
                bi3.EndInit();
                imagenMicrofono.Source = bi3;
                desactivarMicrofonoSiempre = false;
                conectaActiva();
            }
            else {
                bi3.BeginInit();
                bi3.UriSource = new Uri("Resources/iconoMicrofonoDesactivado.png", UriKind.Relative);
                bi3.EndInit();
                imagenMicrofono.Source = bi3;
                desactivarMicrofonoSiempre = true;
            }
                
        }
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            //Hacemos un switch para ver cual es el estado del dispositivo
            switch (e.Status)
            {
                //En caso de que el status sea Connected quiere decir que hay una conexion correcta entre la PC y el Kinect
                case (KinectStatus.Connected):
                    //De la misma forma mandamos llamar al metodo conectaActiva() el cual inicializara el dispositivo Kinect
                    conectaActiva();
                    break;
                //En caso de que el status sea Disconnected se la variable _sensor se volvera nula e intentaremos buscar otro dispositivo Kinect cuyo estado sea Connected si no se encuentra mandaremos un mensaje indicando que No hay ningun Kinect conectado
                case (KinectStatus.Disconnected):
                    if (this.sensor == e.Sensor)
                    {
                        this.sensor = null;
                        this.sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                        if (this.sensor == null)
                        {
                            Error err = new VME.Error(RecursosLocalizables.StringResources.KinectNoDetect, 3);
                            err.Show();
                        }
                    }
                    break;
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
                            
                            opciones.Add(RecursosLocalizables.StringResources.cerrar, "UNO");
                            //En esta linea "dos" es el valor de opcion y "DOS" es la llave
                            opciones.Add(RecursosLocalizables.StringResources.Reflexes, "DOS");
                            opciones.Add(RecursosLocalizables.StringResources.configuracion, "TRES");
                            opciones.Add(RecursosLocalizables.StringResources.opciones, "TRES");
                            opciones.Add(RecursosLocalizables.StringResources.usuario, "TRES");
                            opciones.Add(RecursosLocalizables.StringResources.acercaDe1, "TRES");
                            opciones.Add(RecursosLocalizables.StringResources.tabInformacion, "TRES");
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
            if (e.Result.Confidence > igualdad && desactivarMicrofono == false && desactivarMicrofonoSiempre == false)
            {
                Uri src;
                BitmapImage img;
                //haremos un switch para aquellos valores que se componen de unicamente una palabra
                if (e.Result.Text.Equals(RecursosLocalizables.StringResources.cerrar))
                {
                    this.Close();
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.Reflexes))
                {
                    PruebaReflejos reflejos = new PruebaReflejos(this);
                    reflejos.Show();
                    desactivarMicrofono = true;
                    this.Hide();
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.configuracion))
                {
                    Opciones opciones = new Opciones(this);
                    opciones.Show();
                    desactivarMicrofono = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.opciones))
                {
                    Opciones opciones = new Opciones(this);
                    opciones.Show();
                    desactivarMicrofono = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.usuario))
                {
                    InformacionUsuario infoUsuario = new InformacionUsuario(this);
                    infoUsuario.Show();
                    desactivarMicrofono = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.acercaDe1))
                {
                    AcercaDe acercaDe = new AcercaDe(this);
                    acercaDe.Show();
                    desactivarMicrofono = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.tabInformacion))
                {
                    AcercaDe acercaDe = new AcercaDe(this);
                    acercaDe.Show();
                    desactivarMicrofono = true;
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

        private void atrasCirculoMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            ScrollPruebasMenuPrincipal.Visibility = Visibility.Hidden;
            fisicoCirculoMenuPrincipal.Visibility = Visibility.Visible;
            psicologicoCirculoMenuPrincipal.Visibility = Visibility.Visible;
            atrasCirculoMenuPrincipal.Visibility = Visibility.Hidden;
            estadoPrueba = 0;
        }
    }
}
