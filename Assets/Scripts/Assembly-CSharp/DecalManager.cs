using System;
using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
	public enum DecalType
	{
		BulletMetal = 0,
		BulletConcrete = 1,
		BulletPlaster = 2,
		BulletSand = 3,
		BulletSnow = 4,
		BulletGeneric = 5,
		GrenadeExplosion = 6
	}

	private static DecalManager smInstance;

	private float mWallOffset = 0.01f;

	private int MaxBulletHoles = 30;

	private int MaxExplosionMarks = 5;

	private LinkedList<Decal> BulletHoles;

	private LinkedList<Decal> ExplosionMarks;

	public GameObject[] BulletHoleMetal;

	public GameObject[] BulletHoleConcrete;

	public GameObject[] BulletHolePlaster;

	public GameObject[] BulletHoleSand;

	public GameObject[] BulletHoleSnow;

	public GameObject[] BulletHoleGeneric;

	public GameObject GrenadeExplosionMark;

	public static DecalManager Instance
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
			throw new Exception("Can not have multiple DecalManager");
		}
		smInstance = this;
		BulletHoles = new LinkedList<Decal>();
		ExplosionMarks = new LinkedList<Decal>();
		if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.LotsOfDecals))
		{
			MaxBulletHoles = 15;
			MaxExplosionMarks = 3;
		}
	}

	private void Start()
	{
		for (int i = 0; i < MaxBulletHoles; i++)
		{
			BulletHoles.AddFirst(CreateDecal(DecalType.BulletGeneric));
		}
		for (int j = 0; j < MaxExplosionMarks; j++)
		{
			ExplosionMarks.AddFirst(CreateDecal(DecalType.GrenadeExplosion));
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private GameObject PickRandomPrefabFromArray(GameObject[] array)
	{
		int num = UnityEngine.Random.Range(0, array.Length);
		return array[num];
	}

	public void AddToFloor(Vector3 pos, DecalType decal)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(pos, -Vector3.up, out hitInfo, 1f, 1))
		{
			Add(hitInfo.point, Vector3.up, decal);
		}
	}

	public void Add(Vector3 pos, Vector3 heading, DecalType decal)
	{
		Quaternion quaternion = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), heading);
		Quaternion rotation = default(Quaternion);
		rotation.SetLookRotation(heading, quaternion * (Vector3.up + Vector3.right));
		Decal decalOfType = GetDecalOfType(decal);
		if (decalOfType != null)
		{
			float fadeTime = 0.2f;
			float lifeTime = ((decal != DecalType.GrenadeExplosion) ? 15 : 6);
			decalOfType.transform.position = pos + heading * mWallOffset;
			decalOfType.transform.rotation = rotation;
			decalOfType.Setup(decal, lifeTime, fadeTime);
			MoveDecalToEnd(decalOfType);
		}
	}

	private GameObject GetPrefabFor(DecalType decal)
	{
		GameObject result = null;
		switch (decal)
		{
		case DecalType.BulletMetal:
			result = PickRandomPrefabFromArray(BulletHoleMetal);
			break;
		case DecalType.BulletConcrete:
			result = PickRandomPrefabFromArray(BulletHoleConcrete);
			break;
		case DecalType.BulletPlaster:
			result = PickRandomPrefabFromArray(BulletHolePlaster);
			break;
		case DecalType.BulletSand:
			result = PickRandomPrefabFromArray(BulletHoleSand);
			break;
		case DecalType.BulletSnow:
			result = PickRandomPrefabFromArray(BulletHoleSnow);
			break;
		case DecalType.BulletGeneric:
			result = PickRandomPrefabFromArray(BulletHoleGeneric);
			break;
		case DecalType.GrenadeExplosion:
			result = GrenadeExplosionMark;
			break;
		}
		return result;
	}

	private Decal CreateDecal(DecalType decal)
	{
		GameObject prefabFor = GetPrefabFor(decal);
		if (prefabFor != null)
		{
			Vector3 localScale = ((decal != DecalType.GrenadeExplosion) ? new Vector3(8f, 8f, 1f) : new Vector3(200f, 200f, 1f));
			GameObject gameObject = SceneNanny.Instantiate(prefabFor) as GameObject;
			gameObject.transform.localScale = localScale;
			Decal result = gameObject.AddComponent<Decal>();
			gameObject.layer = LayerMask.NameToLayer("TransparentFX");
			return result;
		}
		return null;
	}

	public void RemoveBulletHolesInRadius(Vector3 position, float radius)
	{
		foreach (Decal bulletHole in BulletHoles)
		{
			if ((position - bulletHole.gameObject.transform.position).magnitude < radius)
			{
				bulletHole.DeactivateNow();
			}
		}
	}

	public void Update()
	{
		if ((bool)GameController.Instance && GameController.Instance.IsPaused)
		{
			return;
		}
		foreach (Decal bulletHole in BulletHoles)
		{
			if (bulletHole.IsActive)
			{
				bulletHole.ManagerUpdate();
			}
		}
		foreach (Decal explosionMark in ExplosionMarks)
		{
			if (explosionMark.IsActive)
			{
				explosionMark.ManagerUpdate();
			}
		}
	}

	private void MoveDecalToEnd(Decal a)
	{
		if (a.mDecalType == DecalType.GrenadeExplosion)
		{
			ExplosionMarks.Remove(a);
			ExplosionMarks.AddLast(a);
		}
		else
		{
			BulletHoles.Remove(a);
			BulletHoles.AddLast(a);
		}
	}

	private Decal GetDecalOfType(DecalType type)
	{
		LinkedList<Decal> linkedList = ((type != DecalType.GrenadeExplosion) ? BulletHoles : ExplosionMarks);
		Decal value = linkedList.First.Value;
		LinkedListNode<Decal> linkedListNode = linkedList.First;
		if (linkedListNode != null)
		{
			do
			{
				if (!linkedListNode.Value.IsActive)
				{
					value = linkedListNode.Value;
					break;
				}
				linkedListNode = linkedListNode.Next;
			}
			while (linkedListNode != null);
		}
		value.SetMesh(GetPrefabFor(type).GetComponent<MeshFilter>().sharedMesh);
		return value;
	}
}
