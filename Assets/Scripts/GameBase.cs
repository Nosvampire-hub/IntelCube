using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameBase : MonoBehaviour
{
    public int enemiesSpawned;
    public int enemyWave = 0;
    public GameObject objectToSpawn;
    private GameObject playerActor;
    public GameObject floorCubes;
    public GameObject badCubes;
    public GameObject badCubesB;
    public GameObject MainMenuUI;
    public GameObject PhoneControlUI;
    public GameObject ScoreBoardUI;
    public GameObject UsernameTag;
    public Vector3 spawnCubes;
    public int baseX = 3;
    public int baseZ = 20;
    public bool enemySpawned = false;
    private GameObject evilCubes;
    private GameObject[,,] groundCubes;
    public float zOffset = -5;
    public float yOffset = 1;
    private GameObject adBanner;
    private GameObject player;

    void Start()
    {
        //spawns name which later moves to players feet
        GameObject playerTagUI = Instantiate(UsernameTag, new Vector3(0, -10, 10), Quaternion.Euler(new Vector3(90, 0, 0)));
        playerTagUI.name = "NameTag";
        
        //
        GameObject mainMenuUI = Instantiate(MainMenuUI, new Vector3(0, 0, 0), Quaternion.identity);
        mainMenuUI.name = "MainMenu";


        playerActor = Instantiate(objectToSpawn, new Vector3(0,5,0), Quaternion.identity);
        playerActor.name = "Me";

        GameObject[,,] groundCubes = new GameObject[1,999,999];

        
        groundCubes[0,0,0] = Instantiate(floorCubes, new Vector3(0,4,0), Quaternion.identity);
        groundCubes[0, 0, 0].name = "Ground,Spawn";

        player = playerActor;



    }

    void Update()
    {

		transform.position = player.transform.position - new Vector3((player.transform.position.x / 10),yOffset,zOffset);
        transform.LookAt(player.transform);
        
        if (enemySpawned)
        {
            GameObject score = GameObject.Find("Score");
            score.transform.position = player.transform.position + new Vector3(0f, 0f, -3f);
            
            GameObject nameTag = GameObject.Find("NameTag");
            nameTag.transform.position = player.transform.position + new Vector3(0f, 0f, -4f);

            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                GameObject player = GameObject.Find("Me");
                if (player.GetComponent<PlayerController>().PlayerScore > player.GetComponent<PlayerController>().goalScore)
                {
                    StartCoroutine(levelExtendX(0.01f));
                    player.GetComponent<PlayerController>().MoveGoal(2);
                }
                if (player.GetComponent<PlayerController>().missedCubes == 0)
                {
                    StartCoroutine(levelExtendZ(0.01f));
                }
                else
                {
                    player.GetComponent<PlayerController>().ResetMisses();
                }

                StartCoroutine(EnemySpawner(0.5f));
            }
        }

    }


    IEnumerator EnemySpawner(float halt)
    {
        enemyWave = enemyWave + 1;

        System.Random rnd = new System.Random();

        evilCubes = new GameObject();

        enemiesSpawned = baseZ / 10;

        print(enemiesSpawned);

        for (var x = baseZ - baseZ / 10; x < baseZ; x++)
        {
            for (var i = 0; i < baseX; i++)
            {
                int selector = rnd.Next(10);
                if (selector > 8)
                {
                    evilCubes = Instantiate(badCubesB, new Vector3(-baseX + (i * 2.0f + 1), 10, x * 2.0f), Quaternion.identity);
                }
                else
                {
                    evilCubes = Instantiate(badCubes, new Vector3(-baseX + (i * 2.0f + 1), 10, x * 2.0f), Quaternion.identity);
                }
                evilCubes.name = "Evil," + i + ',' + x + "," + enemyWave;
            }
            yield return new WaitForSeconds(halt);
            enemySpawned = true;
        }


    }


    IEnumerator spawnOffsetmove(float halt)
    {
        for (var x = 0; x < 30; x++)
        {
            zOffset += x/13;
            yOffset -= x/20;

            yield return new WaitForSeconds(halt);
        }
    }

    IEnumerator levelExtendZ(float halt)
    {
        baseZ = baseZ + 1;


        GameObject tempFloor0 = Instantiate(floorCubes, new Vector3(0, 0, (baseZ * 2.0f) -2), Quaternion.identity);
        tempFloor0.name = "Ground," + "0," + baseZ;

        if (baseX > 3)
        {
            for (var i = 1; i < (baseX + 1) / 2; i++)
            {
                GameObject tempFloor = Instantiate(floorCubes, new Vector3((i * 2.0f), 0, (baseZ * 2.0f) - 2), Quaternion.identity);
                tempFloor.name = "Ground," + i + "," + baseZ;
                GameObject tempFloorNeg = Instantiate(floorCubes, new Vector3((-i * 2.0f), 0, (baseZ * 2.0f) - 2), Quaternion.identity);
                tempFloorNeg.name = "Ground," + -i + "," + baseZ;
                yield return new WaitForSeconds(halt);
                zOffset = zOffset + (2 / baseZ);

            }
        } 
        else
        {
            GameObject tempFloor = Instantiate(floorCubes, new Vector3(2, 0, (baseZ * 2.0f) - 2), Quaternion.identity);
            tempFloor.name = "Ground," + 1 + "," + baseZ;
            GameObject tempFloorNeg = Instantiate(floorCubes, new Vector3(-2, 0, (baseZ * 2.0f) - 2), Quaternion.identity);
            tempFloorNeg.name = "Ground," + -1 + "," + baseZ;
        }

    }

    IEnumerator levelExtendX(float halt)
    {
        baseX = baseX + 2;



        for (var i = 0; i < baseZ; i++)
        {
            GameObject tempFloor = Instantiate(floorCubes, new Vector3(baseX-1, 0, i * 2.0f), Quaternion.identity);
            tempFloor.name = "Ground," + baseX + "," + i;
            GameObject tempFloorNeg = Instantiate(floorCubes, new Vector3(-baseX+1, 0, i * 2.0f), Quaternion.identity);
            tempFloorNeg.name = "Ground," + -baseX + "," + i;
            yield return new WaitForSeconds(halt);
            yOffset = yOffset + (2 / baseZ);

        }
        
    }

    IEnumerator BlockBuilder(float halt)
    {
       

        groundCubes = new GameObject[2, baseX, baseZ];
        for (var x = 0; x < baseZ; x++)
        {
            for (var i = 0; i < baseX; i++)
            {
                groundCubes[1, i, x] = Instantiate(floorCubes, new Vector3(i * 2.0f - baseX + 1, 0, x * 2.0f), Quaternion.identity);
                groundCubes[1, i, x].name = "Ground," + i + ',' + x;
            }
            
            yield return new WaitForSeconds(halt);

        
        }
        StartCoroutine(EnemySpawner(0.5f));
    }




    public void LoadLevel()
    {

        playerActor.GetComponent<PlayerController>().KillUnKill();

        StartCoroutine(BlockBuilder(0.1f));

        StartCoroutine(spawnOffsetmove(0.01f));

        GameObject phoneControlUI = Instantiate(PhoneControlUI, new Vector3(0, 0, 0), Quaternion.identity);
        phoneControlUI.name = "PhoneControl";
        
        GameObject scoreBoardUI = Instantiate(ScoreBoardUI, new Vector3(0, -10, 10), Quaternion.Euler(new Vector3(90,0,0)));
        scoreBoardUI.name = "Score";

    }

    public void ResetLevel()
    {
        SceneManager.LoadScene("SinglePlayer", LoadSceneMode.Single);
    }

}
