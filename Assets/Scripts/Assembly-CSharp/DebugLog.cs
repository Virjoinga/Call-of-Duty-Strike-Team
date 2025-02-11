using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("DebugLog")]
public class DebugLog
{
	[XmlArray("DebugLogEntrys")]
	[XmlArrayItem("DebugLogEntry")]
	public List<DebugLogEntry> DebugLogEntrys { get; set; }

	public DebugLog()
	{
		DebugLogEntrys = new List<DebugLogEntry>();
	}
}
