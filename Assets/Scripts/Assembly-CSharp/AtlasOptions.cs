using UnityEngine;

public class AtlasOptions : MonoBehaviour
{
	public bool RegenerateSecondaryUVs;

	private void Awake()
	{
		Object.Destroy(this);
	}
}
