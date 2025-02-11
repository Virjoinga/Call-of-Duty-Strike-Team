using System;
using UnityEngine;

public class BagElement : MonoBehaviour
{
	public string m_Guid = Guid.NewGuid().ToString();

	public bool m_CanBeMoved;
}
