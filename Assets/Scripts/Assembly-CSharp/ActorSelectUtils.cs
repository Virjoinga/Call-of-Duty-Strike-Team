using UnityEngine;

public class ActorSelectUtils
{
	public static PickerColliderSettings[] NormalColliderSettings = new PickerColliderSettings[4]
	{
		new PickerColliderSettings(1.3f, 0.66f, 2.5f),
		new PickerColliderSettings(1.3f, 1.3f, 4f),
		new PickerColliderSettings(1.3f, 1.3f, 4f),
		new PickerColliderSettings(1.3f, 0.66f, 2.5f)
	};

	public static PickerColliderSettings DeadColliderSettings = new PickerColliderSettings(1f, 0.66f, 2f);

	public static bool Debugging = false;

	public static void CreateActorSelectCollider(Actor actor, PickerColliderSettings settings)
	{
		GameObject gameObject = new GameObject("Picker");
		CapsuleCollider picker = gameObject.AddComponent<CapsuleCollider>();
		gameObject.transform.parent = actor.transform;
		gameObject.transform.localPosition = Vector3.zero;
		actor.Picker = picker;
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		UpdateActorSelectCollider(actor, settings);
	}

	public static void EnableActorSelectCollider(Actor actor, bool val)
	{
		actor.Picker.GetComponent<Collider>().enabled = val;
	}

	public static void UpdateActorSelectCollider(Actor actor, PickerColliderSettings settings)
	{
		actor.Picker.transform.localPosition = new Vector3(0f, settings.Offset, 0f);
		actor.Picker.radius = settings.Radius;
		actor.Picker.height = settings.Height;
	}
}
