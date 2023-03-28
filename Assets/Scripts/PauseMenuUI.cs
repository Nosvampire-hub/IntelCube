using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameObject Player = GameObject.Find("Me");
        Player.GetComponent<PlayerController>().Resume();
        GameObject Self = GameObject.Find("PauseMenu");
        Destroy(Self);
    }

    public void RestartGame()
    {
        GameObject MainCamera = GameObject.Find("Main Camera");
        MainCamera.GetComponent<GameBase>().ResetLevel();
        GameObject Player = GameObject.Find("Me");
        Player.GetComponent<PlayerController>().Resume();
        GameObject Self = GameObject.Find("PauseMenu");
        Destroy(Self);
    }
}
