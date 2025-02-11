public class SquadStats : StatsManagerUpdatable
{
	private CharacterStat m_EntireSquadGameTotal = new CharacterStat();

	private CharacterStat m_EntireSquadCurrentMission = new CharacterStat();

	protected override void SetEventListeners()
	{
	}

	public override void SessionEnd()
	{
		BuildFromCharacters();
	}

	public CharacterStat GetGameTotal()
	{
		return m_EntireSquadGameTotal;
	}

	public CharacterStat GetCurrentMission()
	{
		return m_EntireSquadCurrentMission;
	}

	public override void Reset()
	{
		m_EntireSquadGameTotal.Reset();
		m_EntireSquadCurrentMission.Reset();
	}

	public override void Load()
	{
		BuildFromCharacters();
	}

	public override void Save()
	{
	}

	public void BuildFromCharacters()
	{
		m_EntireSquadGameTotal = StatsManager.Instance.CharacterStats().GetGameTotalCombinedStat();
		m_EntireSquadCurrentMission = StatsManager.Instance.CharacterStats().GetCurrentMissionCombinedStat();
	}

	public int PlayerPurchasedSoldierXP(int soldierIndex)
	{
		int result = StatsManager.Instance.CharacterXPStats().PlayerPurchasedSoldierXP(soldierIndex);
		BuildFromCharacters();
		return result;
	}
}
