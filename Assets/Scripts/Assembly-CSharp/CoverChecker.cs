using UnityEngine;

[ExecuteInEditMode]
public class CoverChecker : MonoBehaviour
{
	public NewCoverPointManager ncpm;

	public int closestCover;

	private void Start()
	{
	}

	private void Update()
	{
		if (ncpm != null)
		{
			CoverPointCore coverPointCore = ncpm.FindClosestCoverPoint_Fast(base.transform.position);
			if (coverPointCore != null)
			{
				base.name = "Closest Cover: " + coverPointCore.name;
				closestCover = coverPointCore.index;
			}
			else
			{
				closestCover = -1;
				base.name = "Closest Cover: NO CLOSEST COVER!!!";
			}
		}
	}
}
