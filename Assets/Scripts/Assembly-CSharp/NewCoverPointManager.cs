using System.Collections.Generic;
using UnityEngine;

public class NewCoverPointManager : MonoBehaviour
{
	private const int kMaxFactions = 2;

	private const float COVER_TOO_CLOSE_TO_ENEMY_DIST = 8f;

	public const float COVER_TOO_CLOSE_TO_ENEMY_DIST_SQR = 64f;

	private const float COVER_TOO_FAR_TO_SEE = 10f;

	private const float COVER_TOO_FAR_TO_SEE_SQR = 100f;

	private const float COVER_TOO_CLOSE_TO_CROUCH = 8f;

	private const float kMaxNeighbourNavmeshDistance = 20f;

	private static NewCoverPointManager instance = null;

	public static NewCoverPointManager pendingInstance = null;

	public float tileside = 0.8f;

	public int footprint;

	public int covergrid_footprint;

	public int coverneighbour_footprint;

	public int goodcover_footprint;

	public int concealmentField_footprint;

	public int distanceTable_footprint;

	public int CoverProvided_footprint;

	public int routeMatrix_footprint;

	public NewCoverPointBakedData baked;

	public CoverPointCore[] coverPoints;

	[HideInInspector]
	private int[] routeMatrix;

	public bool dirty;

	private float[,] distanceTable;

	private Vector3[,] gridPos;

	private int[,] originalClosest;

	private float[,] gridDistance;

	private CoverCluster campSites;

	private bool quickBake;

	public bool showProblemAreas;

	public int subsectionMask;

	public int Savings;

	public bool paintMode;

	public int bakestage = -1;

	private int walkableMask = -1;

	private int walkableMaskNoDoors;

	private NewCoverPoint[] sourceList;

	private int sortRelativeTo;

	private bool showHit;

	public static Vector3 standingHeightOffset = new Vector3(0f, 1.7f, 0f);

	public static Vector3 crouchingHeightOffset = new Vector3(0f, 0.9f, 0f);

	public static Vector3 deadHeightOffset = new Vector3(0f, 0.1f, 0f);

	private int defaultAndRoofLayers;

	private int defaultLayerMask;

	public static NewCoverPointManager Instance()
	{
		return instance;
	}

	public static void ReplaceInstance(NewCoverPointManager ncp)
	{
		instance = ncp;
	}

	public void JustBakeUncertainty()
	{
		quickBake = false;
		PackedCoverData.InitDirectionTable();
		CreateCoverGrid();
		ProduceUncertaintyMap(true);
	}

	public void StartBake(bool quick, bool showProblems)
	{
		quickBake = quick;
		showProblemAreas = showProblems;
		PackedCoverData.InitDirectionTable();
		BakeStaticData();
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			pendingInstance = this;
			base.enabled = false;
		}
		PackedCoverData.InitDirectionTable();
		if (baked == null)
		{
			baked = ScriptableObject.CreateInstance<NewCoverPointBakedData>();
		}
		baked.FixUpAnythingMissing();
	}

	private void Start()
	{
		walkableMaskNoDoors |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default");
		walkableMaskNoDoors |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Jump");
		CoverPointCore[] array = coverPoints;
		foreach (CoverPointCore coverPointCore in array)
		{
			if (coverPointCore == null)
			{
				TBFAssert.DoAssert(false, "Null cover point in the cover point manager - you will need to re-bake the cover points");
			}
			coverPointCore.Init();
		}
		if (dirty && Application.isPlaying)
		{
			Debug.LogWarning("Cover Point Baked Data Out Of Date!");
		}
		GameObject gameObject = GameObject.Find("Campsites");
		if (gameObject != null)
		{
			campSites = gameObject.GetComponent<CoverCluster>();
		}
	}

	private void Update()
	{
		if (baked == null)
		{
			baked = ScriptableObject.CreateInstance<NewCoverPointBakedData>();
		}
	}

	private void OnDestroy()
	{
		instance = null;
		pendingInstance = null;
		campSites = null;
		for (int i = 0; i < coverPoints.Length; i++)
		{
			if (coverPoints[i].Occupant != null)
			{
				coverPoints[i].Cancel(coverPoints[i].Occupant);
			}
		}
		coverPoints = null;
	}

	public void Expunge(uint invident)
	{
		int length = coverPoints.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			coverPoints[i].Expunge(invident);
		}
	}

	public bool IsCampsite(CoverPointCore cpc)
	{
		if (campSites == null || cpc == null)
		{
			return false;
		}
		return campSites.Includes(cpc);
	}

	public float GetCoverToCoverDistance(int index1, int index2)
	{
		return (int)baked.distanceTable[index1 + index2 * coverPoints.Length];
	}

	public float GetCoverToCoverDistance(CoverPointCore cp1, CoverPointCore cp2)
	{
		return (int)baked.distanceTable[cp1.index + cp2.index * coverPoints.Length];
	}

	public void UpdateInterest(uint ident, int closestCover)
	{
		CoverPointCore coverPointCore = coverPoints[closestCover];
		int num = coverPointCore.neighbours.Length;
		for (int i = 0; i < num; i++)
		{
			int num2 = CoverNeighbour.coverIndex(coverPointCore.neighbours[i]);
			switch (baked.coverTable.GetCoverProvided(num2, closestCover))
			{
			case CoverTable.CoverProvided.Low:
			case CoverTable.CoverProvided.Full:
			case CoverTable.CoverProvided.Stupid:
				coverPoints[num2].HiddenTo(ident);
				break;
			case CoverTable.CoverProvided.None:
			case CoverTable.CoverProvided.High:
				coverPoints[num2].VisibleTo(ident);
				break;
			}
		}
	}

	public CoverPointCore GetNextCoverOnRoute(CoverPointCore f, ref int neighbour, out float distance)
	{
		uint data = (uint)f.routeOnward[neighbour];
		distance = PackedCoverData.UnpackDistance(data);
		neighbour = PackedCoverData.UnpackSecondIndex(data);
		return coverPoints[PackedCoverData.UnpackFirstIndex(data)];
	}

	public void ValidateCoverPoints(uint ident, int closestCover, bool docked, Vector3 pos)
	{
		int length = coverPoints.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			UpdateCoverProvided(i, closestCover, ident);
		}
		if (!docked)
		{
			ApplyUncertaintyData(closestCover, pos, ident);
		}
	}

	public void ValidateCoverPoints_LocationUnchanged(uint ident, int closestCover, bool docked, Vector3 pos)
	{
		CoverPointCore coverPointCore = coverPoints[closestCover];
		if (coverPointCore.significantCover != null)
		{
			int length = coverPointCore.significantCover.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				int fromCover = coverPointCore.significantCover[i];
				UpdateCoverProvided(fromCover, closestCover, ident);
			}
			if (!docked)
			{
				ApplyUncertaintyData(closestCover, pos, ident);
			}
		}
	}

	public void ApplyUncertaintyData(int toCover, Vector3 pos, uint ident)
	{
		CoverPointCore coverPointCore = coverPoints[toCover];
		if (coverPointCore.significantCover == null)
		{
			return;
		}
		int length = coverPointCore.significantCover.GetLength(0);
		int gridx;
		int gridz;
		PositionToGrid(pos, out gridx, out gridz);
		uint num = baked.ConcealmentField(gridx, gridz);
		if (num == 0)
		{
			return;
		}
		for (int i = 0; i < length; i++)
		{
			if ((num & (uint)(1 << i)) == 0)
			{
				continue;
			}
			int num2 = coverPointCore.significantCover[i];
			bool visibilityUncertain;
			bool coverUncertain;
			CoverTable.CoverProvided coverProvided = baked.coverTable.GetCoverProvided(num2, toCover, out visibilityUncertain, out coverUncertain);
			if ((int)coverProvided >= 4)
			{
				if (coverUncertain)
				{
					coverPoints[num2].NoCoverAgainst(ident);
				}
				else if (coverPoints[num2].type == CoverPointCore.Type.ShootOver)
				{
					coverPoints[num2].LowCoverAgainst(ident);
				}
				else
				{
					coverPoints[num2].HighCoverAgainst(ident);
				}
			}
			else if (coverProvided == CoverTable.CoverProvided.Low && coverUncertain)
			{
				coverPoints[num2].NoCoverAgainst(ident);
			}
			else
			{
				coverPoints[num2].FullCoverAgainst(ident);
			}
		}
	}

	public void UpdateCoverProvided(int fromCover, int toCover, uint ident)
	{
		switch (baked.coverTable.GetCoverProvided(fromCover, toCover))
		{
		case CoverTable.CoverProvided.Stupid:
			coverPoints[fromCover].StupidCoverAgainst(ident);
			break;
		case CoverTable.CoverProvided.Full:
			coverPoints[fromCover].FullCoverAgainst(ident);
			break;
		case CoverTable.CoverProvided.High:
			coverPoints[fromCover].HighCoverAgainst(ident);
			break;
		case CoverTable.CoverProvided.Low:
			coverPoints[fromCover].LowCoverAgainst(ident);
			break;
		case CoverTable.CoverProvided.Crouch:
			coverPoints[fromCover].CrouchCoverAgainst(ident);
			break;
		case CoverTable.CoverProvided.None:
			coverPoints[fromCover].NoCoverAgainst(ident);
			break;
		}
		CoverTable.CoverProvided coverProvided = baked.coverTable.GetCoverProvided(toCover, fromCover);
		coverPoints[fromCover].GoodForFlanking(ident, coverProvided == CoverTable.CoverProvided.None);
	}

	public CoverPointCore FindClosestCoverPoint_Fast(Vector3 position)
	{
		int gridx;
		int gridz;
		PositionToGrid(position, out gridx, out gridz);
		uint data = baked.CoverGrid(gridx, gridz);
		int num = PackedCoverData.UnpackFirstIndex(data);
		if (num == 1023)
		{
			return null;
		}
		int num2 = PackedCoverData.UnpackSecondIndex(data);
		if (num2 != 0)
		{
			num2 = (num + num2) & 0x3FF;
			Vector2 vagueDir;
			float num3 = CalcNavMeshDistance(position, coverPoints[num2].gamePos, out vagueDir, walkableMaskNoDoors);
			float num4 = PackedCoverData.UnpackDistance(data);
			if (num3 < num4 + tileside * 4f)
			{
				return coverPoints[num2];
			}
			return coverPoints[num];
		}
		return coverPoints[num];
	}

	public CoverPointCore FindClosestInAirCoverPoint(Vector3 position)
	{
		float num = float.MaxValue;
		CoverPointCore result = null;
		for (int i = 0; i < baked.inAirCover.Length; i++)
		{
			float sqrMagnitude = (coverPoints[baked.inAirCover[i]].gamePos - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = coverPoints[baked.inAirCover[i]];
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public CoverPointCore FindClosestCoverPoint_Fast(Vector3 position, out Vector2 vagueDirection, out float distance)
	{
		vagueDirection = Vector2.zero;
		int gridx;
		int gridz;
		PositionToGrid(position, out gridx, out gridz);
		uint data = baked.CoverGrid(gridx, gridz);
		int num = PackedCoverData.UnpackFirstIndex(data);
		if (num == 1023)
		{
			distance = float.MaxValue;
			vagueDirection = Vector2.zero;
			return null;
		}
		int num2 = PackedCoverData.UnpackSecondIndex(data);
		if (num2 != 0)
		{
			num2 = (num + num2) & 0x3FF;
			distance = CalcNavMeshDistance(position, coverPoints[num2].gamePos, out vagueDirection, walkableMaskNoDoors);
			float num3 = PackedCoverData.UnpackDistance(data);
			if (distance < num3 + tileside * 4f)
			{
				return coverPoints[num2];
			}
			distance = num3;
			vagueDirection = PackedCoverData.UnpackDirection(data);
			return coverPoints[num];
		}
		distance = PackedCoverData.UnpackDistance(data);
		vagueDirection = PackedCoverData.UnpackDirection(data);
		return coverPoints[num];
	}

	private void PositionToGrid(Vector3 pos, out int gridx, out int gridz)
	{
		gridx = Mathf.Clamp((int)((pos.x - baked.corner.x) / baked.tileX), 0, baked.coverGridXDim - 1);
		gridz = Mathf.Clamp((int)((pos.z - baked.corner.y) / baked.tileZ), 0, baked.coverGridZDim - 1);
	}

	private int FindClosestCoverPoint_List(int result, List<int> l, Vector3 position, ref float distance, ref Vector2 vagueDir, int walkableToUse)
	{
		if (l == null)
		{
			return result;
		}
		float num = distance;
		int count = l.Count;
		float num2 = num * num;
		for (int i = 0; i < count; i++)
		{
			if ((position - coverPoints[l[i]].gamePos).sqrMagnitude < num2)
			{
				Vector2 vagueDir2;
				float num3 = CalcNavMeshDistance(position, coverPoints[l[i]].gamePos, out vagueDir2, walkableToUse);
				if (num3 < num)
				{
					num = num3;
					result = l[i];
					vagueDir = vagueDir2;
					num2 = num * num;
				}
			}
		}
		distance = num;
		return result;
	}

	private void ClearProgressBar()
	{
	}

	private bool ShowProgressBar(string name, float progress, bool cancel)
	{
		return false;
	}

	public void BakeStaticData()
	{
		defaultLayerMask = 1 << LayerMask.NameToLayer("Default");
		defaultAndRoofLayers = defaultLayerMask | (1 << LayerMask.NameToLayer("RoofCollider"));
		NewCoverPoint[] componentsInChildren = GetComponentsInChildren<NewCoverPoint>();
		if (componentsInChildren.Length == 0)
		{
			Debug.Log("No Coverpoints to bake");
			return;
		}
		bakestage = 0;
		footprint = 0;
		covergrid_footprint = 0;
		coverneighbour_footprint = 0;
		goodcover_footprint = 0;
		concealmentField_footprint = 0;
		distanceTable_footprint = 0;
		CoverProvided_footprint = 0;
		routeMatrix_footprint = 0;
		walkableMask = -1;
		walkableMaskNoDoors = 0;
		walkableMaskNoDoors |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default");
		walkableMaskNoDoors |= 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Jump");
		baked = ScriptableObject.CreateInstance<NewCoverPointBakedData>();
		PerformSpeedTests();
		Savings = 0;
		UncertaintyTag.Reset();
		Debug.Log("Collating information...");
		CollateInformation();
		Debug.Log("Determining Cover Extent...");
		DetermineCoverExtent();
		Debug.Log("Generating Static Cover Table...");
		GenerateStaticCoverTable();
		Debug.Log("Calculating Navmesh Distances...");
		CalculateNavmeshDistances();
		Debug.Log("Apprehending Adjacencies...");
		ApprehendAdjacency();
		Debug.Log("Creating Cover Grid...");
		CreateCoverGrid();
		Debug.Log("Producing Uncertainty Map...");
		ProduceUncertaintyMap(false);
		Debug.Log("Divining Route Matrix...");
		DivineRouteMatrix();
		Debug.Log("Extrapolating Concealment Field...");
		ExtrapolateConcealmentField();
		Debug.Log("Cleaning up...");
		BakeBack();
		ClearProgressBar();
		baked.baked = true;
		bakestage = -1;
		dirty = false;
	}

	public void CleanHouse()
	{
		NewCoverPoint[] array = Object.FindObjectsOfType(typeof(NewCoverPoint)) as NewCoverPoint[];
		int num = 0;
		for (num = 0; num < array.Length; num++)
		{
			if (array[num] != null)
			{
				if (array[num].transform.childCount == 0 || TryDeleteCoverPoint(array[num].transform.GetChild(0)))
				{
					TryDeleteCoverPoint(array[num].transform);
				}
				else if (array[num].GetComponent<GuidRefHook>() == null)
				{
					Object.DestroyImmediate(array[num]);
				}
			}
		}
	}

	public void SprinkleCover()
	{
		NewCoverPoint[] array = Object.FindObjectsOfType(typeof(NewCoverPoint)) as NewCoverPoint[];
		for (int i = 0; i < coverPoints.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].index == i)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				NewCoverPoint newCoverPoint = new GameObject().AddComponent<NewCoverPoint>();
				newCoverPoint.core = (CoverPointCore)Object.Instantiate(coverPoints[i]);
				newCoverPoint.snap = false;
				newCoverPoint.PositionFromCore();
				newCoverPoint.transform.parent = base.transform;
			}
		}
		dirty = false;
	}

	private bool TryDeleteCoverPoint(Transform t)
	{
		GuidRefHook component = t.GetComponent<GuidRefHook>();
		if (component == null)
		{
			Object.DestroyImmediate(t.gameObject);
			return true;
		}
		return false;
	}

	private void PerformSpeedTests()
	{
	}

	private void CollateInformation()
	{
		NewCoverPoint[] componentsInChildren = GetComponentsInChildren<NewCoverPoint>();
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		Vector2 vector3 = vector2;
		Vector2 vector4 = vector;
		CoverAreaTester coverAreaTester = (CoverAreaTester)Object.FindObjectOfType(typeof(CoverAreaTester));
		if (coverAreaTester != null)
		{
			vector3.x = coverAreaTester.transform.position.x - coverAreaTester.transform.localScale.x * 0.5f;
			vector3.y = coverAreaTester.transform.position.z - coverAreaTester.transform.localScale.z * 0.5f;
			vector4.x = coverAreaTester.transform.position.x + coverAreaTester.transform.localScale.x * 0.5f;
			vector4.y = coverAreaTester.transform.position.z + coverAreaTester.transform.localScale.z * 0.5f;
		}
		int num = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].cpc.gamePos.x > vector3.x && componentsInChildren[i].cpc.gamePos.x < vector4.x && componentsInChildren[i].cpc.gamePos.z > vector3.y && componentsInChildren[i].cpc.gamePos.z < vector4.y)
			{
				num++;
			}
		}
		sourceList = new NewCoverPoint[num];
		num = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].cpc.gamePos.x > vector3.x && componentsInChildren[i].cpc.gamePos.x < vector4.x && componentsInChildren[i].cpc.gamePos.z > vector3.y && componentsInChildren[i].cpc.gamePos.z < vector4.y)
			{
				sourceList[num] = componentsInChildren[i];
				num++;
			}
			else
			{
				componentsInChildren[i].index = -1;
				componentsInChildren[i].core.index = -1;
			}
		}
		coverPoints = new CoverPointCore[sourceList.GetLength(0)];
		int num2 = 0;
		for (int i = 0; i < coverPoints.GetLength(0); i++)
		{
			coverPoints[i] = (CoverPointCore)Object.Instantiate(sourceList[i].core);
			coverPoints[i].index = i;
			coverPoints[i].subsectionMask = sourceList[i].subsectionMask;
			coverPoints[i].name = i.ToString();
			sourceList[i].core.index = i;
			ShowProgressBar("Collating Information", (float)i / (float)coverPoints.GetLength(0), false);
			if (coverPoints[i].type == CoverPointCore.Type.InAir)
			{
				num2++;
				continue;
			}
			vector.x = Mathf.Min(coverPoints[i].snappedPos.x, vector.x);
			vector.y = Mathf.Min(coverPoints[i].snappedPos.z, vector.y);
			vector2.x = Mathf.Max(coverPoints[i].snappedPos.x, vector2.x);
			vector2.y = Mathf.Max(coverPoints[i].snappedPos.z, vector2.y);
		}
		baked.inAirCover = new int[num2];
		CoverCluster[] array = (CoverCluster[])Object.FindObjectsOfType(typeof(CoverCluster));
		CoverCluster[] array2 = array;
		foreach (CoverCluster coverCluster in array2)
		{
			if (coverCluster.myManager == this)
			{
				coverCluster.RefreshContents();
			}
		}
		num2 = 0;
		for (int i = 0; i < coverPoints.GetLength(0); i++)
		{
			sourceList[i].index = i;
			if (coverPoints[i].type == CoverPointCore.Type.InAir)
			{
				baked.inAirCover[num2] = i;
				num2++;
			}
		}
		baked.corner = vector - new Vector2(5f, 5f);
		baked.extent = new Vector2(10f, 10f) + (vector2 - vector);
	}

	private void BakeBack()
	{
		for (int i = 0; i < coverPoints.GetLength(0); i++)
		{
			sourceList[i].core = (CoverPointCore)Object.Instantiate(coverPoints[i]);
		}
		sourceList = null;
	}

	public static bool ClearToShootOver(Vector3 floorPos)
	{
		return !Physics.CheckSphere(floorPos + new Vector3(0f, 1.5f, 0f), 0.2f, 1);
	}

	public static bool LowLowCover(Vector3 floorPos)
	{
		return !Physics.CheckSphere(floorPos + new Vector3(0f, 1.2f, 0f), 0.2f, 1);
	}

	private void DetermineCoverExtent()
	{
		int length = coverPoints.GetLength(0);
		Ray ray = default(Ray);
		Vector3 vector = new Vector3(0f, 0.5f, 0f);
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Determining Cover Extent (1/2)", (float)i / (float)length, false);
			coverPoints[i].adjacentRight = -1;
			coverPoints[i].adjacentLeft = -1;
			if (coverPoints[i].type == CoverPointCore.Type.ShootOver || coverPoints[i].type == CoverPointCore.Type.HighWall)
			{
				coverPoints[i].isLowLowCover = LowLowCover(coverPoints[i].snappedPos);
				float num = 0.2f;
				ray.direction = coverPoints[i].snappedNormal * -1f;
				Vector3 vector2 = new Vector3(ray.direction.z, 0f, 0f - ray.direction.x);
				float num2 = 0.6f;
				ray.origin = coverPoints[i].gamePos + vector;
				ray.direction = vector2;
				int num3 = 25;
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 5f))
				{
					num3 = (int)(Mathf.Max(0f, hitInfo.distance - 0.6f) / 0.2f);
				}
				ray.direction = coverPoints[i].snappedNormal * -1f;
				bool flag = false;
				float num4 = num;
				UnityEngine.AI.NavMeshHit hit;
				for (int j = 0; j < num3; j++)
				{
					ray.origin = coverPoints[i].gamePos + vector + vector2 * num;
					if (Physics.Raycast(ray, out hitInfo, 2f, 1) && Mathf.Abs(num2 - hitInfo.distance) < 0.5f)
					{
						if (num4 == num && ClearToShootOver(coverPoints[i].snappedPos + vector2 * num))
						{
							num4 = ((!UnityEngine.AI.NavMesh.SamplePosition(ray.origin, out hit, 1f, -1) || !(Mathf.Abs(hit.position.y - coverPoints[i].gamePos.y) < 0.3f) || !((hit.position - ray.origin).xz().sqrMagnitude < 0.0025f)) ? (num4 - 0.2f) : (num4 + 0.2f));
						}
						num += 0.2f;
						continue;
					}
					if (flag)
					{
						j = num3;
						num -= 0.2f;
						num4 -= 0.2f;
					}
					else
					{
						num += 0.2f;
					}
					flag = true;
				}
				coverPoints[i].maxExtent = num;
				coverPoints[i].maxProprietaryExtent = num4;
				if (!flag)
				{
					num = 5f;
				}
				float f = 0.61f + num * num;
				coverPoints[i].coverAngleSinR = -0.6f / Mathf.Sqrt(f);
				ray.origin = coverPoints[i].gamePos + vector;
				ray.direction = -vector2;
				num3 = 25;
				if (Physics.Raycast(ray, out hitInfo, 5f))
				{
					num3 = (int)(Mathf.Max(0f, hitInfo.distance - 0.6f) / 0.2f);
				}
				ray.direction = coverPoints[i].snappedNormal * -1f;
				num = -0.2f;
				float num5 = num;
				flag = false;
				for (int j = 0; j < num3; j++)
				{
					ray.origin = coverPoints[i].gamePos + vector + vector2 * num;
					if (Physics.Raycast(ray, out hitInfo, 2f, 1) && Mathf.Abs(num2 - hitInfo.distance) < 0.5f)
					{
						if (num5 == num && ClearToShootOver(coverPoints[i].snappedPos + vector2 * num))
						{
							num5 = ((!UnityEngine.AI.NavMesh.SamplePosition(ray.origin, out hit, 1f, -1) || !(Mathf.Abs(hit.position.y - coverPoints[i].gamePos.y) < 0.3f) || !((hit.position - ray.origin).xz().sqrMagnitude < 0.0025f)) ? (num5 + 0.2f) : (num5 - 0.2f));
						}
						num -= 0.2f;
						continue;
					}
					if (flag)
					{
						j = num3;
						num += 0.2f;
						num5 += 0.2f;
					}
					else
					{
						num -= 0.2f;
					}
					flag = true;
				}
				if (num4 < 0f)
				{
					num4 = 0f;
				}
				if (num5 > 0f)
				{
					num5 = 0f;
				}
				coverPoints[i].minExtent = num;
				coverPoints[i].minProprietaryExtent = num5;
				if (!flag)
				{
					num = -5f;
				}
				f = 0.61f + num * num;
				coverPoints[i].coverAngleSinL = -0.6f / Mathf.Sqrt(f);
			}
			else if (coverPoints[i].type == CoverPointCore.Type.HighCornerLeft)
			{
				coverPoints[i].coverAngleSinL = -0.6f;
				coverPoints[i].coverAngleSinR = -0.4f;
			}
			else if (coverPoints[i].type == CoverPointCore.Type.HighCornerRight)
			{
				coverPoints[i].coverAngleSinL = -0.4f;
				coverPoints[i].coverAngleSinR = -0.6f;
			}
			coverPoints[i].cappedLookL = VectorFromAxesAndSin(coverPoints[i].snappedNormal.xz(), -coverPoints[i].snappedTangent.xz(), coverPoints[i].coverAngleSinL);
			coverPoints[i].cappedLookR = VectorFromAxesAndSin(coverPoints[i].snappedNormal.xz(), coverPoints[i].snappedTangent.xz(), coverPoints[i].coverAngleSinR);
		}
		for (int i = 0; i < length - 1; i++)
		{
			ShowProgressBar("Determining Cover Extent (2/2)", (float)i / (float)length, false);
			if (coverPoints[i].type != CoverPointCore.Type.ShootOver)
			{
				continue;
			}
			Vector3 snappedTangent = coverPoints[i].snappedTangent;
			for (int j = i + 1; j < length; j++)
			{
				if (coverPoints[j].type != CoverPointCore.Type.ShootOver || Vector3.Dot(coverPoints[i].snappedNormal, coverPoints[j].snappedNormal) < 0.5f)
				{
					continue;
				}
				Vector3 snappedTangent2 = coverPoints[j].snappedTangent;
				Vector3 lhs = coverPoints[j].snappedPos - coverPoints[i].snappedPos;
				float num6 = Vector3.Dot(lhs, snappedTangent);
				if (num6 < coverPoints[i].minProprietaryExtent - 0.2f || num6 > coverPoints[i].maxProprietaryExtent + 0.2f)
				{
					continue;
				}
				float num7 = 0f - Vector3.Dot(lhs, snappedTangent2);
				if (!(num7 < coverPoints[j].minProprietaryExtent - 0.2f) && !(num7 > coverPoints[j].maxProprietaryExtent + 0.2f) && !(Mathf.Abs(Vector3.Dot(lhs, coverPoints[i].snappedNormal)) > 1f))
				{
					if (num6 > 0f)
					{
						coverPoints[i].adjacentRight = j;
						coverPoints[j].adjacentLeft = i;
						coverPoints[i].maxProprietaryExtent = num6 * 0.5f;
						coverPoints[j].minProprietaryExtent = num7 * 0.5f;
					}
					else
					{
						coverPoints[i].adjacentLeft = j;
						coverPoints[j].adjacentRight = i;
						coverPoints[i].minProprietaryExtent = num6 * 0.5f;
						coverPoints[j].maxProprietaryExtent = num7 * 0.5f;
					}
				}
			}
		}
	}

	private Vector2 VectorFromAxesAndSin(Vector2 n, Vector2 t, float s)
	{
		return n * s + t * Mathf.Sqrt(Mathf.Max(0f, 1f - s * s));
	}

	private void ApprehendAdjacency()
	{
		int length = coverPoints.GetLength(0);
		float[,] array = new float[length, length];
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Apprehending Adjacency (1/3)", (float)i / (float)length, false);
			Vector3 snappedTangent = coverPoints[i].snappedTangent;
			if (coverPoints[i].type == CoverPointCore.Type.InAir || coverPoints[i].type == CoverPointCore.Type.OpenGround || (coverPoints[i].adjacentLeft != -1 && coverPoints[i].adjacentRight != -1))
			{
				continue;
			}
			if (coverPoints[i].type == CoverPointCore.Type.HighCornerRight)
			{
				snappedTangent *= -1f;
			}
			float num = 4f;
			for (int j = 0; j < length; j++)
			{
				if (i == j || coverPoints[j].type == CoverPointCore.Type.InAir || coverPoints[j].type == CoverPointCore.Type.OpenGround || Vector3.Dot(coverPoints[i].snappedNormal, coverPoints[j].snappedNormal) < 0.4f)
				{
					continue;
				}
				Vector3 lhs = coverPoints[j].snappedPos - coverPoints[i].snappedPos;
				float num2 = Vector3.Dot(lhs, snappedTangent);
				array[i, j] = Mathf.Abs(num2);
				if (array[i, j] > num || array[i, j] < 1f)
				{
					continue;
				}
				if (num2 < 0f)
				{
					if (coverPoints[i].adjacentLeft != -1)
					{
						continue;
					}
				}
				else if (coverPoints[i].adjacentRight != -1)
				{
					continue;
				}
				if (!(Mathf.Abs(Vector3.Dot(lhs, coverPoints[i].snappedNormal)) > 1.2f) && !(GetCoverToCoverDistance(coverPoints[i], coverPoints[j]) > num2 + 2f))
				{
					num = array[i, j];
					if (num2 > 0f)
					{
						coverPoints[i].adjacentRight = j;
					}
					else
					{
						coverPoints[i].adjacentLeft = j;
					}
				}
			}
		}
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Apprehending Adjacency (2/3)", (float)i / (float)length, false);
			if (coverPoints[i].adjacentLeft != -1 && coverPoints[coverPoints[i].adjacentLeft].adjacentRight == -1)
			{
				coverPoints[coverPoints[i].adjacentLeft].adjacentRight = i;
			}
			if (coverPoints[i].adjacentRight != -1 && coverPoints[coverPoints[i].adjacentRight].adjacentLeft == -1)
			{
				coverPoints[coverPoints[i].adjacentRight].adjacentLeft = i;
			}
		}
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Apprehending Adjacency (3/3)", (float)i / (float)length, false);
			CoverPointCore coverPointCore = coverPoints[i];
			if (coverPointCore.adjacentLeft == -1 || coverPointCore.adjacentRight == -1)
			{
				continue;
			}
			if (coverPoints[coverPointCore.adjacentRight].type == CoverPointCore.Type.HighWall)
			{
				break;
			}
			bool flag = false;
			switch (coverPointCore.type)
			{
			case CoverPointCore.Type.ShootOver:
			case CoverPointCore.Type.HighWall:
				flag = array[i, coverPointCore.adjacentLeft] > array[i, coverPointCore.adjacentRight];
				break;
			case CoverPointCore.Type.HighCornerRight:
				if (coverPoints[coverPointCore.adjacentLeft].type != CoverPointCore.Type.HighCornerLeft && coverPoints[coverPointCore.adjacentRight].type != CoverPointCore.Type.HighCornerLeft)
				{
					flag = array[i, coverPointCore.adjacentLeft] > array[i, coverPointCore.adjacentRight];
				}
				break;
			case CoverPointCore.Type.HighCornerLeft:
				flag = coverPoints[coverPointCore.adjacentRight].type == CoverPointCore.Type.HighCornerRight || coverPoints[coverPointCore.adjacentLeft].type == CoverPointCore.Type.HighCornerRight || array[i, coverPointCore.adjacentLeft] > array[i, coverPointCore.adjacentRight];
				break;
			}
			if (flag)
			{
				int adjacentLeft = coverPointCore.adjacentLeft;
				coverPointCore.adjacentLeft = coverPointCore.adjacentRight;
				coverPointCore.adjacentRight = adjacentLeft;
			}
		}
	}

	private int SortGoodCover(int n1, int n2)
	{
		if (distanceTable[sortRelativeTo, n1] < distanceTable[sortRelativeTo, n2])
		{
			return 1;
		}
		if (distanceTable[sortRelativeTo, n1] > distanceTable[sortRelativeTo, n2])
		{
			return -1;
		}
		return 0;
	}

	private void ExtrapolateConcealmentField()
	{
		List<int> list = new List<int>();
		int length = coverPoints.GetLength(0);
		int num = 0;
		bool visibilityUncertain;
		bool coverUncertain;
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Extrapolating Concealment Field (1/4)", (float)i / (float)length, false);
			list.Clear();
			for (int j = 0; j < length; j++)
			{
				if (i == j || !SameSubsection(i, j))
				{
					continue;
				}
				switch (baked.coverTable.GetCoverProvided(j, i, out visibilityUncertain, out coverUncertain))
				{
				case CoverTable.CoverProvided.Low:
				case CoverTable.CoverProvided.High:
					list.Add(j);
					break;
				case CoverTable.CoverProvided.Full:
					if (visibilityUncertain && !coverUncertain)
					{
						list.Add(j);
					}
					break;
				}
			}
			sortRelativeTo = i;
			list.Sort(SortGoodCover);
			CoverPointCore coverPointCore = coverPoints[i];
			if (list.Count > 0)
			{
				coverPointCore.significantCover = list.ToArray();
			}
			else
			{
				coverPointCore.significantCover = null;
			}
			if (list.Count > num)
			{
				num = list.Count;
			}
		}
		GameObject gameObject = null;
		bool[,] array = new bool[length, num];
		CoverTable.CoverProvided frcover;
		CoverTable.CoverProvided tocover;
		bool crouchToCrouchBlocked;
		bool frCrouchToStandingBlocked;
		bool frStandingToCrouchBlocked;
		for (int k = 0; k < baked.coverGridXDim; k++)
		{
			ShowProgressBar("Extrapolating Concealment Field (2/4)", (float)k / (float)baked.coverGridXDim, false);
			for (int l = 0; l < baked.coverGridZDim; l++)
			{
				if (!(gridPos[k, l].y < 100000f))
				{
					continue;
				}
				uint data = baked.CoverGrid(k, l);
				int num2 = PackedCoverData.UnpackFirstIndex(data);
				if (num2 == 1023)
				{
					continue;
				}
				CoverPointCore coverPointCore = coverPoints[num2];
				if (coverPointCore.significantCover == null)
				{
					continue;
				}
				for (int i = 0; i < coverPointCore.significantCover.Length; i++)
				{
					CoverPointCore coverPointCore2 = coverPoints[coverPointCore.significantCover[i]];
					DeduceCoverProvided(gridPos[k, l], coverPointCore2.coverCheckPos - gridPos[k, l], CoverPointCore.Type.OpenGround, coverPointCore2.coverCheckPos, coverPointCore2.snappedNormal, coverPointCore2.type, out frcover, out tocover, out crouchToCrouchBlocked, out frCrouchToStandingBlocked, out frStandingToCrouchBlocked, 0f, 0f, 0f, 0f);
					switch (baked.coverTable.GetCoverProvided(coverPointCore.significantCover[i], num2, out visibilityUncertain, out coverUncertain))
					{
					case CoverTable.CoverProvided.Full:
					case CoverTable.CoverProvided.Stupid:
						if ((int)tocover < 4)
						{
							array[num2, i] = true;
						}
						break;
					case CoverTable.CoverProvided.Low:
						if ((int)tocover >= 4 || tocover == CoverTable.CoverProvided.None)
						{
							array[num2, i] = true;
						}
						break;
					default:
						if ((int)tocover >= 4)
						{
							array[num2, i] = true;
						}
						break;
					}
				}
			}
		}
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Extrapolating Concealment Field (3/4)", (float)i / (float)length, false);
			if (coverPoints[i].significantCover == null)
			{
				continue;
			}
			list.Clear();
			for (int j = 0; j < coverPoints[i].significantCover.GetLength(0); j++)
			{
				if (array[i, j])
				{
					list.Add(coverPoints[i].significantCover[j]);
				}
			}
			if (list.Count > 0)
			{
				if (list.Count > 32)
				{
					list.RemoveRange(32, list.Count - 32);
				}
				coverPoints[i].significantCover = list.ToArray();
				footprint += coverPoints[i].significantCover.GetLength(0) * 4;
				goodcover_footprint += coverPoints[i].significantCover.GetLength(0) * 4;
			}
			else
			{
				coverPoints[i].significantCover = null;
			}
		}
		for (int k = 0; k < baked.coverGridXDim; k++)
		{
			ShowProgressBar("Extrapolating Concealment Field (4/4)", (float)k / (float)baked.coverGridXDim, false);
			for (int l = 0; l < baked.coverGridZDim; l++)
			{
				if (!(gridPos[k, l].y < 100000f))
				{
					continue;
				}
				uint data = baked.CoverGrid(k, l);
				int num2 = PackedCoverData.UnpackFirstIndex(data);
				uint num3 = 0u;
				if (num2 != 1023)
				{
					CoverPointCore coverPointCore = coverPoints[num2];
					if (coverPointCore.significantCover != null)
					{
						for (int i = 0; i < coverPointCore.significantCover.Length; i++)
						{
							CoverPointCore coverPointCore3 = coverPoints[coverPointCore.significantCover[i]];
							DeduceCoverProvided(gridPos[k, l], coverPointCore3.coverCheckPos - gridPos[k, l], CoverPointCore.Type.OpenGround, coverPointCore3.coverCheckPos, coverPointCore3.snappedNormal, coverPointCore3.type, out frcover, out tocover, out crouchToCrouchBlocked, out frCrouchToStandingBlocked, out frStandingToCrouchBlocked, 0f, 0f, 0f, 0f);
							switch (baked.coverTable.GetCoverProvided(coverPointCore.significantCover[i], num2))
							{
							case CoverTable.CoverProvided.Full:
							case CoverTable.CoverProvided.Stupid:
								if ((int)tocover < 4)
								{
									num3 |= (uint)(1 << i);
									if (gameObject != null)
									{
										gameObject.GetComponent<Renderer>().material.color = Color.red;
									}
								}
								continue;
							case CoverTable.CoverProvided.Low:
								if ((int)tocover >= 4 || tocover == CoverTable.CoverProvided.None)
								{
									num3 |= (uint)(1 << i);
								}
								continue;
							}
							if ((int)tocover >= 4)
							{
								num3 |= (uint)(1 << i);
								if (gameObject != null)
								{
									gameObject.GetComponent<Renderer>().material.color = Color.green;
								}
							}
						}
					}
				}
				baked.ConcealmentField(k, l, num3);
			}
		}
	}

	private void DivineRouteMatrix()
	{
		int length = coverPoints.GetLength(0);
		routeMatrix = new int[length * length];
		for (int i = 0; i < length * length; i++)
		{
			routeMatrix[i] = -1;
		}
		int[] array = new int[length];
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Divining Route Matrix", (float)i / (float)length, false);
			int num = coverPoints[i].neighbours.Length;
			for (int j = 0; j < num; j++)
			{
				int num2 = CoverNeighbour.coverIndex(coverPoints[i].neighbours[j]);
				if (i == num2)
				{
					uint data = 0u;
					PackedCoverData.PackFirstIndex(ref data, i);
					routeMatrix[i * length + num2] = (int)data;
				}
				else
				{
					if (routeMatrix[i * length + num2] != -1)
					{
						continue;
					}
					UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
					UnityEngine.AI.NavMesh.CalculatePath(coverPoints[i].gamePos, coverPoints[num2].gamePos, walkableMask, navMeshPath);
					int num3 = 0;
					if (navMeshPath.corners.Length < 2)
					{
						uint data = 0u;
						PackedCoverData.PackFirstIndex(ref data, 1023);
						routeMatrix[i * length + num2] = (int)data;
						continue;
					}
					array[0] = i;
					num3 = 1;
					for (int k = 0; k < navMeshPath.corners.Length - 1; k++)
					{
						Vector3 vector = navMeshPath.corners[k];
						Vector3 vector2 = navMeshPath.corners[k + 1];
						Vector3 vector3 = vector2 - vector;
						float magnitude = vector3.magnitude;
						for (float num4 = 0f; num4 < magnitude; num4 += 1f)
						{
							Vector3 vector4 = vector + vector3 * (num4 / magnitude);
							CoverPointCore coverPointCore = FindClosestCoverPoint_Fast(vector4);
							if (!(coverPointCore != null))
							{
								continue;
							}
							int l;
							for (l = 0; l < num3 && array[l] != coverPointCore.index; l++)
							{
							}
							if (l < num3)
							{
								continue;
							}
							Vector3 normalized = (coverPointCore.gamePos - vector4).normalized;
							if (Vector3.Dot(normalized, vector3) > magnitude * 0.4f)
							{
								uint data = 0u;
								PackedCoverData.PackFirstIndex(ref data, coverPointCore.index);
								PackedCoverData.PackDistance(ref data, distanceTable[array[num3 - 1], coverPointCore.index]);
								routeMatrix[array[num3 - 1] * length + num2] = (int)data;
								array[num3++] = coverPointCore.index;
								if (routeMatrix[coverPointCore.index * length + num2] != -1 || coverPointCore.index == num2)
								{
									k = navMeshPath.corners.Length;
									num4 = magnitude;
									array[num3 - 1] = num2;
								}
							}
						}
					}
					if (array[num3 - 1] != num2)
					{
						uint data = 0u;
						PackedCoverData.PackFirstIndex(ref data, num2);
						PackedCoverData.PackDistance(ref data, distanceTable[array[num3 - 1], num2]);
						routeMatrix[array[num3 - 1] * length + num2] = (int)data;
					}
				}
			}
		}
		for (int i = 0; i < length; i++)
		{
			int num = coverPoints[i].neighbours.Length;
			coverPoints[i].routeOnward = new int[num];
			routeMatrix_footprint += num * 4;
			footprint += num * 4;
			for (int num2 = 0; num2 < num; num2++)
			{
				int k = CoverNeighbour.coverIndex(coverPoints[i].neighbours[num2]);
				uint data = (uint)routeMatrix[i * length + k];
				int num5 = PackedCoverData.UnpackFirstIndex((uint)routeMatrix[i * length + k]);
				for (int m = 0; m < coverPoints[num5].neighbours.Length; m++)
				{
					if (CoverNeighbour.coverIndex(coverPoints[num5].neighbours[m]) == k)
					{
						PackedCoverData.PackSecondIndex(ref data, m);
					}
				}
				coverPoints[i].routeOnward[num2] = (int)data;
			}
		}
	}

	private void ProduceUncertaintyMap(bool justSlowBit)
	{
		Vector3 zero = Vector3.zero;
		zero.y = base.transform.position.y + 80f;
		Vector3 vector = new Vector3(0f, 1.7f, 0f);
		int coverGridXDim = baked.coverGridXDim;
		int coverGridZDim = baked.coverGridZDim;
		bool[,] array = new bool[coverPoints.GetLength(0), coverPoints.GetLength(0)];
		int num = 1;
		for (int i = 0; i < coverPoints.GetLength(0); i++)
		{
			for (int j = 0; j < coverPoints.GetLength(0); j++)
			{
				array[i, j] = false;
				baked.coverTable.SetVisibilityUncertainAwayFromEither(i, j, false);
			}
		}
		for (int i = 0; i < coverPoints.GetLength(0); i++)
		{
			if (!sourceList[i].name.Contains("U:"))
			{
				continue;
			}
			for (int j = 0; j < coverPoints.GetLength(0); j++)
			{
				if (i != j && sourceList[i].name.Contains(sourceList[j].name))
				{
					array[i, j] = true;
					array[j, i] = true;
					baked.coverTable.SetVisibilityUncertainAwayFromEither(i, j, true);
					baked.coverTable.SetVisibilityUncertainAwayFromEither(j, i, true);
					baked.coverTable.SetVisibilityUncertainAwayFromThere(i, j, true);
					baked.coverTable.SetVisibilityUncertainAwayFromThere(j, i, true);
				}
			}
		}
		UncertaintyTag.ForceOff();
		for (int k = 0; k < coverGridXDim; k++)
		{
			ShowProgressBar("Producing Uncertainty Map (1/4: Place To Cover)", (float)k / (float)coverGridXDim, false);
			for (int l = 0; l < coverGridZDim; l++)
			{
				if (originalClosest[k, l] < 0)
				{
					continue;
				}
				Vector3 vector2 = gridPos[k, l];
				if (vector2.y > 100000f || gridDistance[k, l] > 10f)
				{
					continue;
				}
				CoverPointCore coverPointCore = coverPoints[originalClosest[k, l]];
				for (int i = 0; i < coverPoints.GetLength(0); i++)
				{
					if (i == coverPointCore.index || !SameSubsection(i, coverPointCore.index))
					{
						continue;
					}
					CoverTable.CoverProvided coverProvided = baked.coverTable.GetCoverProvided(i, coverPointCore.index);
					CoverTable.CoverProvided frcover;
					CoverTable.CoverProvided tocover;
					bool crouchToCrouchBlocked;
					bool frCrouchToStandingBlocked;
					bool frStandingToCrouchBlocked;
					DeduceCoverProvided(vector2, coverPoints[i].coverCheckPos - vector2, CoverPointCore.Type.OpenGround, coverPoints[i].coverCheckPos, coverPoints[i].snappedNormal, coverPoints[i].type, out frcover, out tocover, out crouchToCrouchBlocked, out frCrouchToStandingBlocked, out frStandingToCrouchBlocked, 0f, 0f, coverPoints[i].coverAngleSinL, coverPoints[i].coverAngleSinR);
					switch (coverProvided)
					{
					case CoverTable.CoverProvided.None:
					case CoverTable.CoverProvided.Crouch:
						if (frcover == CoverTable.CoverProvided.Full)
						{
							baked.coverTable.SetVisibilityUncertainAwayFromThere(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, i, true);
							array[i, coverPointCore.index] = true;
							array[coverPointCore.index, i] = true;
						}
						break;
					case CoverTable.CoverProvided.Low:
						switch (tocover)
						{
						case CoverTable.CoverProvided.Full:
							baked.coverTable.SetVisibilityUncertainAwayFromThere(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, i, true);
							array[i, coverPointCore.index] = true;
							array[coverPointCore.index, i] = true;
							break;
						case CoverTable.CoverProvided.None:
							baked.coverTable.SetCoverHereWorseAwayFromThere(i, coverPointCore.index, true);
							break;
						}
						break;
					case CoverTable.CoverProvided.High:
						baked.coverTable.SetVisibilityUncertainAwayFromThere(i, coverPointCore.index, true);
						baked.coverTable.SetVisibilityUncertainAwayFromEither(i, coverPointCore.index, true);
						baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, i, true);
						array[i, coverPointCore.index] = true;
						array[coverPointCore.index, i] = true;
						if (tocover == CoverTable.CoverProvided.None)
						{
							baked.coverTable.SetCoverHereWorseAwayFromThere(i, coverPointCore.index, true);
						}
						break;
					case CoverTable.CoverProvided.Full:
					case CoverTable.CoverProvided.Stupid:
						if (frcover == CoverTable.CoverProvided.None)
						{
							baked.coverTable.SetVisibilityUncertainAwayFromThere(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(i, coverPointCore.index, true);
							baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, i, true);
							array[i, coverPointCore.index] = true;
							array[coverPointCore.index, i] = true;
						}
						if (tocover == CoverTable.CoverProvided.None)
						{
							baked.coverTable.SetCoverHereWorseAwayFromThere(i, coverPointCore.index, true);
						}
						break;
					}
				}
			}
		}
		baked.coverTable.uncertaintyMapInvalid = true;
		if (!quickBake)
		{
			List<int>[,] array2 = new List<int>[coverGridXDim, coverGridZDim];
			Debug.Log("Doing The Slow Bit...");
			for (int m = 0; m < coverGridXDim; m++)
			{
				ShowProgressBar("Producing Uncertainty Map (2/4: Place To Place (init))", (float)m / (float)coverGridXDim, false);
				for (int n = 0; n < coverGridZDim; n++)
				{
					array2[m, n] = new List<int>();
					if (originalClosest[m, n] < 0 || gridPos[m, n].y > 100000f)
					{
						continue;
					}
					array2[m, n].Add(originalClosest[m, n]);
					for (int num2 = m - 1; num2 <= m + 1; num2++)
					{
						if (num2 < 0 || num2 >= coverGridXDim)
						{
							continue;
						}
						for (int num3 = n - 1; num3 <= n + 1; num3++)
						{
							if (num3 >= 0 && num3 < coverGridZDim && originalClosest[num2, num3] >= 0 && !(gridPos[num2, num3].y > 100000f) && !array2[m, n].Contains(originalClosest[num2, num3]) && !Physics.Linecast(gridPos[m, n] + vector, gridPos[num2, num3] + vector, defaultAndRoofLayers))
							{
								array2[m, n].Add(originalClosest[num2, num3]);
							}
						}
					}
				}
			}
			Savings = 0;
			int num4 = 0;
			baked.coverTable.uncertaintyMapInvalid = false;
			int[] array3 = new int[8] { 4, 2, 6, 1, 3, 5, 7, 0 };
			int k = -8;
			num = 0;
			for (int m = 0; m < coverGridXDim; m++)
			{
				k += 8;
				if (k >= coverGridXDim)
				{
					k = array3[num];
					num++;
				}
				if (ShowProgressBar("Producing Uncertainty Map (3/4: Place To Place)", (float)m / (float)coverGridXDim, true))
				{
					baked.coverTable.uncertaintyMapInvalid = true;
					m = coverGridXDim;
					break;
				}
				int l = -8;
				int num5 = 0;
				for (int n = 0; n < coverGridZDim; n++)
				{
					l += 8;
					if (l >= coverGridZDim)
					{
						l = array3[num5];
						num5++;
					}
					if (originalClosest[k, l] < 0)
					{
						continue;
					}
					Vector3 vector2 = gridPos[k, l];
					if (vector2.y > 100000f || gridDistance[k, l] > 10f)
					{
						continue;
					}
					int num6 = 0;
					int num2 = -8;
					for (int num7 = 0; num7 < coverGridXDim; num7++)
					{
						num2 += 8;
						if (num2 >= coverGridXDim)
						{
							num2 = array3[num6];
							num6++;
						}
						int num3 = -8;
						int num8 = 0;
						for (int num9 = 0; num9 < coverGridZDim; num9++)
						{
							num3 += 8;
							if (num3 >= coverGridZDim)
							{
								num3 = array3[num8];
								num8++;
							}
							if (k * coverGridZDim + l > num2 * coverGridZDim + num3)
							{
								continue;
							}
							Vector3 vector3 = gridPos[num2, num3];
							if (vector3.y > 100000f)
							{
								continue;
							}
							bool flag = false;
							bool flag2 = false;
							for (int i = 0; i < array2[k, l].Count; i++)
							{
								CoverPointCore coverPointCore = coverPoints[array2[k, l][i]];
								for (int j = 0; j < array2[num2, num3].Count; j++)
								{
									int num10 = array2[num2, num3][j];
									if (array[coverPointCore.index, num10])
									{
										continue;
									}
									CoverPointCore coverPointCore2 = coverPoints[num10];
									if (!SameSubsection(coverPointCore2, coverPointCore))
									{
										continue;
									}
									if (!flag)
									{
										flag2 = Physics.Linecast(vector2 + vector, vector3 + vector, defaultAndRoofLayers);
										num4++;
									}
									flag = true;
									switch (baked.coverTable.GetCoverProvided(coverPointCore.index, coverPointCore2.index))
									{
									case CoverTable.CoverProvided.None:
									case CoverTable.CoverProvided.Crouch:
									case CoverTable.CoverProvided.Low:
									case CoverTable.CoverProvided.High:
										if (flag2)
										{
											baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, coverPointCore2.index, true);
											baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore2.index, coverPointCore.index, true);
											array[coverPointCore.index, coverPointCore2.index] = true;
											array[coverPointCore2.index, coverPointCore.index] = true;
										}
										break;
									case CoverTable.CoverProvided.Full:
									case CoverTable.CoverProvided.Stupid:
										if (!flag2)
										{
											array[coverPointCore.index, coverPointCore2.index] = true;
											array[coverPointCore2.index, coverPointCore.index] = true;
											baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore.index, coverPointCore2.index, true);
											baked.coverTable.SetVisibilityUncertainAwayFromEither(coverPointCore2.index, coverPointCore.index, true);
										}
										break;
									}
								}
							}
						}
					}
				}
			}
			for (int i = 0; i < coverPoints.GetLength(0); i++)
			{
				for (int j = 0; j < coverPoints.GetLength(0); j++)
				{
					array[i, j] = false;
					baked.coverTable.SetCrouchCoverUncertain(i, j, false);
				}
			}
			k = -8;
			num = 0;
			for (int m = 0; m < coverGridXDim; m++)
			{
				k += 8;
				if (k >= coverGridXDim)
				{
					k = array3[num];
					num++;
				}
				if (ShowProgressBar("Producing Uncertainty Map (4/4: Crouch Cover Uncertainty)", (float)m / (float)coverGridXDim, true))
				{
					baked.coverTable.uncertaintyMapInvalid = true;
					m = coverGridXDim;
					break;
				}
				int l = -8;
				int num5 = 0;
				for (int n = 0; n < coverGridZDim; n++)
				{
					l += 8;
					if (l >= coverGridZDim)
					{
						l = array3[num5];
						num5++;
					}
					if (originalClosest[k, l] < 0)
					{
						continue;
					}
					Vector3 vector2 = gridPos[k, l];
					if (gridDistance[k, l] > 10f)
					{
						continue;
					}
					CoverPointCore coverPointCore = coverPoints[originalClosest[k, l]];
					int num6 = 0;
					int num2 = -8;
					for (int num7 = 0; num7 < coverGridXDim; num7++)
					{
						num2 += 8;
						if (num2 >= coverGridXDim)
						{
							num2 = array3[num6];
							num6++;
						}
						int num3 = -8;
						int num8 = 0;
						for (int num9 = 0; num9 < coverGridZDim; num9++)
						{
							num3 += 8;
							if (num3 >= coverGridZDim)
							{
								num3 = array3[num8];
								num8++;
							}
							int num11 = originalClosest[num2, num3];
							if (k * coverGridZDim + l >= num2 * coverGridZDim + num3 || num11 < 0 || (array[coverPointCore.index, num11] && array[num11, coverPointCore.index]))
							{
								continue;
							}
							Vector3 vector4 = gridPos[num2, num3];
							CoverPointCore coverPointCore3 = coverPoints[num11];
							if (!SameSubsection(coverPointCore3, coverPointCore))
							{
								array[coverPointCore.index, num11] = true;
								array[num11, coverPointCore.index] = true;
								continue;
							}
							if (baked.coverTable.EstimateVisibility(coverPointCore.index, false, coverPointCore3.index, false) == CoverTable.VisibilityEstimate.Impossible)
							{
								array[coverPointCore.index, num11] = true;
								array[num11, coverPointCore.index] = true;
								continue;
							}
							if (baked.coverTable.GetShootingFromCrouchToStandingBlocked(coverPointCore.index, coverPointCore3.index) != Physics.Linecast(vector2 + crouchingHeightOffset, vector4 + standingHeightOffset, defaultAndRoofLayers))
							{
								baked.coverTable.SetCrouchCoverUncertain(coverPointCore.index, coverPointCore3.index, true);
								array[coverPointCore.index, num11] = true;
							}
							if (baked.coverTable.GetShootingFromCrouchToStandingBlocked(coverPointCore3.index, coverPointCore.index) != Physics.Linecast(vector2 + standingHeightOffset, vector4 + crouchingHeightOffset, defaultAndRoofLayers))
							{
								baked.coverTable.SetCrouchCoverUncertain(coverPointCore3.index, coverPointCore.index, true);
								array[num11, coverPointCore.index] = true;
							}
						}
					}
				}
			}
			Debug.Log("Done The Slow Bit! " + Savings + " " + num4);
		}
		UncertaintyTag.Restore();
		bakestage++;
	}

	private void CreateCoverGrid()
	{
		UncertaintyTag.ForceOff();
		Vector3 zero = Vector3.zero;
		zero.y = base.transform.position.y + 80f;
		Ray ray = default(Ray);
		baked.coverGridXDim = (int)(baked.extent.x / tileside);
		baked.coverGridZDim = (int)(baked.extent.y / tileside);
		baked.tileX = baked.extent.x / (float)(baked.coverGridXDim - 1);
		baked.tileZ = baked.extent.y / (float)(baked.coverGridZDim - 1);
		baked.coverGrid = new int[baked.coverGridXDim * baked.coverGridZDim];
		baked.concealmentField = new int[baked.coverGridXDim * baked.coverGridZDim];
		int[,] array = new int[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		int[,] array2 = new int[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		uint[,] array3 = new uint[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		footprint += 8 * baked.coverGridXDim * baked.coverGridZDim;
		covergrid_footprint = 4 * baked.coverGridXDim * baked.coverGridZDim;
		concealmentField_footprint = 4 * baked.coverGridXDim * baked.coverGridZDim;
		gridPos = new Vector3[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		originalClosest = new int[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		gridDistance = new float[baked.coverGridXDim + 1, baked.coverGridZDim + 1];
		GameObject gameObject = null;
		if (showProblemAreas)
		{
			gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			gameObject.name = "DELETEME";
			gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			Object.DestroyImmediate(gameObject.GetComponent<BoxCollider>());
		}
		GameObject gameObject2 = null;
		int num = baked.coverGridXDim / 10 + 1;
		int num2 = baked.coverGridZDim / 10 + 1;
		List<int>[,] array4 = new List<int>[num + 1, num2 + 1];
		int length = coverPoints.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			ShowProgressBar("Creating Cover Grid (1/6: Coarse Gridding)", (float)i / (float)length, false);
			CoverPointCore coverPointCore = coverPoints[i];
			if (coverPointCore.type != CoverPointCore.Type.InAir)
			{
				int num3 = (int)((coverPointCore.gamePos.x - baked.corner.x) / (baked.tileX * 10f));
				int num4 = (int)((coverPointCore.gamePos.z - baked.corner.y) / (baked.tileZ * 10f));
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (array4[num3, num4] == null)
				{
					array4[num3, num4] = new List<int>();
				}
				array4[num3, num4].Add(coverPointCore.index);
			}
		}
		int[,] array5 = new int[25, 2]
		{
			{ 0, 0 },
			{ 1, 0 },
			{ -1, 0 },
			{ 0, -1 },
			{ 0, 1 },
			{ -1, -1 },
			{ -1, 1 },
			{ 1, -1 },
			{ 1, 1 },
			{ 0, -2 },
			{ 0, 2 },
			{ 2, 0 },
			{ -2, 0 },
			{ 1, -2 },
			{ 1, 2 },
			{ 2, 1 },
			{ -2, 1 },
			{ -1, -2 },
			{ -1, 2 },
			{ 2, -1 },
			{ -2, -1 },
			{ -2, -2 },
			{ -2, 2 },
			{ 2, -2 },
			{ 2, 2 }
		};
		RaycastHit hitInfo;
		UnityEngine.AI.NavMeshHit hit;
		for (int j = 0; j <= baked.coverGridXDim; j++)
		{
			ShowProgressBar("Creating Cover Grid (2/6: Fine Gridding)", (float)j / (float)baked.coverGridXDim, false);
			for (int k = 0; k <= baked.coverGridZDim; k++)
			{
				zero.x = baked.corner.x + baked.tileX * (float)j;
				zero.z = baked.corner.y + baked.tileZ * (float)k;
				gridPos[j, k] = zero;
				ray.direction = Vector3.down;
				ray.origin = zero;
				if (Physics.Raycast(ray, out hitInfo, 150f, defaultLayerMask))
				{
					gridPos[j, k] = hitInfo.point;
				}
				if (UnityEngine.AI.NavMesh.SamplePosition(gridPos[j, k], out hit, 1f, -1))
				{
					gridPos[j, k] = hit.position;
					array2[j, k] = hit.mask;
				}
				else
				{
					gridPos[j, k] = Vector3.zero;
					gridPos[j, k].y = 1000000f;
				}
			}
		}
		Vector2 vagueDir;
		for (int j = 0; j <= baked.coverGridXDim; j++)
		{
			ShowProgressBar("Creating Cover Grid (3/6: Initial Grid To Cover Distance)", (float)j / (float)baked.coverGridXDim, false);
			for (int k = 0; k <= baked.coverGridZDim; k++)
			{
				if (gridPos[j, k].y < 100000f)
				{
					float distance = float.MaxValue;
					int num3 = j / 10;
					int num4 = k / 10;
					vagueDir = Vector2.zero;
					array[j, k] = -1;
					for (int l = 0; l < 16; l++)
					{
						if (num3 + array5[l, 0] >= 0 && num3 + array5[l, 0] < num && num4 + array5[l, 1] >= 0 && num4 + array5[l, 1] < num2)
						{
							array[j, k] = FindClosestCoverPoint_List(array[j, k], array4[num3 + array5[l, 0], num4 + array5[l, 1]], gridPos[j, k], ref distance, ref vagueDir, walkableMaskNoDoors | array2[j, k]);
						}
					}
					originalClosest[j, k] = array[j, k];
					gridDistance[j, k] = distance;
					uint data = 0u;
					PackedCoverData.PackDirection(ref data, vagueDir);
					array3[j, k] = data;
					if (array[j, k] == -1)
					{
						gridPos[j, k].y = 1000000f;
					}
				}
				else
				{
					array[j, k] = -1;
				}
			}
		}
		bool flag = true;
		int num5 = 10;
		while (flag && num5 > 0)
		{
			flag = false;
			num5--;
			for (int j = 1; j < baked.coverGridXDim; j++)
			{
				ShowProgressBar("Creating Cover Grid (4/6: Spreading Under Geometry)", (float)j / (float)baked.coverGridXDim, false);
				for (int k = 1; k < baked.coverGridZDim; k++)
				{
					if (!(gridPos[j, k].y > 100000f))
					{
						continue;
					}
					for (int m = -1; m <= 1; m++)
					{
						for (int n = -1; n <= 1; n++)
						{
							if (array[j + m, k + n] < 0)
							{
								continue;
							}
							zero.x = baked.corner.x + baked.tileX * (float)j;
							zero.z = baked.corner.y + baked.tileZ * (float)k;
							Vector3 origin = zero;
							origin.y = gridPos[j + m, k + n].y + 1f;
							ray.direction = Vector3.down;
							ray.origin = origin;
							if (Physics.Raycast(ray, out hitInfo, 150f, defaultLayerMask) && UnityEngine.AI.NavMesh.SamplePosition(hitInfo.point, out hit, 1f, -1) && Mathf.Abs(hit.position.y - gridPos[j, k].y) > 0.1f)
							{
								gridPos[j, k] = hit.position;
								n = 2;
								m = 2;
								flag = true;
								if (num5 == 0 && gameObject != null)
								{
									gameObject2 = (GameObject)Object.Instantiate(gameObject);
									gameObject2.name += "Spreading ran out of iterations here";
									gameObject2.transform.position = hit.position;
									gameObject2.GetComponent<Renderer>().material.color = Color.red;
								}
								break;
							}
						}
					}
				}
			}
			if (!flag)
			{
				continue;
			}
			for (int j = 0; j <= baked.coverGridXDim; j++)
			{
				ShowProgressBar("Creating Cover Grid (5/6: Final Grid To Cover Distance)", (float)j / (float)baked.coverGridXDim, false);
				for (int k = 0; k <= baked.coverGridZDim; k++)
				{
					if (!(gridPos[j, k].y < 100000f) || array[j, k] != -1)
					{
						continue;
					}
					float distance2 = float.MaxValue;
					int num3 = j / 10;
					int num4 = k / 10;
					vagueDir = Vector2.zero;
					array[j, k] = -1;
					for (int num6 = 0; num6 < 16; num6++)
					{
						if (num3 + array5[num6, 0] >= 0 && num3 + array5[num6, 0] < num && num4 + array5[num6, 1] >= 0 && num4 + array5[num6, 1] < num2)
						{
							array[j, k] = FindClosestCoverPoint_List(array[j, k], array4[num3 + array5[num6, 0], num4 + array5[num6, 1]], gridPos[j, k], ref distance2, ref vagueDir, walkableMaskNoDoors | array2[j, k]);
						}
					}
					originalClosest[j, k] = array[j, k];
					gridDistance[j, k] = distance2;
					uint data2 = 0u;
					PackedCoverData.PackDirection(ref data2, vagueDir);
					array3[j, k] = data2;
				}
			}
		}
		if (num5 <= 0)
		{
			Debug.Log("WARNING: COVER GRID SPREADING TIMED OUT: Check your geometry!");
		}
		if (gameObject != null)
		{
			Object.DestroyImmediate(gameObject);
		}
		for (int j = 0; j < baked.coverGridXDim; j++)
		{
			ShowProgressBar("Creating Cover Grid (5/5: Finding Spanned Walls)", (float)j / (float)baked.coverGridXDim, false);
			for (int k = 0; k < baked.coverGridZDim; k++)
			{
				int num7 = -1;
				int num8 = -1;
				uint num9 = 0u;
				float num10 = 0f;
				float num11 = float.MaxValue;
				for (int num3 = j; num3 <= j + 1; num3++)
				{
					for (int num4 = k; num4 <= k + 1; num4++)
					{
						if (array[num3, num4] < 0)
						{
							continue;
						}
						if (num8 < 0 && num11 > gridDistance[num3, num4])
						{
							num7 = array[num3, num4];
							num11 = gridDistance[num3, num4];
							num9 = array3[num3, num4];
						}
						for (int num12 = j; num12 <= j + 1; num12++)
						{
							for (int num13 = k; num13 <= k + 1; num13++)
							{
								if ((num3 != num12 || (num4 != num13 && array[num12, num13] >= 0)) && array[num12, num13] >= 0 && array[num3, num4] != array[num12, num13])
								{
									num10 = CalcNavMeshDistance(gridPos[num3, num4], coverPoints[array[num12, num13]].gamePos, out vagueDir, walkableMaskNoDoors);
									if (num10 > gridDistance[num3, num4] + tileside * 3f && (num8 < 0 || gridDistance[num12, num13] < num11))
									{
										num8 = array[num3, num4];
										num7 = array[num12, num13];
										num11 = gridDistance[num12, num13];
										num9 = array3[num12, num13];
									}
								}
							}
						}
					}
				}
				uint data3 = 0u;
				num8 = ((num8 >= 0) ? ((num8 - num7) & 0x3FF) : 0);
				PackedCoverData.PackFirstIndex(ref data3, num7);
				PackedCoverData.PackSecondIndex(ref data3, num8);
				PackedCoverData.PackDistance(ref data3, num11);
				data3 |= num9;
				baked.CoverGrid(j, k, data3);
			}
		}
		bakestage++;
		UncertaintyTag.Restore();
	}

	private void GenerateStaticCoverTable()
	{
		baked.coverTable = ScriptableObject.CreateInstance<CoverTable>();
		baked.coverTable.CreateTable(coverPoints.GetLength(0));
		footprint += 1 * coverPoints.GetLength(0) * coverPoints.GetLength(0);
		CoverProvided_footprint = 1 * coverPoints.GetLength(0) * coverPoints.GetLength(0);
		CoverProvided_footprint += baked.coverTable.crouchCoverUncertain.Length * 4;
		UncertaintyTag.ForceOn();
		for (int i = 0; i < coverPoints.GetLength(0) - 1; i++)
		{
			ShowProgressBar("Generating Static Cover Table", (float)i / (float)coverPoints.GetLength(0), false);
			Vector3 coverCheckPos = coverPoints[i].coverCheckPos;
			Vector3 snappedNormal = coverPoints[i].snappedNormal;
			for (int j = i + 1; j < coverPoints.GetLength(0); j++)
			{
				Vector3 coverCheckPos2 = coverPoints[j].coverCheckPos;
				Vector3 snappedNormal2 = coverPoints[j].snappedNormal;
				showHit = false;
				CoverTable.CoverProvided frcover;
				CoverTable.CoverProvided tocover;
				bool crouchToCrouchBlocked;
				bool frCrouchToStandingBlocked;
				bool frStandingToCrouchBlocked;
				if (SameSubsection(coverPoints[j], coverPoints[i]))
				{
					DeduceCoverProvided(coverCheckPos, snappedNormal, coverPoints[i].type, coverCheckPos2, snappedNormal2, coverPoints[j].type, out frcover, out tocover, out crouchToCrouchBlocked, out frCrouchToStandingBlocked, out frStandingToCrouchBlocked, coverPoints[i].coverAngleSinL, coverPoints[i].coverAngleSinR, coverPoints[j].coverAngleSinL, coverPoints[j].coverAngleSinR);
				}
				else
				{
					tocover = CoverTable.CoverProvided.Full;
					frcover = CoverTable.CoverProvided.Full;
					crouchToCrouchBlocked = true;
					frCrouchToStandingBlocked = true;
					frStandingToCrouchBlocked = true;
					Savings++;
				}
				baked.coverTable.SetCoverProvided(j, i, tocover);
				baked.coverTable.SetCoverProvided(i, j, frcover);
				baked.coverTable.SetShootingFromCrouchToCrouchBlocked(i, j, crouchToCrouchBlocked);
				baked.coverTable.SetShootingFromCrouchToStandingBlocked(i, j, frCrouchToStandingBlocked);
				baked.coverTable.SetShootingFromCrouchToStandingBlocked(j, i, frStandingToCrouchBlocked);
			}
		}
		bakestage++;
		UncertaintyTag.Restore();
	}

	public bool SameSubsection(CoverPointCore cpc1, CoverPointCore cpc2)
	{
		return true;
	}

	public bool SameSubsection(int i, int j)
	{
		return true;
	}

	private Vector3 DeduceCoverProvided(Vector3 frpos, Vector3 frforward, CoverPointCore.Type ftype, Vector3 topos, Vector3 toforward, CoverPointCore.Type ttype, out CoverTable.CoverProvided frcover, out CoverTable.CoverProvided tocover, out bool crouchToCrouchBlocked, out bool frCrouchToStandingBlocked, out bool frStandingToCrouchBlocked, float coverAngleSinFrL, float coverAngleSinFrR, float coverAngleSinToL, float coverAngleSinToR)
	{
		Vector3 vector = topos - frpos;
		float magnitude = vector.magnitude;
		frcover = CoverTable.CoverProvided.None;
		tocover = CoverTable.CoverProvided.None;
		toforward.Normalize();
		frforward.Normalize();
		crouchToCrouchBlocked = true;
		frCrouchToStandingBlocked = true;
		frStandingToCrouchBlocked = true;
		vector.Normalize();
		float num = Vector3.Dot(frforward, vector);
		float num2 = 0f - Vector3.Dot(toforward, vector);
		float num3 = ((!(frforward.x * vector.z - frforward.z * vector.x < 0f)) ? coverAngleSinFrR : coverAngleSinFrL);
		float num4 = ((!(toforward.x * vector.z - toforward.z * vector.x > 0f)) ? coverAngleSinToR : coverAngleSinToL);
		RaycastHit hitInfo;
		if (Physics.Linecast(frpos + standingHeightOffset, topos + standingHeightOffset, out hitInfo, defaultAndRoofLayers))
		{
			frcover = CoverTable.CoverProvided.Full;
			tocover = CoverTable.CoverProvided.Full;
			if (ftype != 0 && num >= num3)
			{
				frcover = CoverTable.CoverProvided.Stupid;
			}
			if (ttype != 0 && num2 >= num4)
			{
				tocover = CoverTable.CoverProvided.Stupid;
			}
			switch (ftype)
			{
			case CoverPointCore.Type.HighCornerLeft:
				if (Vector3.Dot(vector, new Vector3(0f - frforward.z, 0f, frforward.x)) < 0f)
				{
					frcover = CoverTable.CoverProvided.Stupid;
				}
				break;
			case CoverPointCore.Type.HighCornerRight:
				if (Vector3.Dot(vector, new Vector3(0f - frforward.z, 0f, frforward.x)) > 0f)
				{
					frcover = CoverTable.CoverProvided.Stupid;
				}
				break;
			}
			switch (ttype)
			{
			case CoverPointCore.Type.HighCornerLeft:
				if (Vector3.Dot(vector, new Vector3(0f - toforward.z, 0f, toforward.x)) > 0f)
				{
					tocover = CoverTable.CoverProvided.Stupid;
				}
				break;
			case CoverPointCore.Type.HighCornerRight:
				if (Vector3.Dot(vector, new Vector3(0f - toforward.z, 0f, toforward.x)) < 0f)
				{
					tocover = CoverTable.CoverProvided.Stupid;
				}
				break;
			}
			if (showHit)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				Object.DestroyImmediate(gameObject.GetComponent<SphereCollider>());
				gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				gameObject.transform.position = hitInfo.point;
			}
			return hitInfo.point;
		}
		if (num < num3)
		{
			switch (ftype)
			{
			case CoverPointCore.Type.ShootOver:
				frcover = CoverTable.CoverProvided.Low;
				break;
			case CoverPointCore.Type.HighCornerLeft:
			case CoverPointCore.Type.HighCornerRight:
				frcover = CoverTable.CoverProvided.High;
				break;
			}
		}
		if (num2 < num4)
		{
			switch (ttype)
			{
			case CoverPointCore.Type.ShootOver:
				tocover = CoverTable.CoverProvided.Low;
				break;
			case CoverPointCore.Type.HighCornerLeft:
			case CoverPointCore.Type.HighCornerRight:
				tocover = CoverTable.CoverProvided.High;
				break;
			}
		}
		Vector3 start = frpos;
		Vector3 start2 = frpos;
		Vector3 end = topos;
		Vector3 end2 = topos;
		if (ftype != CoverPointCore.Type.InAir)
		{
			start += standingHeightOffset;
			start2 += crouchingHeightOffset;
		}
		if (ttype != CoverPointCore.Type.InAir)
		{
			end += standingHeightOffset;
			end2 += crouchingHeightOffset;
		}
		frStandingToCrouchBlocked = Physics.Linecast(start, end2, out hitInfo, defaultAndRoofLayers);
		frCrouchToStandingBlocked = Physics.Linecast(start2, end, out hitInfo, defaultAndRoofLayers);
		crouchToCrouchBlocked = Physics.Linecast(start2, end2, out hitInfo, defaultAndRoofLayers);
		if (magnitude > 8f)
		{
			if (tocover == CoverTable.CoverProvided.None && frStandingToCrouchBlocked)
			{
				tocover = CoverTable.CoverProvided.Crouch;
			}
			if (frcover == CoverTable.CoverProvided.None && frCrouchToStandingBlocked)
			{
				frcover = CoverTable.CoverProvided.Crouch;
			}
		}
		return Vector3.zero;
	}

	private int SortNeighbouringCover(int cn1, int cn2)
	{
		float num = CoverNeighbour.navMeshDistance(cn1);
		float num2 = CoverNeighbour.navMeshDistance(cn2);
		if (num > num2)
		{
			return 1;
		}
		if (num < num2)
		{
			return -1;
		}
		return 0;
	}

	private float CalcNavMeshDistance(Vector3 fr, Vector3 to, out Vector2 vagueDir, int navMeshMask)
	{
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		UnityEngine.AI.NavMesh.CalculatePath(fr, to, navMeshMask, navMeshPath);
		float num = 0f;
		vagueDir = Vector2.zero;
		if (navMeshPath.corners.GetLength(0) < 2 || navMeshPath.status != 0)
		{
			return float.MaxValue;
		}
		for (int i = 0; i < navMeshPath.corners.GetLength(0) - 1; i++)
		{
			num += (navMeshPath.corners[i] - navMeshPath.corners[i + 1]).magnitude;
		}
		vagueDir = (navMeshPath.corners[1] - navMeshPath.corners[0]).xz().normalized;
		return num;
	}

	private void CalculateNavmeshDistances()
	{
		UncertaintyTag.ForceOff();
		int length = coverPoints.GetLength(0);
		distanceTable = new float[length, length];
		baked.distanceTable = new byte[length * length];
		distanceTable_footprint = length * length;
		footprint += length * length;
		List<int>[] array = new List<int>[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = new List<int>();
		}
		for (int i = 0; i < length - 1; i++)
		{
			distanceTable[i, i] = 0f;
			ShowProgressBar("Calculating Nav Mesh Distances", (float)i / (float)length, false);
			if (coverPoints[i].type == CoverPointCore.Type.InAir)
			{
				continue;
			}
			for (int j = i + 1; j < coverPoints.GetLength(0); j++)
			{
				if (coverPoints[j].type == CoverPointCore.Type.InAir)
				{
					continue;
				}
				if (SameSubsection(i, j))
				{
					Vector2 vagueDir;
					float num = CalcNavMeshDistance(coverPoints[i].gamePos, coverPoints[j].gamePos, out vagueDir, walkableMask);
					distanceTable[i, j] = num;
					baked.distanceTable[i + length * j] = (byte)Mathf.Min(127f, num);
					if (coverPoints[j].type != 0 && num < 20f)
					{
						int packed = 0;
						footprint += 4;
						coverneighbour_footprint += 4;
						CoverNeighbour.coverIndex(ref packed, j);
						CoverNeighbour.navMeshDistance(ref packed, num);
						CoverNeighbour.vagueDirection(ref packed, vagueDir);
						array[i].Add(packed);
					}
					num = CalcNavMeshDistance(coverPoints[j].gamePos, coverPoints[i].gamePos, out vagueDir, walkableMask);
					distanceTable[j, i] = num;
					baked.distanceTable[j + length * i] = (byte)Mathf.Min(127f, num);
					if (coverPoints[i].type != 0 && num < 20f)
					{
						int packed = 0;
						footprint += 4;
						coverneighbour_footprint += 4;
						CoverNeighbour.coverIndex(ref packed, i);
						CoverNeighbour.navMeshDistance(ref packed, num);
						CoverNeighbour.vagueDirection(ref packed, vagueDir);
						array[j].Add(packed);
					}
				}
				else
				{
					distanceTable[i, j] = float.MaxValue;
					Savings++;
				}
			}
		}
		for (int i = 0; i < length; i++)
		{
			footprint += 4;
			coverneighbour_footprint += 4;
			int packed = 0;
			CoverNeighbour.coverIndex(ref packed, i);
			CoverNeighbour.navMeshDistance(ref packed, 0f);
			CoverNeighbour.vagueDirection(ref packed, Vector2.zero);
			array[i].Add(packed);
			array[i].Sort(SortNeighbouringCover);
			coverPoints[i].neighbours = array[i].ToArray();
			coverPoints[i].doorMasks = null;
			for (int k = 1; k < coverPoints[i].neighbours.Length; k++)
			{
				int j = CoverNeighbour.coverIndex(coverPoints[i].neighbours[k]);
				int num2 = FindOutWhichDoorsNeedToBeOpen(coverPoints[i].gamePos, coverPoints[j].gamePos);
				if (num2 != 0)
				{
					if (coverPoints[i].doorMasks == null)
					{
						coverPoints[i].doorMasks = new int[coverPoints[i].neighbours.Length];
						coverneighbour_footprint += 4 * coverPoints[i].neighbours.Length;
					}
					coverPoints[i].doorMasks[k] = num2;
				}
			}
		}
		UncertaintyTag.Restore();
		bakestage++;
	}

	private int FindOutWhichDoorsNeedToBeOpen(Vector3 fr, Vector3 to)
	{
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		int num = walkableMaskNoDoors;
		UnityEngine.AI.NavMesh.CalculatePath(fr, to, walkableMaskNoDoors, navMeshPath);
		if (navMeshPath.status != 0)
		{
			float num2 = CalculatePathLength(navMeshPath);
			for (int num3 = 31; num3 >= 0; num3--)
			{
				int num4 = num | (1 << num3);
				if (num4 != num)
				{
					UnityEngine.AI.NavMesh.CalculatePath(fr, to, num4, navMeshPath);
					if (navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
					{
						return num4;
					}
					float num5 = CalculatePathLength(navMeshPath);
					if (num5 > num2 + 0.1f)
					{
						num2 = num5;
						num = num4;
						num3 = 32;
					}
				}
			}
			return -1;
		}
		return 0;
	}

	private float CalculatePathLength(UnityEngine.AI.NavMeshPath path)
	{
		float num = 0f;
		for (int i = 0; i < path.corners.GetLength(0) - 1; i++)
		{
			num += (path.corners[i] - path.corners[i + 1]).magnitude;
		}
		return num;
	}
}
