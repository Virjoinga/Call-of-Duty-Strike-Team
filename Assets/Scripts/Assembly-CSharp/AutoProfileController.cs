using System.Collections;
using UnityEngine;

public class AutoProfileController : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(KickOffProfiler());
	}

	public static void DeleteDebugCams()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("AutoProfileWP");
		if (gameObject != null)
		{
			Object.DestroyImmediate(gameObject);
		}
	}

	private IEnumerator KickOffProfiler()
	{
		GameSettings.DisableLoadoutAndBriefing = true;
		while (CommonHudController.Instance == null && HudStateController.Instance == null)
		{
			yield return null;
		}
		while (HudStateController.Instance.State != HudStateController.HudState.TPP && HudStateController.Instance.State != HudStateController.HudState.FPPLockedMountedWeapon && HudStateController.Instance.State != HudStateController.HudState.FPP)
		{
			yield return null;
		}
		GameObject WayPointsMan = GameObject.FindGameObjectWithTag("AutoProfileWP");
		float profileTime = 60f;
		Camera[] cameras = null;
		if (WayPointsMan != null)
		{
			OverwatchTrigger ot = WayPointsMan.GetComponent<OverwatchTrigger>();
			if (ot != null)
			{
				ot.Activate();
				GameObject cam2 = GameObject.Find("StrategyCamera");
				if (cam2 != null)
				{
					OverWatchShaderInterface inst = cam2.GetComponent<OverWatchShaderInterface>();
					if (inst != null)
					{
						Object.Destroy(inst);
					}
				}
				profileTime = ot.m_Interface.DurationSeconds;
			}
			else
			{
				cameras = WayPointsMan.GetComponentsInChildren<Camera>();
			}
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
		}
		if (!(SectionManager.GetSectionManager() != null) || !(ActStructure.Instance != null))
		{
			yield break;
		}
		Actor[] actors = Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		Actor[] array = actors;
		foreach (Actor actor in array)
		{
			if (actor.behaviour.PlayerControlled)
			{
				actor.health.Invulnerable = true;
			}
		}
		TBFUtils.StartAutoProfile(SectionManager.GetSectionManager().m_MissonName + "_S" + (1 + SectionManager.GetSectionIndex()));
		if (cameras != null && cameras.Length > 0)
		{
			CameraManager.Instance.CurrentCamera.enabled = false;
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
			Camera[] array2 = cameras;
			foreach (Camera cam in array2)
			{
				cam.enabled = true;
				cam.useOcclusionCulling = true;
				for (int ii = 0; ii < 6; ii++)
				{
					string profTag = string.Format("C{0} {1:D2}", cam.gameObject.name, ii * 60);
					TBFUtils.SetAutoProfileTag(profTag);
					cam.gameObject.transform.Rotate(0f, 60f, 0f);
					yield return new WaitForSeconds(1.5f);
				}
				cam.enabled = false;
			}
			CameraManager.Instance.CurrentCamera.enabled = true;
		}
		else
		{
			yield return new WaitForSeconds(profileTime);
		}
		TBFUtils.StopAutoProfile();
		TBFUtils.PostResults();
		if (SectionManager.GetSectionIndex() != SectionManager.GetSectionManager().m_Sections.Count - 1)
		{
			SectionManager.GetSectionManager().LoadNextSection(-1);
			yield break;
		}
		ActStructure.Instance.MissionQuit();
		Application.LoadLevel("GlobeSelectCore");
	}
}
