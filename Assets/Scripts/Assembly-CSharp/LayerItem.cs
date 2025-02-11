using System;

[Serializable]
public class LayerItem
{
	public enum LayerType
	{
		Section_Layer = 0,
		Transition_Layer = 1,
		Shared_Layer = 2,
		External_Layer = 3
	}

	public string m_name;

	public LevelLayer m_layer;

	public LayerType m_type;

	public bool m_HasLightmap;

	public bool m_DevelopmentLayer;
}
