using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
//
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Common
{
	public class EmailConfiguration
	{
		#region Data

		private string _sender;
		private List<string> _listRecipients = new List<string>();
		private int _port;
		private string _server;
		private bool _requiresAuthentication;
		private string _userName;
		private string _password;
        private bool _sSLEnabled; // Added for ID 66125/66652

		#endregion Data

		#region Properties

		public List<string> ListRecipients
		{
			get { return _listRecipients; }
			set { _listRecipients = value; }
		}

		public string Recipients
		{
			get { return Utility.ListToString(_listRecipients, ';'); }
		}

		public int Port
		{
			get { return _port; }
			set { _port = value; }
		}

		public string Sender
		{
			get { return _sender; }
			set { _sender = value; }
		}

		public string Server
		{
			get { return _server; }
			set { _server = value; }
		}

		public bool RequiresAuthentication
		{
			get { return _requiresAuthentication; }
			set { _requiresAuthentication = value; }
		}

		public string UserName
		{
			get { return _userName; }
			set { _userName = value; }
		}

		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

        /// <summary>
        /// Added for ID 66125/66652
        /// </summary>
        public bool SSLEnabled
        {
            get { return _sSLEnabled; }
            set { _sSLEnabled = value; }
        }
		#endregion Properties

		#region Constructor


		/// <summary>
		/// Primary constructor loads the Email configuration from the database
		/// </summary>
		public EmailConfiguration()
		{
			// Get the database object
			SettingsDAO lwDataAccess = new SettingsDAO();

			// List of email recipients
			string recipientList = lwDataAccess.GetSetting(MailSettingsKeys.MailAddress, false);
			if (recipientList != null)
			{
				string[] recipients = recipientList.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string recipient in recipients)
				{
					this.ListRecipients.Add(recipient);
				}
			}

			// Get the email configuration from the database
			Sender = lwDataAccess.GetSetting(MailSettingsKeys.MailSender, false);

			// It's possible that the Port hasn't been set so this conversion could fail
			try
			{
				Port = Convert.ToInt32(lwDataAccess.GetSetting(MailSettingsKeys.MailPort, false));
			}
			catch (Exception)
			{
				Port = 23;
			}
			Server = lwDataAccess.GetSetting(MailSettingsKeys.MailServer, false);
			RequiresAuthentication = lwDataAccess.GetSettingAsBoolean(MailSettingsKeys.MailRequiresAuthentication, false);
			UserName = lwDataAccess.GetSetting(MailSettingsKeys.MailUserName, false);
			Password = lwDataAccess.GetSetting(MailSettingsKeys.MailPassword, true);
            SSLEnabled = lwDataAccess.GetSettingAsBoolean(MailSettingsKeys.MailSSLEnabled, false); //Added for ID 66125/66652
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Return a simple status to indicate whether or not the Email configuration is valid.
		/// 
		/// To be valid we must have a sender and recipient Email address and the Mail Server
		/// </summary>
		/// <returns>true if valid email configuration</returns>
		public bool IsValid()
		{
			if ((_listRecipients.Count == 0)				// If no recipients
			|| (_sender == "")								// or no sender
			|| (_server == "")								// or no Server
			|| (_port == 0)									// or Port set to 0
			|| ((_requiresAuthentication == true) && (_userName == "")))
															// or requires authentication but no user
				return false;
			else
				return true;
		}



		/// <summary>
		/// Send an email usingthis configuration
		/// </summary>
		/// <param name="message"></param>
		/// <param name="body"></param>
		/// <param name="IsHtml"></param>
		/// <returns></returns>
		public void SendEmail(string subject, string body, bool isHtml)
		{
			// Create the Mail Message
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(_sender);

			// Add the recipients
			foreach (string recipient in _listRecipients)
			{
				mailMessage.To.Add(new MailAddress(recipient));
			}

			// Other mail configuration
			mailMessage.Subject = subject;
			mailMessage.Body = body;
			mailMessage.IsBodyHtml = isHtml;

			// Create the SmtpClient object
			SmtpClient emailClient = new SmtpClient(_server, _port);
            emailClient.EnableSsl = SSLEnabled; // Added for ID 66125/66652
			if (_requiresAuthentication)
			{
				NetworkCredential credentials = new NetworkCredential(_userName ,_password);
				emailClient.Credentials = credentials;
			}

			// Send the email
			emailClient.Send(mailMessage);
		}

        public void SendEmail(string subject, string body, List<string> attachmentFileName)
        {
            // Create the Mail Message
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_sender);

            // Add the recipients
            foreach (string recipient in _listRecipients)
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            // Other mail configuration
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = false;

            foreach (string fileName in attachmentFileName)
            {
                mailMessage.Attachments.Add(new Attachment(fileName));
            }

            // Create the SmtpClient object
            SmtpClient emailClient = new SmtpClient(_server, _port);
            emailClient.EnableSsl = SSLEnabled; // Added for ID 66125/66652
            if (_requiresAuthentication)
            {
                NetworkCredential credentials = new NetworkCredential(_userName, _password);
                emailClient.Credentials = credentials;
            }

            // Send the email
            emailClient.Send(mailMessage);
        }


		#endregion Methods
	}
}
