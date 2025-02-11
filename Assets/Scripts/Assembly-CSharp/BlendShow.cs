using UnityEngine;

[ExecuteInEditMode]
public class BlendShow : MonoBehaviour
{
	public AnimDirector.BlendEasing ein;

	public AnimDirector.BlendEasing eout;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 localScale = base.transform.localScale;
		localScale.x = Mathf.Min(10f, localScale.x);
		localScale.y = AnimDirector.Blender.Evaluate(0f, 10f, localScale.x / 10f, ein, eout);
		base.transform.localScale = localScale;
	}
}
