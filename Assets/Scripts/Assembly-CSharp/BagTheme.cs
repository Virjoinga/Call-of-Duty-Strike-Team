using System;
using System.Collections.Generic;

[Serializable]
public class BagTheme
{
	public string m_name;

	public List<BagType> m_types;

	public BagTheme(string name)
	{
		m_name = name;
		m_types = new List<BagType>();
	}
}
