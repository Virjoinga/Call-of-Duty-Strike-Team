using System;
using System.Collections.Generic;

[Serializable]
public class BagCategory
{
	public string m_path;

	public string m_name;

	public List<string> m_objects;

	public BagCategory(string name, string path)
	{
		m_name = name;
		m_path = path;
		m_objects = new List<string>();
	}
}
