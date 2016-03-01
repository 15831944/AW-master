using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Xml;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections;
//
using Layton.AuditWizard.Common;
using Layton.Common.Controls;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.AuditWizardService
{
	public class TcpUploadController : MarshalByRefObject
	{
		#region Win32

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct LOCALGROUP_USERS_INFO_0
		{
			public string groupname;
		}

		[DllImport("Netapi32.dll", SetLastError = true)]
		public extern static int NetUserGetGroups
			([MarshalAs(UnmanagedType.LPWStr)] string servername,
			 [MarshalAs(UnmanagedType.LPWStr)] string username,
			 int level,
			 out IntPtr bufptr,
			 int prefmaxlen,
			 out int entriesread,
			 out int totalentries);

		[DllImport("Netapi32.dll", SetLastError = true)]
		public extern static int NetGetAnyDCName(string server, string domain, out IntPtr bufptr);

		[DllImport("Netapi32.dll", SetLastError = true)]
		public static extern int NetApiBufferFree(IntPtr Buffer);

		#endregion

		private const int port = 31730;
		private Hashtable clients = new Hashtable();
		private TcpListener listener;
		private Thread listenerThread;
		private bool isRunning = false;
		private AuditWizardService _service;
		
		public TcpUploadController(AuditWizardService service)
		{
			_service = service;
		}

		public override Object InitializeLifetimeService()
		{
			return null; // makes the object live indefinitely
		}

		/// <summary>
		/// Starts the NamedPipes server which listens for messages from connecting clients
		/// </summary>
		public void Start()
		{
			listenerThread = new Thread(new ThreadStart(StartTcpListener));
			listenerThread.Start();
			isRunning = true;
		}

		private void StartTcpListener()
		{
			LogFile ourLog = LogFile.Instance;
		
			try
			{
				// Listen for new connections.
				listener = new TcpListener(System.Net.IPAddress.Any, port);
				listener.Start();

				EventLog.WriteEntry("AuditWizardService", "Starting AuditWizard TCP Server on port " + port.ToString(), EventLogEntryType.Information);

				while (true)
				{
					// Create a new user connection using the TcpClient returned by TcpListener.AcceptTcpClient()
					TcpClient tcpClient = listener.AcceptTcpClient();
					IPAddress ipAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

					// Try and translate this to a host name as this is what AuditWizard knows...
					// If we get an error here then we have no choice other than to discard the client connection request
					// as we cannot identify the client
					IPHostEntry host = null;

					try
					{
						host = Dns.GetHostEntry(ipAddress);
					}
					catch (Exception e)
					{
						ourLog.Write("Failed to translate an incoming connection request from : " + ipAddress.ToString() + Environment.NewLine + " Reason: " + e.Message + Environment.NewLine + "This client connection request will be discarded" ,true);
					}

					// If we did not translate the client IP address to its host name then ignore this request
					if (host == null)
					{
						tcpClient.Close();
						continue;
					}

					// all ok so we can continue to process the connection request
					string[] hostDetails = host.HostName.Split('.');
					string hostName = hostDetails[0].ToUpper();

					// check if the hostname is just an ip address
					if (hostDetails.Length > 3)
					{
						// set it to the ip address
						hostName = host.HostName;
					}

					/// ...and create a new TCP connection for this client
					TcpConnection client = new TcpConnection(tcpClient, ipAddress.ToString(), hostName);

					// Add the new client to the client HashTable
					if (clients.Contains(hostName))
						clients.Remove(hostName);

					clients.Add(hostName, client);

					// Create an event handler for the TcpConnection to handle the message received
					client.TcpMessageReceived += new TcpMessageReceived(client_TcpMessageReceived);

					// Log that we have now accepted the connection
					ourLog.Write("Accepted client connection from " + hostName + " (" + ipAddress + ")", true);
				}
			}

			catch (Exception e)
			{
				String message = "Stopping TCP Server due to the following: " + Environment.NewLine + e.Message;
				ourLog.Write(message ,true);
				EventLog.WriteEntry("AuditWizardService", message , EventLogEntryType.Warning);
				Stop();
			}
		}



		/// <summary>
		/// This function is called when we have received a TCP message - the message should be an ADF file transmitted
		/// as the result of an audit completing.  We need to write this message to a temporary file so that we can upload
		/// it using the standard upload mechanisms
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="message"></param>
		private void client_TcpMessageReceived(TcpConnection sender, string message)
		{
			LogFile ourLog = LogFile.Instance;
			try
			{
				HandleClientMessage(sender, message);
			}
			catch (Exception e)
			{
				ourLog.Write("Error handling message: " + e.Message, true);
			}
		}

		/// <summary>
		/// Stops the TcpLister server, and cleans up resources.
		/// </summary>
		public void Stop()
		{
			try
			{
				if (isRunning)
				{
					// Stop the TcpListener server connection
					listener.Stop();
					listener = null;
					EventLog.WriteEntry("AuditWizardService", "AuditWizard TCP Server has stopped.", EventLogEntryType.Information);
				}
				isRunning = false;
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("AuditWizardService", "Error stopping AuditWizard TCP Server:" + Environment.NewLine + e.Message, EventLogEntryType.Information);
			}
		}


		/// <summary>
		/// Handle a message coming in from an audited client - this should be an ADF file streamed
		/// </summary>
		/// <param name="message">Message received from the TCP server</param>
		private void HandleClientMessage(TcpConnection sender, string message)
		{
			// ...and try and upload this as an audit file
			LogFile ourLog = LogFile.Instance;
			ourLog.Write("Uploading Audit File received as an incoming TCP Stream from " + sender.HostName, true);
			
			// clean up the message, so it can be parsed by XmlDocument.LoadXml
			message = message.Trim();
			message = message.Remove(message.LastIndexOf('>') + 1, message.Length - message.LastIndexOf('>') - 1);

			// Create an XmlTextReader and populate it with the string which should be the XML text
			XmlTextReader xmlTextReader = new XmlTextReader(new System.IO.StringReader(message));
			
			// Create an AuditDataFile from the stream
			AuditDataFile auditDataFile = new AuditDataFile();
			auditDataFile.Read(xmlTextReader);
			
			// Create an uploader
			AuditUploader auditUploader = new AuditUploader("", _service.LicenseCount);
			
			// ...and upload the file		
			auditUploader.UploadAuditDataFile(auditDataFile);
		}
	}
}