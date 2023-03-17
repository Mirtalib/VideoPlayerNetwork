using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient client;
        IPEndPoint remoteEP;
        bool stop = false;
        public MainWindow()
        {
            InitializeComponent();
            client = new UdpClient();
            remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4678);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            stop = false;
            btnStop.Visibility = Visibility.Visible;
            btnStart.Visibility = Visibility.Collapsed;
            var buffer = new byte[ushort.MaxValue - 29];
            await client.SendAsync(buffer, buffer.Length, remoteEP);
            var list = new List<byte>();
            var maxLen = buffer.Length;
            var len = 0;
            while (true)
            {
                do
                {
                    try
                    {
                        var result = await client.ReceiveAsync();
                        buffer = result.Buffer;
                        len = buffer.Length;
                        list.AddRange(buffer);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                } while (len == maxLen);
                var image = GetImage(list.ToArray());
                if (image != null)
                    Image1.Source = image;

                if (stop)
                    break;
                list.Clear();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            stop = true;
            btnStop.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Visible;
        }

        private static BitmapImage GetImage(byte[] imageInfo)
        {
            var image = new BitmapImage();

            using (var memoryStream = new MemoryStream(imageInfo))
            {
                memoryStream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }

    }
}
