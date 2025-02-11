using UnityEngine;

public class DetonatorTest : MonoBehaviour
{
	public GameObject currentDetonator;

	private int _currentExpIdx = -1;

	public GameObject[] detonatorPrefabs;

	public float explosionLife = 10f;

	public float timeScale = 1f;

	public float detailLevel = 1f;

	public GameObject wall;

	private GameObject _currentWall;

	private float _spawnWallTime = -1000f;

	private Rect _guiRect;

	private Rect checkRect = new Rect(0f, 0f, 260f, 180f);

	private void Start()
	{
		SpawnWall();
		if (!currentDetonator)
		{
			NextExplosion();
		}
		else
		{
			_currentExpIdx = 0;
		}
	}

	private void OnGUI()
	{
		_guiRect = new Rect(7f, Screen.height - 180, 250f, 200f);
		GUILayout.BeginArea(_guiRect);
		GUILayout.BeginVertical();
		string text = currentDetonator.name;
		if (GUILayout.Button(text + " (Click For Next)"))
		{
			NextExplosion();
		}
		if (GUILayout.Button("Rebuild Wall"))
		{
			SpawnWall();
		}
		if (GUILayout.Button("Camera Far"))
		{
			Camera.main.transform.position = new Vector3(0f, 0f, -7f);
			Camera.main.transform.eulerAngles = new Vector3(13.5f, 0f, 0f);
		}
		if (GUILayout.Button("Camera Near"))
		{
			Camera.main.transform.position = new Vector3(0f, -8.664466f, 31.38269f);
			Camera.main.transform.eulerAngles = new Vector3(1.213462f, 0f, 0f);
		}
		GUILayout.Label("Time Scale");
		timeScale = GUILayout.HorizontalSlider(timeScale, 0f, 1f);
		GUILayout.Label("Detail Level (re-explode after change)");
		detailLevel = GUILayout.HorizontalSlider(detailLevel, 0f, 1f);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void NextExplosion()
	{
		if (_currentExpIdx >= detonatorPrefabs.Length - 1)
		{
			_currentExpIdx = 0;
		}
		else
		{
			_currentExpIdx++;
		}
		currentDetonator = detonatorPrefabs[_currentExpIdx];
	}

	private void SpawnWall()
	{
		if ((bool)_currentWall)
		{
			Object.Destroy(_currentWall);
		}
		_currentWall = Object.Instantiate(wall, new Vector3(-7f, -12f, 48f), Quaternion.identity) as GameObject;
		_spawnWallTime = Time.time;
	}

	private void Update()
	{
		_guiRect = new Rect(7f, Screen.height - 150, 250f, 200f);
		if ((double)(Time.time + _spawnWallTime) > 0.5)
		{
			if (!checkRect.Contains(Input.mousePosition) && Input.GetMouseButtonDown(0))
			{
				SpawnExplosion();
			}
			Time.timeScale = timeScale;
		}
	}

	private void SpawnExplosion()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		GameObject gameObject = null;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000f))
		{
			float num = currentDetonator.GetComponent<Detonator>().size / 3f;
			Vector3 position = hitInfo.point + Vector3.Scale(hitInfo.normal, new Vector3(num, num, num));
			gameObject = Object.Instantiate(currentDetonator, position, Quaternion.identity) as GameObject;
			gameObject.GetComponent<Detonator>().detail = detailLevel;
		}
		if (gameObject != null)
		{
			Object.Destroy(gameObject, explosionLife);
		}
	}
}
