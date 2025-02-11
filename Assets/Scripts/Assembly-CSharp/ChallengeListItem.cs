using UnityEngine;

public class ChallengeListItem : MonoBehaviour
{
	public ChallengeOverviewItem OverviewItemPrefab;

	public UIListItemContainer Container;

	public ChallengeOverviewItem OverviewItem { get; private set; }

	private void Awake()
	{
		OverviewItem = Object.Instantiate(OverviewItemPrefab) as ChallengeOverviewItem;
		ChallengeOverviewItem overviewItem = OverviewItem;
		overviewItem.name = overviewItem.name + "+" + base.name;
		if (OverviewItem != null)
		{
			OverviewItem.transform.parent = base.transform;
			Container = OverviewItem.GetComponent<UIListItemContainer>();
		}
	}
}
