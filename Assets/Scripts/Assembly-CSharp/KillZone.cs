using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
	protected List<Actor> m_ActorsWithin = new List<Actor>();

	public virtual void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null) && !Contains(component))
		{
			m_ActorsWithin.Add(component);
		}
	}

	public virtual void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null) && Contains(component))
		{
			m_ActorsWithin.Remove(component);
		}
	}

	public bool Contains(Actor actor)
	{
		if (m_ActorsWithin == null)
		{
			return false;
		}
		return m_ActorsWithin.Contains(actor);
	}
}
