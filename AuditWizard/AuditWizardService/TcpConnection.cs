using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
//
using Layton.Common.Controls;

namespace Layton.AuditWizard.AuditWizardService
{
    public delegate void TcpMessageReceived(TcpConnection sender, string message);

    /// <summary>
    /// The TcpConnection class encapsulates the functionality of a TcpClient connection
    /// with streaming for a single client.
    /// </summary>
    public class TcpConnection
    {
        //private const string messageDelimiter = "\r\n\r\n";
        private const string messageDelimiter = "***EOF#123***";
        private const int READ_BUFFER_SIZE = 4096;
        private TcpClient client;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
        private string ipAddress;
        private string splitMessage;
        private string hostName;

        public event TcpMessageReceived TcpMessageReceived;

        public TcpConnection(TcpClient client, string ipAddress, string hostName)
        {
            this.client = client;
            this.ipAddress = ipAddress;
            this.hostName = hostName;
            this.splitMessage = "";
            
            // This starts the asynchronous read thread.  The data will be saved into readBuffer.
            this.client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
        }

        public TcpClient TcpClient
        {
            get { return client; }
        }

        /// <summary>
        /// The host name of this client.
        /// </summary>
        public string HostName
        {
            get { return hostName; }
        }

        /// <summary>
        /// The IP Address of this client.
        /// </summary>
        public string IpAddress
        {
            get { return ipAddress; }
        }

        /// <summary>
        /// This subroutine uses a StreamWriter to send a message to the user.
        /// </summary>
        /// <param name="Data">The message to send to the client.</param>
        public void SendData(string message)
        {
            try
            {
                //lock ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    StreamWriter writer = new StreamWriter(client.GetStream());
                    writer.Write(message + '\0');
                    // Make sure all data is sent now.
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
				System.Diagnostics.EventLog.WriteEntry("AuditWizardService", "Error send message to " + ipAddress + ": " + e.Message, System.Diagnostics.EventLogEntryType.Warning); 
            }
        }

        /// <summary>
        /// This is the callback function for TcpClient.GetStream.Begin. It begins an 
        /// asynchronous read from a stream.
        /// </summary>
        /// <param name="ar"></param>
		//private void StreamReceiver(IAsyncResult ar)
		//{
		//    // Get the network stream
		//    NetworkStream networkStream = client.GetStream();
        
		//    // The only communications the server gets is for file transfers from Audit clients.  As such if we get here
		//    // we know that we are receiving an audit file to upload.  This is sent in two stages
		//    //
		//    // 1> An identifier '***ADF***' is sent
		//    // 2> The name of the asset is sent ***ASSET***
		//    // 3> the size of the file is sent 
		//    // 4> The contents of the file itself are sent
		//    int bytesRead;
		//    String completeMessage = "";

		//    try
		//    {
		//        // Ensure that no other threads try to use the stream at the same time.
		//        lock (networkStream)
		//        {
		//            // Finish asynchronous read into readBuffer and get number of bytes read.
		//            bytesRead = networkStream.EndRead(ar);

		//            // ...add to the buffer read
		//            completeMessage = String.Concat(completeMessage, Encoding.ASCII.GetString(readBuffer, 0, bytesRead));

		//            // message received may be larger than buffer size so loop through until you have it all.
		//            while (networkStream.DataAvailable)
		//            {

		//                networkStream.BeginRead(readBuffer, 0, readBuffer.Length
		//                                        , new AsyncCallback(StreamReceiver)
		//                                        , readBuffer);
		//            }
                
		//            // We have a complete message now - check to ensure that it complies with the required format
		//            ProcessAuditReceived(completeMessage);
				
		//            // Re-establish an asynchronous read on this stream to receive new messages				
		//            client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
		//        }
		//    }
		//    catch (Exception e)
		//    {
		//        System.Diagnostics.EventLog.WriteEntry("AuditWizardService", "Error reading from " + ipAddress + ": " + e.Message, System.Diagnostics.EventLogEntryType.Warning);
		//        TcpClient.Close();
		//    }
		//}



		/// <summary>
		/// This is the callback function for TcpClient.GetStream.Begin. It begins an 
		/// asynchronous read from a stream.
		/// </summary>
		/// <param name="ar"></param>
		private void StreamReceiver(IAsyncResult ar)
		{
			LogFile ourLog = LogFile.Instance;
			int bytesRead;
            try
            {
                // Ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    // Finish asynchronous read into readBuffer and get number of bytes read.
                    bytesRead = client.GetStream().EndRead(ar);
				}
                // Convert the byte array the message was saved into
                splitMessage += Encoding.ASCII.GetString(readBuffer, 0, bytesRead);

                while (splitMessage.Contains(messageDelimiter))
                {
					int endOfMessageIndex = splitMessage.IndexOf(messageDelimiter);
                    string message = splitMessage.Substring(0, endOfMessageIndex);
					ourLog.Write("StreamReceiver:: Incoming AUDIT Received, file length is " + message.Length.ToString(), true);
					if (TcpMessageReceived != null)
                        TcpMessageReceived(this, message);
                }

                // Ensure that no other threads try to use the stream at the same time.
                lock (client.GetStream())
                {
                    // Start a new asynchronous read into readBuffer.
                    client.GetStream().BeginRead(readBuffer, 0, READ_BUFFER_SIZE, new AsyncCallback(StreamReceiver), null);
                }
            }
            catch (Exception e)
            {
				System.Diagnostics.EventLog.WriteEntry("AuditWizardService", "Error reading from " + ipAddress + ": " + e.Message, System.Diagnostics.EventLogEntryType.Warning);
                TcpClient.Close();
            }
		}


    }
}