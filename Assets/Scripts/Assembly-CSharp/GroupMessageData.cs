using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupMessageData
{
	public List<GameObject> targetObjs;

	public List<string> Message;

	public List<string> StringParam;

	public List<GameObject> ObjectParam;

	public bool OneShot;
}
