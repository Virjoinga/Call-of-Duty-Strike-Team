using UnityEngine;

public class BufferObjectHelper : MonoBehaviour
{
	public static BufferObject CreateBufferObject()
	{
		return (BufferObject)ScriptableObject.CreateInstance(typeof(BufferObject));
	}

	public static BufferObject LoadBufferObject(string name)
	{
		return Resources.Load("BufferObjects/" + name) as BufferObject;
	}
}
