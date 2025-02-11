using System;
using System.Collections.Generic;

[Serializable]
public class DirectedAnim
{
	public List<RawAnimation> Anims;

	public bool PickRandom;

	public DirectedAnim()
	{
		RawAnimation item = new RawAnimation();
		Anims = new List<RawAnimation>();
		Anims.Add(item);
		PickRandom = false;
	}

	public bool HasAnimation()
	{
		if (Anims.Count > 0)
		{
			return true;
		}
		return false;
	}
}
