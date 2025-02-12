using System.Collections.Generic;
using UnityEngine;

public class SetPieceSignaller : MonoBehaviour
{
	public string Signal;

	public SetPieceLogic[] setPieceLogics;

	private void Awake()
	{
		List<SetPieceLogic> list = new List<SetPieceLogic>();
		int num = 0;
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(SetPieceLogic));
		for (int i = 0; i < array.Length; i++)
		{
			SetPieceLogic setPieceLogic = (SetPieceLogic)array[i];
			RaycastHit hitInfo;
			if (base.GetComponent<Collider>().Raycast(new Ray(setPieceLogic.transform.position + Vector3.down * 3f, Vector3.up), out hitInfo, 6f))
			{
				list.Add(setPieceLogic.GetComponent<SetPieceLogic>());
			}
			num++;
		}
		setPieceLogics = list.ToArray();
	}

	private void SendSignal()
	{
		for (int i = 0; i < setPieceLogics.GetLength(0); i++)
		{
			setPieceLogics[i].ReceiveSignal(Signal);
		}
	}
}
