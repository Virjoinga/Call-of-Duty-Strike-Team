using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NodesFromObject : MonoBehaviour
{
	public class PosAndNormal
	{
		public Vector3 Pos;

		public Vector3 Norm;

		public PosAndNormal[] mLinks = new PosAndNormal[2];

		public PosAndNormal(Vector3 pos, Vector3 norm)
		{
			Pos = pos;
			Norm = norm;
		}

		public void MoveAlongNormal(float d)
		{
			Pos += Norm * d;
		}

		public void AddLink(PosAndNormal link)
		{
			if (mLinks[0] == null)
			{
				mLinks[0] = link;
				return;
			}
			if (mLinks[1] == null)
			{
				mLinks[1] = link;
				return;
			}
			throw new Exception("shouldnt be able to get more than 2 links");
		}

		public PosAndNormal GetLinkWhichIsnt(PosAndNormal notThis)
		{
			if (mLinks[0] != null && !mLinks[0].Equals(notThis))
			{
				return mLinks[0];
			}
			if (mLinks[1] != null && !mLinks[1].Equals(notThis))
			{
				return mLinks[1];
			}
			return null;
		}

		public void DrawGizmo()
		{
			DrawGizmo(Color.yellow, Color.red, Color.white);
		}

		public void DrawGizmo(Color nodeCol, Color normCol, Color linkCol)
		{
			Gizmos.color = nodeCol;
			Gizmos.DrawCube(Pos, NodeSize);
			Gizmos.color = normCol;
			Gizmos.DrawLine(Pos, Pos + Norm * NormalSize);
			Gizmos.color = linkCol;
			if (mLinks[0] != null)
			{
				Gizmos.DrawLine(Pos, mLinks[0].Pos);
			}
			if (mLinks[1] != null)
			{
				Gizmos.DrawLine(Pos, mLinks[1].Pos);
			}
		}
	}

	public static Vector3 NodeSize = new Vector3(0.1f, 0.1f, 0.1f);

	public static float NormalSize = 0.3f;

	public Transform CuttingPlane;

	public float WallOffset = 1f;

	public float CornerOffset = 0.5f;

	public float MergeNodeTollerance = 0.1f;

	private List<PosAndNormal> mNoDupes = new List<PosAndNormal>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Generate();
		foreach (PosAndNormal mNoDupe in mNoDupes)
		{
			mNoDupe.DrawGizmo(Color.yellow, Color.yellow, Color.yellow);
		}
		foreach (PosAndNormal mNoDupe2 in mNoDupes)
		{
			mNoDupe2.MoveAlongNormal(WallOffset);
		}
		foreach (PosAndNormal mNoDupe3 in mNoDupes)
		{
			mNoDupe3.DrawGizmo(Color.green, Color.green, Color.white);
		}
		mNoDupes = MoveAlongLinks(mNoDupes, CornerOffset);
		mNoDupes = MergerCloseNodes(mNoDupes, MergeNodeTollerance);
		foreach (PosAndNormal mNoDupe4 in mNoDupes)
		{
			mNoDupe4.DrawGizmo(Color.cyan, Color.cyan, Color.blue);
		}
	}

	public List<PosAndNormal> GetPosAndNormalList()
	{
		List<PosAndNormal> list = new List<PosAndNormal>();
		foreach (PosAndNormal mNoDupe in mNoDupes)
		{
			list.Add(mNoDupe);
		}
		foreach (PosAndNormal item in list)
		{
			item.MoveAlongNormal(WallOffset);
		}
		list = MoveAlongLinks(list, CornerOffset);
		return MergerCloseNodes(list, MergeNodeTollerance);
	}

	public void Generate()
	{
		if (!(CuttingPlane == null))
		{
			List<PosAndNormal> list = null;
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				Vector3 vector = component.size * 0.5f;
				Vector3[] array = new Vector3[36];
				Vector3[] array2 = new Vector3[36];
				int[] tris = new int[36]
				{
					0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
					10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
					20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
					30, 31, 32, 33, 34, 35
				};
				array[0] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[1] = component.center + new Vector3(vector.x, vector.y, 0f - vector.z);
				array[2] = component.center + new Vector3(0f - vector.x, vector.y, 0f - vector.z);
				array[3] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[4] = component.center + new Vector3(0f - vector.x, vector.y, 0f - vector.z);
				array[5] = component.center + new Vector3(0f - vector.x, vector.y, vector.z);
				array2[0] = new Vector3(0f, 1f, 0f);
				array2[1] = new Vector3(0f, 1f, 0f);
				array2[2] = new Vector3(0f, 1f, 0f);
				array2[3] = new Vector3(0f, 1f, 0f);
				array2[4] = new Vector3(0f, 1f, 0f);
				array2[5] = new Vector3(0f, 1f, 0f);
				array[6] = component.center + new Vector3(vector.x, 0f - vector.y, vector.z);
				array[7] = component.center + new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
				array[8] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[9] = component.center + new Vector3(vector.x, 0f - vector.y, vector.z);
				array[10] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[11] = component.center + new Vector3(0f - vector.x, 0f - vector.y, vector.z);
				array2[6] = new Vector3(0f, -1f, 0f);
				array2[7] = new Vector3(0f, -1f, 0f);
				array2[8] = new Vector3(0f, -1f, 0f);
				array2[9] = new Vector3(0f, -1f, 0f);
				array2[10] = new Vector3(0f, -1f, 0f);
				array2[11] = new Vector3(0f, -1f, 0f);
				array[12] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[13] = component.center + new Vector3(vector.x, vector.y, 0f - vector.z);
				array[14] = component.center + new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
				array[15] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[16] = component.center + new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
				array[17] = component.center + new Vector3(vector.x, 0f - vector.y, vector.z);
				array2[12] = new Vector3(1f, 0f, 0f);
				array2[13] = new Vector3(1f, 0f, 0f);
				array2[14] = new Vector3(1f, 0f, 0f);
				array2[15] = new Vector3(1f, 0f, 0f);
				array2[16] = new Vector3(1f, 0f, 0f);
				array2[17] = new Vector3(1f, 0f, 0f);
				array[18] = component.center + new Vector3(0f - vector.x, vector.y, vector.z);
				array[19] = component.center + new Vector3(0f - vector.x, vector.y, 0f - vector.z);
				array[20] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[21] = component.center + new Vector3(0f - vector.x, vector.y, vector.z);
				array[22] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[23] = component.center + new Vector3(0f - vector.x, 0f - vector.y, vector.z);
				array2[18] = new Vector3(-1f, 0f, 0f);
				array2[19] = new Vector3(-1f, 0f, 0f);
				array2[20] = new Vector3(-1f, 0f, 0f);
				array2[21] = new Vector3(-1f, 0f, 0f);
				array2[22] = new Vector3(-1f, 0f, 0f);
				array2[23] = new Vector3(-1f, 0f, 0f);
				array[24] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[25] = component.center + new Vector3(vector.x, 0f - vector.y, vector.z);
				array[26] = component.center + new Vector3(0f - vector.x, 0f - vector.y, vector.z);
				array[27] = component.center + new Vector3(vector.x, vector.y, vector.z);
				array[28] = component.center + new Vector3(0f - vector.x, 0f - vector.y, vector.z);
				array[29] = component.center + new Vector3(0f - vector.x, vector.y, vector.z);
				array2[24] = new Vector3(0f, 0f, 1f);
				array2[25] = new Vector3(0f, 0f, 1f);
				array2[26] = new Vector3(0f, 0f, 1f);
				array2[27] = new Vector3(0f, 0f, 1f);
				array2[28] = new Vector3(0f, 0f, 1f);
				array2[29] = new Vector3(0f, 0f, 1f);
				array[30] = component.center + new Vector3(vector.x, vector.y, 0f - vector.z);
				array[31] = component.center + new Vector3(vector.x, 0f - vector.y, 0f - vector.z);
				array[32] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[33] = component.center + new Vector3(vector.x, vector.y, 0f - vector.z);
				array[34] = component.center + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z);
				array[35] = component.center + new Vector3(0f - vector.x, vector.y, 0f - vector.z);
				array2[30] = new Vector3(0f, 0f, -1f);
				array2[31] = new Vector3(0f, 0f, -1f);
				array2[32] = new Vector3(0f, 0f, -1f);
				array2[33] = new Vector3(0f, 0f, -1f);
				array2[34] = new Vector3(0f, 0f, -1f);
				array2[35] = new Vector3(0f, 0f, -1f);
				list = GetIntersectPositionsAndNormals(tris, array, array2);
			}
			MeshCollider component2 = base.gameObject.GetComponent<MeshCollider>();
			if (component2 != null)
			{
				list = GetIntersectPositionsAndNormals(component2.sharedMesh.triangles, component2.sharedMesh.vertices, component2.sharedMesh.normals);
			}
			mNoDupes.Clear();
			if (list != null)
			{
				mNoDupes = RemoveDuplicatesAndLinkCrossTri(list);
			}
		}
	}

	private List<PosAndNormal> RemoveDuplicatesAndLinkCrossTri(List<PosAndNormal> posAndNorms)
	{
		List<PosAndNormal> list = new List<PosAndNormal>();
		foreach (PosAndNormal posAndNorm in posAndNorms)
		{
			int num = 0;
			foreach (PosAndNormal posAndNorm2 in posAndNorms)
			{
				if (!posAndNorm.Equals(posAndNorm2))
				{
					if (posAndNorm.Pos == posAndNorm2.Pos && posAndNorm.Norm == posAndNorm2.Norm)
					{
						num++;
						posAndNorm.AddLink(posAndNorm2);
					}
					else if (posAndNorm.Pos == posAndNorm2.Pos)
					{
						posAndNorm.AddLink(posAndNorm2);
					}
				}
			}
			if (num == 0)
			{
				list.Add(posAndNorm);
			}
		}
		return ReLinkWithoutDuplicates(list);
	}

	private List<PosAndNormal> MoveAlongLinks(List<PosAndNormal> posAndNorms, float d)
	{
		List<Vector3> list = new List<Vector3>();
		int num = 0;
		foreach (PosAndNormal posAndNorm in posAndNorms)
		{
			if (posAndNorm.mLinks[num] == null)
			{
				list.Add(posAndNorm.Pos);
				continue;
			}
			Vector3 vector = posAndNorm.mLinks[num].Pos - posAndNorm.Pos;
			float max = vector.magnitude * 0.5f;
			float num2 = Mathf.Clamp(d, 0f, max);
			Vector3 item = posAndNorm.Pos + vector.normalized * num2;
			list.Add(item);
		}
		for (int i = 0; i < posAndNorms.Count; i++)
		{
			posAndNorms[i].Pos = list[i];
		}
		return posAndNorms;
	}

	private List<PosAndNormal> MergerCloseNodes(List<PosAndNormal> posAndNorms, float tollerance)
	{
		List<PosAndNormal> list = new List<PosAndNormal>();
		bool flag = true;
		while (flag)
		{
			flag = false;
			PosAndNormal posAndNormal = null;
			foreach (PosAndNormal posAndNorm in posAndNorms)
			{
				foreach (PosAndNormal posAndNorm2 in posAndNorms)
				{
					if (!posAndNorm.Equals(posAndNorm2))
					{
						Vector3 vector = posAndNorm2.Pos - posAndNorm.Pos;
						if (vector.magnitude < tollerance)
						{
							posAndNorm.Pos += vector * 0.5f;
							posAndNormal = posAndNorm2;
							break;
						}
					}
				}
				if (posAndNormal != null)
				{
					break;
				}
			}
			if (posAndNormal != null)
			{
				flag = true;
				posAndNorms.Remove(posAndNormal);
				posAndNormal = null;
			}
		}
		return ReLinkWithoutDuplicates(posAndNorms);
	}

	private List<PosAndNormal> ReLinkWithoutDuplicates(List<PosAndNormal> noDupes)
	{
		foreach (PosAndNormal noDupe in noDupes)
		{
			if (noDupe.mLinks[0] != null)
			{
				PosAndNormal notThis = noDupe;
				PosAndNormal posAndNormal = noDupe.mLinks[0];
				while (posAndNormal != null && !noDupes.Contains(posAndNormal))
				{
					PosAndNormal linkWhichIsnt = posAndNormal.GetLinkWhichIsnt(notThis);
					notThis = posAndNormal;
					posAndNormal = linkWhichIsnt;
				}
				noDupe.mLinks[0] = posAndNormal;
			}
			if (noDupe.mLinks[1] != null)
			{
				PosAndNormal notThis2 = noDupe;
				PosAndNormal posAndNormal2 = noDupe.mLinks[1];
				while (posAndNormal2 != null && !noDupes.Contains(posAndNormal2))
				{
					PosAndNormal linkWhichIsnt2 = posAndNormal2.GetLinkWhichIsnt(notThis2);
					notThis2 = posAndNormal2;
					posAndNormal2 = linkWhichIsnt2;
				}
				noDupe.mLinks[1] = posAndNormal2;
			}
		}
		return noDupes;
	}

	private List<PosAndNormal> GetIntersectPositionsAndNormals(int[] tris, Vector3[] verts, Vector3[] meshNormals)
	{
		List<PosAndNormal> list = new List<PosAndNormal>();
		for (int i = 0; i < tris.Length / 3; i++)
		{
			Vector3[] array = new Vector3[3];
			Vector3[] array2 = new Vector3[3];
			int num = i * 3;
			array[0] = verts[tris[num]];
			array[1] = verts[tris[num + 1]];
			array[2] = verts[tris[num + 2]];
			array2[0] = meshNormals[tris[num]];
			array2[1] = meshNormals[tris[num + 1]];
			array2[2] = meshNormals[tris[num + 2]];
			array[0] = base.gameObject.transform.localToWorldMatrix.MultiplyPoint(array[0]);
			array[1] = base.gameObject.transform.localToWorldMatrix.MultiplyPoint(array[1]);
			array[2] = base.gameObject.transform.localToWorldMatrix.MultiplyPoint(array[2]);
			array2[0] = base.gameObject.transform.localToWorldMatrix.MultiplyVector(array2[0]);
			array2[1] = base.gameObject.transform.localToWorldMatrix.MultiplyVector(array2[1]);
			array2[2] = base.gameObject.transform.localToWorldMatrix.MultiplyVector(array2[2]);
			Plane plane = new Plane(CuttingPlane.transform.up, CuttingPlane.transform.position);
			Vector3 norm = array2[0] + array2[1] + array2[2];
			norm.Normalize();
			Vector3 intersect = Vector3.zero;
			PosAndNormal[] array3 = new PosAndNormal[3];
			if (EdgePlaneIntersect(plane, array[0], array[1], out intersect))
			{
				array3[0] = new PosAndNormal(intersect, norm);
				list.Add(array3[0]);
			}
			if (EdgePlaneIntersect(plane, array[1], array[2], out intersect))
			{
				array3[1] = new PosAndNormal(intersect, norm);
				list.Add(array3[1]);
			}
			if (EdgePlaneIntersect(plane, array[2], array[0], out intersect))
			{
				array3[2] = new PosAndNormal(intersect, norm);
				list.Add(array3[2]);
			}
			if (array3[0] != null)
			{
				if (array3[1] != null)
				{
					array3[0].AddLink(array3[1]);
				}
				if (array3[2] != null)
				{
					array3[0].AddLink(array3[2]);
				}
			}
			if (array3[1] != null)
			{
				if (array3[0] != null)
				{
					array3[1].AddLink(array3[0]);
				}
				if (array3[2] != null)
				{
					array3[1].AddLink(array3[2]);
				}
			}
			if (array3[2] != null)
			{
				if (array3[1] != null)
				{
					array3[2].AddLink(array3[1]);
				}
				if (array3[0] != null)
				{
					array3[2].AddLink(array3[0]);
				}
			}
		}
		return list;
	}

	public bool MySameSide(Plane p, Vector3 inPt0, Vector3 inPt1)
	{
		float num = 0f - Vector3.Dot(p.normal, inPt0);
		float num2 = 0f - Vector3.Dot(p.normal, inPt1);
		return (num > p.distance && num2 > p.distance) || (num < p.distance && num2 < p.distance);
	}

	private bool EdgePlaneIntersect(Plane plane, Vector3 p0, Vector3 p1, out Vector3 intersect)
	{
		intersect = Vector3.zero;
		if (MySameSide(plane, p0, p1))
		{
			return false;
		}
		Ray ray = new Ray(p0, p1 - p0);
		float enter = 0f;
		if (!plane.Raycast(ray, out enter))
		{
			return false;
		}
		if (enter > (p0 - p1).magnitude)
		{
			return false;
		}
		intersect = ray.GetPoint(enter);
		return true;
	}
}
