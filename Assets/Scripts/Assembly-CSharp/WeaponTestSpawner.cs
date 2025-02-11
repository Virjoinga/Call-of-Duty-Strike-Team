using UnityEngine;

public class WeaponTestSpawner : MonoBehaviour
{
	public ActorDescriptor Spawn;

	public WeaponDescriptor PrimaryWeapon;

	public WeaponDescriptor SecondaryWeapon;

	private static int mCount;

	private void Update()
	{
		ProcessSpawn();
		base.enabled = false;
	}

	public void ProcessSpawn()
	{
		Transform transform = base.transform;
		ActorDescriptor actorDescriptor = Object.Instantiate(Spawn) as ActorDescriptor;
		actorDescriptor.Name = string.Format("{0} ({1})", PrimaryWeapon.name, SecondaryWeapon.name);
		actorDescriptor.DefaultPrimaryWeapon = PrimaryWeapon;
		actorDescriptor.DefaultSecondaryWeapon = SecondaryWeapon;
		actorDescriptor.name = string.Format("{0}{1}", mCount, actorDescriptor.name);
		mCount++;
		GameObject gameObject = SceneNanny.CreateActor(actorDescriptor, transform.position, transform.rotation);
		Actor component = gameObject.GetComponent<Actor>();
		if (component != null)
		{
			component.realCharacter.EnableNavMesh(true);
			component.health.Invulnerable = true;
		}
	}
}
