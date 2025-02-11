public class DummyCharacter : RealCharacter
{
	private bool m_FakeFirstPerson;

	public override bool IsFirstPerson
	{
		get
		{
			return m_FakeFirstPerson;
		}
	}

	public void SetFirstPerson(bool val)
	{
		m_FakeFirstPerson = val;
	}

	protected override void Update()
	{
	}
}
