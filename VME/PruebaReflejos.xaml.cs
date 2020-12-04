using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace VME
{
    /// <summary>
    /// Interaction logic for PruebaReflejos.xaml
    /// </summary>
    public partial class PruebaReflejos : Window
    {
        private KinectSensor sensor;
        int contador = 0;
        private byte[] pixelData;
        SolidColorBrush colorCorrecto;
        Skeleton[] totalSkeleton = new Skeleton[6];
        SolidColorBrush colorMalo;
        SolidColorBrush colorNegro = new SolidColorBrush(Colors.Black);
        DispatcherTimer flasheador = new DispatcherTimer();
        Line lineaReferencia = new Line();
        MainWindow menuPrincipal;
        Boolean desactivarMicrofono = true;
        bool pruebaRaton = false;
        SpeechRecognitionEngine speechengine;
        public PruebaReflejos(MainWindow n)
        {
            menuPrincipal = n;
            InitializeComponent();
            // Cogemos el idioma almacenado en la configuracion y la aplicamos
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma();

            // Cogemos el tamaño de letra que hay almacenado en la configuracion y la aplicamos
            if (Properties.Settings.Default.tamanoLetra == "P")
                AplicarTamanoLetra(-3);
            else if (Properties.Settings.Default.tamanoLetra == "G")
                AplicarTamanoLetra(3);
            else
                AplicarTamanoLetra(0);

            //Aplicamos el estilo almacenado en la configuracion
            AplicarEstilo();
            aplicarDaltonismo();
        }

        private void AplicarIdioma()
        {
            //Traduciones a cualquier idioma de los String de la ventana, OJO!, al añadir nuevos Strings hay que añadirlo aqui de manera manual así como en los archivos de idiomas
            //Titulo
            Title = RecursosLocalizables.StringResources.VMEPruebaReflejosTitulo;

            //tabInicio
            labeltitulo.Content = RecursosLocalizables.StringResources.VMEPruebaReflejosTitulo;
            instrucciones1.Text = RecursosLocalizables.StringResources.instrucciones1Reflejos;
            instrucciones2.Text = RecursosLocalizables.StringResources.instrucciones2Reflejos;
            instrucciones3.Text = RecursosLocalizables.StringResources.instrucciones3Reflejos;

            //tabKinect

            //tabRatonTactil

            //tabResultados

            resultadoPruebaReflejos1.Content = RecursosLocalizables.StringResources.resultadoPruebaReflejos1;
            labelAciertos.Content = RecursosLocalizables.StringResources.Aciertos;
            labelFallos.Content = RecursosLocalizables.StringResources.Fallos;
            salirSinGuardar.Content = RecursosLocalizables.StringResources.salirSinGuardar;
            guardarYSalir.Content = RecursosLocalizables.StringResources.salirYGuardar;

        }

        private void AplicarTamanoLetra(int v)
        {
            //tabInicio
            labeltitulo.FontSize = labeltitulo.FontSize + v;
            instrucciones1.FontSize = instrucciones1.FontSize + v;
            instrucciones2.FontSize = instrucciones2.FontSize + v;
            instrucciones3.FontSize = instrucciones3.FontSize + v;

            //tabKinect

            //tabRatonTactil

            //tabResultados
            resultadoPruebaReflejos1.FontSize = resultadoPruebaReflejos1.FontSize + v;
            labelAciertos.FontSize = labelAciertos.FontSize + v;
            labelFallos.FontSize = labelFallos.FontSize + v;
            analisisPrueba.FontSize = analisisPrueba.FontSize + v;
            salirSinGuardar.FontSize = salirSinGuardar.FontSize + v;
            guardarYSalir.FontSize = guardarYSalir.FontSize + v;
        }

        private void AplicarEstilo()
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

            this.Background = color1;
            tabInicio.Background = color1;
            tabKinect.Background = color1;
            tabRatonTactil.Background = color1;
            tabResultados.Background = color1;

            grid1.Background = color1;
            grid2.Background = color1;
            grid3.Background = color1;
            grid4.Background = color1;

            labeltitulo.Foreground = color2;
            felicidadesPruebaReflejos.Foreground = color2;
            instrucciones1.Foreground = color2;
            instrucciones2.Foreground = color2;
            instrucciones3.Foreground = color2;

            resultadoPruebaReflejos1.Foreground = color2;
            labelAciertos.Foreground = color2;
            labelFallos.Foreground = color2;
            analisisPrueba.Foreground = color2;

        }

        private void tabInicio_Loaded(object sender, RoutedEventArgs e)
        {
            tabKinect.IsEnabled = false;
            tabRatonTactil.IsEnabled = false;
            tabResultados.IsEnabled = false;
        }

        private void botonKinect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                 tabInicio.IsEnabled = false;
                 tabKinect.IsEnabled = true;
                 tabControl.SelectedIndex = 1;
                 initKinect();
                 lineaReferencia.Stroke = System.Windows.Media.Brushes.LightGreen;
                 lineaReferencia.X1 = KinectImage.Width / 2;
                 lineaReferencia.Y1 = 0;
                 lineaReferencia.X2 = KinectImage.Height;
                 lineaReferencia.Y2 = KinectImage.Width / 2;
                 lineaReferencia.HorizontalAlignment = HorizontalAlignment.Left;
                 lineaReferencia.VerticalAlignment = VerticalAlignment.Center;
                 lineaReferencia.StrokeThickness = 2;
                 grid2.Children.Add(lineaReferencia);
                 Error err = new VME.Error(RecursosLocalizables.StringResources.ManoCabeza,3);
                 err.Show();
                flasheador.Interval = TimeSpan.FromMilliseconds(250);
                flasheador.Tick += Flasheador_Tick;
                flasheador.Start();
            }
            else
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.KinectNoDetect, 3);
                err.Show();
            }    
        }

        private void botonTactil_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tabInicio.IsEnabled = false;
            tabRatonTactil.IsEnabled = true;
            tabControl.SelectedIndex = 2;
            pruebaRaton = true;
            new System.Threading.Thread(pruebaRatonTactil).Start();
            flasheador.Interval = TimeSpan.FromMilliseconds(250);
            flasheador.Tick += Flasheador_Tick;
            flasheador.Start();
        }

        private void Flasheador_Tick(object sender, EventArgs e)
        {
            if (boton1.Background == colorCorrecto)
            {
                boton1.Background = colorNegro;
            }
            else if (boton1.Background == colorNegro) {
                boton1.Background = colorCorrecto;
            }

            if (boton2.Background == colorCorrecto)
            {
                boton2.Background = colorNegro;
            }
            else if (boton2.Background == colorNegro)
            {
                boton2.Background = colorCorrecto;
            }

            if (boton3.Background == colorCorrecto)
            {
                boton3.Background = colorNegro;
            }
            else if (boton3.Background == colorNegro)
            {
                boton3.Background = colorCorrecto;
            }

            if (boton4.Background == colorCorrecto)
            {
                boton4.Background = colorNegro;
            }
            else if (boton4.Background == colorNegro)
            {
                boton4.Background = colorCorrecto;
            }

            if (boton5.Background == colorCorrecto)
            {
                boton5.Background = colorNegro;
            }
            else if (boton5.Background == colorNegro)
            {
                boton5.Background = colorCorrecto;
            }

            if (boton6.Background == colorCorrecto)
            {
                boton6.Background = colorNegro;
            }
            else if (boton6.Background == colorNegro)
            {
                boton6.Background = colorCorrecto;
            }

            if (boton7.Background == colorCorrecto)
            {
                boton7.Background = colorNegro;
            }
            else if (boton7.Background == colorNegro)
            {
                boton7.Background = colorCorrecto;
            }

            if (boton8.Background == colorCorrecto)
            {
                boton8.Background = colorNegro;
            }
            else if (boton8.Background == colorNegro)
            {
                boton8.Background = colorCorrecto;
            }

            if (button1.Background == colorCorrecto)
            {
                button1.Background = colorNegro;
            }
            else if (button1.Background == colorNegro)
            {
                button1.Background = colorCorrecto;
            }
            if (button2.Background == colorCorrecto)
            {
                button2.Background = colorNegro;
            }
            else if (button2.Background == colorNegro)
            {
                button2.Background = colorCorrecto;
            }
            if (button3.Background == colorCorrecto)
            {
                button3.Background = colorNegro;
            }
            else if (button3.Background == colorNegro)
            {
                button3.Background = colorCorrecto;
            }
            if (button4.Background == colorCorrecto)
            {
                button4.Background = colorNegro;
            }
            else if (button4.Background == colorNegro)
            {
                button4.Background = colorCorrecto;
            }

        }



        /* El siguiente codigo corresponde a la prueba en si mismo, todo lo demas no tiene que ver con la prueba en si mismo */

        public int aciertosTotales = 0;
        public int fallosTotales = 0;
        public int nAciertosRonda = 0;
        public int nFallosRonda = 0;

        private void pruebaRatonTactil()
        {
            int nVecesPrueba = 0;
            int contadorTiempointerno;
            //Contador cuenta atrás antes de empezar la prueba
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicio.Content = 3));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicio.Content = 2));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicio.Content = 1));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicio.Content = 0));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicio.Visibility = Visibility.Hidden));


            //Aqui ya podemos ejecutar la prueba


            while (nVecesPrueba < 5)
            {
                //Mostramos los botones
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton1.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton2.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton3.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton4.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton5.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton6.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton7.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton8.Visibility = Visibility.Visible));

                //Cambiamos los colores de los botones
                cambiarColoresBotones(nVecesPrueba);

                //Iniciamos el contador de tiempo
                contadorTiempointerno = establecerTiempoCuentaAtras()*50;

                //En la ronda esperamos a que el numero de aciertos sea 2, o el numero de fallos, o el tiempo de la ronda se ha acabado
                while (nAciertosRonda < 2 && nFallosRonda < 2 && contadorTiempointerno > 0)
                {
                    contadorTiempointerno--;
                    Thread.Sleep(20);
                }


                nVecesPrueba++;
                //Almacenamos los datos de la ronda
                aciertosTotales += nAciertosRonda;
                fallosTotales += nFallosRonda;
                nAciertosRonda = 0;
                nFallosRonda = 0;

            }
            //Abrimos los resultados
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabRatonTactil.IsEnabled = false));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabResultados.IsEnabled = true));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabControl.SelectedIndex = 3));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => labelAciertos.Content = labelAciertos.Content + " " + aciertosTotales));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => labelFallos.Content = labelFallos.Content + " " + fallosTotales));

            if (aciertosTotales > 7 && fallosTotales < 3)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => analisisPrueba.Text = RecursosLocalizables.StringResources.analisisPruebaReflejosBueno));
            else
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => analisisPrueba.Text = RecursosLocalizables.StringResources.analisisPruebaRelejosMalo));

        }

        private void pruebaKinect()
        {
            int nVecesPrueba = 0;
            int contadorTiempointerno;
            //Contador cuenta atrás antes de empezar la prueba
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Visibility = Visibility.Visible));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Content = 3));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Content = 2));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Content = 1));
            Thread.Sleep(1000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Content = 0));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => contadorInicioKinect.Visibility = Visibility.Hidden));


            while (nVecesPrueba < 5)
            {
                //Mostramos los botones
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => KinectImage.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button1.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button2.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button3.Visibility = Visibility.Visible));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button4.Visibility = Visibility.Visible));
                //Cambiamos los colores de los botones
                cambiarColoresBotonesKinect(nVecesPrueba);

                //Iniciamos el contador de tiempo
                contadorTiempointerno = establecerTiempoCuentaAtras() * 50;

                //En la ronda esperamos a que el numero de aciertos sea 2, o el numero de fallos, o el tiempo de la ronda se ha acabado
                while (nAciertosRonda < 2 && nFallosRonda < 2 && contadorTiempointerno > 0)
                {
                    contadorTiempointerno--;
                    Thread.Sleep(20);
                }


                nVecesPrueba++;
                //Almacenamos los datos de la ronda
                aciertosTotales += nAciertosRonda;
                fallosTotales += nFallosRonda;
                nAciertosRonda = 0;
                nFallosRonda = 0;

            }
            //Abrimos los resultados
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabKinect.IsEnabled = false));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabResultados.IsEnabled = true));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => tabControl.SelectedIndex = 3));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => labelAciertos.Content = labelAciertos.Content + " " + aciertosTotales));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => labelFallos.Content = labelFallos.Content + " " + fallosTotales));

            if (aciertosTotales > 3 && fallosTotales < 3)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => analisisPrueba.Text = RecursosLocalizables.StringResources.analisisPruebaReflejosBueno));
            else
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => analisisPrueba.Text = RecursosLocalizables.StringResources.analisisPruebaRelejosMalo));


           
            
        }

        void iniciarPruebaKinect()
        {

            lineaReferencia.Visibility = Visibility.Hidden;
            KinectImage.Visibility = Visibility.Hidden;
            contadorInicioKinect.Visibility = Visibility.Hidden;


            new System.Threading.Thread(pruebaKinect).Start();

        }

        void initKinect()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);
                this.sensor.Start();
                this.sensor.ColorStream.Enable();
                this.sensor.ColorFrameReady += this.sensor_ColorFrameReady;
            }

            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_skeletonFrameReady);
                sensor.SkeletonStream.Enable();
            }
        }

        const int PostureDetectionNumber = 10;
        int accumulator = 0;
        Posture postureInDetection = Posture.None;
        Posture previousPosture = Posture.None;

        

        public bool PostureDetector(Posture posture)
        {
            if(postureInDetection != posture)
            {
                accumulator = 0;
                postureInDetection = posture;
                return false;
            }
            if (accumulator < PostureDetectionNumber)
            {
                accumulator++;
                return false;
            }
            if (posture != previousPosture)
            {
                previousPosture = posture;
                accumulator = 0;
                return true;
            }
            else
            {
                accumulator = 0;
            }
            return false;
        }

        public enum Posture
        {
            None,
            posturaSimple,
            manoDerechaAbajo,
            manoIzquierdaAbajo,
            manoDerechaArriba,
            manoIzquierdaArriba
        }

        private void kinect_skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            try
            {
                using (SkeletonFrame frameEsqueleto = e.OpenSkeletonFrame())
                {
                    if (frameEsqueleto != null)
                    {
                        totalSkeleton = new Skeleton[frameEsqueleto.SkeletonArrayLength];
                        frameEsqueleto.CopySkeletonDataTo(totalSkeleton);
                    }
                    if (totalSkeleton == null) return;

                    foreach (Skeleton esqueleto in totalSkeleton)
                    {
                        if (esqueleto.TrackingState == SkeletonTrackingState.Tracked)
                        {

                            Vector3 elbow_left = new Vector3();
                            elbow_left.X = esqueleto.Joints[JointType.ElbowLeft].Position.X;
                            elbow_left.Y = esqueleto.Joints[JointType.ElbowLeft].Position.Y;
                            elbow_left.Z = esqueleto.Joints[JointType.ElbowLeft].Position.Z;

                            Vector3 elbow_right = new Vector3();
                            elbow_right.X = esqueleto.Joints[JointType.ElbowRight].Position.X;
                            elbow_right.Y = esqueleto.Joints[JointType.ElbowRight].Position.Y;
                            elbow_right.Z = esqueleto.Joints[JointType.ElbowRight].Position.Z;

                            Vector3 shoulder_left = new Vector3();
                            shoulder_left.X = esqueleto.Joints[JointType.ShoulderLeft].Position.X;
                            shoulder_left.Y = esqueleto.Joints[JointType.ShoulderLeft].Position.Y;
                            shoulder_left.Z = esqueleto.Joints[JointType.ShoulderLeft].Position.Z;

                            Vector3 shoulder_right = new Vector3();
                            shoulder_right.X = esqueleto.Joints[JointType.ShoulderRight].Position.X;
                            shoulder_right.Y = esqueleto.Joints[JointType.ShoulderRight].Position.Y;
                            shoulder_right.Z = esqueleto.Joints[JointType.ShoulderRight].Position.Z;

                            Vector3 handLeft = new Vector3();
                            handLeft.X = esqueleto.Joints[JointType.HandLeft].Position.X;
                            handLeft.Y = esqueleto.Joints[JointType.HandLeft].Position.Y;
                            handLeft.Z = esqueleto.Joints[JointType.HandLeft].Position.Z;

                            Vector3 handRight = new Vector3();
                            handRight.X = esqueleto.Joints[JointType.HandRight].Position.X;
                            handRight.Y = esqueleto.Joints[JointType.HandRight].Position.Y;
                            handRight.Z = esqueleto.Joints[JointType.HandRight].Position.Z;

                            Vector3 head = new Vector3();
                            head.X = esqueleto.Joints[JointType.Head].Position.X;
                            head.Y = esqueleto.Joints[JointType.Head].Position.Y;
                            head.Z = esqueleto.Joints[JointType.Head].Position.Z;

                            

                            if (contador == 0)
                            {
                                if (posturaSimple(handRight, head))
                                {
                                    if (PostureDetector(Posture.posturaSimple))
                                    {
                                        iniciarPruebaKinect();
                                        contador++;
                                    }
                                }
                            }
                            if (manoDerechaAbajo(handRight))
                            {
                                if (PostureDetector(Posture.manoDerechaAbajo))
                                {
                                    if (button4.Background == colorCorrecto)
                                    {
                                        nAciertosRonda++;
                                        SystemSounds.Beep.Play();
                                    }
                                    else
                                    {
                                        nFallosRonda++;
                                        SystemSounds.Hand.Play();
                                    }
                                    button4.Visibility = Visibility.Hidden;
                                }
                            }

                            if (manoDerechaArriba(handRight))
                            {
                                if (PostureDetector(Posture.manoDerechaArriba))
                                {
                                    if (button3.Background == colorCorrecto)
                                    {
                                        nAciertosRonda++;
                                        SystemSounds.Beep.Play();
                                    }
                                    else
                                    {
                                        nFallosRonda++;
                                        SystemSounds.Hand.Play();
                                    }
                                    button3.Visibility = Visibility.Hidden;
                                }
                            }

                            if (manoIzquierdaAbajo(handLeft))
                            {
                                if (PostureDetector(Posture.manoIzquierdaAbajo))
                                {
                                    if (button1.Background == colorCorrecto)
                                    {
                                        nAciertosRonda++;
                                        SystemSounds.Beep.Play();
                                    }
                                    else
                                    {
                                        nFallosRonda++;
                                        SystemSounds.Hand.Play();
                                    }
                                    button1.Visibility = Visibility.Hidden;
                                    
                                }
                            }

                            if (manoIzquierdaArriba(handLeft))
                            {
                                if (PostureDetector(Posture.manoIzquierdaArriba))
                                {
                                    if (button2.Background == colorCorrecto)
                                    {
                                        nAciertosRonda++;
                                        SystemSounds.Beep.Play();
                                    }
                                    else
                                    {
                                        nFallosRonda++;
                                        SystemSounds.Hand.Play();
                                    }
                                    button2.Visibility = Visibility.Hidden;
                                }
                            }



                        }
                    }

                }
            }
            catch (Exception exc)
            {

            }
        }

        public bool manoDerechaAbajo(Vector3 handRight)
        {
            if(handRight.X>0.4 && handRight.Y < 0.1)
            {
                return true;
            }else
            {
                return false;
            }
        }

        public bool manoIzquierdaAbajo(Vector3 handLeft)
        {
            if (handLeft.X < -0.4 && handLeft.Y < 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool manoDerechaArriba(Vector3 handRight)
        {
            if (handRight.X > 0.4 && handRight.Y > 0.4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool manoIzquierdaArriba(Vector3 handLeft)
        {
            if (handLeft.X < -0.4 && handLeft.Y > 0.4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool posturaSimple(Vector3 hand, Vector3 head)
        {
            float distance = (hand.X - head.X) + (hand.Y - head.Y) + (hand.Z - head.Z);

            if (Math.Abs(distance) > 0.05f)
            {
                return false;
            }else
            {
                return true;
            }

        }
        

        [Serializable]
        public struct Vector3
        {
            public float X;
            public float Y;
            public float Z;
        }

        private void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame == null)
                {
                    return;
                }
                else
                {
                    this.pixelData = new byte[imageFrame.PixelDataLength];
                    imageFrame.CopyPixelDataTo(this.pixelData);
                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                    this.KinectImage.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
                }
            }
        }
        private void cambiarColoresBotones(int nVecesPrueba)
        {


            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton1.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton2.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton3.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton4.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton5.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton6.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton7.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton8.Background = colorMalo));

            //Debería de ser aleatorio, pero bueno, puede pasar.
            if (nVecesPrueba == 0)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton3.Background = colorCorrecto));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton7.Background = colorCorrecto));
            }
            else if (nVecesPrueba == 1)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton2.Background = colorCorrecto));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton5.Background = colorCorrecto));
            }
            else if (nVecesPrueba == 2)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton4.Background = colorCorrecto));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton8.Background = colorCorrecto));
            }
            else if (nVecesPrueba == 3)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton1.Background = colorCorrecto));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton4.Background = colorCorrecto));
            }
            else if (nVecesPrueba == 4)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton2.Background = colorCorrecto));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => boton6.Background = colorCorrecto));
            }
        }

        private void cambiarColoresBotonesKinect(int nVecesPrueba)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button1.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button2.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button3.Background = colorMalo));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button4.Background = colorMalo));
           
            //Debería de ser aleatorio, pero bueno, puede pasar.
            if (nVecesPrueba == 0)
            {
                
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button4.Background = colorCorrecto));
            }
            else if (nVecesPrueba == 1)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button2.Background = colorCorrecto));
               
            }
            else if (nVecesPrueba == 2)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button1.Background = colorCorrecto));
                
            }
            else if (nVecesPrueba == 3)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button4.Background = colorCorrecto));
                
            }
            else if (nVecesPrueba == 4)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => button3.Background = colorCorrecto));
                
            }
        }

        private int establecerTiempoCuentaAtras()
        {
            int tiempo = 2;

            if (Convert.ToInt16(Properties.Settings.Default.añoNacimiento) <= Convert.ToInt16(DateTime.Today.ToString("yyyy")) - 30)
            {
                tiempo += 2;
            }

            if (Convert.ToInt16(Properties.Settings.Default.añoNacimiento) <= Convert.ToInt16(DateTime.Today.ToString("yyyy")) - 60)
            {
                tiempo += 2;
            }

            if (Properties.Settings.Default.tipoMovilidad > 2)
                tiempo+=3;

            return tiempo;
        }


        //Este evento se ejecuta cuando alguno de los botones de la prueba es accionado, y ve si el color del boton era el correcto o no, y actua en consecuencia
        private void botonesPrueba_Click(object sender, RoutedEventArgs e)
        {


            Button btn = sender as Button;
            if (((sender as Button).Background as SolidColorBrush).Color == colorCorrecto.Color || ((sender as Button).Background as SolidColorBrush).Color == colorNegro.Color)
            {
                nAciertosRonda++;
                SystemSounds.Beep.Play();
            }
            else if (((sender as Button).Background as SolidColorBrush).Color == colorMalo.Color)
            {
                nFallosRonda++;
                SystemSounds.Hand.Play();
            }
            btn.Visibility = Visibility.Hidden;
        }

        private void guardarYSalir_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.pruebaReflejosAciertos = aciertosTotales;
            Properties.Settings.Default.pruebaReflejosFallos = fallosTotales;
            menuPrincipal.desactivarMicrofono = false;
            Close();
        }

        private void salirSinGuardar_Click(object sender, RoutedEventArgs e)
        {
            menuPrincipal.desactivarMicrofono = false;
            Close();
        }




        private void aplicarDaltonismo()
        {
            if (Properties.Settings.Default.Daltonico == true)
            {

                 colorCorrecto = new SolidColorBrush(Colors.Blue);
                 colorMalo = new SolidColorBrush(Colors.Yellow);
                instrucciones1.Text = RecursosLocalizables.StringResources.Instrucciones1Daltonico;
                instrucciones2.Text = RecursosLocalizables.StringResources.Instrucciones2Daltonico;

            }
            else
            {
                colorCorrecto = new SolidColorBrush(Colors.Green);
                colorMalo = new SolidColorBrush(Colors.Red);
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show(RecursosLocalizables.StringResources.mensajeSalir, RecursosLocalizables.StringResources.confirmarSalir, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                e.Cancel = false;
                menuPrincipal.Show();
                menuPrincipal.desactivarMicrofono = false;
            }
            else
            {
                e.Cancel = true;
            }
            
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
                        opciones.Add("Kinect", "DOS");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosRaton, "DOS");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosTactil, "DOS");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton1, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton2, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton3, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton4, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton5, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton6, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton7, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.pruebaReflejosBoton8, "TRES");
                        opciones.Add(RecursosLocalizables.StringResources.salirSinGuardar, "CUATRO");
                        opciones.Add(RecursosLocalizables.StringResources.salirYGuardar, "CUATRO");
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
                    this.Close();
                    menuPrincipal.desactivarMicrofono = false;
                }
                else if (e.Result.Text.Equals("Kinect") && tabControl.SelectedIndex == 0)
                {
                    botonKinect_MouseUp(null, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosRaton) && tabControl.SelectedIndex == 0)
                {
                    botonTactil_MouseUp(null, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosTactil) && tabControl.SelectedIndex == 0)
                {
                    botonTactil_MouseUp(null, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton1) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton1, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton2) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton2, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton3) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton3, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton4) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton4, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton5) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton5, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton6) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton6, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton7) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton7, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.pruebaReflejosBoton8) && tabControl.SelectedIndex == 2)
                {
                    botonesPrueba_Click(boton8, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.salirSinGuardar) && tabControl.SelectedIndex == 3)
                {
                    salirSinGuardar_Click(null,null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.salirYGuardar) && tabControl.SelectedIndex == 3)
                {
                    guardarYSalir_Click(null, null);
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
