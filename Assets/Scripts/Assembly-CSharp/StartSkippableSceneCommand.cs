using System.Collections;

public class StartSkippableSceneCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		SceneNanny.BeginSkippableScene();
		yield break;
	}
}
