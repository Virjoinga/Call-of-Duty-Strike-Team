using System;
using UnityEngine;

[Serializable]
public class FakeGunmanData
{
	public ActorDescriptor Actor;

	public GameObject ObjectToMessageOnDeath;

	public string FunctionToCallOnDeath;

	public void CopyContainerData(FakeGunman f)
	{
		f.Actor = Actor;
	}
}
