using System;
using UnityEngine;

public class SceneNanny : MonoBehaviour
{
	public enum TugType
	{
		kDestroy = 0,
		kDisable = 1,
		kEnable = 2
	}

	private enum SceneType
	{
		kMainGame = 0,
		kSkippableCutScene = 1,
		kCount = 2
	}

	public delegate void ApronString(TugType tug);

	private SceneType activeScene;

	private GameObject[] sceneRoots;

	private ApronString[] apronStrings;

	public static SceneNanny smInstance;

	public static float TimeOfLastSpawn;

	public void OnActivateSection()
	{
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Cannot have multiple SceneNannies");
		}
		smInstance = this;
		sceneRoots = new GameObject[2];
		sceneRoots[0] = new GameObject("GameScene");
		sceneRoots[1] = new GameObject("CutScene");
		for (int i = 0; i < 2; i++)
		{
			sceneRoots[i].transform.parent = base.transform;
		}
		activeScene = SceneType.kMainGame;
		apronStrings = new ApronString[2];
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void Update()
	{
	}

	private void Tug(SceneType s, TugType t)
	{
		if (apronStrings[(int)s] != null)
		{
			apronStrings[(int)s](t);
		}
	}

	public static void EndSkippableScene()
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		smInstance.activeScene = SceneType.kMainGame;
		foreach (Transform item in smInstance.sceneRoots[1].transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		smInstance.Tug(SceneType.kSkippableCutScene, TugType.kDestroy);
		smInstance.Tug(SceneType.kMainGame, TugType.kEnable);
		smInstance.sceneRoots[0].SetActive(true);
	}

	public static void BeginSkippableScene()
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		smInstance.activeScene = SceneType.kSkippableCutScene;
		smInstance.sceneRoots[0].SetActive(false);
		smInstance.Tug(SceneType.kMainGame, TugType.kDisable);
	}

	public static void EndMainGame()
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		smInstance.Tug(SceneType.kMainGame, TugType.kDestroy);
	}

	public new static UnityEngine.Object Instantiate(UnityEngine.Object original)
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		return smInstance.intInstantiate(original);
	}

	public static GameObject CreateModel(GameObject original, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = CreateModel(original);
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	public static GameObject CreateModel(GameObject original)
	{
		GameObject gameObject = Instantiate(original) as GameObject;
		SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			skinnedMeshRenderer.quality = ((!skinnedMeshRenderer.name.EndsWith("_LOD0", StringComparison.InvariantCultureIgnoreCase)) ? SkinQuality.Bone1 : SkinQuality.Bone2);
		}
		return gameObject;
	}

	private UnityEngine.Object intInstantiate(UnityEngine.Object original)
	{
		UnityEngine.Object @object = UnityEngine.Object.Instantiate(original);
		Component component = @object as Component;
		if (component != null)
		{
			component.transform.parent = sceneRoots[(int)activeScene].transform;
		}
		GameObject gameObject = @object as GameObject;
		if (gameObject != null)
		{
			gameObject.transform.parent = sceneRoots[(int)activeScene].transform;
		}
		return @object;
	}

	public static GameObject NewGameObject()
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		return smInstance.intNewGameObject();
	}

	private GameObject intNewGameObject()
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = sceneRoots[(int)activeScene].transform;
		return gameObject;
	}

	public static GameObject NewGameObject(string name)
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		return smInstance.intNewGameObject(name);
	}

	private GameObject intNewGameObject(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = sceneRoots[(int)activeScene].transform;
		return gameObject;
	}

	public static void NannyMe(ApronString a)
	{
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		ApronString reference = smInstance.apronStrings[(int)smInstance.activeScene];
		reference = (ApronString)Delegate.Combine(reference, a);
	}

	public static GameObject CreateExtra(ActorDescriptor descriptor, Vector3 position, Quaternion rotation)
	{
		return ActorGenerator.Instance().GenerateFake(descriptor, position, rotation);
	}

	public static GameObject CreateActor(ActorDescriptor descriptor, Vector3 position, Quaternion rotation)
	{
		return CreateActor(descriptor, position, rotation, UnityEngine.Random.Range(0, int.MaxValue));
	}

	public static GameObject CreateActor(ActorDescriptor descriptor, Vector3 position, Quaternion rotation, int seed)
	{
		TimeOfLastSpawn = Time.time;
		if (smInstance == null)
		{
			new GameObject("SceneNanny", typeof(SceneNanny));
		}
		switch (smInstance.activeScene)
		{
		case SceneType.kMainGame:
			return ActorGenerator.Instance().Generate(descriptor, position, rotation, seed);
		case SceneType.kSkippableCutScene:
			return ActorGenerator.Instance().GenerateFake(descriptor, position, rotation);
		default:
			return null;
		}
	}

	public static GameObject CopySkinMesh(SkinnedMeshRenderer sourceSkin, SkinnedMeshRenderer replaceSkin, GameObject targetBones)
	{
		GameObject result = replaceSkin.gameObject;
		replaceSkin.sharedMesh = sourceSkin.sharedMesh;
		replaceSkin.sharedMaterials = sourceSkin.sharedMaterials;
		Transform[] array = new Transform[sourceSkin.bones.Length];
		int num = -1;
		for (int i = 0; i < sourceSkin.bones.Length; i++)
		{
			Transform transform = null;
			bool flag = false;
			string text = sourceSkin.bones[i].name;
			char c = text[text.Length - 1];
			if (c >= '0' && c <= '9')
			{
				text = text.Substring(0, text.Length - 1);
				if (sourceSkin.bones[i].parent == sourceSkin.transform.parent)
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
							if (component.name.Length <= 3)
							{
								continue;
							}
							string text2 = component.name.Substring(0, component.name.Length - 3);
							if (text2 == text)
							{
								string s = component.name.Substring(component.name.Length - 3);
								int result2;
								if (int.TryParse(s, out result2))
								{
									num = result2;
								}
								break;
							}
						}
					}
					else
					{
						num++;
					}
					text = ((num <= 0) ? sourceSkin.bones[i].name : (text + string.Format("{0:000}", num + 1)));
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
				if (i > 0)
				{
					array[i] = array[i - 1];
				}
			}
			else
			{
				array[i] = transform;
			}
		}
		replaceSkin.bones = array;
		replaceSkin.localBounds = replaceSkin.localBounds;
		return result;
	}
}
