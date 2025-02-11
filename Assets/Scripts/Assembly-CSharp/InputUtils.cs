using UnityEngine;

public static class InputUtils
{
	public class PointerInfo
	{
		public Vector2 InputDelta { get; private set; }

		public Vector2 CurrentPosition { get; private set; }

		public Vector2 OriginalPosition { get; private set; }

		public PointerInfo(Vector2 inputDelta, Vector2 currentPosition, Vector2 originalPosition)
		{
			InputDelta = inputDelta;
			CurrentPosition = currentPosition;
			OriginalPosition = originalPosition;
		}
	}

	public static Vector2 GetTrackpadInput(PointerInfo ptr)
	{
		float num = 256f;
		Vector2 inputDelta = ptr.InputDelta;
		return Vector2.Scale(inputDelta, new Vector2(num / (float)Screen.width, num / (float)Screen.height));
	}

	public static Vector2 GetThumbstickInput(PointerInfo ptr, Vector3[] notches)
	{
		Vector2 a = ptr.CurrentPosition - ptr.OriginalPosition;
		Vector2 input = Vector2.Scale(a, new Vector2(1f / (float)Screen.width, 1f / (float)Screen.height));
		return GetThumbstickInput(input, notches);
	}

	public static Vector2 GetThumbstickInput(Vector2 input, Vector3[] notches)
	{
		float magnitude = input.magnitude;
		if (magnitude > 1E-05f)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < notches.Length; i++)
			{
				if (!(magnitude >= num))
				{
					break;
				}
				Vector3 vector = notches[i];
				float x = vector.x;
				float t = Mathf.InverseLerp(num, x, magnitude);
				num2 = Mathf.Lerp(vector.y, vector.z, t);
				num = x;
			}
			return num2 / magnitude * input;
		}
		return Vector2.zero;
	}
}
