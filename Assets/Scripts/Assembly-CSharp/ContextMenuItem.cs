public class ContextMenuItem
{
	public string mLabel;

	public string mMethodName;

	public ContextMenuIcons mIconId;

	public bool mEnabled;

	public ContextMenuItem(string label, string method, ContextMenuIcons iconId, bool enabled)
	{
		mMethodName = method;
		mIconId = iconId;
		mEnabled = enabled;
		mLabel = label;
	}
}
