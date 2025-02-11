using System.Collections.Generic;
using UnityEngine;

public class MoreInfoTrigger : MonoBehaviour
{
	public MoreInfoTriggerData m_Interface;

	public Collider TriggerVolume;

	public bool RequireSelectedSoldiers = true;

	public ScriptedSequence ScriptSeq;

	private List<Actor> mEntered;

	private void Start()
	{
		if (TriggerVolume == null)
		{
			TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		RequireSelectedSoldiers = m_Interface.RequireSelectedSoldiers;
		mEntered = new List<Actor>();
	}

	private void Update()
	{
	}

	public void TriggerSequence()
	{
		if (ScriptSeq != null)
		{
			ScriptSeq.RestartSequence();
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Actor component = other.GetComponent<Actor>();
		if (RequireSelectedSoldiers && component != null && component.behaviour.PlayerControlled && !mEntered.Contains(component))
		{
			mEntered.Add(component);
			Activate();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Actor component = other.GetComponent<Actor>();
		if (RequireSelectedSoldiers && component != null && component.behaviour.PlayerControlled && mEntered.Contains(component))
		{
			mEntered.Remove(component);
			Deactivate();
		}
	}

	public void Activate()
	{
		CommonHudController.Instance.SetMoreInfoButton(this);
	}

	public void Deactivate()
	{
		CommonHudController.Instance.SetMoreInfoButton(null);
	}

	public void OnDrawGizmos()
	{
		Vector3 center = base.transform.position + new Vector3(0f, 0.5f, 0f);
		Gizmos.DrawIcon(center, "Tutorial");
	}

	public void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.magenta.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
