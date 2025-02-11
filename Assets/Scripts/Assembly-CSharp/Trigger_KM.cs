using UnityEngine;

public class Trigger_KM : MonoBehaviour
{
	public GameObject Detector;

	public GameObject Mine;

	public HaloEffect MineLight;

	public GameObject MsgTarget;

	public string Message;

	public GameObject ScriptedVariable;

	public bool DoExplosion;

	public bool OnceOnly;

	public bool ShowResults;

	private bool isActive = true;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isActive)
		{
			return;
		}
		string text = other.name;
		if (text == Detector.name)
		{
			if (MsgTarget == null)
			{
				Detector.gameObject.SendMessage(Message);
			}
			else if (MsgTarget != null)
			{
				Container.SendMessage(MsgTarget, Message, base.gameObject);
			}
			if (DoExplosion)
			{
				RulesSystem.DoAreaOfEffectDamage(base.transform.position, 3f, 0.01f, null, ExplosionManager.ExplosionType.Grenade, "Explosion");
			}
			if (Mine != null)
			{
				Container.SendMessage(Mine, "CancelHacking");
				Container.SendMessage(Mine, "Deactivate");
				MineLight.SetBlinkPattern(HaloEffect.BlinkPattern.Off);
				MineLight.SetColour(HaloEffect.HaloColour.Green);
			}
			if (ScriptedVariable != null)
			{
				Container.SendMessage(ScriptedVariable, "Activate");
			}
			if (OnceOnly)
			{
				isActive = false;
			}
			if (ShowResults)
			{
				CommonHudController.Instance.ShowGMGResults(true);
			}
		}
	}

	public void Deactivate()
	{
		isActive = false;
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.blue.Alpha(0.2f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
