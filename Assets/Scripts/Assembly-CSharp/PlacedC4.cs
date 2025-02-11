using UnityEngine;

public class PlacedC4 : MonoBehaviour
{
	public PlacedC4Data mInterface;

	private SetPieceLogic m_logic;

	public GameObject ObjectiveBlipPrefab;

	private ObjectiveBlip mBlip;

	private void Awake()
	{
		m_logic = GetComponent<SetPieceLogic>();
	}

	private void Update()
	{
		if (!(m_logic != null) || !m_logic.WasSuccessful)
		{
			return;
		}
		if (mInterface.ObjectToCallOnSuccess != null)
		{
			Container.SendMessage(mInterface.ObjectToCallOnSuccess, mInterface.FunctionToCallOnSuccess, base.gameObject);
		}
		string message = string.Empty;
		GameObject gameObject = null;
		int num = 0;
		foreach (GameObject item in mInterface.ObjectsToMessageOnCollection)
		{
			if (num < mInterface.FunctionsToCallOnCollection.Count)
			{
				message = mInterface.FunctionsToCallOnCollection[num];
			}
			if (mInterface.ParamToPass != null && num < mInterface.ParamToPass.Count)
			{
				gameObject = mInterface.ParamToPass[num];
			}
			if (gameObject != null)
			{
				Container.SendMessageWithParam(item, message, gameObject, base.gameObject);
			}
			else
			{
				Container.SendMessage(item, message, base.gameObject);
			}
			num++;
		}
		if ((bool)mBlip)
		{
			mBlip.SwitchOff();
		}
		base.enabled = false;
	}

	public void BlipUp()
	{
		if (base.enabled)
		{
			GameObject gameObject = Object.Instantiate(ObjectiveBlipPrefab) as GameObject;
			mBlip = gameObject.GetComponent<ObjectiveBlip>();
			if ((bool)mBlip)
			{
				mBlip.mObjectiveText = Language.Get("S_TARGET");
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.position = base.transform.position + new Vector3(0f, 1f, 0f);
				mBlip.Target = gameObject2.transform;
				mBlip.AllowHighlight = false;
				mBlip.ColourBlip(Color.green);
				mBlip.SwitchOn();
			}
		}
	}
}
