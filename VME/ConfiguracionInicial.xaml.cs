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
using System.IO;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Threading;
using System.Management;
using System.Threading;
using System.Globalization;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Diagnostics;

namespace VME
{
    /// <summary>
    /// Interaction logic for ConfiguracionInicial.xaml
    /// </summary>
    public partial class ConfiguracionInicial : Window
    {
        SpeechRecognitionEngine speechengine;
        public int cerrar =0;
        public int cerradaAntesDeTiempo = 0;
        private KinectSensor sensor;
        private byte[] pixelData;
        MainWindow menuPrincipal;
        bool seguir = true;
        Skeleton[] totalSkeleton = new Skeleton[6];
        int valor;
        int contador = 0;
        Boolean desactivarMicrofono = true;      

        public ConfiguracionInicial(MainWindow n)
        {

            
            
            InitializeComponent();

            //Al crear la configuracion inicial, le pasamos como parametro la ventana del menu pricipal, ahora lo asignamos a la variable global de la configuracion inicial
            menuPrincipal = n;

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


            pantallaTactilBienvenido.IsEnabled = false;
            altavocesBienvenido.IsEnabled = false;
            kinectBienvenido.IsEnabled = false;
            microfonoBienvenido.IsEnabled = false;

            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectBienvenido.IsChecked = true;
            }

        }

        public void AplicarIdioma()
        {
            //Traduciones a cualquier idioma de los String de la ventana, OJO!, al añadir nuevos Strings hay que añadirlo aqui de manera manual así como en los archivos de idiomas
            //Titulo
            Title = RecursosLocalizables.StringResources.VMEConfiguracionInicialTitulo;

            //Botones
            atrasBienvenida.Content = RecursosLocalizables.StringResources.atras;
            siguienteBienvenida.Content = RecursosLocalizables.StringResources.siguiente;

            //Nombres tabs
            tabIdioma.Header = RecursosLocalizables.StringResources.tabIdioma;
            tabInformacion.Header = RecursosLocalizables.StringResources.tabInformacion;
            tabColores.Header = RecursosLocalizables.StringResources.tabColores;
            tabMovilidad.Header = RecursosLocalizables.StringResources.tabMovilidad;
            tabTecnologia.Header = RecursosLocalizables.StringResources.tabTecnologia;
            reguKine.Header = RecursosLocalizables.StringResources.reguKine;
            tabFin.Header = RecursosLocalizables.StringResources.tabFin;
            tabVista.Header = RecursosLocalizables.StringResources.tabVista;

            //TabVista
            label1.Content = RecursosLocalizables.StringResources.label1;
            tamañoLetra.Content = RecursosLocalizables.StringResources.tamanoLetra;
            RegularKinectBienvenida.Content = RecursosLocalizables.StringResources.RegularKinectBienvenida;
            regularKinectBienvenida2.Content = RecursosLocalizables.StringResources.regularKinectBienvenida2;
            cuerpoDetectado.Content = RecursosLocalizables.StringResources.cuerpoDetectado;

            interfazClaraBienvenida.Content = RecursosLocalizables.StringResources.interfazClara;
            interfazOscuraBienvenida.Content = RecursosLocalizables.StringResources.interfazOscura;

            //TabInformacion
            guardarImagen.Content = RecursosLocalizables.StringResources.guardarImagen;
            labelBienvenida3.Content = RecursosLocalizables.StringResources.labelBienvenida3;
            labelBienvenida4.Content = RecursosLocalizables.StringResources.labelBienvenida4;
            nombreBienvenida.Content = RecursosLocalizables.StringResources.nombreBienvenida;
            apellidosBienvenida.Content = RecursosLocalizables.StringResources.apellidosBienvenida;
            anoDeNacimientoBienvenida.Content = RecursosLocalizables.StringResources.anoDeNacimientoBienvenida;
            pesoBienvenida.Content = RecursosLocalizables.StringResources.pesoBienvenida;

            

            //TabColores
            cegueraLabelColorBienvenida.Content = RecursosLocalizables.StringResources.cegueraLabelColorBienvenida;

            //TabProblemasMovilidad
            movilidadBienvenida.Content = RecursosLocalizables.StringResources.movilidadBienvenida;
            movilidad2Bienvenida.Content = RecursosLocalizables.StringResources.movilidad2Bienvenida;
            movilidad3Bienvenida.Content = RecursosLocalizables.StringResources.movilidad3Bienvenida;
            movilidad4Bienvenida.Content = RecursosLocalizables.StringResources.movilidad4Bienvenida;
            movilidad5Bienvenida.Content = RecursosLocalizables.StringResources.movilidad5Bienvenida;

            //TabTecnologia
            dispositivosUtilizarLabel.Content = RecursosLocalizables.StringResources.dispositivosUtilizarLabel;
            ProbarMicrofonoBienvenida.Content = RecursosLocalizables.StringResources.probar;
            AtavocesBienvenida.Content = RecursosLocalizables.StringResources.probar;
            KinectBienvenida.Content = RecursosLocalizables.StringResources.probar;

            estadoPantallaTactilBienvenida.Content = RecursosLocalizables.StringResources.estadoPantallaTactilBienvenida;
            estadoMicrofonoBienvenida.Content = RecursosLocalizables.StringResources.estadoMicrofonoBienvenida;
            estadoAltavocesBienvenida.Content = RecursosLocalizables.StringResources.estadoAltavocesBienvenida;
            estadoKinectBienvenida.Content = RecursosLocalizables.StringResources.estadoKinectBienvenida;
            pantallaTactilBienvenido.Content = RecursosLocalizables.StringResources.estadoPantallaTactilBienvenida;
            microfonoBienvenido.Content = RecursosLocalizables.StringResources.microfonoBienvenido;
            altavocesBienvenido.Content = RecursosLocalizables.StringResources.altavocesBienvenido;



            //TabFin
            felicidadesBienvenida.Content = RecursosLocalizables.StringResources.felicidadesBienvenida;
        //    finalBienvenida.Content = RecursosLocalizables.StringResources.finalBienvenida;
            siguienteSalirBienvenida.Content = RecursosLocalizables.StringResources.siguienteSalirBienvenida;


        }

        public void AplicarTamanoLetra(int tamano) {
            // Tamaño letra, en la configuracion se almacena P = pequeño, M = mediano y G = grande -- tamano = -3,0,+3

            siguienteBienvenida.FontSize = siguienteBienvenida.FontSize + tamano;
            atrasBienvenida.FontSize = atrasBienvenida.FontSize + tamano;
            label1.FontSize = label1.FontSize + tamano;
            labelBienvenida4.FontSize = labelBienvenida4.FontSize + tamano;
            labelBienvenida3.FontSize = labelBienvenida3.FontSize + tamano;
            nombreBienvenida.FontSize = nombreBienvenida.FontSize + tamano;
            apellidosBienvenida.FontSize = apellidosBienvenida.FontSize + tamano;
            anoDeNacimientoBienvenida.FontSize = anoDeNacimientoBienvenida.FontSize + tamano;
            pesoBienvenida.FontSize = pesoBienvenida.FontSize + tamano;
            cegueraLabelColorBienvenida.FontSize = cegueraLabelColorBienvenida.FontSize + tamano;
            movilidadBienvenida.FontSize = movilidadBienvenida.FontSize + tamano;
            movilidad2Bienvenida.FontSize = movilidad2Bienvenida.FontSize + tamano;
            movilidad3Bienvenida.FontSize = movilidad3Bienvenida.FontSize + tamano;
            movilidad4Bienvenida.FontSize = movilidad4Bienvenida.FontSize + tamano;
            movilidad5Bienvenida.FontSize = movilidad5Bienvenida.FontSize + tamano;
            tamañoLetra.FontSize = tamañoLetra.FontSize + tamano;
            estadoPantallaTactilBienvenida.FontSize = estadoPantallaTactilBienvenida.FontSize + tamano;
            estadoMicrofonoBienvenida.FontSize = estadoMicrofonoBienvenida.FontSize + tamano;
            estadoAltavocesBienvenida.FontSize = estadoAltavocesBienvenida.FontSize + tamano;
            estadoKinectBienvenida.FontSize = estadoKinectBienvenida.FontSize + tamano;
            pantallaTactilBienvenido.FontSize = pantallaTactilBienvenido.FontSize + tamano;
            microfonoBienvenido.FontSize = microfonoBienvenido.FontSize + tamano;
            kinectBienvenido.FontSize = kinectBienvenido.FontSize + tamano;
            dispositivosUtilizarLabel.FontSize = dispositivosUtilizarLabel.FontSize + tamano;
            RegularKinectBienvenida.FontSize = RegularKinectBienvenida.FontSize + tamano;
            felicidadesBienvenida.FontSize = felicidadesBienvenida.FontSize + tamano;
          //  finalBienvenida.FontSize = finalBienvenida.FontSize + tamano;
            siguienteSalirBienvenida.FontSize = siguienteSalirBienvenida.FontSize + tamano;
            altavocesBienvenido.FontSize = altavocesBienvenido.FontSize + tamano;
        }

        public void AplicarEstilo() {
            //Inicializamos los colores a la interfaz oscura, por si acaso hay un problema en la configuracion y no entra en ningun if, crear tantos colores como sean necesarios
            SolidColorBrush color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
            SolidColorBrush color2 = new SolidColorBrush(Colors.White); //Blanco

            if (Properties.Settings.Default.tipoInterfaz == "Oscuro")
            {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1D1B1B")); //Gris oscuro
                color2 = new SolidColorBrush(Colors.White); //Blanco

            }
            else if (Properties.Settings.Default.tipoInterfaz == "Claro") {
                color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B7AAAA")); // -- No se que color es --
                color2 = new SolidColorBrush(Colors.Black); //Negro

            }

            grid.Background = color1;
            grid2.Background = color1;
            grid3.Background = color1;
            grid4.Background = color1;
            grid5.Background = color1;
            grid6.Background = color1;
            grid7.Background = color1;
            grid8.Background = color1;
            grid9.Background = color1;

            label1.Foreground = color2;
            labelBienvenida.Foreground = color2;
            labelBienvenida1.Foreground = color2;
            labelBienvenida4.Foreground = color2;
            labelBienvenida3.Foreground = color2;
            nombreBienvenida.Foreground = color2;
            apellidosBienvenida.Foreground = color2;
            anoDeNacimientoBienvenida.Foreground = color2;
            pesoBienvenida.Foreground = color2;
            cegueraLabelColorBienvenida.Foreground = color2;
            movilidadBienvenida.Foreground = color2;
            movilidad2Bienvenida.Foreground = color2;
            movilidad3Bienvenida.Foreground = color2;
            movilidad4Bienvenida.Foreground = color2;
            movilidad5Bienvenida.Foreground = color2;
            tamañoLetra.Foreground = color2;
            RegularKinectBienvenida.Foreground = color2;
            estadoPantallaTactilBienvenida.Foreground = color2;
            estadoMicrofonoBienvenida.Foreground = color2;
            estadoAltavocesBienvenida.Foreground = color2;
            estadoKinectBienvenida.Foreground = color2;
            pantallaTactilBienvenido.Foreground = color2;
            microfonoBienvenido.Foreground = color2;
            kinectBienvenido.Foreground = color2;
            dispositivosUtilizarLabel.Foreground = color2;

            felicidadesBienvenida.Foreground = color2;
           // finalBienvenida.Foreground = color2;
            siguienteSalirBienvenida.Foreground = color2;
            altavocesBienvenido.Foreground = color2;

           // regularKinectBienvenida2.Foreground = color2;
            cuerpoDetectado.Foreground = color2;

            tabControl.Background = color1;
            tabVista.Background = color1;
            tabColores.Background = color1;
            tabInformacion.Background = color1;
            tabMovilidad.Background = color1;
            tabTecnologia.Background = color1;
            tabFin.Background = color1;
            reguKine.Background = color1;
            this.Background = color1;
        }


        private void banderaInglesa_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Cambiamos el idioma en el archivo de configuracion, aplicamos el idioma y pasamos a la siguiente pestaña
            Properties.Settings.Default.idioma = "en-US";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma();
            tabControl.SelectedIndex++;
            AplicarEstilo();
            comboBox.SelectedIndex = 1;
            RecognizerInfo ri = obtenerLP();
            desactivarMicrofono = true;
        }

        private void banderaEspanola_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Cambiamos el idioma en el archivo de configuracion, aplicamos el idioma y pasamos a la siguiente pestaña
            Properties.Settings.Default.idioma = "es-ES";
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.idioma);
            AplicarIdioma();
            tabControl.SelectedIndex++;
            AplicarEstilo();
            comboBox.SelectedIndex = 1;
            RecognizerInfo ri = obtenerLP();
            desactivarMicrofono = true;
        }

        private void siguienteBienvenida_Click(object sender, RoutedEventArgs e)
        {


            if (tabControl.SelectedIndex == 2)
            {
                Properties.Settings.Default.nombre = NombreBienvenida.Text;
                Properties.Settings.Default.apellidos = ApellidosBienvenida.Text;
                Properties.Settings.Default.añoNacimiento = NacimientoBienvenida.Text;
                Properties.Settings.Default.peso = PesoBienvenida.Text;


            }
            else if (tabControl.SelectedIndex == 3)
            {

                if (escribirCegueraBienvenida.Text == "6" && escribirCeguera2Bienvenida.Text == "42")
                {
                    Properties.Settings.Default.Daltonico = false;
                }
                else
                {
                    Properties.Settings.Default.Daltonico = true;
                }

            }
            
            else if (tabControl.SelectedIndex == 4)
            {
                if (movilidad2Bienvenida.IsChecked == true)
                {
                    Properties.Settings.Default.tipoMovilidad = 4;
                }
                else if (movilidad3Bienvenida.IsChecked == true)
                {
                    Properties.Settings.Default.tipoMovilidad = 3;
                }
                else if (movilidad4Bienvenida.IsChecked == true)
                {
                    Properties.Settings.Default.tipoMovilidad = 2;
                }
                else if (movilidad5Bienvenida.IsChecked == true)
                {
                    Properties.Settings.Default.tipoMovilidad = 1;
                }


            }
            if ((comboBox.SelectedItem != interfazClaraBienvenida && comboBox.SelectedItem != interfazOscuraBienvenida) && tabControl.SelectedIndex == 1)
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();
            }
            else if (Properties.Settings.Default.tipoMovilidad == 0 && tabControl.SelectedIndex == 4)
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();
            }
            else if (tabControl.SelectedIndex == 6 && cuerpoDetectado.IsChecked == false)
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();
            }
            else if (((Properties.Settings.Default.nombre == "") || (Properties.Settings.Default.apellidos == "") || (Properties.Settings.Default.añoNacimiento == "") || (Properties.Settings.Default.peso == "")) && tabControl.SelectedIndex == 2)
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();

            }
            else if ((escribirCegueraBienvenida.Text == "" || escribirCeguera2Bienvenida.Text == "") && tabControl.SelectedIndex == 3)
            {
                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();
            }
            else if (tabControl.SelectedIndex == 0)
            {

                Error err = new VME.Error(RecursosLocalizables.StringResources.faltanParametros, 1);
                err.Show();
            }

            else if (tabControl.SelectedIndex == 7)
            {
                menuPrincipal.desactivarMicrofono = false;
                this.Close();
                Properties.Settings.Default.configuracionInicial = true;
            }
            else
            {
                if (tabControl.SelectedIndex == 5 && KinectSensor.KinectSensors.Count == 0)
                {
                    tabControl.SelectedIndex = tabControl.SelectedIndex + 2;
                }
                else
                {
                    
                    tabControl.SelectedIndex++;


                }
            }
        }

        private void atrasBienvenida_Click(object sender, RoutedEventArgs e)
        {

            if (tabControl.SelectedIndex == 7 && KinectSensor.KinectSensors.Count<=0)
            {
                tabControl.SelectedIndex -= 2;
            }
            else if (tabControl.SelectedIndex != 0)
            {
                AplicarEstilo();
                tabControl.SelectedIndex--;
            }



        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            cuerpoDetectado.IsEnabled = false;
            tabVista.IsEnabled = false;
            tabIdioma.IsEnabled = false;
            tabInformacion.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            tabFin.IsEnabled = false;
            initKinect();
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_skeletonFrameReady);
                sensor.SkeletonStream.Enable();
            }
        }

        private void kinect_skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {try
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
                            // Joint handJoint = esqueleto.Joints[JointType.HandRight];
                            // Joint elbowJoint = esqueleto.Joints[JointType.ElbowRight];
                            
                            if (contador == 0 && tabControl.SelectedIndex==6)
                            {
                                
                                Error err = new VME.Error(RecursosLocalizables.StringResources.cuerpoDetectado, 3);
                                err.Show();
                                contador++;
                            }
                            cuerpoDetectado.IsChecked = true;

                        }
                    }

                }
            }catch(Exception exc)
            {

            }
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
            else
            {
                reguKine.IsEnabled = false;
            }
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
                    this.cameraViewer.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
                    this.image.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, stride);
                }
            }
        }

        private void Detener_Click(object sender, RoutedEventArgs e)
        {
            this.sensor.Stop();
        }

        private void Iniciar_Click(object sender, RoutedEventArgs e)
        {
            this.sensor.Start();

        }

        private void guardarImagen_Click(object sender, RoutedEventArgs e)
        {
            string foto = "Photo";
            if (File.Exists(foto))
            {
                File.Delete(foto);
            }
            // se guarda en la carpeta del proyecto bin/debug
            using (FileStream savesnap = new FileStream(foto, FileMode.CreateNew))
            {
                BitmapSource image = (BitmapSource)cameraViewer.Source;
                JpegBitmapEncoder jpg = new JpegBitmapEncoder();
                jpg.QualityLevel = 70;
                jpg.Frames.Add(BitmapFrame.Create(image));
                jpg.Save(savesnap);
                savesnap.Close();
            }
        }


        private void tabIdioma_Loaded(object sender, RoutedEventArgs e)
        {
            AplicarEstilo();
            tabInformacion.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabFin.IsEnabled = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                microfono.Visibility = Visibility.Visible;
            }else
            {
                microfono.Visibility = Visibility.Hidden;
            }
        }

        private void tabInformacion_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count <= 0)
            {
                guardarImagen.IsEnabled = false;
                importarDatos.IsEnabled = false;
                image.IsEnabled = false;
                microfono.Visibility = Visibility.Hidden;
            }
            else
            {
                microfono.Visibility = Visibility.Visible;
            }
            tabIdioma.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabFin.IsEnabled = false;
        }

        private void tabColores_Loaded(object sender, RoutedEventArgs e)
        {
            tabIdioma.IsEnabled = false;
            tabInformacion.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabFin.IsEnabled = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                microfono.Visibility = Visibility.Visible;
            }
            else
            {
                microfono.Visibility = Visibility.Hidden;
            }
        }

        private void tabMovilidad_Loaded(object sender, RoutedEventArgs e)
        {
            tabIdioma.IsEnabled = false;
            tabInformacion.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabFin.IsEnabled = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                microfono.Visibility = Visibility.Visible;
            }
            else
            {
                microfono.Visibility = Visibility.Hidden;
            }
        }

        private void tabTecnologia_Loaded(object sender, RoutedEventArgs e)
        {
            tabIdioma.IsEnabled = false;
            tabInformacion.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabFin.IsEnabled = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                microfono.Visibility = Visibility.Visible;
            }
            else
            {
                microfono.Visibility = Visibility.Hidden;
            }
        }

        private void tabFin_Loaded(object sender, RoutedEventArgs e)
        {
            tabIdioma.IsEnabled = false;
            tabInformacion.IsEnabled = false;
            tabColores.IsEnabled = false;
            tabMovilidad.IsEnabled = false;
            reguKine.IsEnabled = false;
            tabTecnologia.IsEnabled = false;
            if (KinectSensor.KinectSensors.Count > 0)
            {
                microfono.Visibility = Visibility.Visible;
            }
            else
            {
                microfono.Visibility = Visibility.Hidden;
            }
        }

        private void KinectBienvenida_Click(object sender, RoutedEventArgs e)
        {
            Window1 n = new Window1();
            n.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //Descomentar para que no se ejecute mas la configuracion inicial
            //Properties.Settings.Default.configuracionInicial = true;
            menuPrincipal.AplicarEstilo();
            menuPrincipal.AplicarIdioma();
            menuPrincipal.desactivarMicrofono = false;
            // Cogemos el tamaño de letra que hay almacenado en la configuracion y la aplicamos
            if (Properties.Settings.Default.tamanoLetra == "P")
                menuPrincipal.AplicarTamanoLetra(-3);
            else if (Properties.Settings.Default.tamanoLetra == "G")
                menuPrincipal.AplicarTamanoLetra(3);
            else
                menuPrincipal.AplicarTamanoLetra(0);

            if (cerrar == 0)
            {
                menuPrincipal.Show();
            }
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem == interfazClaraBienvenida)
            {
                //Almacenamos el tipo de interfaz en la configuracion del programa
                Properties.Settings.Default.tipoInterfaz = "Claro";
                AplicarEstilo();
                
            }
            else if (comboBox.SelectedItem == interfazOscuraBienvenida)
            {
                //Almacenamos el tipo de interfaz en la configuracion del programa
                Properties.Settings.Default.tipoInterfaz = "Oscuro";
                AplicarEstilo();

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (tabControl.SelectedIndex != 7)
            {
                
                if (MessageBox.Show(RecursosLocalizables.StringResources.mensajeSalir,RecursosLocalizables.StringResources.confirmarSalir,MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                    cerrar = 1;
                    cerradaAntesDeTiempo = 1;
                    menuPrincipal.Close();
                    
                }
                else
                {
                    e.Cancel = true;
                }
                
            }
        }
       

        private void escribirCegueraBienvenida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }


        private void escribirCeguera2Bienvenida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void PesoBienvenida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void letraPequeña_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.tamanoLetra == "M")
            {
                AplicarTamanoLetra(-3);
            }
            else if (Properties.Settings.Default.tamanoLetra == "G")
            {
                AplicarTamanoLetra(-6);
            }
            Properties.Settings.Default.tamanoLetra = "P";
        }

        private void letraMediana_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.tamanoLetra == "P")
            {
                AplicarTamanoLetra(3);
            }
            else if (Properties.Settings.Default.tamanoLetra == "G") {
                AplicarTamanoLetra(-3);
            }
            Properties.Settings.Default.tamanoLetra = "M";

        }

        private void letraGrande_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.tamanoLetra == "P")
            {
                AplicarTamanoLetra(6);
            }
            else if (Properties.Settings.Default.tamanoLetra == "M")
            {
                AplicarTamanoLetra(3);
            }
            Properties.Settings.Default.tamanoLetra = "G";

        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            valor=(int)slider.Value;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sensor.ElevationAngle = valor;
            }catch(System.InvalidOperationException exc)
            {
                Error err = new VME.Error("Error la kinect no puede ser movida tantas veces simultáneamente, debe esperar a que el movimiento anterior termine.", 0);
                err.Show();
            }
        }

        private void NacimientoBienvenida_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void importarDatos_Click(object sender, RoutedEventArgs e)
        {
            ImportarDatos qrImport = new ImportarDatos(this);
            qrImport.Show();
        }

        private void microfono_click(object sender, RoutedEventArgs e)
        {
            BitmapImage bi3 = new BitmapImage();
            if (tabControl.SelectedIndex != 0)
            {
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
                    
            }else
            {
                 Error err = new VME.Error(RecursosLocalizables.StringResources.IdiomaVoz, 3);
                 err.Show();
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
                            opciones.Add(RecursosLocalizables.StringResources.siguiente, "DOS");
                            opciones.Add(RecursosLocalizables.StringResources.atras, "DOS");
                            //En esta linea "windows ocho" es el valor de opcion y "TRES" es la llave y asi sucesivamente
                            opciones.Add(RecursosLocalizables.StringResources.movilidadBienvenida,"UNO");
                            opciones.Add(RecursosLocalizables.StringResources.movilidad2Bienvenida,"UNO");
                            opciones.Add(RecursosLocalizables.StringResources.movilidad3Bienvenida,"UNO");
                            opciones.Add(RecursosLocalizables.StringResources.movilidad4Bienvenida,"UNO");
                            opciones.Add(RecursosLocalizables.StringResources.movilidad5Bienvenida,"UNO");
                            opciones.Add(RecursosLocalizables.StringResources.reguKine);                        
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
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.siguiente))
                {
                    siguienteBienvenida_Click(null, null);
                }else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.atras))
                {
                    atrasBienvenida_Click(null, null);
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.movilidad2Bienvenida) && tabControl.SelectedIndex == 4)
                {
                    movilidad2Bienvenida.IsChecked = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.movilidad3Bienvenida) && tabControl.SelectedIndex == 4)
                {
                    movilidad3Bienvenida.IsChecked = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.movilidad4Bienvenida) && tabControl.SelectedIndex == 4)
                {
                    movilidad4Bienvenida.IsChecked = true;
                }
                else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.movilidad5Bienvenida) && tabControl.SelectedIndex == 4)
                {
                    movilidad5Bienvenida.IsChecked = true;
                }else if (e.Result.Text.Equals(RecursosLocalizables.StringResources.reguKine) && tabControl.SelectedIndex==6)
                {
                    button_Click(null, null);
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

        private void tecladoPantalla_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("osk.exe");
        }
    }
}
