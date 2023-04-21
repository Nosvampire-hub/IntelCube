using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMultiplayerMenu : MonoBehaviour
{
	public void RestartGame()
	{
		GameObject MainCamera = GameObject.Find("Main Camera");
		MainCamera.GetComponent<MultiplayerGameBase>().ResetLevel();
		GameObject Self = GameObject.Find("PauseMenu");
		Destroy(Self);
	}
	public void StartGame()
	{
		GameObject Self = GameObject.Find("PauseMenu");
		Destroy(Self);
	}
}
