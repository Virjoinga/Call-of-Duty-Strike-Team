using UnityEngine;

[ExecuteInEditMode]
public class DebugSkinReplacer : MonoBehaviour
{
	public GameObject m_Source;

	public GameObject m_TargetBones;

	public GameObject m_ReplaceObject;

	public bool m_Convert;

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_Convert)
		{
			return;
		}
		if (m_Source != null && m_ReplaceObject != null)
		{
			if (m_TargetBones != null)
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				SkinnedMeshRenderer skinnedMeshRenderer = m_Source.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				SkinnedMeshRenderer component = m_ReplaceObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer != null && component != null)
				{
					Debug.Log("Converting Skinned mesh");
					gameObject2 = CopySMRenderer(skinnedMeshRenderer, component, m_TargetBones);
					m_ReplaceObject = gameObject2;
				}
				if (gameObject != null)
				{
					Object.DestroyImmediate(gameObject);
				}
			}
			else
			{
				GameObject gameObject3 = Object.Instantiate(m_Source, m_ReplaceObject.transform.position, m_ReplaceObject.transform.rotation) as GameObject;
				gameObject3.transform.parent = m_ReplaceObject.transform.parent;
				Object.DestroyImmediate(m_ReplaceObject);
				m_ReplaceObject = gameObject3;
			}
		}
		m_Convert = false;
	}

	private GameObject CopySMRenderer(SkinnedMeshRenderer skinRenderer, SkinnedMeshRenderer replaceSkin, GameObject targetBones)
	{
		GameObject gameObject = new GameObject(replaceSkin.name + " (Replaced with: " + skinRenderer.name + ")");
		SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)gameObject.AddComponent(typeof(SkinnedMeshRenderer));
		skinnedMeshRenderer.sharedMesh = skinRenderer.sharedMesh;
		skinnedMeshRenderer.sharedMaterials = skinRenderer.sharedMaterials;
		Transform[] array = new Transform[skinRenderer.bones.Length];
		int num = -1;
		for (int i = 0; i < skinRenderer.bones.Length; i++)
		{
			Transform transform = null;
			bool flag = false;
			string text = skinRenderer.bones[i].name;
			char c = text[text.Length - 1];
			if (c >= '0' && c <= '9')
			{
				text = text.Substring(0, text.Length - 1);
				if (skinRenderer.bones[i].parent == skinRenderer.transform.parent)
				{
					flag = true;
				}
				else
				{
					if (num == -1)
					{
						Component[] componentsInChildren = targetBones.GetComponentsInChildren(typeof(Transform), true);
						foreach (Component component in componentsInChildren)
						{
							if (component.name.Length > 3)
							{
								string text2 = component.name.Substring(0, component.name.Length - 3);
								if (text2 == text)
								{
									num = int.Parse(component.name.Substring(component.name.Length - 3));
									break;
								}
							}
						}
					}
					else
					{
						num++;
					}
					text = ((num <= 0) ? skinRenderer.bones[i].name : (text + string.Format("{0:000}", num + 1)));
				}
			}
			Component[] componentsInChildren2 = targetBones.GetComponentsInChildren(typeof(Transform), true);
			foreach (Component component2 in componentsInChildren2)
			{
				string text3 = component2.name;
				if (flag)
				{
					text3 = text3.Substring(0, text3.Length - 1);
				}
				if (text3 == text)
				{
					transform = component2.transform;
					break;
				}
				if (text3.Length > 3)
				{
					string text4 = text3.Substring(0, text3.Length - 3);
					if (text4 == text)
					{
						transform = component2.transform;
						break;
					}
				}
			}
			if (transform == null)
			{
				Debug.Log("Bone not found: " + text);
			}
			array[i] = transform;
		}
		skinnedMeshRenderer.bones = array;
		gameObject.transform.parent = replaceSkin.transform.parent;
		Object.DestroyImmediate(replaceSkin.gameObject);
		return gameObject;
	}
}
