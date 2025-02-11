using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BufferObject : ScriptableObject
{
	public List<GameObject> mObjectList = new List<GameObject>();

	public void AddGameObject(GameObject go)
	{
		mObjectList.Add(go);
	}

	public void ClearObjects()
	{
		mObjectList.Clear();
	}

	public GameObject[] GetGameObjects()
	{
		return mObjectList.ToArray();
	}

	public void SaveOutBufferObject(string name)
	{
	}
}
