using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
    #region PickList Class

    public class PickList : List<PickItem>
    {
        #region Data

        private int _picklistID;
        private string _name;
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Data

        #region Properties

        public int PicklistID
        {
            get { return _picklistID; }
            set { _picklistID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion Properties

        #region Constructor

        public PickList()
        {
            _picklistID = 0;
            _name = "";
        }

        public PickList(DataRow dataRow)
            : this()
        {
            try
            {
                _picklistID = (int)dataRow["_PICKLISTID"];
                _name = (string)dataRow["_NAME"];
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception occurred creating a PICKLIST Object, please check database schema.  The message was " + ex.Message);
            }
        }



        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public PickList(PickList other)
        {
            _picklistID = other._picklistID;
            _name = other._name;
        }


        /// <summary>
        /// Equality test - check to see if this instance matches another
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(this.GetType()))
            {
                PickList other = obj as PickList;

                if ((object)other != null)
                {
                    bool equality;
                    equality = other.PicklistID == PicklistID
                            && other.Name == Name;
                    return equality;
                }
            }
            return base.Equals(obj);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Override the ToString function and return just the name of the picklist
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }



        /// <summary>
        /// Add this PickList to the database
        /// </summary>
        /// <returns></returns>
        public int Add()
        {
            int status = -1;

            // If this supplier already exists then call the update instead
            if (_picklistID != 0)
                return Update();

            // Add the PickList to the database
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            int id = lwDataAccess.PickListAdd(this);
            if (id != 0)
            {
                PicklistID = id;
                status = 0;
            }

            return status;
        }


        /// <summary>
        /// Update this PICKLIST in the database
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            if (this.PicklistID == 0)
                Add();
            else
                lwDataAccess.PickListUpdate(this);

            return 0;
        }


        /// <summary>
        /// Delete the current PickList from the database
        /// </summary>
        public void Delete()
        {
            // Is this Picklist referenced by any User Defined Data Fields?  To find out we will need
            // to recover a list of all User Defined Data Fields and check each
            UserDataCategoryList listCategories = new UserDataCategoryList(UserDataCategory.SCOPE.Any);
            listCategories.Populate();

            // Iterate through the categories
            foreach (UserDataCategory category in listCategories)
            {
                // ...and through each field looking for Picklists
                foreach (UserDataField field in category)
                {
                    if ((field.Type != UserDataField.FieldType.Picklist) || (field.Value1 != this.Name)) continue;

                    if (MessageBox.Show(
                        String.Format("The picklist '{0}' is used by one or more user-defined data fields." +
                                      Environment.NewLine + Environment.NewLine +
                                      "Do you wish to delete the picklist AND all related user-defined data fields?", Name
                            ), "Delete picklist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DeleteUserDefinedDataByPicklist(listCategories);
                        new PicklistsDAO().PickListDelete(_picklistID);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            // got here because of no references in user-defined data so just delete the picklist
            new PicklistsDAO().PickListDelete(_picklistID);
        }

        private void DeleteUserDefinedDataByPicklist(UserDataCategoryList listCategories)
        {
            foreach (UserDataCategory category in listCategories)
            {
                foreach (UserDataField field in category)
                {
                    if ((field.Type == UserDataField.FieldType.Picklist) && (field.Value1 == Name))
                    {
                        field.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Find and return a PickItem given it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PickItem FindPickItem(string name)
        {
            foreach (PickItem item in this)
            {
                if (item.Name == name)
                    return item;
            }

            return null;
        }


        /// <summary>
        /// Find and return a PickList given it's index
        /// </summary>
        /// <param name="pickListID">Index of the category to find</param>
        /// <returns></returns>
        public PickItem FindPickItem(int pickItemID)
        {
            foreach (PickItem item in this)
            {
                if (item.PickItemID == pickItemID)
                    return item;
            }

            return null;
        }

        #endregion Methods
    }


    #endregion PicklistClass

    #region PickItem Class

    public class PickItem
    {
        #region Data

        private int _pickitemID;
        private string _name;
        private int _parentID;

        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Data

        #region Properties

        public int PickItemID
        {
            get { return _pickitemID; }
            set { _pickitemID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }

        #endregion Properties

        #region Constructor

        public PickItem()
        {
            _pickitemID = 0;
            _name = "";
            _parentID = 0;
        }

        public PickItem(DataRow dataRow)
            : this()
        {
            try
            {
                _pickitemID = dataRow.IsNull("_PICKLISTID") ? 0 : (int)dataRow["_PICKLISTID"];
                _name = (string)dataRow["_NAME"];
                _parentID = (int)dataRow["_PARENTID"];
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                Utility.DisplayErrorMessage("Exception occurred creating a PICKITEM Object, please check database schema.  The message was " + ex.Message);
            }
        }


        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public PickItem(PickItem other)
        {
            _pickitemID = other._pickitemID;
            _name = other._name;
            _parentID = other._parentID;
        }


        /// <summary>
        /// Equality test - check to see if this instance matches another
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(this.GetType()))
            {
                PickItem other = obj as PickItem;

                if ((object)other != null)
                {
                    bool equality;
                    equality = other.PickItemID == PickItemID
                            && other.Name == Name
                            && other.ParentID == ParentID;
                    return equality;
                }
            }
            return base.Equals(obj);
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Override the ToString function and return just the name of the pick item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Add this PickList to the database
        /// </summary>
        /// <returns></returns>
        public int Add()
        {
            int status = -1;

            // If this supplier already exists then call the update instead
            if (_pickitemID != 0)
                return Update();

            // Add the PickList to the database
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            int id = lwDataAccess.PickItemAdd(this);
            if (id != 0)
            {
                PickItemID = id;
                status = 0;
            }

            return status;
        }


        /// <summary>
        /// Update this PICKITEM in the database
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            if (this.PickItemID == 0)
                Add();
            else
                lwDataAccess.PickItemUpdate(this);

            return 0;
        }


        /// <summary>
        /// Delete the current PickList from the database
        /// </summary>
        public void Delete()
        {
            // Delete from the database
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            lwDataAccess.PickListDelete(this.PickItemID);
        }


        #endregion Methods
    }


    #endregion PickitemClass

    #region PicklistList Class

    public class PickListList : List<PickList>
    {
        public PickListList()
        {
        }

        /// <summary>
        /// Populate the list of PickLists and items from the database
        /// </summary>
        /// <returns></returns>
        public int Populate()
        {
            // Clear any existing items
            this.Clear();

            // ...and re-read all Picklists and items from the database
            PicklistsDAO lwDataAccess = new PicklistsDAO();
            DataTable actionsTable = lwDataAccess.EnumeratePickLists();
            foreach (DataRow row in actionsTable.Rows)
            {
                AddItem(row);
            }

            return Count;
        }


        /// <summary>
        /// Add a PickList or PickItem to the existing list
        /// </summary>
        /// <param name="row"></param>
        public void AddItem(DataRow row)
        {
            // Are we adding a PickList or PickItem?
            bool isPickList = row.IsNull("_PARENTID");
            if (isPickList)
                AddPickList(row);
            else
                AddPickItem(row);
        }


        /// <summary>
        /// Add a new PickList entry to the list
        /// </summary>
        /// <param name="row"></param>
        public void AddPickList(DataRow row)
        {
            // First of all do we already have a definition for this PickList?
            // If we do then we simply return as no more to do
            if (FindPickList((string)row["_NAME"]) != null)
                return;

            // No so add it
            PickList pickList = new PickList(row);
            this.Add(pickList);
        }


        /// <summary>
        /// Add a new PickItem (to an existing PickList)
        /// </summary>
        /// <param name="row"></param>
        public void AddPickItem(DataRow row)
        {
            // Can we find a definition for the associated PickList?  
            // If not then we fail as we cannot add a PickItem to a non-existant PickList
            PickList pickList = FindPickList((int)row["_PARENTID"]);
            if (pickList != null)
            {
                //throw new Exception("Invalid PickItem with index [" + ((int)row["_PARENTID"]).ToString() + "] Associated with PickItem [" + ((int)row["_NAME"]).ToString() + "]");
                PickItem pickItem = new PickItem(row);
                pickList.Add(pickItem);
            }
        }



        /// <summary>
        /// Find and return a PickList given it's name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PickList FindPickList(string name)
        {
            foreach (PickList pickList in this)
            {
                if (pickList.Name == name)
                    return pickList;
            }

            return null;
        }


        /// <summary>
        /// Find and return a PickList given it's index
        /// </summary>
        /// <param name="pickListID">Index of the category to find</param>
        /// <returns></returns>
        public PickList FindPickList(int pickListID)
        {
            foreach (PickList pickList in this)
            {
                if (pickList.PicklistID == pickListID)
                    return pickList;
            }

            return null;
        }
    }

    #endregion PickListList Class

    #region InteractivePickList Class

    public class InteractivePickList
    {
        #region Data

        /// <summary>Name assigned to this alert definition</summary>
        private string _name;

        /// <summary>The list of alert triggers</summary>
        private PickList _pickList = new PickList();

        #endregion Data

        #region Properties

        /// <summary>Name of the Alert Definition</summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PickList PickListItems
        {
            get { return _pickList; }
            set { _pickList = value; }
        }

        #endregion Properties

        #region Constructor

        public InteractivePickList()
        {
        }

        #endregion Constructor
    }

    #endregion
}
