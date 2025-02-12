using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingWithInterior : MonoBehaviour
{
	private class PendingExit
	{
		public Actor actor;

		public float frameCount;

		public PendingExit(Actor a, int f)
		{
			actor = a;
			frameCount = f;
		}
	}

	private enum BuildingState
	{
		None = 0,
		Inside = 1,
		TransitionToOutside = 2,
		Outside = 3,
		TransitionToInside = 4,
		WindowOpening = 5,
		WindowOpen = 6,
		WindowClosing = 7
	}

	private List<PendingExit> pendingExits = new List<PendingExit>();

	public BuildingWithInteriorData m_Interface;

	private static List<BuildingWithInterior> mBuildingList = new List<BuildingWithInterior>();

	public bool KeepCurrentCeilingState;

	private int mMenInsideCount;

	private BuildingState mState;

	private float mStateTime;

	private float mTimeToOpenWindow = 0.5f;

	private float mTimeToCloseWindow = 0.5f;

	private float mWindowPeekAmount = 4f;

	private BuildingWindow mOpeningWindow;

	private bool mFirstEntry;

	private Material[] mCheapExtMaterials;

	private Material[] mComplextExtMaterials;

	private bool mUsingCheapExteriorShaders;

	private Renderer mExternalRenderer;

	private bool mEnemyBlipsOn;

	private List<Actor> mEnemiesInside;

	public void AddInternalObject(GameObject ob)
	{
	}

	private GameObject FindChildNamesEndsWith(string endsWith)
	{
		int childCount = base.gameObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
			if (BagObject.CheckEndTag(gameObject.name, endsWith))
			{
				return gameObject;
			}
		}
		return null;
	}

	private void Awake()
	{
		mEnemyBlipsOn = false;
		mEnemiesInside = new List<Actor>();
		List<CMWindow> list = new List<CMWindow>();
		CMWindow[] windows = m_Interface.Windows;
		foreach (CMWindow cMWindow in windows)
		{
			if (cMWindow.gameObject.activeInHierarchy)
			{
				list.Add(cMWindow);
			}
			else
			{
				Object.DestroyImmediate(cMWindow.gameObject);
			}
		}
		m_Interface.Windows = list.ToArray();
	}

	private void OnDestroy()
	{
		mCheapExtMaterials = null;
		mComplextExtMaterials = null;
	}

	public static void ClearList()
	{
		if (mBuildingList != null)
		{
			mBuildingList.Clear();
		}
	}

	public static void ForceShowHideRoofs(bool OnOff)
	{
		foreach (BuildingWithInterior mBuilding in mBuildingList)
		{
			if (mBuilding != null)
			{
				mBuilding.ForceShowHideRoof(OnOff);
			}
		}
	}

	public void ForceShowHideRoof(bool onOff)
	{
		if (m_Interface.Exterior != null)
		{
			m_Interface.Exterior.SetActive(onOff);
		}
		if (onOff)
		{
			SetState(BuildingState.Outside);
		}
		else
		{
			SetState(BuildingState.Inside);
		}
		KeepCurrentCeilingState = true;
	}

	public void Activate()
	{
		TransisionToInside();
	}

	public void Deactivate()
	{
		TransitionToOutside();
	}

	private void Start()
	{
		mBuildingList.Add(this);
		if (m_Interface.Exterior == null)
		{
			m_Interface.Exterior = FindChildNamesEndsWith("_ext");
		}
		if (m_Interface.Interior == null)
		{
			m_Interface.Interior = FindChildNamesEndsWith("_int");
		}
		if (m_Interface.Exterior != null)
		{
			mExternalRenderer = m_Interface.Exterior.GetComponent<Renderer>();
			if (mExternalRenderer == null)
			{
				Transform transform = m_Interface.Exterior.transform.Find(m_Interface.Exterior.name + "_geom_LOD0");
				if (transform != null)
				{
					mExternalRenderer = transform.gameObject.GetComponent<Renderer>();
				}
				else
				{
					Transform transform2 = m_Interface.Exterior.transform.Find(m_Interface.Exterior.name + "_geom");
					if (transform2 != null)
					{
						mExternalRenderer = transform2.gameObject.GetComponent<Renderer>();
					}
					else
					{
						transform2 = m_Interface.Exterior.transform.Find(m_Interface.Exterior.name);
						if (transform2 != null)
						{
							mExternalRenderer = transform2.gameObject.GetComponent<Renderer>();
						}
					}
				}
			}
		}
		MeshCollider[] componentsInChildren = m_Interface.Interior.GetComponentsInChildren<MeshCollider>();
		MeshCollider[] array = componentsInChildren;
		foreach (MeshCollider meshCollider in array)
		{
			if (meshCollider.isTrigger)
			{
				BuildingInteriorVolume buildingInteriorVolume = meshCollider.gameObject.AddComponent<BuildingInteriorVolume>();
				buildingInteriorVolume.ParentBuilding = this;
				break;
			}
		}
		mMenInsideCount = 0;
		if (m_Interface.TransitionFromPoint == null)
		{
			m_Interface.TransitionFromPoint = m_Interface.Exterior.transform;
		}
		mStateTime = 0f;
		mFirstEntry = true;
		BuildShaders();
		SetState(BuildingState.Outside);
		StartCoroutine(SwitchOffCameraBlips());
	}

	private IEnumerator SwitchOffCameraBlips()
	{
		if (m_Interface.SecurityCameras == null || m_Interface.SecurityCameras.Count <= 0)
		{
			yield break;
		}
		ActorWrapper[] cameras = m_Interface.SecurityCameras.ToArray();
		int camerasLeft = cameras.Length;
		while (camerasLeft > 0)
		{
			camerasLeft = cameras.Length;
			for (int i = 0; i < cameras.Length; i++)
			{
				if (cameras[i] != null)
				{
					Actor ac = cameras[i].GetActor();
					if (ac != null && ac.realCharacter != null)
					{
						ac.realCharacter.ChangeHUDStatus(false);
						cameras[i] = null;
					}
				}
				else
				{
					camerasLeft--;
				}
			}
			yield return null;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		for (int i = 0; i < pendingExits.Count; i++)
		{
			if (pendingExits[i].actor == component)
			{
				pendingExits.RemoveAt(i);
				return;
			}
		}
		if (!(component != null))
		{
			return;
		}
		component.realCharacter.Location = this;
		if (component.behaviour.PlayerControlled)
		{
			mMenInsideCount++;
			if (mMenInsideCount > 0)
			{
				m_Interface.TransitionFromPoint = other.transform;
			}
		}
		else if (component.realCharacter != null)
		{
			mEnemiesInside.Add(component);
			SetHudStateForActor(component.realCharacter, true);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			pendingExits.Add(new PendingExit(component, Time.frameCount + 5));
		}
	}

	public void ProcessExit(Actor actor)
	{
		actor.realCharacter.Location = null;
		if (actor.behaviour.PlayerControlled)
		{
			mMenInsideCount--;
			if (mMenInsideCount > 0)
			{
			}
		}
		else if (mEnemiesInside.Contains(actor))
		{
			mEnemiesInside.Remove(actor);
			if (actor.realCharacter != null)
			{
				SetHudStateForActor(actor.realCharacter, false);
			}
		}
	}

	private void SetHudStateForActor(RealCharacter realBoy, bool inside)
	{
		if (mState != BuildingState.Inside && mState != BuildingState.WindowOpen)
		{
			realBoy.ChangeHUDStatus(!inside);
		}
		else
		{
			realBoy.ChangeHUDStatus(inside);
		}
	}

	private void UpdateBlipsForEnemiesInside()
	{
		if (m_Interface.SecurityCameras != null)
		{
			foreach (ActorWrapper securityCamera in m_Interface.SecurityCameras)
			{
				Actor actor = securityCamera.GetActor();
				if (actor != null && actor.realCharacter != null)
				{
					actor.realCharacter.ChangeHUDStatus(mEnemyBlipsOn);
				}
			}
		}
		for (int num = mEnemiesInside.Count - 1; num >= 0; num--)
		{
			bool flag = false;
			if (mEnemiesInside[num] != null)
			{
				if (mEnemiesInside[num].realCharacter != null)
				{
					SetHudStateForActor(mEnemiesInside[num].realCharacter, true);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				mEnemiesInside.RemoveAt(num);
			}
		}
	}

	public void OpenWindow(BuildingWindow window)
	{
		if (mState == BuildingState.Outside && mOpeningWindow == null)
		{
			mOpeningWindow = window;
			SetState(BuildingState.WindowOpening);
		}
	}

	public void CloseWindow(BuildingWindow window)
	{
		if (mOpeningWindow == window)
		{
			SetState(BuildingState.WindowClosing);
		}
	}

	private void DisableCollisionOnExterior()
	{
		MeshCollider[] componentsInChildren = m_Interface.Exterior.GetComponentsInChildren<MeshCollider>();
		MeshCollider[] array = componentsInChildren;
		foreach (MeshCollider meshCollider in array)
		{
			meshCollider.enabled = false;
		}
	}

	private void EnableWindowContextMenus(bool enable)
	{
		if (m_Interface.Windows != null)
		{
			CMWindow[] windows = m_Interface.Windows;
			foreach (CMWindow cMWindow in windows)
			{
				cMWindow.enabled = enable;
			}
		}
	}

	private void TransisionToInside()
	{
		if (KeepCurrentCeilingState)
		{
			return;
		}
		if (mFirstEntry)
		{
			mFirstEntry = false;
		}
		if (mState == BuildingState.Outside)
		{
			mStateTime = 0f;
			SetState(BuildingState.TransitionToInside);
			DisableCollisionOnExterior();
		}
		else if (mState == BuildingState.TransitionToOutside)
		{
			SetState(BuildingState.TransitionToInside);
			DisableCollisionOnExterior();
		}
		else if (mState == BuildingState.WindowOpening || mState == BuildingState.WindowOpen || mState == BuildingState.WindowClosing)
		{
			mOpeningWindow = null;
		}
		EnableWindowContextMenus(false);
		if (m_Interface.ObjectsToHide == null)
		{
			return;
		}
		GameObject[] objectsToHide = m_Interface.ObjectsToHide;
		foreach (GameObject gameObject in objectsToHide)
		{
			if (gameObject != null)
			{
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = false;
				}
				if (gameObject.name.Contains("light"))
				{
					gameObject.SetActive(false);
				}
			}
		}
	}

	private void TransitionToOutside()
	{
		if (KeepCurrentCeilingState)
		{
			return;
		}
		m_Interface.Exterior.SetActive(true);
		if (mState == BuildingState.Inside)
		{
			mStateTime = 0f;
			SetState(BuildingState.TransitionToOutside);
		}
		else if (mState == BuildingState.TransitionToInside)
		{
			SetState(BuildingState.TransitionToOutside);
		}
		GameObject[] objectsToHide = m_Interface.ObjectsToHide;
		foreach (GameObject gameObject in objectsToHide)
		{
			if (gameObject != null)
			{
				Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = true;
				}
				if (gameObject.name.Contains("light"))
				{
					gameObject.SetActive(true);
				}
			}
		}
	}

	private void BuildShaders()
	{
		mUsingCheapExteriorShaders = true;
		mCheapExtMaterials = mExternalRenderer.materials;
		mComplextExtMaterials = new Material[mExternalRenderer.materials.Length];
		for (int i = 0; i < mExternalRenderer.materials.Length; i++)
		{
			Material material = new Material(EnterableBuildingManager.Instance.ComplextExteriorShader);
			material.mainTexture = mExternalRenderer.materials[i].mainTexture;
			material.SetTexture("_EdgeTex", EnterableBuildingManager.Instance.DefaultEdgeTexture);
			material.SetTexture("_BlendTex", EnterableBuildingManager.Instance.DefaultBlendTexture);
			mComplextExtMaterials[i] = material;
		}
	}

	private void SwitchExtShadersForComplex()
	{
		if (mUsingCheapExteriorShaders)
		{
			mUsingCheapExteriorShaders = false;
			mExternalRenderer.materials = mComplextExtMaterials;
		}
	}

	private void SwitchExtShadersForSimple()
	{
		if (!mUsingCheapExteriorShaders)
		{
			mUsingCheapExteriorShaders = true;
			mExternalRenderer.materials = mCheapExtMaterials;
		}
	}

	private void SetState(BuildingState newState)
	{
		BuildingState buildingState = mState;
		mState = newState;
		if (newState == BuildingState.Outside)
		{
			SwitchExtShadersForSimple();
		}
		else
		{
			SwitchExtShadersForComplex();
		}
		if (buildingState == mState)
		{
			return;
		}
		BuildingState buildingState2 = mState;
		if (buildingState2 == BuildingState.Inside || buildingState2 == BuildingState.WindowOpen)
		{
			if (!mEnemyBlipsOn)
			{
				mEnemyBlipsOn = true;
				UpdateBlipsForEnemiesInside();
			}
		}
		else if (mEnemyBlipsOn)
		{
			mEnemyBlipsOn = false;
			UpdateBlipsForEnemiesInside();
		}
	}

	private void SetExternalShaderFadeHeight(float val)
	{
		Material[] materials = mExternalRenderer.materials;
		foreach (Material material in materials)
		{
			material.SetFloat("_FadeHeight", val);
		}
	}

	private void SetExternalShaderFadeOrigin(Vector3 pos)
	{
		Material[] materials = mExternalRenderer.materials;
		foreach (Material material in materials)
		{
			material.SetVector("_FadeOrigin", pos);
		}
	}

	private void ProcessPendingExits()
	{
		int num = 0;
		while (num < pendingExits.Count)
		{
			if (pendingExits[num].frameCount > (float)Time.frameCount)
			{
				num++;
				continue;
			}
			Actor actor = pendingExits[num].actor;
			if (actor != null)
			{
				ProcessExit(actor);
			}
			pendingExits.RemoveAt(num);
		}
	}

	private void Update()
	{
		ProcessPendingExits();
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mMenInsideCount > 0)
		{
			if (mFirstPersonActor != null && mFirstPersonActor.realCharacter.Location != this)
			{
				if (mState == BuildingState.Inside || mState == BuildingState.TransitionToInside)
				{
					TransitionToOutside();
				}
			}
			else if (mState == BuildingState.Outside || mState == BuildingState.TransitionToOutside)
			{
				TransisionToInside();
			}
		}
		else if (mState == BuildingState.Inside || mState == BuildingState.TransitionToInside)
		{
			TransitionToOutside();
		}
		float num = 0f;
		switch (mState)
		{
		case BuildingState.Inside:
			break;
		case BuildingState.TransitionToOutside:
			mStateTime += Time.deltaTime;
			num = 1f - Mathf.Clamp01(mStateTime / m_Interface.TransisionTime);
			num *= m_Interface.MaxPeekAmount;
			SetExternalShaderFadeHeight(num);
			SetExternalShaderFadeOrigin(m_Interface.TransitionFromPoint.position);
			if (mStateTime >= m_Interface.TransisionTime)
			{
				mStateTime = 0f;
				SetState(BuildingState.Outside);
				EnableWindowContextMenus(true);
			}
			break;
		case BuildingState.Outside:
			break;
		case BuildingState.TransitionToInside:
			mStateTime += Time.deltaTime;
			num = Mathf.Clamp01(mStateTime / m_Interface.TransisionTime);
			num *= m_Interface.MaxPeekAmount;
			SetExternalShaderFadeHeight(num);
			SetExternalShaderFadeOrigin(m_Interface.TransitionFromPoint.position);
			if (mStateTime >= m_Interface.TransisionTime)
			{
				mStateTime = 0f;
				SetState(BuildingState.Inside);
				m_Interface.Exterior.SetActive(false);
			}
			break;
		case BuildingState.WindowOpening:
			mStateTime += Time.deltaTime;
			num = Mathf.Clamp01(mStateTime / mTimeToOpenWindow);
			num *= mWindowPeekAmount;
			SetExternalShaderFadeHeight(num);
			SetExternalShaderFadeOrigin(mOpeningWindow.transform.position);
			if (mStateTime >= m_Interface.TransisionTime)
			{
				mStateTime = 0f;
				SetState(BuildingState.WindowOpen);
				SetExternalShaderFadeHeight(mWindowPeekAmount);
			}
			break;
		case BuildingState.WindowOpen:
			SetExternalShaderFadeHeight(mWindowPeekAmount);
			SetExternalShaderFadeOrigin(mOpeningWindow.transform.position);
			break;
		case BuildingState.WindowClosing:
			mStateTime += Time.deltaTime;
			num = 1f - Mathf.Clamp01(mStateTime / mTimeToCloseWindow);
			num *= mWindowPeekAmount;
			SetExternalShaderFadeHeight(num);
			SetExternalShaderFadeOrigin(mOpeningWindow.transform.position);
			if (mStateTime >= m_Interface.TransisionTime)
			{
				mStateTime = 0f;
				SetState(BuildingState.Outside);
				SetExternalShaderFadeHeight(0f);
				mOpeningWindow = null;
			}
			break;
		}
	}

	public bool ShouldShowInteriorObjects()
	{
		return mState == BuildingState.Inside;
	}
}
