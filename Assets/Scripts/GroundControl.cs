using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundControl : MonoBehaviour
{
    public Material selectedMaterial;
    public Material normalMaterial;
    private string evilCubeCol;
    public bool activated = false;
    private string cubeType;
    private int fuseLoop;
    public float fuseTimer;
    public GameObject countDownText;
    private GameObject playerTagUI;
    private ParticleSystem blood;


	void Start()
	{
		blood = GetComponentInChildren<ParticleSystem>();
	}

	void OnCollisionExit(Collision collisionInfo)
    {
        if (gameObject.name == "Ground,Spawn")
        {
            Destroy(gameObject);
        }
        
    }


    public void BlockSelect()
    {
        GameObject GameController = GameObject.Find("Main Camera");
        fuseLoop = GameController.GetComponent<GameBase>().enemiesSpawned;
		playerTagUI = Instantiate(countDownText, gameObject.transform.position + new Vector3(0,1,0), Quaternion.Euler(new Vector3(0, 0, 0)));
		playerTagUI.name = "CountDown";
		transform.GetComponentInChildren<Renderer>().material = selectedMaterial;
        activated = true;
        FindObjectOfType<AudioManager>().Play("Select");

        StartCoroutine(BlockDelete());
    }

	IEnumerator BlockDelete()
    {
		for (int i = 0; i < fuseLoop; i++)
		{
            playerTagUI.GetComponent<TextMeshPro>().SetText((fuseLoop - i).ToString());

			yield return new WaitForSeconds(fuseTimer);
		}
        Destroy(playerTagUI);
        BlockDeSelect();
    }


	public void BlockDeSelect()
    {
		blood.Play();
		transform.GetComponentInChildren<Renderer>().material = normalMaterial;
        DeleteCube();
        FindObjectOfType<AudioManager>().Play("DeSelect");
        activated = false;  
    }

    public void DeleteCube()
    {

        if (activated)
        {
            Ray ray = new Ray(transform.position, transform.up);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData))
            {
                evilCubeCol = hitData.collider.gameObject.name;
                var cubeDetail = evilCubeCol.Split(",");
                cubeType = cubeDetail[0];
            }
            else
            {
                cubeType = "";
            }

            if (cubeType == "Evil")
            {
                GameObject deletCube = GameObject.Find(evilCubeCol);
                try
                {
                    deletCube.GetComponent<EnemyControl>().DeleteCube();
                }
                catch
                {
                    deletCube.GetComponent<EnemyBControl>().DeleteCube();
                }
            }
        }

    }

    
}
