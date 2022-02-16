using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AtDrive_Assignment
{
    public partial class MainWindow : Window
    {
        private Rectangle ballRec;
        private Canvas ballCanvas;

        private int XValues;
        private int YValues;
        private string speed;
        private bool isImageLoaded = true;
        private string backgroundImageName;
        private string forgroundImageName;
        private int fps = 10;
        private bool takeImage;

        Microsoft.Win32.OpenFileDialog openFileDlg;

        int heightPerFram;
        int currentHeight = 0;
        int currentFram = 0;

        string path = "C:\\Users\\91777\\OneDrive\\Desktop\\atDrive\\Output\\file.gif";
        GifBitmapEncoder gif = new GifBitmapEncoder();

        public MainWindow()
        {
            InitializeComponent();

            openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.DefaultExt = ".png";
            openFileDlg.Filter = "Image documents (.png)|*.png";

            BackgroundBrowse.Click += BrowseFile;
            ForgroundBrows.Click += BrowseFile;

            btnStart.Click += OnStartClick;
            btnStart.Click += LoadButton_Click;
            CompositionTarget.Rendering += Update;
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Int32.TryParse(SpeedYTextBox.Text, out YValues);
            heightPerFram = (int)YValues / fps;
            currentFram = 0;
            currentHeight = 0;

            Console.WriteLine("Canvas:" + bgImg.ActualWidth + " He: " + bgImg.ActualHeight);

            if(isImageLoaded)
            takeImage = true;
           
        }

        private void Update(object sender, EventArgs e)
        {
           
            if (takeImage)
            {
                Console.WriteLine("Taken ----------------------------");

                OnStartAnimation();

                currentFram++;

                if (currentFram >= fps)
                {
                    Console.WriteLine("Gathered Frame: " + gif.Frames.Count);
                    FileStream saveStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                    gif.Save(saveStream);
                    saveStream.Close();
                    takeImage = false;
                }
               
            }
        }

        private void OnStartAnimation()
        {
            int direction = 2;
            if (currentFram > fps / 2) direction = -2;
            currentHeight += heightPerFram * direction;

            Console.WriteLine("Height: " + currentHeight);
            Canvas.SetBottom(ballImg, currentHeight);

            var rtb = new RenderTargetBitmap((int)(bgImg.ActualWidth*1.5), (int)(bgImg.ActualHeight*1.5), 96, 96, PixelFormats.Pbgra32);
            rtb.Render(bgImg);

            BitmapFrame singleFrame = BitmapFrame.Create(rtb);

            gif.Frames.Add(singleFrame);
        }

        

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            FrameworkElement arg = e.Source as FrameworkElement;

            bool? result = openFileDlg.ShowDialog();
            if (result.HasValue && result.Value)
            {
                if(arg.Name == "ForgroundBrows")
                {
                    forgroundImageName = openFileDlg.FileName;
                    fgTextBox.Text = forgroundImageName.Split('\\').Last();
                }
                else
                {
                    backgroundImageName = openFileDlg.FileName;
                    bgTextBox.Text = backgroundImageName.Split('\\').Last();
                }
            }
        }



        public void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (forgroundImageName != null && backgroundImageName != null)
            {
                ImageBrush Mainbrush = new ImageBrush();
                ImageBrush Innerbrush = new ImageBrush();
                Mainbrush.ImageSource = new BitmapImage(new Uri(@backgroundImageName, UriKind.Relative));
                bgImg.Background = Mainbrush;
                //bgImg.Source = 

                Innerbrush.ImageSource = new BitmapImage(new Uri(@forgroundImageName, UriKind.Relative));
                //  bgImg.Source = new BitmapImage(new Uri(@backgroundImageName, UriKind.Relative)); 
                ballImg.Fill = Innerbrush;
                isImageLoaded = true;
            }
            else
            {
                MessageBox.Show("Both file should be added");

            }
        }

        private void SpeedComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)SpeedComboBox.SelectedItem;
            speed = cbi.Content.ToString();
         }

       
    }
}
