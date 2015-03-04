// **************************
// *** INIFile class V1.0 ***
// **************************
// *** (C)2009 S.T.A. snc ***
// **************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FreezingArcher.Settings
{
    /// <summary>
    /// Ini config class.
    /// </summary>
    public class IniConfig
    {
	#region Declarations

	// *** Lock for thread-safe access to file and local cache ***
	private object m_Lock = new object ();

	// *** File name ***
	private string m_FileName = null;

	internal string FileName
	{
	    get
	    {
		return m_FileName;
	    }
	}

	// *** Lazy loading flag ***
	private bool m_Lazy = false;

	// *** Local cache ***
	private Dictionary<string, Dictionary<string, string>> m_Sections =
	    new Dictionary<string,Dictionary<string, string>> ();

	// *** Local cache modified flag ***
	private bool m_CacheModified = false;

	#endregion

	#region Methods

	// *** Constructor ***
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.IniConfig"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
	public IniConfig (string fileName)
	{
	    Initialize (fileName, false);
	}

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Settings.IniConfig"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="lazy">If set to <c>true</c> lazy.</param>
	public IniConfig (string fileName, bool lazy)
	{
	    Initialize (fileName, lazy);
	}

	// *** Initialization ***
	private void Initialize (string fileName, bool lazy)
	{
	    m_FileName = fileName;
	    m_Lazy = lazy;
	    if (!m_Lazy)
		Refresh ();
	}

	// *** Read file contents into local cache ***
	internal void Refresh ()
	{
	    lock (m_Lock)
	    {
		StreamReader sr = null;
		try
		{
		    // *** Clear local cache ***
		    m_Sections.Clear ();

		    // *** Open the INI file ***
		    try
		    {
			sr = new StreamReader (m_FileName);
		    }
		    catch (FileNotFoundException)
		    {
			return;
		    }

		    // *** Read up the file content ***
		    Dictionary<string, string> currentSection = null;
		    string s;
		    while ((s = sr.ReadLine ()) != null)
		    {
			s = s.Trim ();
						
			// *** Check for section names ***
			if (s.StartsWith ("[", StringComparison.Ordinal) && s.EndsWith ("]", StringComparison.Ordinal))
			{
			    if (s.Length > 2)
			    {
				string sectionName = s.Substring (1, s.Length - 2);
								
				// *** Only first occurrence of a section is loaded ***
				if (m_Sections.ContainsKey (sectionName))
				{
				    currentSection = null;
				}
				else
				{
				    currentSection = new Dictionary<string, string> ();
				    m_Sections.Add (sectionName, currentSection);
				}
			    }
			}
			else
			if (currentSection != null)
			{
			    // *** Check for key+value pair ***
			    int i;
			    if ((i = s.IndexOf ('=')) > 0)
			    {
				int j = s.Length - i - 1;
				string key = s.Substring (0, i).Trim ();
				if (key.Length > 0)
				{
				    // *** Only first occurrence of a key is loaded ***
				    if (!currentSection.ContainsKey (key))
				    {
					string Value = (j > 0) ? (s.Substring (i + 1, j).Trim ()) : ("");
					currentSection.Add (key, Value);
				    }
				}
			    }
			}
		    }
		}
		finally
		{
		    // *** Cleanup: close file ***
		    if (sr != null)
			sr.Close ();
		    sr = null;
		}
	    }
	}
		
	// *** Flush local cache content ***
	internal void Flush ()
	{
	    lock (m_Lock)
	    {
		// *** If local cache was not modified, exit ***
		if (!m_CacheModified)
		    return;				
		m_CacheModified = false;

		// *** Open the file ***
		StreamWriter sw = new StreamWriter (m_FileName);

		try
		{
		    // *** Cycle on all sections ***
		    bool first = false;
		    foreach (KeyValuePair<string, Dictionary<string, string>> SectionPair in m_Sections)
		    {
			Dictionary<string, string> section = SectionPair.Value;
			if (first)
			    sw.WriteLine ();
			first = true;

			// *** Write the section name ***
			sw.Write ('[');
			sw.Write (SectionPair.Key);
			sw.WriteLine (']');
					
			// *** Cycle on all key+value pairs in the section ***
			foreach (KeyValuePair<string, string> valuePair in section)
			{
			    // *** Write the key+value pair ***
			    sw.Write (valuePair.Key);
			    sw.Write ('=');
			    sw.WriteLine (valuePair.Value);
			}
		    }
		}
		finally
		{
		    // *** Cleanup: close file ***
		    if (sw != null)
			sw.Close ();
		    sw = null;
		}
	    }
	}
		
	// *** Read a value from local cache ***
	internal string GetValue (string sectionName, string key, string defaultValue)
	{
	    // *** Lazy loading ***
	    if (m_Lazy)
	    {
		m_Lazy = false;
		Refresh ();
	    }

	    lock (m_Lock)
	    {
		// *** Check if the section exists ***
		Dictionary<string, string> section;
		if (!m_Sections.TryGetValue (sectionName, out section))
		    return defaultValue;

		// *** Check if the key exists ***
		string value;
		// *** Return the found value ***
		return !section.TryGetValue (key, out value) ? defaultValue : value;
	    }
	}

	// *** Insert or modify a value in local cache ***
	internal void SetValue (string sectionName, string key, string value)
	{
	    // *** Lazy loading ***
	    if (m_Lazy)
	    {
		m_Lazy = false;
		Refresh ();
	    }

	    lock (m_Lock)
	    {
		// *** Flag local cache modification ***
		m_CacheModified = true;

		// *** Check if the section exists ***
		Dictionary<string, string> section;
		if (!m_Sections.TryGetValue (sectionName, out section))
		{
		    // *** If it doesn't, add it ***
		    section = new Dictionary<string, string> ();
		    m_Sections.Add (sectionName, section);
		}

		// *** Modify the value ***
		if (section.ContainsKey (key))
		    section.Remove (key);
		section.Add (key, value);
	    }
	}

	// *** Encode byte array ***
	private string encodeByteArray (byte[] value)
	{
	    if (value == null)
		return null;

	    StringBuilder sb = new StringBuilder ();
	    foreach (byte b in value)
	    {
		string hex = Convert.ToString (b, 16);
		int l = hex.Length;
		if (l > 2)
		{
		    sb.Append (hex.Substring (l - 2, 2));
		}
		else
		{
		    if (l < 2)
			sb.Append ("0");
		    sb.Append (hex);
		}
	    }
	    return sb.ToString ();
	}

	// *** Decode byte array ***
	private byte[] decodeByteArray (string value)
	{
	    if (value == null)
		return null;

	    int l = value.Length;
	    if (l < 2)
		return new byte[] { };
			
	    l /= 2;
	    byte[] result = new byte[l];
	    for (int i = 0; i < l; i++)
		result [i] = Convert.ToByte (value.Substring (i * 2, 2), 16);
	    return result;
	}

	// *** Getters for various types ***
	internal bool GetValue (string sectionName, string key, bool defaultValue)
	{
	    string stringValue = GetValue (sectionName, key, defaultValue.ToString (CultureInfo.InvariantCulture));
	    int value;
	    return int.TryParse (stringValue, out value) ? (value != 0) : defaultValue;
	}

	internal int GetValue (string sectionName, string key, int defaultValue)
	{
	    string stringValue = GetValue (sectionName, key, defaultValue.ToString (CultureInfo.InvariantCulture));
	    int value;
	    return int.TryParse (stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ?
		value : defaultValue;
	}

	internal double GetValue (string sectionName, string key, double defaultValue)
	{
	    string stringValue = GetValue (sectionName, key, defaultValue.ToString (CultureInfo.InvariantCulture));
	    double value;
	    return double.TryParse (stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ?
		value : defaultValue;
	}

	internal byte[] GetValue (string sectionName, string key, byte[] defaultValue)
	{
	    string StringValue = GetValue (sectionName, key, encodeByteArray (defaultValue));
	    try
	    {
		return decodeByteArray (StringValue);
	    }
	    catch (FormatException)
	    {
		return defaultValue;
	    }
	}

	// *** Setters for various types ***
	internal void SetValue (string sectionName, string key, bool value)
	{
	    SetValue (sectionName, key, (value) ? ("1") : ("0"));
	}

	internal void SetValue (string sectionName, string key, int value)
	{
	    SetValue (sectionName, key, value.ToString (CultureInfo.InvariantCulture));
	}

	internal void SetValue (string sectionName, string key, double value)
	{
	    SetValue (sectionName, key, value.ToString (CultureInfo.InvariantCulture));
	}

	internal void SetValue (string sectionName, string key, byte[] value)
	{
	    SetValue (sectionName, key, encodeByteArray (value));
	}

	#endregion
    }
}
