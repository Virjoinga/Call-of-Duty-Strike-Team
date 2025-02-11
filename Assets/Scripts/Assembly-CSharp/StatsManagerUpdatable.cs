public abstract class StatsManagerUpdatable : ISaveLoad
{
	public int Index { get; set; }

	public virtual void PreSessionEnd()
	{
	}

	public virtual void PostSessionEnd()
	{
	}

	public virtual void Update()
	{
	}

	public abstract void SessionEnd();

	public abstract void Reset();

	protected abstract void SetEventListeners();

	public virtual void Init()
	{
		SetEventListeners();
		Reset();
	}

	public virtual void ClearCurrentMission()
	{
	}

	public abstract void Load();

	public abstract void Save();
}
