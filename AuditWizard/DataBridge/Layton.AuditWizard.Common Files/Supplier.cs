using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Layton.AuditWizard.Common
{
	#region Supplier Class

	public class Supplier
	{
		#region Data

		private int		_supplierID;
		private string	_name;
		private string	_address1;
		private string	_address2;
		private string	_city;
		private string	_state;
		private string	_zip;
		private string	_contact;
		private string _email;
		private string _www;
		private string _telephone;
		private string	_fax;
		private string	_notes;

		#endregion Data

		#region Properties

		public int SupplierID
		{
			get { return _supplierID; }
			set { _supplierID = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string AddressLine1
		{
			get { return _address1; }
			set { _address1 = value; }
		}

		public string AddressLine2
		{
			get { return _address2; }
			set { _address2 = value; }
		}

		public string City
		{
			get { return _city; }
			set { _city = value; }
		}

		public string State
		{
			get { return _state; }
			set { _state = value; }
		}

		public string Zip
		{
			get { return _zip; }
			set { _zip = value; }
		}

		public string Contact
		{
			get { return _contact; }
			set { _contact = value; }
		}

		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}

		public string WWW
		{
			get { return _www; }
			set { _www = value; }
		}

		public string Telephone
		{
			get { return _telephone; }
			set { _telephone = value; }
		}

		public string Fax
		{
			get { return _fax; }
			set { _fax = value; }
		}

		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}

		#endregion Properties

		#region Constructor

		public Supplier()
		{
			_supplierID = 0;
			_name = "";
			_address1 = "";
			_address2 = "";
			_city = "";
			_state = "";
			_zip = "";
			_contact = "";
			_email = "";
			_www = "";
			_telephone = "";
			_fax = "";
			_notes = "";
		}


		public Supplier(DataRow dataRow)
		{
			try
			{
				SupplierID	= (int)dataRow["_SUPPLIERID"];
				Name		= (string)dataRow["_NAME"];
				AddressLine1 = (string)dataRow["_ADDRESS1"];
				AddressLine2 = (string)dataRow["_ADDRESS2"];
				City		= (string)dataRow["_CITY"];
				State		= (string)dataRow["_STATE"];
				Zip			= (string)dataRow["_ZIP"];
				Contact		= (string)dataRow["_CONTACT_NAME"];
				Email		= (string)dataRow["_CONTACT_EMAIL"];
				WWW			= (string)dataRow["_WWW"];
				Telephone = (string)dataRow["_TELEPHONE"];
				Fax			= (string)dataRow["_FAX"];
				Notes		= (string)dataRow["_NOTES"];
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("Exception occured creating a SUPPLIER Object, please check database schema.  The message was " + ex.Message);
			}
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Add this supplier to the database
		/// </summary>
		/// <returns></returns>
		public int Add()
		{
			int status = -1;

			// If this SUPPLIER already exists then call the update instead
			if (_supplierID != 0)
				return Update(null);

			// Add the supplier to the database
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			int supplierID = lwDataAccess.SupplierAdd(this);
			if (supplierID != 0)
			{
				AuditChanges(null);
				_supplierID = supplierID;
				status = 0;
			}

			return status;
		}


		/// <summary>
		/// Update this supplier defininition in the database
		/// </summary>
		/// <returns></returns>
		public int Update(Supplier oldSupplier)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			if (this._supplierID == 0)
			{
				Add();
			}
			else
			{
				lwDataAccess.SupplierUpdate(this);
				AuditChanges(oldSupplier);
			}

			return 0;
		}

		/// <summary>
		/// Delete the current license from the database
		/// </summary>
		public void Delete()
		{
			// Delete from the database
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();
			lwDataAccess.SupplierDelete(this);

			// ...and audit the deletion
			AuditTrailEntry ate = new AuditTrailEntry();
			ate.Date = DateTime.Now;
			ate.Class = AuditTrailEntry.CLASS.supplier;
			ate.Type = AuditTrailEntry.TYPE.deleted;
			ate.Key = _name;
			ate.AssetID = 0;
			ate.AssetName = "";
			ate.Username = System.Environment.UserName;
			lwDataAccess.AuditTrailAdd(ate);
		}

		#endregion Methods

		#region Change Handling

		/// <summary>
		/// Return a list of changes between this object and an old version
		/// For suppliers we really aren't that interested  in changes to these details just adding
		/// and deletion of suppliers
		/// </summary>
		/// <param name="oldObject"></param>
		/// <returns></returns>
		public List<AuditTrailEntry> AuditChanges(Supplier oldObject)
		{
			AuditWizardDataAccess lwDataAccess = new AuditWizardDataAccess();

			// Construct the return list
			List<AuditTrailEntry> listChanges = new List<AuditTrailEntry>();

			// Is this a new item or an update to an existing one
			if (SupplierID == 0)
			{
				AuditTrailEntry ate = new AuditTrailEntry();
				ate.Date = DateTime.Now;
				ate.Class = AuditTrailEntry.CLASS.supplier;
				ate.Type = AuditTrailEntry.TYPE.added;
				ate.Key = _name;
				ate.AssetID = 0;
				ate.AssetName = "";
				ate.Username = System.Environment.UserName;
				listChanges.Add(ate);
			}

			// Add all of these changes to the Audit Trail
			foreach (AuditTrailEntry entry in listChanges)
			{
				lwDataAccess.AuditTrailAdd(entry);
			}

			// Return the constructed list
			return listChanges;
		}

		#endregion Change Handling

		/// <summary>
		/// Override ToString to return the supplier name
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}
	}

	#endregion Supplier Class

	#region SupplierList Class

	public class SupplierList : List<Supplier>
	{
		// Find an application in the list
		public Supplier FindSupplier(string supplier)
		{
			int index = GetIndex(supplier);
			return (index == -1) ? null : this[index];
		}


		/// <summary>
		/// Return the index of the Supplier with the specified name
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		public int GetIndex(string supplier)
		{
			int index = 0;
			foreach (Supplier thisSupplier in this)
			{
				if (thisSupplier.Name == supplier)
					return index;
				index++;
			}

			return -1;
		}

		/// <summary>
		/// Does the list contain a Supplier with the specified name?
		/// </summary>
		/// <param name="supplier"></param>
		/// <returns></returns>
		public bool Contains(string supplier)
		{
			return (GetIndex(supplier) != -1);
		}
	}
	#endregion SupplierList Class

}
