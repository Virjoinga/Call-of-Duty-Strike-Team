using System.Collections.Generic;
using UnityEngine;

public class RagdollCannon : MonoBehaviour
{
	public List<ActorDescriptor> ActorDescriptors;

	private float mCountdown;

	private void Update()
	{
		mCountdown -= Time.deltaTime;
		if (ActorDescriptors.Count <= 0 || !(mCountdown < 0f))
		{
			return;
		}
		GameObject gameObject = ActorGenerator.Instance().GenerateCorpse(ActorDescriptors[Random.Range(0, ActorDescriptors.Count - 1)], base.transform.position, base.transform.rotation);
		BaseCharacter component = gameObject.GetComponent<BaseCharacter>();
		if (component != null)
		{
			component.Ragdoll.SwitchToRagdoll();
			HitLocation[] bones = component.Ragdoll.Bones;
			foreach (HitLocation hitLocation in bones)
			{
				hitLocation.GetComponent<Rigidbody>().AddExplosionForce(600f, base.transform.position - 10f * base.transform.up, 0f);
			}
		}
		mCountdown = 2.5f;
	}
}
