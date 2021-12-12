using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace p7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private readonly Socket client;

		public MainWindow()
        {
			Socket cSocket = Connect();
			client = Accept(cSocket);
			InitializeComponent();
		}
		
		// Méthode qui initialise la barre de progression 
		void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			for (int i = 1; i <= 100; i++)
			{
				(sender as BackgroundWorker).ReportProgress(i);
				
				Thread.Sleep(2000);
			}
		}

		void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{   
			//initialisation de la barre de progression avec le pourcentage de progression
			pbstatus1.Value   = e.ProgressPercentage;

			client.Send(BitConverter.GetBytes(e.ProgressPercentage));

			//Affichage de la progression sur un label
			lb_etat_prog_server.Content = pbstatus1.Value.ToString() + "%";
		}

		// lancer la barre de progression en créant un objet de type BackgroundWorker
		//BackgroundWorker :
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//création, initialisation et mise à jour de l'objet BackgroundWorker
			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerAsync();
		}

		private static Socket Connect()
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
			Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(localEndPoint);
			listener.Listen(100);
			return listener;
		}

		private static Socket Accept(Socket socket)
		{
			return socket.Accept();
		}
	}
}
