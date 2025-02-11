using System.Collections.Generic;

public class EventLog
{
	private List<string> m_LogData = new List<string>();

	private string m_Log;

	private string m_CurrentSearchFilter1;

	private string m_CurrentSearchFilter2;

	private int m_CurrentListCount;

	private int m_NumFilteredItems;

	public EventLog()
	{
		Clear();
	}

	public string GetDisplayString(string filter1, string filter2)
	{
		if (filter1 != m_CurrentSearchFilter1 || filter2 != m_CurrentSearchFilter2)
		{
			m_CurrentSearchFilter1 = filter1;
			m_CurrentSearchFilter2 = filter2;
			m_Log = string.Empty;
			m_NumFilteredItems = 0;
			foreach (string logDatum in m_LogData)
			{
				AddItem(logDatum);
			}
			m_CurrentListCount = m_LogData.Count;
		}
		if (m_LogData.Count > m_CurrentListCount)
		{
			for (int i = m_CurrentListCount; i < m_LogData.Count; i++)
			{
				AddItem(m_LogData[i]);
			}
			m_CurrentListCount = m_LogData.Count;
		}
		return m_Log;
	}

	private void AddItem(string s)
	{
		if (m_CurrentSearchFilter1.Length == 0 && m_CurrentSearchFilter2.Length == 0)
		{
			m_Log = m_Log + s + "\n";
		}
		else if (s.ToLower().Contains(m_CurrentSearchFilter1.ToLower()) && s.ToLower().Contains(m_CurrentSearchFilter2.ToLower()))
		{
			string log = m_Log;
			m_Log = log + m_NumFilteredItems + ": " + s + "\n";
			m_NumFilteredItems++;
		}
	}

	public void Add(string text)
	{
		m_LogData.Add(text);
	}

	public void Clear()
	{
		m_Log = string.Empty;
		m_CurrentSearchFilter1 = "none";
		m_CurrentSearchFilter2 = "none";
		m_CurrentListCount = 0;
		m_NumFilteredItems = 0;
		m_LogData.Clear();
	}
}
