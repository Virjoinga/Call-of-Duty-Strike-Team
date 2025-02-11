using System;
using UnityEngine;

public class EnterableBuildingManager : MonoBehaviour
{
	public Shader ComplextExteriorShader;

	public Texture DefaultBlendTexture;

	public Texture DefaultEdgeTexture;

	private static EnterableBuildingManager smInstance;

	public static EnterableBuildingManager Instance
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
			throw new Exception("Can not have multiple EnterableBuildingManager");
		}
		smInstance = this;
	}

	private void OnDestroy()
	{
		smInstance = null;
	}
}
