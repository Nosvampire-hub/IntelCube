using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    Vector3 m_EulerAngleVelocity;
    bool activated = false;
    private GameObject counter;
    private ParticleSystem blood;

    void Start()
    {
        counter = GameObject.Find("Me");
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();

        //Set the angular velocity of the Rigidbody (rotating around the Y axis, 100 deg/sec)
        m_EulerAngleVelocity = new Vector3(-225, 0, 0);

        blood = GetComponentInChildren<ParticleSystem>();
        
    }

    void FixedUpdate()
    {
        if (m_Rigidbody.position.z <= -1.5f)
        {
            m_Rigidbody.isKinematic = false;
            print("yoyo");
        }

        if (m_Rigidbody.position.y <= -5)
        {
            Destroy(gameObject);

            counter.GetComponent<PlayerController>().AlterScore(-1);
        }
    }

    IEnumerator BlockMove(float halt)
    {
        yield return new WaitForSeconds(halt);
        for (var i = 0; i < 20; i++)
        {
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            m_Rigidbody.MovePosition(transform.position + new Vector3(0, 0, -2f / 20));
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(halt);

        StartCoroutine(BlockMove(0.5f));
    }


    public void DeleteCube()
    {
        StartCoroutine(BlockDelete());
    }

    IEnumerator BlockDelete()
    {
        yield return new WaitForSeconds(0.1f);
        //blood.Play();
        m_Rigidbody.isKinematic = false;
        for (var i = 0; i < 30; i++)
        {
            transform.position = transform.position + new Vector3(0, i * 0.25f, 0);
            yield return new WaitForSeconds(0.01f);
            if (transform.localScale.x > 0f)
            { 
                transform.localScale += new Vector3(-0.1f, -0.1f, -0.1f);
            }
        }
        Destroy(gameObject);
        FindObjectOfType<AudioManager>().Play("CubeHit");
        counter.GetComponent<PlayerController>().AlterScore(1);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
       
       if (!activated)
        {
            m_Rigidbody.isKinematic = true;
            StartCoroutine(BlockMove(1f));
            activated = true;
        }

    }





}
