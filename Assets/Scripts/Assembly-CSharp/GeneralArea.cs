using UnityEngine;

public class GeneralArea : MonoBehaviour
{
	private const int kProceduralLocationCount = 10;

	private int nextChild;

	public bool LoopLocations;

	public float Radius = 5f;

	private float rootRad;

	private int locationCount;

	private int validSpecifiedLocations;

	private Vector3[] locations;

	private void Start()
	{
		rootRad = Mathf.Sqrt(Radius);
		locations = new Vector3[base.transform.childCount + 10];
		locationCount = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (!(base.transform.GetChild(i).name == "General Area(Clone)"))
			{
				Vector3 position = base.transform.GetChild(i).position;
				UnityEngine.AI.NavMeshHit hit;
				if (UnityEngine.AI.NavMesh.SamplePosition(position, out hit, 1f, 1))
				{
					locations[locationCount++] = hit.position;
				}
			}
		}
		validSpecifiedLocations = locationCount;
		if (locationCount != 0 && LoopLocations)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 100; i++)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
			float num2 = Random.Range(0f, rootRad);
			num2 *= num2;
			num2 = Radius - num2;
			Vector3 sourcePosition = base.transform.position + quaternion * new Vector3(num2, 0f, 0f);
			UnityEngine.AI.NavMeshHit hit2;
			if (UnityEngine.AI.NavMesh.SamplePosition(sourcePosition, out hit2, 2f, 1))
			{
				locations[locationCount++] = hit2.position;
				if (locationCount >= locations.Length)
				{
					break;
				}
			}
			else
			{
				num++;
			}
		}
		if (num > 0)
		{
			Debug.LogWarning("GENERAL AREA " + base.name + " PLACED INCORRECTLY (some of the area is off-navmesh)");
		}
	}

	public Vector3 NextLocation()
	{
		Vector3 position = base.transform.position;
		position = locations[nextChild++];
		if (nextChild >= locationCount)
		{
			nextChild = ((!LoopLocations) ? validSpecifiedLocations : 0);
		}
		return position;
	}

	public static Vector3 GetLocation(GameObject go)
	{
		GeneralArea component = go.GetComponent<GeneralArea>();
		if (component == null)
		{
			return go.transform.position;
		}
		return component.NextLocation();
	}

	public static Vector3 GetLocation(Transform t)
	{
		GeneralArea component = t.gameObject.GetComponent<GeneralArea>();
		if (component == null)
		{
			return t.position;
		}
		return component.NextLocation();
	}
}
