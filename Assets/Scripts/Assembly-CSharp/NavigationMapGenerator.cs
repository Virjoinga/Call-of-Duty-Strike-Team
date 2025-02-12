using System.Collections.Generic;
using UnityEngine;

public class NavigationMapGenerator : MonoBehaviour
{
	public Mesh DebugModelPrimitive;

	public Material DebugModelMaterial;

	public static string NAVMAP_CONTAINER_NAME = "NavMap";

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
	}

	public void ClearPathfindingData(GameObject colliderMesh)
	{
		Object[] array = Resources.FindObjectsOfTypeAll(typeof(WaypointGameObject));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			WaypointGameObject waypointGameObject = (WaypointGameObject)array2[i];
			if (waypointGameObject.transform.parent == colliderMesh.transform)
			{
				Object.DestroyImmediate(waypointGameObject.gameObject);
			}
		}
		Transform transform = colliderMesh.transform.Find(NAVMAP_CONTAINER_NAME);
		if (transform != null)
		{
			GameObject gameObject = transform.gameObject;
			if (gameObject != null)
			{
				Object.DestroyImmediate(gameObject);
			}
		}
	}

	public void GeneratePathfindingDataCover(GameObject colliderMesh)
	{
		if (colliderMesh.GetComponent<MeshCollider>() == null)
		{
			MonoBehaviour.print("Object '" + colliderMesh.name + "' has no MeshCollider. Unable to generate Waypoints.");
			return;
		}
		NodesFromObject nodesFromObject = colliderMesh.AddComponent<NodesFromObject>();
		nodesFromObject.WallOffset = 0.5f;
		nodesFromObject.CornerOffset = 0.3f;
		GameObject gameObject = new GameObject("CuttingPlane");
		nodesFromObject.CuttingPlane = gameObject.transform;
		Vector3 position = colliderMesh.transform.position;
		position.y += 0.2f;
		gameObject.transform.position = position;
		nodesFromObject.Generate();
		List<NodesFromObject.PosAndNormal> posAndNormalList = nodesFromObject.GetPosAndNormalList();
		if (posAndNormalList.Count == 0)
		{
			Debug.Log(string.Format("WARNING: GeneratePathfindingDataCover - No Nodes generated. Fault with Cutting Plane positioning {0}?", gameObject.transform.position));
		}
		List<WaypointGameObject> list = new List<WaypointGameObject>();
		int num = 0;
		for (int i = 0; i < posAndNormalList.Count; i++)
		{
			NodesFromObject.PosAndNormal posAndNormal = posAndNormalList[i];
			WaypointGameObject waypointGameObject = CreateWaypoint(posAndNormal.Pos, string.Format("CoverPoint_{0}_{1}", colliderMesh.transform.parent.transform.parent.name, num++), colliderMesh);
			waypointGameObject.Configuration |= WaypointGameObject.Flavour.Cover;
			waypointGameObject.Facing = new Vector2(posAndNormal.Norm.x, posAndNormal.Norm.z);
			Debug.Log(string.Format("Waypoint='{0}' Normal={1}", waypointGameObject.name, posAndNormal.Norm));
			list.Add(waypointGameObject);
		}
		Object.DestroyImmediate(nodesFromObject);
		Object.DestroyImmediate(gameObject);
	}

	public static WaypointGameObject CreateWaypoint(Vector3 worldPosition, string debugName, GameObject parent)
	{
		WaypointGameObject waypointGameObject = new GameObject().AddComponent<WaypointGameObject>();
		waypointGameObject.transform.parent = parent.transform;
		SetNavMapContainerParent(waypointGameObject, parent);
		waypointGameObject.Initialise(worldPosition, debugName);
		return waypointGameObject;
	}

	private static void SetNavMapContainerParent(WaypointGameObject wpObj, GameObject colliderMesh)
	{
		GameObject gameObject = null;
		Transform transform = colliderMesh.transform.Find(NAVMAP_CONTAINER_NAME);
		if (transform == null)
		{
			gameObject = new GameObject(NAVMAP_CONTAINER_NAME);
			gameObject.transform.parent = colliderMesh.transform;
		}
		else
		{
			gameObject = transform.gameObject;
		}
		wpObj.gameObject.transform.parent = gameObject.transform;
	}
}
