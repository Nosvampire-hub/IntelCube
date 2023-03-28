using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject SettingUI;

    public void StartGame()
    {
        GameObject MainCamera = GameObject.Find("Main Camera");
        MainCamera.GetComponent<GameBase>().LoadLevel();
        GameObject mainMenu = GameObject.Find("MainMenu");
        Destroy(mainMenu);
    }

    public void ShowAchievement()
    {

	}

    public void ShowLeaderboard()
    {

	}

    public void ShowSettings()
    {
		GameObject SettingsUI = Instantiate(SettingUI, new Vector3(0, 0, 0), Quaternion.identity);
		SettingsUI.name = "Settings";
	}


}
