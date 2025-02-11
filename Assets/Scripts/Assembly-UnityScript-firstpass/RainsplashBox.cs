using System;
using UnityEngine;

[Serializable]
public class RainsplashBox : MonoBehaviour
{
	private MeshFilter mf;

	private Bounds bounds;

	private RainsplashManager manager;

	public virtual void Start()
	{
		transform.localRotation = Quaternion.identity;
		manager = transform.parent.GetComponent<RainsplashManager>();
		bounds = new Bounds(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(manager.areaSize, Mathf.Max(manager.areaSize, manager.areaHeight), manager.areaSize));
		mf = GetComponent<MeshFilter>();
		mf.sharedMesh = manager.GetPreGennedMesh();
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

	public virtual void OnDrawGizmos()
	{
		if ((bool)transform.parent)
		{
			manager = transform.parent.GetComponent<RainsplashManager>();
			Gizmos.color = new Color(0.5f, 0.5f, 0.65f, 0.5f);
			if ((bool)manager)
			{
				Gizmos.DrawWireCube(transform.position + transform.up * manager.areaHeight * 0.5f, new Vector3(manager.areaSize, manager.areaHeight, manager.areaSize));
			}
		}
	}
}
