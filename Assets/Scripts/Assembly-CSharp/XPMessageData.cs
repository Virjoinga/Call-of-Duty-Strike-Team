using UnityEngine;

public class XPMessageData
{
	public string Message;

	public int Score;

	public GameObject Target;

	public XPMessageData(int score, string msg, GameObject target)
	{
		Message = msg;
		Score = score;
		Target = target;
	}
}
