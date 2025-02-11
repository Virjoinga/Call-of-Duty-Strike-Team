using System;
using UnityEngine;

[Serializable]
public class RainBox : MonoBehaviour
{
	private MeshFilter mf;

	private Vector3 defaultPosition;

	private Bounds bounds;

	private RainManager manager;

	private Transform cachedTransform;

	private float cachedMinY;

	private float cachedAreaHeight;

	private float cachedFallingSpeed;

	public virtual void Start()
	{
		manager = transform.parent.GetComponent<RainManager>();
		bounds = new Bounds(new Vector3(transform.position.x, manager.minYPosition, transform.position.z), new Vector3(manager.areaSize * 1.35f, Mathf.Max(manager.areaSize, manager.areaHeight) * 1.35f, manager.areaSize * 1.35f));
		mf = GetComponent<MeshFilter>();
		mf.sharedMesh = manager.GetPreGennedMesh();
		cachedTransform = transform;
		cachedMinY = manager.minYPosition;
		cachedAreaHeight = manager.areaHeight;
		cachedFallingSpeed = manager.fallingSpeed;
		enabled = false;
	}

	public virtual void OnBecameVisible()
	{
		enabled = true;
	}

	public virtual void OnBecameInvisible()
	{
		enabled = false;
	}

	public virtual void Update()
	{
		cachedTransform.position -= Vector3.up * Time.deltaTime * cachedFallingSpeed;
		if (!(cachedTransform.position.y + cachedAreaHeight >= cachedMinY))
		{
			cachedTransform.position += Vector3.up * cachedAreaHeight * 2f;
		}
	}

	public virtual void OnDrawGizmos()
	{
		if ((bool)transform.parent)
		{
			Gizmos.color = new Color(0.2f, 0.3f, 3f, 0.35f);
			RainManager rainManager = ((RainManager)transform.parent.GetComponent(typeof(RainManager))) as RainManager;
			if ((bool)rainManager)
			{
				Gizmos.DrawWireCube(transform.position + transform.up * rainManager.areaHeight * 0.5f, new Vector3(rainManager.areaSize, rainManager.areaHeight, rainManager.areaSize));
			}
		}
	}
}
