using UnityEngine;

public interface ILightingOverride
{
	void CalculateLighting(Vector3 anchorPosition, float[] coefficients);

	bool Valid();
}
