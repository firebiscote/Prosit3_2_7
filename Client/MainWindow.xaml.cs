using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Socket sSocket;

        public MainWindow()
        {
            sSocket = Connect();
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            worker.DoWork += Listen;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private static Socket Connect()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(remoteEP);
            return client;
        }

        private void Listen(object sender, DoWorkEventArgs e)
        {
            int data;
            while (true)
            {
                byte[] bytes = new byte[1024];
                sSocket.Receive(bytes);
                data = BitConverter.ToInt32(bytes, 0);
                (sender as BackgroundWorker).ReportProgress(data);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //initialisation de la barre de progression avec le pourcentage de progression
            pbstatus1.Value = e.ProgressPercentage;
            //Affichage de la progression sur un label
            lb_etat_prog_server.Content = pbstatus1.Value.ToString() + "%";
        }
    }
}
