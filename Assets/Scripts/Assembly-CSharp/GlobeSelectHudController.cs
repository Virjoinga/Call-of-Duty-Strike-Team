using System;
using UnityEngine;

public class GlobeSelectHudController : MonoBehaviour
{
	private static GlobeSelectHudController smInstance;

	public static GlobeSelectHudController Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple GlobeSelectHudController");
		}
		smInstance = this;
		if (GlobeSelect.Instance != null)
		{
			base.gameObject.transform.parent = GlobeSelect.Instance.gameObject.transform;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
