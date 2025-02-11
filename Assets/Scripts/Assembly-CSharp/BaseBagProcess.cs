using UnityEngine;

public class BaseBagProcess : MonoBehaviour
{
	public static GameObject TransformReplace(GameObject srcObj, GameObject dstObj)
	{
		GameObject gameObject = Object.Instantiate(srcObj, dstObj.transform.position, dstObj.transform.rotation) as GameObject;
		if (gameObject != null)
		{
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			Transform[] componentsInChildren2 = dstObj.GetComponentsInChildren<Transform>(true);
			if (componentsInChildren != null && ((componentsInChildren2 != null) & (componentsInChildren.Length == componentsInChildren2.Length)))
			{
				int num = componentsInChildren.Length;
				for (int i = 0; i < num; i++)
				{
					Transform transform = componentsInChildren[i];
					Transform transform2 = componentsInChildren2[i];
					if (!(transform == null) && !(transform2 == null))
					{
						transform.name = transform2.name;
						transform.position = transform2.position;
						transform.rotation = transform2.rotation;
						if (!transform2.gameObject.activeInHierarchy)
						{
							transform.gameObject.SetActive(false);
						}
						transform.gameObject.isStatic = transform2.gameObject.isStatic;
					}
				}
				componentsInChildren[0].parent = componentsInChildren2[0].parent;
				componentsInChildren[0].position = componentsInChildren2[0].position;
			}
			Object.DestroyImmediate(componentsInChildren2[0].gameObject);
		}
		return gameObject;
	}

	public GameObject TransformMerge(GameObject srcObj, GameObject dstObj, bool CopyShadersOver)
	{
		GameObject gameObject = Container.GetContainerFromObject(dstObj) as GameObject;
		GameObject gameObject2 = Object.Instantiate(srcObj, dstObj.transform.position, dstObj.transform.rotation) as GameObject;
		if (gameObject2 != null)
		{
			Component[] componentsInChildren = gameObject2.GetComponentsInChildren(typeof(Transform), true);
			Component[] componentsInChildren2 = dstObj.GetComponentsInChildren(typeof(Transform), true);
			if (componentsInChildren != null && componentsInChildren2 != null && componentsInChildren.Length > 0 && componentsInChildren2.Length > 0)
			{
				Transform transform = componentsInChildren[0] as Transform;
				Transform transform2 = componentsInChildren2[0] as Transform;
				int num = 0;
				int num2 = 0;
				bool flag = false;
				transform2 = componentsInChildren2[num2] as Transform;
				Component[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					Transform transform3 = (Transform)array[i];
					if (NameCheck(transform3.name, transform2.name))
					{
						transform3.parent = transform2.parent;
						transform = transform3;
						gameObject2 = transform.gameObject;
						flag = true;
						break;
					}
					num++;
				}
				if (!flag)
				{
					Debug.Log("Couldn't merge replacement object - Source: " + srcObj.name + " Dest: " + dstObj.name + ". Just doing a straight swap instead");
					Object.DestroyImmediate(gameObject2);
					gameObject2 = Object.Instantiate(srcObj, dstObj.transform.position, dstObj.transform.rotation) as GameObject;
					gameObject2.transform.parent = dstObj.transform.parent;
					gameObject2.name = dstObj.name;
					Object.DestroyImmediate(dstObj);
					return gameObject2;
				}
				Component[] array2 = componentsInChildren2;
				for (int j = 0; j < array2.Length; j++)
				{
					Transform transform4 = (Transform)array2[j];
					flag = false;
					for (int k = num; k < componentsInChildren.Length; k++)
					{
						Transform transform5 = componentsInChildren[k] as Transform;
						if (!NameCheck(transform5.name, transform4.name))
						{
							continue;
						}
						transform5.name = transform4.name;
						if (!transform4.gameObject.activeInHierarchy)
						{
							transform5.gameObject.SetActive(false);
						}
						transform5.localPosition = transform4.localPosition;
						transform5.localRotation = transform4.localRotation;
						if (CopyShadersOver)
						{
							Renderer component = transform4.gameObject.GetComponent<Renderer>();
							Renderer component2 = transform5.gameObject.GetComponent<Renderer>();
							if (component2 != null && component != null && component2.sharedMaterials.Length == component.sharedMaterials.Length)
							{
								for (int l = 0; l < component.sharedMaterials.Length; l++)
								{
									component2.sharedMaterials[l].shader = component.sharedMaterials[l].shader;
								}
							}
						}
						ReplacedTransform(transform5, transform4);
						transform = transform5;
						flag = true;
						num++;
						break;
					}
					if (flag)
					{
						continue;
					}
					Transform transform6 = FindChildWithNameCheck(transform4.parent.name, componentsInChildren);
					if (transform6 != null)
					{
						GameObject gameObject3 = Container.GetContainerFromObject(transform6) as GameObject;
						if (gameObject3 != null && gameObject3 != gameObject)
						{
							Debug.Log("Disgarding container object:" + transform6.name + " in container " + gameObject3.name);
						}
						else
						{
							transform4.parent = transform6;
						}
						continue;
					}
					Component[] componentsInChildren3 = transform.GetComponentsInChildren(typeof(Transform), true);
					Transform transform7 = FindChildWithNameCheck(transform4.name, componentsInChildren3);
					if (transform7 != null)
					{
						transform4.parent = transform7;
					}
				}
				Object.DestroyImmediate(transform2.gameObject);
				Object.DestroyImmediate(componentsInChildren[0].gameObject);
			}
			if (gameObject2 != null && gameObject2.transform.parent == null)
			{
				Debug.Log("Couldn't merge object " + gameObject2.name + " correctly");
				Object.DestroyImmediate(gameObject2);
			}
		}
		return gameObject2;
	}

	private static Transform FindChildWithNameCheck(string name, Component[] transList)
	{
		for (int i = 0; i < transList.Length; i++)
		{
			Transform transform = (Transform)transList[i];
			if (NameCheck(name, transform.name))
			{
				return transform;
			}
		}
		return null;
	}

	private static bool NameCheck(string srcStr, string dstStr)
	{
		string text = TrimNumbersAtEnd(srcStr);
		string text2 = TrimNumbersAtEnd(dstStr);
		return text == text2;
	}

	public static string TrimNumbersAtEnd(string str)
	{
		int result = 0;
		string result2 = str;
		if (str.Length > 2 && str[str.Length - 2] == ' ')
		{
			str = str.Remove(str.Length - 2, 1);
		}
		for (int num = 3; num > 0; num--)
		{
			if (str.Length > num)
			{
				string s = str.Substring(str.Length - num);
				if (int.TryParse(s, out result))
				{
					return str.Substring(0, str.Length - num);
				}
			}
		}
		return result2;
	}

	public virtual void ApplyProcess(GameObject obj)
	{
	}

	public virtual void PreProcess(GameObject obj)
	{
	}

	public virtual void ReplacedTransform(Transform newTrans, Transform oldTrans)
	{
	}
}
