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
		transform.GetComponentInChildren<Renderer>().material = selectedMaterial;
        activated = true;
        FindObjectOfType<AudioManager>().Play("Select");
    }



	public void BlockDeSelect()
    {
		transform.GetComponentInChildren<Renderer>().material = normalMaterial;
        DeleteCube();
        FindObjectOfType<AudioManager>().Play("DeSelect");
        activated = false;
        //GameObject player = GameObject.Find("Me");
        //player.GetComponent<PlayerController>().cubesSelected--;
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
					FindObjectOfType<AudioManager>().Play("Disintegrate");
					blood.Play();
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
