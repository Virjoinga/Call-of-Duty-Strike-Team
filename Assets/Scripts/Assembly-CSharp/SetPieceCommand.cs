using System.Collections;
using UnityEngine;

public class SetPieceCommand : Command
{
	public bool UseSelectedCharacters = true;

	public SetPieceLogic SetPiece;

	public ContainerReference SetPieceContainer;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (SetPiece == null)
		{
			if (SetPieceContainer != null)
			{
				SetPiece = SetPieceContainer.GetInternalSetPieceLogic(null);
				if (SetPiece == null)
				{
					Debug.Log("Cannot find set piece logic from the container");
					yield return null;
				}
			}
			if (SetPiece == null)
			{
				Debug.Log("Cannot find set piece logic for the setpiece/cutscene");
				yield return null;
			}
		}
		if ((bool)SetPiece.SPModule && SetPiece.SPModule.UsePlaygroundMeshes && UseSelectedCharacters)
		{
			Debug.LogWarning("Set piece set to use playground meshes, but the selected characters are being passed in");
		}
		if ((bool)gameplayController && UseSelectedCharacters)
		{
			int NumActors = SetPiece.GetNumActorsRequired();
			Actor[] characters = gameplayController.Selected.ToArray();
			int i = 0;
			Actor[] array = characters;
			foreach (Actor bod in array)
			{
				SetPiece.SetActor_IndexOnlyCharacters(i, bod);
				i++;
				if (i >= NumActors)
				{
					break;
				}
			}
		}
		if ((bool)SetPiece)
		{
			SetPiece.PlaySetPiece();
			while (!SetPiece.HasFinished())
			{
				yield return null;
			}
		}
	}
}
