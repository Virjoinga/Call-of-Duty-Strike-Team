using System.Collections;

public class EndSkippableSceneCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		SceneNanny.EndSkippableScene();
		yield break;
	}
}
