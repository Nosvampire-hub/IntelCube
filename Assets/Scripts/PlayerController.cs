using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/*
 * This script is designed to control the player character in a 3D game. The script is responsible for handling the movement of the character, as well as managing the player's input and score.

The script Defines several variables, including an Animation object, a Rigidbody object, several GameObjects, and various other variables used to manage the game's state.

The script then defines several input handlers that respond to user input. These handlers include "OnMove", "OnSelectDe", and "OnPauseUnpause". "OnMove" is called whenever the player moves the joystick or the touch screen. "OnSelectDe" is called when the player selects a cube. "OnPauseUnpause" is called when the player pauses or unpauses the game.

The "Start" method is called at the beginning of the game and initializes the Rigidbody and Animation components of the player character. The method also finds the "Main Camera" object in the scene and sets the player character to be "not alive".

The "FixedUpdate" method is called once per frame and handles the game logic. It first checks if the player has pressed the pause button, and if so, starts the coroutine "PauseMenu".

The method then checks if the player has touched the screen. If the player has touched the screen and the touch is new, the script records the start time and position of the touch. If the touch has been present for a certain amount of time, the script spawns a joystick object that the player can use to control the character's movement. Once the touch has ended, the script destroys the joystick object and sets the "touchJoyValue" variable to zero.

The method then checks if the player has selected a cube. If the player has selected a cube and the player is alive, the script starts the coroutine "CubeSelection".

Finally, the method applies the movement vector to the player character's position, which is multiplied by deltaTime and the player's speed. The method also rotates the player character to face the direction of movement and checks if the player has fallen off the edge of the game board. If the player has fallen off the edge of the game board, the method plays a sound effect, calls the "KillUnKill" method, and starts the "ResetPosition" coroutine.
 */
public class PlayerController : MonoBehaviour
{
    Animation m_Anim;
    Rigidbody m_Rigidbody;
    public GameObject PauseMenuUI;
    public GameObject JoyStick;
    private bool joySpawned = false;
    private GameObject GameControl;
    public float m_Speed = 5f;
    public float m_RotateSpeed = 7f;
    public int goalScore = 24;
    public int missedCubes = 0;
    private Vector2 joystickValue;
    private Vector2 StartTouchPos;
    private Vector2 touchJoyValue;
    private Vector3 m_Input;
    private float buttonPress;
    private float pauseButton;
    private string curFloor;
    private string curCol;
    private string cubeType;
    private string selectedCube = "";
    private bool crRunning = false;
    private bool plAlive = false;
    private string floorType;
    public int PlayerScore = 0;
	private float startTouchTime;
	private float endTouchTime;
    private bool touched;
    public float tapVariance;


	// Start is called before the first frame update
	void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Anim = GetComponentInChildren<Animation>();

        GameControl = GameObject.Find("Main Camera");

        m_Anim.Play("Idle");

        plAlive = false;
    }

    private void OnMove(InputValue value)
    {
        joystickValue = value.Get<Vector2>();
    }

    private void OnSelectDe(InputValue value)
    {
        buttonPress = value.Get<float>();
    }

    private void OnPauseUnpause(InputValue value)
    {
        pauseButton = value.Get<float>();
    }


	// Update is called once per frame
	void FixedUpdate()
    {


		if (pauseButton == 1f)
        {
            if (crRunning == false)
            {
                StartCoroutine(PauseMenu());
            }
        }

		




		if (cubeType != "Evil")
        {
		    m_Input = new Vector3(joystickValue.x + touchJoyValue.x, 0, joystickValue.y + touchJoyValue.y);
		}
        else
        {
			m_Input = new Vector3(joystickValue.x, 0, -.5f);
			StartCoroutine(PushBack());
		}



        if (transform.localPosition.x < -GameControl.GetComponent<GameBase>().baseX + 0.5f)
        {
            if (m_Input.x < 0)
            {
                m_Input.x = 0;
            }
        }

        if (transform.localPosition.x > GameControl.GetComponent<GameBase>().baseX - 0.5f)
        {
            if (m_Input.x > 0)
            {
                m_Input.x = 0;
            }
        }

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition


        if (plAlive)
        {

			if (Input.touchCount > 0)
			{
                for (int i = 0;i < Input.touchCount; i++)
                {
					Touch touch = UnityEngine.Input.GetTouch(i);
					
                    if (touch.phase == UnityEngine.TouchPhase.Began)
					{
                        if (i == 0)
                        {
							startTouchTime = Time.time;
							StartTouchPos = touch.position;
						}
						touched = true;
					}

					if (touched == true)
					{
						float joystickTouchTime = Time.time - startTouchTime;
						if (joystickTouchTime > tapVariance)
						{
                            if (i == 0)
                            {
                                if (!joySpawned)
                                {
                                    Vector3 canvasPos = GameObject.Find("Canvas").transform.position;
                                    print("joystick");
                                    GameObject joyStick = Instantiate(JoyStick, canvasPos, Quaternion.identity);
                                    joyStick.name = "JoyStick";
                                    joyStick.transform.parent = GameObject.Find("Canvas").transform;
                                    joyStick.transform.position = new Vector3(touch.position.x, touch.position.y, 0);


                                    joySpawned = true;
                                }
                                else
                                {
                                    GameObject joystick = GameObject.Find("JoyStick");
                                    joystick.transform.position = new Vector3(touch.position.x, touch.position.y, 0);
                                    Vector2 curTouchPos = touch.position;
                                    touchJoyValue = (curTouchPos - StartTouchPos) / 300;
                                    if (touchJoyValue.y > 1)
                                    {
                                        touchJoyValue.y = 1;
                                    }
                                    if (touchJoyValue.y < -1)
                                    {
                                        touchJoyValue.y = -1;
                                    }
                                    if (touchJoyValue.x > 1)
                                    {
                                        touchJoyValue.x = 1;
                                    }
                                    if (touchJoyValue.x < -1)
                                    {
                                        touchJoyValue.x = -1;
                                    }
                                    print(touchJoyValue);

                                }

                            }
						}
					}

					if (touch.phase == UnityEngine.TouchPhase.Ended)
					{
						
                        endTouchTime = Time.time;

						float totalTouchTime = endTouchTime - startTouchTime;

						print(totalTouchTime);

						if (totalTouchTime < tapVariance)
						{
							if (touched)
							{
								if (plAlive)
								{
									if (!crRunning)
									{
										StartCoroutine(CubeSelection());
									}
								}
							}

						}

						if (i == 1)
						{
							if (touched)
							{
								if (plAlive)
								{
									if (!crRunning)
									{
										StartCoroutine(CubeSelection());
									}
								}
							}
						}
                        if (i == 0)
                        {
							GameObject joystick = GameObject.Find("JoyStick");
							Destroy(joystick);
							touchJoyValue = new Vector2(0, 0);
							joySpawned = false;
							touched = false;
						}
					}
				}
			}
				


			if (buttonPress == 1f)
			{
				if (crRunning == false)
				{
					StartCoroutine(CubeSelection());
				}
			}

			if (!crRunning)
            {
                m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * m_Speed);

                if (m_Input != Vector3.zero)
                {
                    Quaternion toRotation = Quaternion.LookRotation(m_Input, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, m_RotateSpeed * Time.deltaTime);
                }

                if (transform.localPosition.y < 0)
                {
					FindObjectOfType<AudioManager>().Play("Fall");
					KillUnKill();
					m_Rigidbody.isKinematic = false;
					m_Anim.Play("Falling");
					StartCoroutine(SendScore());

				}


                if (plAlive)
                {
                    if (m_Input != new Vector3(0f, 0f, 0f))
                    {
                        m_Anim.Play("Run");
                        m_Anim["Run"].speed = 1.0f;

                    }
                    else
                    {
                        m_Anim.Play("Idle");
                    }
                }
            }

        }
        if (transform.localPosition.y < -30)
        {
            GameObject gameController = GameObject.Find("Main Camera");
			gameController.GetComponent<GameBase>().ResetLevel();
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        int evilHit = 0;

        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            curCol = contact.otherCollider.name;
            try
            {
                var tempCol = curCol.Split(",");
                cubeType = tempCol[0];
            }
            catch
            {
            }
            if (cubeType == "Evil")
            {
                evilHit++;
                print("hit yo");
            }
        }

        if (evilHit == 0)
        {
            cubeType = "";
        }
        
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            curFloor = hitData.collider.gameObject.name;
            var floorTypedetail = curFloor.Split(",");
            cubeType = floorTypedetail[0];

            if (cubeType == "Evil")
            {
                GameObject gameController = GameObject.Find("Main Camera");
                gameController.GetComponent<GameBase>().ResetLevel();
            }
        }




    }

    public void KillUnKill()
    {
        if (plAlive == true)
        {
            plAlive = false;

		}
        else
        {
            plAlive = true;
            FindObjectOfType<AudioManager>().Play("BackgroundMusic1");

		}
	}

    IEnumerator PauseMenu()
    {
        crRunning = true;
        Time.timeScale = 0;
        GameObject pauseMenuUI = Instantiate(PauseMenuUI, new Vector3(0, 0, 0), Quaternion.identity);
        pauseMenuUI.name = "PauseMenu";
        yield return new WaitForSeconds(1f);
        crRunning = false;
    }

    IEnumerator SendScore()
    {
        yield return new WaitForSeconds(0.0001f);
	}

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public void AlterScore(int change)
    {
        PlayerScore += change;
        GameObject scoreBoard = GameObject.Find("Score");
        TextMeshPro scoreText = scoreBoard.GetComponent<TextMeshPro>();
        scoreText.SetText(PlayerScore.ToString());

        if (change < 0)
        {
            missedCubes = missedCubes + 1;
        }
    }

    public void ResetMisses()
    {
        missedCubes = 0;
    }

    public void MoveGoal(int factor)
    {
        goalScore = goalScore * factor;

    }

    IEnumerator PushBack()
    {
        for (var x = 0; x < 15; x++)
        {
            m_Rigidbody.MovePosition(transform.position - new Vector3(0,-.1f,.1f) * Time.deltaTime * m_Speed);
            yield return new WaitForSeconds(0.002f);
        }

    }

    IEnumerator CubeSelection()
    {
        crRunning = true;

        m_Anim.Play("Action");
        m_Anim["Action"].speed = 2.0f;

        bool spawnDetect = false;

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData))
        {
            curFloor = hitData.collider.gameObject.name;
            var floorTypedetail = curFloor.Split(",");
            floorType = floorTypedetail[0];

            if (floorTypedetail[1] == "spawn")
            {
                spawnDetect = true;
            }

        }
        else
        {
            floorType = "";
        }
        
        if (!spawnDetect)
        {

            if (floorType == "Ground")
            {
                GameObject selectCube = GameObject.Find(curFloor);
                if (!selectCube.GetComponent<GroundControl>().activated)
                {
                    selectCube.GetComponent<GroundControl>().BlockSelect();
                }

                selectedCube = curFloor;

                yield return new WaitForSeconds(0.5f);

            }
            else
            {
                curFloor = "";
            }

        }
        crRunning = false;
    }

}
