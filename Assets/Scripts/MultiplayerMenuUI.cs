using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerMenuUI : MonoBehaviour
{
	public void StartHost()
	{
		GameObject netWorkCont = GameObject.Find("NetworkManager");
		
		netWorkCont.GetComponent<PlayerLobby>().CreateLobby();

		GameObject mainMenu = GameObject.Find("MainMenu");
		Destroy(mainMenu);

	}
	public void StartClient()
	{
		GameObject netWorkCont = GameObject.Find("NetworkManager");

		netWorkCont.GetComponent<PlayerLobby>().MatchMaking();

		GameObject mainMenu = GameObject.Find("MainMenu");
		Destroy(mainMenu);

	}
	public void RestartGame()
	{
		GameObject MainCamera = GameObject.Find("Main Camera");
		MainCamera.GetComponent<MultiplayerGameBase>().ResetLevel();
		GameObject Self = GameObject.Find("PauseMenu");
		Destroy(Self);
	}
}
