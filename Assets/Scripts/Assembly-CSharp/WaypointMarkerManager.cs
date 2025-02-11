using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMarkerManager : MonoBehaviour
{
	public enum WaypointsVisibleEnum
	{
		None = 0,
		Previews = 1,
		InTransit = 2,
		PreviewsAndInTransit = 3
	}

	private static WaypointMarkerManager smInstance;

	private Dictionary<GameObject, GameObject> mMarkers;

	public GameObject WaypointMarkerObj;

	private int mDeletionLocked;

	public static WaypointMarkerManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple WaypointMarkerManager");
		}
		smInstance = this;
		mMarkers = new Dictionary<GameObject, GameObject>();
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	public WaypointMarker AddMarker(GameObject go, Vector3 pos, WaypointMarker.Type movementType, WaypointMarker.State state)
	{
		GameObject gameObject = null;
		if (mMarkers.ContainsKey(go))
		{
			gameObject = mMarkers[go];
			gameObject.transform.position = pos;
			SetMarkerState(gameObject, movementType, state);
		}
		else
		{
			gameObject = SceneNanny.Instantiate(WaypointMarkerObj) as GameObject;
			SetMarkerState(gameObject, movementType, state);
			gameObject.transform.position = pos;
			gameObject.GetComponent<WaypointMarker>().owner = go;
			mMarkers.Add(go, gameObject);
		}
		return gameObject.GetComponent<WaypointMarker>();
	}

	public WaypointMarker GetMarker(GameObject go)
	{
		if (mMarkers.ContainsKey(go))
		{
			return mMarkers[go].GetComponent<WaypointMarker>();
		}
		return null;
	}

	public void LockOutDeletion()
	{
		mDeletionLocked++;
	}

	public void UnlockDeletion()
	{
		mDeletionLocked--;
	}

	public void SetGameObjectMarkerState(GameObject go, WaypointMarker.Type movementType, WaypointMarker.State state)
	{
		if (mMarkers.ContainsKey(go))
		{
			SetMarkerState(mMarkers[go], movementType, state);
		}
	}

	public void SetGameObjectMarkerFacing(GameObject go, Vector3 facing)
	{
		facing.y = 0f;
		if (mMarkers.ContainsKey(go))
		{
			mMarkers[go].transform.forward = facing;
		}
	}

	public void HideMarker(GameObject go)
	{
		if (mMarkers.ContainsKey(go))
		{
			GameObject gameObject = mMarkers[go];
			WaypointMarker component = gameObject.GetComponent<WaypointMarker>();
			component.Hide();
		}
	}

	public void RemoveMarker(GameObject go)
	{
		if (mDeletionLocked <= 0 && mMarkers.ContainsKey(go))
		{
			GameObject obj = mMarkers[go];
			mMarkers.Remove(go);
			UnityEngine.Object.Destroy(obj);
		}
	}

	public void HideMarkers()
	{
		foreach (KeyValuePair<GameObject, GameObject> mMarker in mMarkers)
		{
			HideMarker(mMarker.Key);
		}
	}

	public void NowRunning(GameObject go)
	{
		if (mMarkers.ContainsKey(go))
		{
			WaypointMarker component = mMarkers[go].GetComponent<WaypointMarker>();
			component.NowRunning();
		}
	}

	public void NowWalking(GameObject go)
	{
		if (mMarkers.ContainsKey(go))
		{
			WaypointMarker component = mMarkers[go].GetComponent<WaypointMarker>();
			component.NowWalking();
		}
	}

	public void EnableRendering()
	{
		foreach (KeyValuePair<GameObject, GameObject> mMarker in mMarkers)
		{
			Renderer componentInChildren = mMarker.Value.renderer;
			if (componentInChildren == null)
			{
				componentInChildren = mMarker.Value.GetComponentInChildren<Renderer>();
			}
			componentInChildren.enabled = true;
			WaypointMarker componentInChildren2 = mMarker.Value.GetComponentInChildren<WaypointMarker>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.EnablePathRender(true);
			}
		}
	}

	public void DisableRendering()
	{
		foreach (KeyValuePair<GameObject, GameObject> mMarker in mMarkers)
		{
			Renderer componentInChildren = mMarker.Value.renderer;
			if (componentInChildren == null)
			{
				componentInChildren = mMarker.Value.GetComponentInChildren<Renderer>();
			}
			componentInChildren.enabled = false;
			WaypointMarker componentInChildren2 = mMarker.Value.GetComponentInChildren<WaypointMarker>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.EnablePathRender(false);
			}
		}
	}

	public WaypointMarker GetHighlightedPreviewMarker(bool includeUnderContextMenu)
	{
		foreach (KeyValuePair<GameObject, GameObject> mMarker in mMarkers)
		{
			WaypointMarker component = mMarker.Value.GetComponent<WaypointMarker>();
			if (component.state != 0)
			{
				return component;
			}
		}
		return null;
	}

	public void HighlightPreviewMarker(WaypointMarker wpm)
	{
	}

	private void SetMarkerState(GameObject marker, WaypointMarker.Type movementType, WaypointMarker.State state)
	{
		WaypointMarker component = marker.GetComponent<WaypointMarker>();
		component.SetTypeAndState(movementType, state);
	}

	private void SetMarkerState(GameObject marker, WaypointMarker.State state)
	{
		WaypointMarker component = marker.GetComponent<WaypointMarker>();
		component.SetState(state);
	}

	public void MutePreviews(WaypointMarker wpm)
	{
		foreach (KeyValuePair<GameObject, GameObject> mMarker in mMarkers)
		{
			mMarker.Value.GetComponent<WaypointMarker>().MutePreview();
		}
	}
}
