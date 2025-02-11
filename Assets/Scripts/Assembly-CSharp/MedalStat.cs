public class MedalStat : SingleStat<MedalStat>
{
	public bool[] MedalStatusNormal = new bool[SectionData.numMedalSlots];

	public bool[] MedalStatusVeteran = new bool[SectionData.numMedalSlots];

	public int MedalTotal;

	public override void Reset()
	{
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			MedalStatusNormal[i] = false;
			MedalStatusVeteran[i] = false;
		}
		MedalTotal = 0;
	}

	public override void CombineStat(MedalStat source)
	{
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			if (source.MedalStatusNormal[i])
			{
				MedalStatusNormal[i] = true;
				MedalTotal++;
			}
			if (source.MedalStatusVeteran[i])
			{
				MedalStatusVeteran[i] = true;
				MedalTotal++;
			}
		}
	}

	public void MedalEarned(MedalType type, int index, DifficultyMode mode)
	{
		if (mode == DifficultyMode.Regular)
		{
			MedalStatusNormal[index] = true;
		}
		else
		{
			MedalStatusVeteran[index] = true;
		}
		EventHub.Instance.Report(new Events.MedalEarned(type, mode));
	}

	public bool GetMedalStatus(int index, DifficultyMode mode)
	{
		if (mode == DifficultyMode.Regular)
		{
			return MedalStatusNormal[index];
		}
		return MedalStatusVeteran[index];
	}

	public override void Save(string prefix)
	{
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			Save(prefix, ref MedalStatusNormal[i], "MedalNormal_" + i);
			Save(prefix, ref MedalStatusVeteran[i], "MedalVeteran_" + i);
		}
	}

	public override void Load(string prefix)
	{
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			Load(prefix, ref MedalStatusNormal[i], "MedalNormal_" + i);
			Load(prefix, ref MedalStatusVeteran[i], "MedalVeteran_" + i);
		}
	}
}
