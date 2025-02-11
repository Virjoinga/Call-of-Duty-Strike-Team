using UnityEngine;

public class SectionIdentifier : MonoBehaviour
{
	public enum SectionType
	{
		Section_Combined = 0,
		Transition_Combined = 1,
		Shared_Combined = 2,
		All_Combined = 3
	}

	public string m_SectionName;

	public SectionType m_Type;

	public int m_Index = -1;

	private void Awake()
	{
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if (sectionManager != null)
		{
			sectionManager.RegisterSection(this);
		}
	}
}
