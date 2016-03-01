using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Layton.AuditWizard.DataAccess;
using System.IO;
using System.Xml.Serialization;

namespace Layton.AuditWizard.Reports
{
    
    public partial class FormAppByLocation : Form
    {
        public class ApplicationItems
        {
            private string m_strManufacturer;

            public string Manufacturer
            {
                get { return m_strManufacturer; }
                set { m_strManufacturer = value; }
            }
            private string m_strApplicationName;

            public string ApplicationName
            {
                get { return m_strApplicationName; }
                set { m_strApplicationName = value; }
            }
            public ApplicationItems()
            {
                m_strManufacturer = "";
                m_strApplicationName = "";
            }
        };

        [Serializable()]

        public class SelectionItem
        {
            private string m_ItemName;

			// 8.3.4 - CMD - Flags to indicate ALL items selected
			private bool _allApplications;
			private bool _allLocations;
            
			public string ItemName
            {
                get { return m_ItemName; }
                set { m_ItemName = value; }
            }
            private List<ApplicationItems> m_ApplicationItemList;

			public bool AllApplications
			{
				get { return _allApplications; }
				set { _allApplications = value; }
			}

			public bool AllLocations
			{
				get { return _allLocations; }
				set { _allLocations = value; }
			}

            public List<ApplicationItems> ApplicationItemList
            {
                get { return m_ApplicationItemList; }
                set { m_ApplicationItemList = value; }
            }
            private List<string> m_LocationNames;

            public List<string> LocationNames
            {
                get { return m_LocationNames; }
                set { m_LocationNames = value; }
            }
            public SelectionItem()
            {
                m_ItemName = "";
                m_ApplicationItemList = new List<ApplicationItems>();
                m_LocationNames = new List<string>();
				_allApplications = _allLocations = false;
            }
        };

        [Serializable()]
        public class SQLProfileManager
        {
            private List<SelectionItem> m_SQLProfileItemList;

            public List<SelectionItem> SQLProfileItemList
            {
                get { return m_SQLProfileItemList; }
                set { m_SQLProfileItemList = value; }
            }
            public SQLProfileManager()
            {
                m_SQLProfileItemList = new List<SelectionItem>();
            }
           
        };

        private SQLProfileManager m_SQLProfileManager;

        public SQLProfileManager SQLProfileListManager
        {
            get { return m_SQLProfileManager; }
            set { m_SQLProfileManager = value; }
        }

        private SelectionItem     m_LastSavedSelectionItem;

        public SelectionItem LastSavedSelectionItem
        {
            get { return m_LastSavedSelectionItem; }
            set { m_LastSavedSelectionItem = value; }
        }

        private bool m_bSaveAsUserDefinedReport = false;

        public bool SaveAsUserDefinedReport
        {
            get { return m_bSaveAsUserDefinedReport; }
            set { m_bSaveAsUserDefinedReport = value; }
        }

        private string m_strUserDefinedReportName = "";

        public string UserDefinedReportName
        {
            get { return m_strUserDefinedReportName; }
            set { m_strUserDefinedReportName = value; }
        }
        private string AW_LOCAL_SELECTION_XML = "AWLocalSelection.xml";
        private string AW_LOCAL_SELECTION_LIST_XML = "AWLocalSelectionList.xml";

#if TEST
        private List<string> m_SelectedApplicationsList = new List<string>();


        public List<string> SelectedApplicationsList
        {
            get { return m_SelectedApplicationsList; }
            set { m_SelectedApplicationsList = value; }
        }

        private List<string> m_SelectedLocationsList = new List<string>();

        public List<string> SelectedLocationsList
        {
            get { return m_SelectedLocationsList; }
            set { m_SelectedLocationsList = value; }
        }
#endif

        public FormAppByLocation()
        {
            m_SQLProfileManager = new SQLProfileManager();
            m_LastSavedSelectionItem = new SelectionItem();
            m_LastSavedSelectionItem.ItemName = "Last Selected Items";
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (checkBoxSave.Checked == true)
            {
                if (textBoxUserDefinedReportName.Text == "")
                {
                    MessageBox.Show("Please Enter the Report Name..");
                    textBoxUserDefinedReportName.Focus();
                    return;
                }
                DataTable lExistingReports = new ReportsDAO().GetReportsByTypeAndName(ReportsDAO.ReportType.SqlReport, textBoxUserDefinedReportName.Text);

                if (lExistingReports.Rows.Count > 0)
                {
                    MessageBox.Show("A SQL report already exists with this name. Please change the Report Name.");
                    textBoxUserDefinedReportName.Focus();
                    return;
                }
                SaveAsUserDefinedReport = true;
                UserDefinedReportName = textBoxUserDefinedReportName.Text;

            }
            this.Cursor = Cursors.WaitCursor;

            LastSavedSelectionItem.ApplicationItemList.Clear();
            LastSavedSelectionItem.LocationNames.Clear();
			LastSavedSelectionItem.AllApplications = false;
			LastSavedSelectionItem.AllLocations = false;

			// Check for all applications being selected and if so set the relevent flag, otherwise add the selected applications				// 8.3.4 - CMD
			if (treeViewApplications.Nodes[0].Checked)
			{
				LastSavedSelectionItem.AllApplications = true;
			}
			else
			{
				for (int i = 0; i < treeViewApplications.Nodes.Count; i++)
				{
					AddAllCheckedChildNodes(treeViewApplications.Nodes[i], true);
				}
			}

			// Same for Locations - ensure we capture the Select All situation																	// 8.3.4 - CMD
			if (treeViewLocations.Nodes[0].Checked)
			{
				LastSavedSelectionItem.AllLocations = true;
			}
			else
			{
	            for (int i = 0; i < treeViewLocations.Nodes.Count; i++)
		        {
			        AddAllCheckedChildNodes(treeViewLocations.Nodes[i], false);
				}
			}

			if ((!LastSavedSelectionItem.AllApplications) && (LastSavedSelectionItem.ApplicationItemList.Count == 0))
            {
                MessageBox.Show("Please check at least one Application..");
                this.Cursor = Cursors.Default;
                return;
            }

			if ((!LastSavedSelectionItem.AllLocations) && (LastSavedSelectionItem.LocationNames.Count == 0))
            {
                MessageBox.Show("Please check at least one Location...");
                this.Cursor = Cursors.Default;
                return;
            }           

            if (checkBoxSave.Checked == true)
            {
                LoadAllSavedSQLProfiles();

                SelectionItem NewItem = new SelectionItem();

                NewItem.ItemName = UserDefinedReportName;
				NewItem.AllApplications = LastSavedSelectionItem.AllApplications;
				NewItem.AllLocations = LastSavedSelectionItem.AllLocations;
                NewItem.ApplicationItemList = LastSavedSelectionItem.ApplicationItemList;
                NewItem.LocationNames = LastSavedSelectionItem.LocationNames;

                SQLProfileListManager.SQLProfileItemList.Add(NewItem);
            }

            DialogResult = DialogResult.OK;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            this.Close();
        }


		/// <summary>
		/// Called as this form is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void FormAppByLocation_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable AppTable = new StatisticsDAO().PerformQuery("SELECT DISTINCT APPLICATIONS._NAME, APPLICATIONS._PUBLISHER FROM APPLICATIONS ORDER BY APPLICATIONS._PUBLISHER");
                string strCurrentVendor = "";
                TreeNode AWNewTreeNode = null;
                for (int i = 0; i < AppTable.Rows.Count; i++)
                {
                    if (strCurrentVendor != AppTable.Rows[i].ItemArray[1].ToString())
                    {
                        strCurrentVendor = AppTable.Rows[i].ItemArray[1].ToString();
                        AWNewTreeNode = treeViewApplications.Nodes["Applications"].Nodes.Add(strCurrentVendor);
                        AWNewTreeNode.Nodes.Add(AppTable.Rows[i].ItemArray[0].ToString());

                    }
                    else
                    {
                        AWNewTreeNode.Nodes.Add(AppTable.Rows[i].ItemArray[0].ToString());
                    }
                }

                DataTable LocationTable = new StatisticsDAO().PerformQuery("SELECT LOCATIONS._NAME FROM LOCATIONS");

                for (int i = 0; i < LocationTable.Rows.Count; i++)
                {
                    treeViewLocations.Nodes["Locations"].Nodes.Add("Locations", LocationTable.Rows[i].ItemArray[0].ToString());
                }

                LoadTheLastSavedProfile();
                LoadAllItemsSelectionToTheTree();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void treeViewApplications_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }

        }

        private void treeViewLocations_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }

        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void AddAllCheckedChildNodes(TreeNode treeNode, bool bApplications)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    AddAllCheckedChildNodes(node, bApplications);
                }
                else
                {
                    if (node.Checked)
                    {
                        if (bApplications)
                        {
                            //SelectedApplicationsList.Add(node.Text);
                            ApplicationItems Items = new ApplicationItems();
                            Items.ApplicationName = node.Text;
                            Items.Manufacturer = node.Parent.Text;
                            LastSavedSelectionItem.ApplicationItemList.Add(Items);
                        }
                        else
                        {
                            LastSavedSelectionItem.LocationNames.Add(node.Text);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        private void checkBoxSave_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSave.Checked == true)
            {
                textBoxUserDefinedReportName.Enabled = true;

            }
            else
            {
                textBoxUserDefinedReportName.Enabled = false;
            }
        }

        private void LoadTheLastSavedProfile()
        {
            //Load last saved item selection..
            //
            LastSavedSelectionItem.ApplicationItemList.Clear();
            LastSavedSelectionItem.LocationNames.Clear();
            LoadDefaultSelection(AW_LOCAL_SELECTION_XML);
            //Check the Default selection items..                       

        }

        private void LoadAllItemsSelectionToTheTree()
        {
			// Check all applications if specified
			if (LastSavedSelectionItem.AllApplications)															// 8.3.4 - CMD
			{
				treeViewApplications.Nodes[0].Checked = true;
			}
			else
			{
				for (int i = 0; i < LastSavedSelectionItem.ApplicationItemList.Count; i++)
				{
					for (int j = 0; j < treeViewApplications.Nodes.Count; j++)
					{
						CheckSelectedApplicationNodes(treeViewApplications.Nodes[j], LastSavedSelectionItem.ApplicationItemList[i]);
					}
				}
			}

			// Check all locations if specified																	// 8.3.4 - CMD
			if (LastSavedSelectionItem.AllLocations)
			{
				treeViewLocations.Nodes[0].Checked = true;
			}
			else
			{
	            for (int i = 0; i < LastSavedSelectionItem.LocationNames.Count; i++)
		        {
			        for (int j = 0; j < treeViewLocations.Nodes.Count; j++)
				    {
					    CheckSelectedLocationNodes(treeViewLocations.Nodes[j], LastSavedSelectionItem.LocationNames[i]);
					}
				}
			}
		}


        private void CheckSelectedApplicationNodes(TreeNode treeNode, ApplicationItems Items)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckSelectedApplicationNodes(node, Items);
                }
                else
                {
                    if ((node.Parent.Text == Items.Manufacturer) && (node.Text == Items.ApplicationName))
                    {
                        node.Checked = true;
                        node.Parent.Checked = true;
                    }
                    
                }
            }
        }

        private void CheckSelectedLocationNodes(TreeNode treeNode, string strLocation)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckSelectedLocationNodes(node, strLocation);
                }
                else
                {
                    if (node.Text == strLocation)
                    {
                        node.Checked = true;
                        
                    }

                }
            }
        }

        public void LoadAllSavedSQLProfiles()
        {
            if (SQLProfileListManager.SQLProfileItemList.Count == 0)
            {
                LoadSQLSelectionProfileList(AW_LOCAL_SELECTION_LIST_XML);
            }
        }
        private void FillSQLProfileCombobox()
        {
            comboBoxReportProfiles.Items.Clear();

            comboBoxReportProfiles.Items.Add("Select a profile from the list...");
            
            for (int i = 0; i < SQLProfileListManager.SQLProfileItemList.Count; i++)
            {
                comboBoxReportProfiles.Items.Add(SQLProfileListManager.SQLProfileItemList[i].ItemName);
            }
            comboBoxReportProfiles.SelectedIndex = 0;

        }
        private void LoadSelectedSQLProfile(string strSelectedSQLProfile)
        {
            //LastSavedSelectionItem
            for (int i = 0; i < SQLProfileListManager.SQLProfileItemList.Count; i++)
            {
                if (SQLProfileListManager.SQLProfileItemList[i].ItemName == strSelectedSQLProfile)
                {
                    LastSavedSelectionItem = SQLProfileListManager.SQLProfileItemList[i];
                    break;
                }
            }
        }

        private void ResetTreeSelection()
        {
            foreach (TreeNode node in treeViewApplications.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, false);
                }
                else
                {
                    node.Checked = false;
                }
            }

            foreach (TreeNode node in treeViewLocations.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, false);
                }
                else
                {
                    node.Checked = false;
                }
            }
        
        }

        public void SaveItemSelection()
        {
            //Save all Item selection to a local file 
            //so that the next time we can reload it on start-up
            SaveSelection(AW_LOCAL_SELECTION_XML);

        }

        public void SaveAllSQLProfilesFromTheList()
        {
            SaveSQLProfileList(AW_LOCAL_SELECTION_LIST_XML);
        }

        public string GetAssemblyDirectory()
        {
            String StrPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(StrPath);

        }

        public bool SaveSelection(string strFileName)
        {

	        string strDataPath=GetAssemblyDirectory(); //Changed to Installed Directory TOM

	        strDataPath+="\\";

            Stream stream = null;

	        try
	        {
                //Opens a file and serializes the object into it in binary format.
                stream = File.Open(strDataPath+strFileName, FileMode.Create);
                //SoapFormatter^ formatter = gcnew SoapFormatter;

                XmlSerializer formatter = new XmlSerializer(LastSavedSelectionItem.GetType());


                //BinaryFormatter* formatter = new BinaryFormatter();
                formatter.Serialize(stream, LastSavedSelectionItem);
                stream.Close();

		        return true;
	        }
	        catch(Exception )
	        {
		        //System::Windows::Forms::MessageBox::Show(Ex->Message);
		        if(stream!=null)
		        {
			        stream.Close();
		        }
		        return false;
	        }
            return false;

        }

        private bool LoadDefaultSelection(string strFileName)
        {
	        //System::String^ strDataPath=Environment::GetFolderPath(Environment::SpecialFolder::ApplicationData);
	        string strDataPath=GetAssemblyDirectory(); //Changed to Installed Directory TOM
	        strDataPath+="\\";
            
	        Stream stream=null;
	        try
	        {
		        //SoapFormatter^ formatter = gcnew SoapFormatter;
		        XmlSerializer formatter = new XmlSerializer(LastSavedSelectionItem.GetType());

		         stream= System.IO.File.Open(strDataPath+strFileName, System.IO.FileMode.Open );
		        //formatter = new BinaryFormatter();
		        LastSavedSelectionItem = (SelectionItem) formatter.Deserialize( stream );
		        stream.Close();

		        return true;
	        }
	        catch(Exception )
	        {
		        //System::Windows::Forms::MessageBox::Show(Ex->Message);
		        if(stream!=null)
		        {
			        stream.Close();
		        }
		        return false;
	        }

        }

        private bool LoadSQLSelectionProfileList(string strFileName)
        {
            //System::String^ strDataPath=Environment::GetFolderPath(Environment::SpecialFolder::ApplicationData);
            string strDataPath = GetAssemblyDirectory(); //Changed to Installed Directory TOM
            strDataPath += "\\";

            Stream stream = null;
            try
            {
                //SoapFormatter^ formatter = gcnew SoapFormatter;
                XmlSerializer formatter = new XmlSerializer(SQLProfileListManager.GetType());

                stream = System.IO.File.Open(strDataPath + strFileName, System.IO.FileMode.Open);
                //formatter = new BinaryFormatter();
                SQLProfileListManager = (SQLProfileManager)formatter.Deserialize(stream);
                stream.Close();

                return true;
            }
            catch (Exception)
            {
                //System::Windows::Forms::MessageBox::Show(Ex->Message);
                if (stream != null)
                {
                    stream.Close();
                }
                return false;
            }

        }
        public bool SaveSQLProfileList(string strFileName)
        {

            string strDataPath = GetAssemblyDirectory(); //Changed to Installed Directory TOM

            strDataPath += "\\";

            Stream stream = null;

            try
            {
                //Opens a file and serializes the object into it in binary format.
                stream = File.Open(strDataPath + strFileName, FileMode.Create);
                //SoapFormatter^ formatter = gcnew SoapFormatter;

                XmlSerializer formatter = new XmlSerializer(SQLProfileListManager.GetType());

                //BinaryFormatter* formatter = new BinaryFormatter();
                formatter.Serialize(stream, SQLProfileListManager);
                stream.Close();

                return true;
            }
            catch (Exception)
            {
                //System::Windows::Forms::MessageBox::Show(Ex->Message);
                if (stream != null)
                {
                    stream.Close();
                }
                return false;
            }
            return false;

        }
        
        private void checkBoxReportProfile_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReportProfile.Checked)
            {
                this.Cursor = Cursors.WaitCursor;
                LoadAllSavedSQLProfiles();
                FillSQLProfileCombobox();
                comboBoxReportProfiles.Visible = true;
                this.Cursor = Cursors.Default;
            }
            else
            {
                comboBoxReportProfiles.Visible = false;
            }
        }

        private void comboBoxReportProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            LoadSelectedSQLProfile(comboBoxReportProfiles.SelectedItem.ToString());
            ResetTreeSelection();
            LoadAllItemsSelectionToTheTree();

            this.Cursor = Cursors.Default;
        }

        public bool DeleteFromProfileList(string strSQLProFileName)
        {
            for (int i =0;i < m_SQLProfileManager.SQLProfileItemList.Count ; i++)
            {
                if (m_SQLProfileManager.SQLProfileItemList[i].ItemName == strSQLProFileName)
                {
                    m_SQLProfileManager.SQLProfileItemList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }


    }

}
