using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class ContainerLinks
{
	public List<Link> m_Links = new List<Link>();

	public static GameObject GetObjectFromGuid(string guid)
	{
		if (guid != string.Empty)
		{
			UnityEngine.Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(Container));
			for (int i = 0; i < array.Length; i++)
			{
				Container container = (Container)array[i];
				if (container.m_Guid == guid)
				{
					return container.gameObject;
				}
			}
			UnityEngine.Object[] array2 = IncludeDisabled.FindSceneObjectsOfType(typeof(BagObject));
			for (int j = 0; j < array2.Length; j++)
			{
				BagObject bagObject = (BagObject)array2[j];
				if (bagObject.m_MissionGuid == guid)
				{
					return bagObject.gameObject;
				}
			}
		}
		return null;
	}

	public static Type GetType(string TypeName)
	{
		if (TypeName == string.Empty)
		{
			return null;
		}
		Type type = Type.GetType(TypeName);
		if (type != null)
		{
			return type;
		}
		if (TypeName.Contains("."))
		{
			string assemblyString = TypeName.Substring(0, TypeName.IndexOf('.'));
			Assembly assembly = Assembly.Load(assemblyString);
			if (assembly == null)
			{
				return null;
			}
			type = assembly.GetType(TypeName);
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}

	public static object GetLink(GameObject gameObj, string exposedLinkName)
	{
		ExposedLink exposedLink = null;
		Container component = gameObj.GetComponent<Container>();
		if (component != null)
		{
			exposedLink = component.GetExposedLink(exposedLinkName);
		}
		BagObject component2 = gameObj.GetComponent<BagObject>();
		if (component2 != null)
		{
			exposedLink = component2.GetExposedLink(exposedLinkName);
		}
		if (exposedLink == null && exposedLinkName == string.Empty && gameObj != null)
		{
			return gameObj;
		}
		if (exposedLink != null)
		{
			LinkReference linkReference = exposedLink.m_LinkReference;
			GameObject gameObject = null;
			if (component != null)
			{
				gameObject = FindChildByName(component.gameObject, linkReference.m_ObjectName);
			}
			else if (component2 != null)
			{
				gameObject = ((!(exposedLinkName != string.Empty)) ? component2.gameObject : FindChildByName(component2.gameObject, linkReference.m_ObjectName));
			}
			if (gameObject != null)
			{
				if (linkReference.m_ComponentName == string.Empty || linkReference.m_ComponentName == "None")
				{
					return gameObject;
				}
				Component componentFromTypeName = GetComponentFromTypeName(gameObject, linkReference.m_ComponentName);
				if (componentFromTypeName != null)
				{
					if (linkReference.m_MemberName == string.Empty || linkReference.m_MemberName == "None")
					{
						return componentFromTypeName;
					}
				}
				else
				{
					Debug.Log("Couldn't find source component: " + linkReference.m_ComponentName + " in container: " + component.name + " on object: " + gameObject.name);
				}
				Type type = componentFromTypeName.GetType();
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.Name == linkReference.m_MemberName)
					{
						return fieldInfo.GetValue(componentFromTypeName);
					}
				}
				Debug.Log("Couldn't find source member: " + linkReference.m_MemberName + " in container: " + component.name);
			}
			else if (component != null && linkReference != null)
			{
				Debug.Log("Couldn't find source object: " + linkReference.m_ComponentName + " in container: " + component.name);
			}
		}
		return null;
	}

	public static Component GetComponentFromTypeName(GameObject obj, string name)
	{
		Component[] components = obj.GetComponents(typeof(Component));
		foreach (Component component in components)
		{
			if (component.GetType().Name == name)
			{
				return component;
			}
		}
		return null;
	}

	public static GameObject FindChildByName(GameObject parent, string name)
	{
		if (parent.name == name)
		{
			return parent;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject.name == name)
			{
				return gameObject;
			}
			gameObject = FindChildByName(gameObject, name);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject FindChildContaining(GameObject parent, string str)
	{
		if (parent.name.Contains(str))
		{
			return parent;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject.name.Contains(str))
			{
				return gameObject;
			}
			gameObject = FindChildContaining(gameObject, str);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject[] FindChildrenContaining(GameObject parent, string str)
	{
		List<GameObject> list = new List<GameObject>();
		if (parent.name.Contains(str))
		{
			list.Add(parent);
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject.name.Contains(str))
			{
				list.Add(gameObject);
			}
			GameObject[] array = FindChildrenContaining(gameObject, str);
			if (array.Length > 0)
			{
				GameObject[] array2 = array;
				foreach (GameObject item in array2)
				{
					list.Add(item);
				}
			}
		}
		return list.ToArray();
	}

	public void ApplyLinks(GameObject gameObj)
	{
		if (gameObj == null)
		{
			return;
		}
		foreach (Link link in m_Links)
		{
			if (link.m_SourceContainer != null && link.m_Source == null)
			{
				link.m_Source = link.m_SourceContainer.gameObject;
			}
			if (link.m_Source == null)
			{
				if (link.m_SourceGuid != string.Empty)
				{
					GameObject objectFromGuid = GetObjectFromGuid(link.m_SourceGuid);
					if (objectFromGuid != null)
					{
						link.m_Source = objectFromGuid;
					}
				}
				if (link.m_Source == null)
				{
					Debug.Log("On container " + gameObj.name + " Couldn't find linked container with GUID" + link.m_SourceGuid + " to apply the links");
					continue;
				}
			}
			object replaceObj = GetLink(link.m_Source, link.m_SourceName);
			ReplaceMember(gameObj, link.m_Destination, ref replaceObj);
		}
	}

	private void ReplaceMember<T>(GameObject srcObj, LinkReference linkRef, ref T replaceObj)
	{
		if (linkRef.m_ObjectName == string.Empty)
		{
			return;
		}
		GameObject gameObject = FindChildByName(srcObj, linkRef.m_ObjectName);
		if (gameObject != null)
		{
			if (linkRef.m_ComponentName == string.Empty || linkRef.m_ComponentName == "None")
			{
				GameObject gameObject2 = replaceObj as GameObject;
				if (gameObject2 != null)
				{
					Vector3 position = gameObject.transform.position;
					Quaternion rotation = gameObject.transform.rotation;
					GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, position, rotation) as GameObject;
					gameObject3.transform.parent = gameObject.transform.parent;
					UnityEngine.Object.DestroyImmediate(gameObject);
					return;
				}
			}
			Type type = GetType(linkRef.m_ComponentName);
			Component component = gameObject.GetComponent(type);
			if (component != null)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.Name == linkRef.m_MemberName)
					{
						fieldInfo.SetValue(component, replaceObj);
						break;
					}
				}
			}
			else
			{
				Debug.Log("Couldn't find component in object to replace: " + linkRef.m_ComponentName);
			}
		}
		else
		{
			Debug.Log("Couldn't find object named " + linkRef.m_ObjectName + " in object to replace: " + linkRef.m_MemberName);
		}
	}
}
