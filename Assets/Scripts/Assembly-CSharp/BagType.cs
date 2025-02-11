using System;
using System.Collections.Generic;

[Serializable]
public class BagType
{
	public string m_path;

	public string m_name;

	public List<BagCategory> m_categories;

	public BagType(string name, string path)
	{
		m_name = name;
		m_path = path;
		m_categories = new List<BagCategory>();
	}
}
