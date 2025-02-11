using System.Collections.Generic;
using UnityEngine;

public class RefNodeResource : MonoBehaviour
{
	public enum Nodes
	{
		AimWallLeft1 = 0,
		AimWallLeft2 = 1,
		AimWallLeft3 = 2,
		AimWallRight1 = 3,
		AimWallRight2 = 4,
		AimWallRight3 = 5
	}

	private const int kMaxNodes = 5;

	public List<GameObject> meshList = new List<GameObject>();

	private static Vector3[] localPos;

	private static Quaternion[] localRot;

	private void Awake()
	{
		if (meshList.Count > 0)
		{
			localPos = new Vector3[meshList.Count];
			localRot = new Quaternion[meshList.Count];
			for (int i = 0; i < meshList.Count; i++)
			{
				localPos[i] = meshList[i].transform.localPosition;
				localRot[i] = WorldHelper.UfM_Rotation(meshList[i].transform.localRotation);
			}
		}
	}

	public static void GetRefLocalTo(Nodes n, Vector3 pos, Quaternion rot, out Vector3 respos, out Quaternion resrot)
	{
		respos = pos + rot * localPos[(int)n];
		resrot = rot * localRot[(int)n];
	}
}
