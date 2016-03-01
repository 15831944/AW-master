using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Text;

namespace Layton.Common.Controls
{
	public class URL : ICloneable, IComparable
	{
		private readonly string schemeDecodeRegex = @"([^:]+):";
		private readonly string mailtoDecodeRegex = @"(mailto:)(([^@]+)@(.+))";
		private readonly string urlDecodeRegex = @"([^:]+)://(([^:@]+)(:([^@]+))?@)?([^:/?#]+)(:([\d]+))?([^?#]+)?(\?([^#]+))?(#(.*))?";

		private URL baseUrl;
		private string scheme;
		private long port;
		private bool useDefaultPort;
		private string hostName;
		private string user;
		private string password;
		private string path;
		private NameValueCollection query;
		private string fragment;
		private bool relative;

		public URL()
		{
			Reset();
		}

		public URL(string url)
		{
			Reset();
			FullURL = url;
		}

		public URL(URL copyUrl)
		{
			Reset();
			CopyFrom(copyUrl);
		}

		public string Scheme
		{
			get { return scheme; }
			set { scheme = value.Trim(); }
		}

		public long Port
		{
			get { return port; }
			set { port = value; useDefaultPort = false; }
		}

		public bool UseDefaultPort
		{
			get { return useDefaultPort; }
			set { useDefaultPort = value; }
		}

		public string User
		{
			get { return user; }
			set { user = value; }
		}

		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		public string HostName
		{
			get { return hostName; }
			set { hostName = value; }
		}

		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		public NameValueCollection Query
		{
			get { return query; }
			set { query = value; }
		}

		public string Fragment
		{
			get { return fragment; }
			set { fragment = value; }
		}

		public string FullURL
		{
			get
			{
				if (Scheme.Equals("mailto"))
					return string.Format("{0}:{1}@{2}", Scheme, User, HostName);
				string newURL = string.Empty;
				if (!Relative)
				{
					newURL += Scheme + "://";
					if (User.Length > 0)
					{
						newURL += User;
						if (Password.Length > 0)
							newURL += ":" + Password;
						newURL += "@";
					}
					newURL += HostName;
					if (!UseDefaultPort)
						newURL += ":" + Port;
				}
				newURL += Path;
				if (QueryString.Length > 0)
					newURL += "?" + QueryString;
				if (Fragment.Length > 0)
					newURL += "#" + Fragment;
				return newURL;
			}
			set
			{
				Reset();
				Match m = new Regex(schemeDecodeRegex).Match(value);
				if (m.Success)
					if (m.Groups[1].Captures[0].Value.ToLower().Equals("mailto"))
						DecodeMailTo(value);
					else
						DecodeURL(value);
			}
		}

		public bool Relative
		{
			get { return relative; }
			set { relative = value; }
		}

		public string QueryString
		{
			get
			{
				string newQueryString = string.Empty;
				for (int queryIdx = 0; queryIdx < Query.Count; queryIdx++)
				{
					newQueryString += (queryIdx == 0 ? "" : "&") + Query.Keys[queryIdx];
					if (Query[queryIdx].Length > 0)
						newQueryString += "=" + Query[queryIdx];
				}
				return newQueryString;
			}
			set
			{
				Query.Clear();
				AppendQueryString(value);
			}
		}

		public URL BaseUrl
		{
			get { return baseUrl; }
			set { baseUrl = value; }
		}

		public void AppendQueryString(string newQueryString)
		{
			string[] pairs = newQueryString.Split('&');
			for (int pairIdx = 0; pairIdx < pairs.Length; pairIdx++)
			{
				string pair = pairs[pairIdx];
				int keyPos = pair.IndexOf('=');
				if (keyPos > 0)
				{
					string key = pair.Substring(0, keyPos);
					string value = pair.Substring(keyPos + 1);
					query[key] = value;
				}
				else
					query[pair] = string.Empty;
			}
		}

		public void Reset()
		{
			Scheme = string.Empty;
			Port = 0;
			UseDefaultPort = true;
			HostName = string.Empty;
			User = string.Empty;
			Password = string.Empty;
			Path = string.Empty;
			Query = new NameValueCollection();
			Fragment = string.Empty;
			Relative = false;
		}

		public void CopyFrom(URL copyUrl)
		{
			Scheme = copyUrl.Scheme;
			User = copyUrl.User;
			Password = copyUrl.Password;
			HostName = copyUrl.HostName;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() != this.GetType()) return false;

			return FullURL == ((URL)obj).FullURL;
		}

		public override int GetHashCode()
		{
			return FullURL.GetHashCode();
		}

		public override string ToString()
		{
			return FullURL;
		}

		private void DecodeURL(string value)
		{
			Match m = new Regex(urlDecodeRegex).Match(value);
			if (m.Success)
			{
				if (m.Groups[1].Captures.Count == 1)
					Scheme = m.Groups[1].Captures[0].Value;
				if (m.Groups[4].Captures.Count == 1)
					User = m.Groups[4].Captures[0].Value;
				if (m.Groups[5].Captures.Count == 1)
					Password = m.Groups[5].Captures[0].Value;
				if (m.Groups[6].Captures.Count == 1)
					HostName = m.Groups[6].Captures[0].Value;
				if (m.Groups[8].Captures.Count == 1)
					Port = Int32.Parse(m.Groups[8].Captures[0].Value);
				if (m.Groups[9].Captures.Count == 1)
					Path = m.Groups[9].Captures[0].Value;
				if (m.Groups[11].Captures.Count == 1)
					QueryString = m.Groups[11].Captures[0].Value;
				if (m.Groups[13].Captures.Count == 1)
					Fragment = m.Groups[13].Captures[0].Value;
			}
		}

		private void DecodeMailTo(string value)
		{
			Match m = new Regex(mailtoDecodeRegex).Match(value);
			if (m.Success)
			{
				if (m.Groups[1].Captures.Count == 1)
					Scheme = m.Groups[1].Captures[0].Value;
				if (m.Groups[2].Captures.Count == 1)
					User = m.Groups[2].Captures[0].Value;
				if (m.Groups[3].Captures.Count == 1)
					HostName = m.Groups[3].Captures[0].Value;
			}
		}

		public object Clone()
		{
			URL newClone = (URL)this.MemberwiseClone();
			newClone.Query = new NameValueCollection(Query);
			return newClone;
		}

		public int CompareTo(object obj)
		{
			if (obj == this) return 0;
			if (!(obj is URL)) return -1;
			return ((URL)obj).FullURL.CompareTo(FullURL);
		}
	}
};
