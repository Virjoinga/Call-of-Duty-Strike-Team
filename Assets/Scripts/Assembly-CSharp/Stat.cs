using System;

[Serializable]
public class Stat
{
	public enum StatType
	{
		Accuracy = 0,
		Damage = 1,
		Range = 2,
		FireRate = 3,
		Mobility = 4,
		LongRange = 5,
		CloseQuarters = 6,
		GroupEfficient = 7,
		NumStatTypes = 8
	}

	public const int NUM_STAT_TYPES = 8;

	public StatType Type;

	public string StringKey;

	public ProgressBar ProgressBarSelected;

	public ProgressBar ProgressBarCurrent;
}
