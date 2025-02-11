public class ChallengeMedalEarnedRecord
{
	public string Name { get; private set; }

	public string Description { get; private set; }

	public ChallengeMedalType MedalType { get; private set; }

	public ChallengeMedalEarnedRecord(string name, string description, ChallengeMedalType medalType)
	{
		Name = name;
		Description = description;
		MedalType = medalType;
	}

	public bool IsAwardedForSameChallenge(ChallengeMedalEarnedRecord otherRecord)
	{
		return Name.Equals(otherRecord.Name);
	}
}
