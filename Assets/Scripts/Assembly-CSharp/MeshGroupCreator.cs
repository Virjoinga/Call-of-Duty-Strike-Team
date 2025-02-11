using System.Collections.Generic;
using UnityEngine;

public class MeshGroupCreator : MonoBehaviour
{
	public Vector3 m_size = Vector3.one;

	public List<GameObject> m_groupedObjects = new List<GameObject>();

	public int m_selected = -1;

	public int m_staticFlags = 63;

	private void Awake()
	{
		if (Application.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan.Alpha(0.1f);
		Gizmos.DrawCube(base.transform.position, m_size);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position, m_size);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan.Alpha(0.25f);
		Gizmos.DrawCube(base.transform.position, m_size);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(base.transform.position, m_size);
	}
}
