using UnityEngine;

[RequireComponent(typeof(CMDumpster))]
public class Dumpster : HidingPlace
{
	public enum DumpsterType
	{
		Kowloon = 0,
		Arctic = 1,
		PackingCrate = 2
	}

	public DumpsterData Interface;

	public GameObject KowModel;

	public GameObject ArcModel;

	public GameObject PacModel;

	private void Awake()
	{
		mContextMenu = GetComponent<CMDumpster>();
	}

	public void SwitchModelToMatchType()
	{
	}

	private void EnableModel(GameObject modelGO, DumpsterType type, bool onOff)
	{
		if (Interface == null)
		{
			return;
		}
		if (modelGO == null)
		{
			if (onOff)
			{
				Debug.LogWarning("To ensure you get the correct dumpster type you're after in the editor, please reimport the dumpster's container.");
			}
		}
		else
		{
			if (onOff)
			{
				modelGO.SetActive(onOff);
				Model = modelGO;
				if (type == DumpsterType.Kowloon)
				{
					return;
				}
				modelGO.name = "prop_d_ext_kow_hidebin01";
				{
					foreach (Transform item in modelGO.transform)
					{
						if (item == null || item.gameObject == null)
						{
							continue;
						}
						if (item.childCount == 0)
						{
							item.gameObject.name = "prop_d_ext_kow_hidebin01_col";
							continue;
						}
						item.gameObject.name = "prop_d_ext_kow_hidebinlid01";
						foreach (Transform item2 in item.transform)
						{
							if (!(item2 == null) && !(item2.gameObject == null))
							{
								item2.gameObject.name = "prop_d_ext_kow_hidebinlid01_col";
							}
						}
					}
					return;
				}
			}
			Object.DestroyImmediate(modelGO);
			modelGO = null;
		}
	}

	public GameObject GetModelForDumpster()
	{
		return null;
	}
}
