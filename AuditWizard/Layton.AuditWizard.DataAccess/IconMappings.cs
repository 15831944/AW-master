using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Layton.AuditWizard.DataAccess
{
	#region IconMapping Class
	public class IconMapping
	{
		#region local data
		private string _className;
		private string _icon;
		#endregion local data

		public enum Iconsize { Small, Medium, Large };

		#region Properties
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}

		public string Icon
		{
			get { return _icon; }
			set { _icon = value; }
		}
		#endregion Properties

		#region Constructor
		public IconMapping()
		{
			_icon = "";
			_className = "";
		}

		public IconMapping(string className, string icon)
		{
			_className = className;
			_icon = icon;
		}

		public IconMapping(DataRow row)
		{
			this.ClassName = (string)row["_CATEGORY"];
			this.Icon = (string)row["_ICON"];
		}

	    /// <summary>
	    /// Static function to load a bitmap for the specified image
	    /// </summary>
	    /// <param name="iconName"></param>
	    /// <param name="iconsize"></param>
	    /// <returns></returns>
	    public static Bitmap LoadIcon(string iconName ,Iconsize iconsize)
		{
			string imageName = GetIconName(iconName, iconsize);

			// Does this file exist?
			if (!File.Exists(imageName))
			{
				// No - use the 'default' image
				imageName = GetIconName("missingimage.png", iconsize);

				// If this does not exist also then we must exit as we cannot set an imae
				if (!File.Exists(imageName))
					return null;
			}

			return new Bitmap(imageName);
		}


		/// <summary>
		/// Static function to load a bitmap for the specified image
		/// </summary>
		/// <param name="iconName"></param>
		/// <returns></returns>
		public static string GetIconName (string iconName, Iconsize iconsize)
		{
			// ok first determine if the image exists
			string imagesPath = Path.Combine(Application.StartupPath, "Icons");
			string subfolder = (iconsize == Iconsize.Small) ? "small" : (iconsize == Iconsize.Medium) ? "medium" : "large";
			imagesPath = Path.Combine(imagesPath, subfolder);
			string imageName = Path.Combine(imagesPath, iconName);

			// If this image name does not specify a file extension then assume .png
			if (Path.GetExtension(imageName) == "")
				imageName += ".png";

			return imageName;
		}

		#endregion Constructor
	}
	#endregion IconMapping Class


	#region IconMappings Class

	public class IconMappings : List<IconMapping>
	{
		public IconMappings(AuditedItemsDAO lwDataAccess)
		{
			// Populate our internal list
			DataTable tableIcons = lwDataAccess.GetIconMappings();
			foreach (DataRow row in tableIcons.Rows)
			{
				IconMapping mapping = new IconMapping(row);
				this.Add(mapping);
			}
		}


		/// <summary>
		/// return the best match icon given the class specified
		/// </summary>
		/// <param name="forClassName"></param>
		/// <returns></returns>
		public IconMapping GetIconMapping(string forClassName)
		{
			IconMapping returnMapping = null;

			// Is there an exact match for this class of item
			foreach (IconMapping mapping in this)
			{
				if (mapping.ClassName == forClassName)
					return mapping;
			}

			// No match, is there a delimiter remaining? If not then we have not found any matches
			int delimiter = forClassName.LastIndexOf('|');
			if (delimiter == -1)
			{
				returnMapping = new IconMapping(forClassName, "hardware");
			}
			else
			{
				// Strip off the last portion of this string and recurse this function
				forClassName = forClassName.Substring(0, delimiter);
				returnMapping = GetIconMapping(forClassName);
			}

			return returnMapping;
		}	
	}

	#endregion IconMappings Class
}
