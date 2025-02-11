using UnityEngine;

public class GlobalConflictMedal : MonoBehaviour
{
	public enum Medal
	{
		Iron = 0,
		Bronze = 1,
		Silver = 2,
		Gold = 3,
		Platinum = 4,
		Master = 5,
		NumMedals = 6
	}

	private PackedSprite mSprite;

	private void Awake()
	{
		mSprite = GetComponent<PackedSprite>();
	}

	public void SetMedal(Medal medalType)
	{
		if (mSprite != null)
		{
			mSprite.SetFrame(0, (int)medalType);
		}
	}
}
