using UnityEngine;

[RequireComponent(typeof(CMWindow))]
public class BuildingWindow : MonoBehaviour
{
	public BuildingWithInterior mAssociatedBuilding;

	public BuildingWithInterior AssociatedBuilding
	{
		get
		{
			return mAssociatedBuilding;
		}
	}

	private void Start()
	{
		if (AssociatedBuilding == null)
		{
			Transform parent = base.transform.parent;
			while (parent != null && AssociatedBuilding == null)
			{
				mAssociatedBuilding = parent.GetComponent(typeof(BuildingWithInterior)) as BuildingWithInterior;
				parent = parent.parent;
			}
		}
		if (mAssociatedBuilding == null)
		{
			Object.Destroy(this);
			Object.Destroy(base.gameObject.GetComponent<CMWindow>());
			return;
		}
		TBFAssert.DoAssert(mAssociatedBuilding != null, string.Format("Window without a Building {0} parent {1}", base.name, base.transform.parent));
		if (base.gameObject.GetComponent<BoxCollider>() == null)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3(0f, 0f, 0f);
			boxCollider.size = new Vector3(1f, 1f, 1f);
		}
	}

	public void Open()
	{
	}

	public void Close()
	{
	}
}
