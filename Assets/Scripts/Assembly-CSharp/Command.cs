using System.Collections;
using UnityEngine;

public abstract class Command : MonoBehaviour
{
	public abstract bool Blocking();

	public abstract IEnumerator Execute();

	public virtual void ResolveGuidLinks()
	{
	}
}
