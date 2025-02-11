using UnityEngine;

internal struct DragHistoryItem
{
	public Vector2 delta;

	public float timeStamp;

	public DragHistoryItem(float x, float y)
	{
		delta = new Vector2(x, y);
		timeStamp = Time.realtimeSinceStartup;
	}
}
