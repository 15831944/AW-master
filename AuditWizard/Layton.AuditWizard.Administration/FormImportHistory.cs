using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
//
using Layton.Common.Controls;
using Layton.AuditWizard.Common;
using Layton.AuditWizard.DataAccess;

namespace Layton.AuditWizard.Administration
{
	public partial class FormImportHistory : Form
	{
		private string _title = "Progress";
		private string _subtitle = "Current Progress...";
        private string _importFile;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string Title
		{ set { _title = value; } }

		public string SubTitle
		{ set { _subtitle = value; } }

		public FormImportHistory(string importFile)
		{
			InitializeComponent();
			_importFile = importFile;
		}

		private void FormProgress_Paint(object sender, PaintEventArgs e)
		{
			LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 100), Color.FromArgb(228, 228, 235), Color.White);
			e.Graphics.FillRectangle(brush, new Rectangle(0, 0, Width, 100));
		}

		private void FormProgress_Load(object sender, EventArgs e)
		{
			this.Text = _title;
			labelTitle.Text = _subtitle;

			// Start the worker thread to perform tha ctual task
			backgroundWorker1.RunWorkerAsync();
		}


		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			int count = 0;
			int total = 0;
			try
			{
				CSVReader csvCounter = new CSVReader(_importFile);
				total = csvCounter.LineCount();
				csvCounter.Dispose();

				using (CSVReader csv = new CSVReader(_importFile))
				{
					// Read each line from the file noting that we MUST have 7 columns and 7 only
					// when importing a History file
					string[] fields;
					while ((fields = csv.GetCSVLine()) != null)
					{
						if (fields.Length != 7)
							continue;

						// OK - get the data from the line 
						backgroundWorker_ImportHistoryLine(fields);
					
						// Count this and update the progress bar
						count++;
						int percent = (int)(((float)count / (float)total) * 100);
						backgroundWorker1.ReportProgress(percent, null);
					}
				}		
			}
			
			catch (Exception)
			{
				e.Result = -1;
				return;
			}
			e.Result = total;
		}

		
		private void backgroundWorker_ImportHistoryLine(string[] fields)
		{
			// Does the Asset exist?
			Asset newAsset = new Asset();
			newAsset.Name = fields[0];
			//
			AssetDAO lwDataAccess = new AssetDAO();
			newAsset.AssetID = lwDataAccess.AssetFind(newAsset);
			
			// If the asset exists then add the history record for it
			if (newAsset.AssetID != 0)
			{
				// Create an Audit Trail Entry record based on the data passed in to us.
				try
				{
                    AuditTrailDAO auditTrailDAO = new AuditTrailDAO();

					AuditTrailEntry ate = new AuditTrailEntry();
					//ate.Date = DateTime.ParseExact(fields[1], "yyyy-MM-dd HH:mm:ss", null);
                    ate.Date = Convert.ToDateTime(fields[1]);
					ate.Class = AuditTrailEntry.CLASS.asset;
					ate.AssetID = newAsset.AssetID;
					ate.AssetName = newAsset.Name;
					ate.Type = AuditTrailEntry.TranslateV7Type((AuditTrailEntry.V7_HIST_OPS)Convert.ToInt32(fields[2]));
					ate.Key = fields[3];
					ate.OldValue = fields[4];
					ate.NewValue = fields[5];
					ate.Username = fields[6];
                    auditTrailDAO.AuditTrailAdd(ate);
				}
				catch (Exception ex)
				{
                    logger.Error(ex.Message);
				}
			}
		}


		private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBar.Value = e.ProgressPercentage;
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
            //bnOK.Enabled = true;
            MessageBox.Show("Import complete." + Environment.NewLine + Environment.NewLine + 
                e.Result.ToString() + " history records were imported.", "AuditWizard");
            Close();
		}
	}
}