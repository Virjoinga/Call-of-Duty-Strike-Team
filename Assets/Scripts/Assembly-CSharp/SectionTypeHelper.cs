public static class SectionTypeHelper
{
	public static bool IsAGMG()
	{
		if (GameController.Instance == null)
		{
			return false;
		}
		if (SectionManager.GetSectionManager() != null)
		{
			return SectionManager.GetSectionManager().IsGMG;
		}
		return GameController.Instance.IsLockedToFirstPerson && GKM.UnitCount(GKM.PlayerControlledMask) == 1;
	}
}
