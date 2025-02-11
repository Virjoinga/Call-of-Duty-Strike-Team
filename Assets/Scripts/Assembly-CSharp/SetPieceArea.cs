using System.Collections.Generic;
using UnityEngine;

public class SetPieceArea : MonoBehaviour
{
	private List<Actor> m_ContainingActors = new List<Actor>();

	private void Start()
	{
	}

	public void DestroyActorsInSetPiece()
	{
		foreach (Actor containingActor in m_ContainingActors)
		{
			if (containingActor != null)
			{
				Object.Destroy(containingActor.gameObject);
			}
		}
		m_ContainingActors.Clear();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (base.enabled)
		{
			Actor component = other.gameObject.GetComponent<Actor>();
			if (!(component == null) && (component.ident & GKM.PlayerControlledMask) == 0)
			{
				m_ContainingActors.Add(component);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null))
		{
			m_ContainingActors.Remove(component);
		}
	}
}
