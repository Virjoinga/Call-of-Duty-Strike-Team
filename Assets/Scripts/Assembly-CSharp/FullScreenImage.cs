using UnityEngine;

[RequireComponent(typeof(GUITexture))]
public class FullScreenImage : MonoBehaviour
{
	public string Image_1136x640_string;

	public string Image_1280x800_string;

	public string Image_1920x1200_string;

	public string Image_2560x1600_string;

	private void Start()
	{
        string path = ((Screen.height <= 640) ? ("FullScreenTextures/" + Image_1136x640_string + ".jpg") : ((Screen.height <= 800) ? ("FullScreenTextures/" + Image_1280x800_string + ".jpg") : ((Screen.height > 1200) ? ("FullScreenTextures/" + Image_2560x1600_string + ".jpg") : ("FullScreenTextures/" + Image_1920x1200_string + ".jpg"))));
		Texture2D texture2D = Resources.Load<Texture2D>(path.Replace(".jpg", ""));
        GUITexture component = base.gameObject.GetComponent<GUITexture>();
		component.texture = texture2D;
		base.transform.position = Vector3.zero;
		base.transform.localScale = Vector3.zero;
		component.pixelInset = new Rect((Screen.width - texture2D.width) / 2, (Screen.height - texture2D.height) / 2, texture2D.width, texture2D.height);
	}
}
