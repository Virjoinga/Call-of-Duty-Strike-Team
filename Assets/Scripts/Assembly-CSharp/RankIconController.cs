using UnityEngine;

public class RankIconController : IconControllerBase
{
	private const int LAST_RANK_POSITION = 17;

	private const int LEVELS_PER_RANK = 3;

	private static int MAX_LEVEL = 50;

	private static int MAX_ICONS = 22;

	protected override void Awake()
	{
		base.Awake();
		if (XPManager.Instance != null)
		{
			MAX_LEVEL = XPManager.Instance.m_XPLevels.Count - 1;
		}
	}

	public void SetNoRank()
	{
		base.Index = 0;
		base.Available = false;
		CalculateSimpleSpriteSettings();
	}

	public void SetRank(int level)
	{
		if (level > MAX_LEVEL)
		{
			level = (level - 1) / MAX_LEVEL;
			base.Index = 17 + level;
		}
		else
		{
			base.Index = ((level != MAX_LEVEL) ? ((level - 1) / 3) : 17);
		}
		base.Index = Mathf.Clamp(base.Index, 0, MAX_ICONS);
		base.Available = true;
		CalculateSimpleSpriteSettings();
	}
}
