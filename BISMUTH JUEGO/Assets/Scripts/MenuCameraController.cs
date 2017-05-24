using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    // Menu References
    public MenuReferences menu1;
    public MenuReferences menu2;
    public MenuReferences menu3;
    private MenuReferences currentMenu;

    // Movement Values
    public float transitionSeconds = 1f;
    private Transform destination = null;
    private float timer = 0f;
    private Vector3 originPosition;
    private Vector3 originRotation;

    private Camera cam;
    private CameraCornerFocus ccf;

    void Start()
    {
        cam = GetComponent<Camera>();
        ccf = GetComponent<CameraCornerFocus>();
        ChangeMenu(menu1, 4f);
    }

    void Update()
    {
        if(destination == null) //not moving
        {
            if (Input.GetMouseButtonDown(0)) // On mouse left click -> raycast
            {
                Debug.Log("mouse left click");
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    switch (hit.transform.tag)
                    {
                        case "LevelSelect":
                            {
                                //Debug.Log("LevelSelect pressed");
                                ChangeMenu(menu2);
                                break;
                            }
                        case "Level 1":
                            {
                                //Debug.Log("Level 1 pressed");

                                GameObject controller = GameObject.Find("GameController");
                                if (controller != null)
                                    controller.GetComponent<GameController>().FadeToNextScene(1);

                                break;
                            }
                        case "Level 2":
                            {
                                //Debug.Log("LevelSelect pressed");

                                /*GameObject controller = GameObject.Find("GameController");
                                if (controller != null)
                                    controller.GetComponent<GameController>().ChangeScene(2);*/

                                break;
                            }
                        case "Level 3":
                            {
                                //Debug.Log("LevelSelect pressed");

                                /*GameObject controller = GameObject.Find("GameController");
                                if (controller != null)
                                    controller.GetComponent<GameController>().ChangeScene(3);*/
                                
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            if(timer > 1f) // camera has arrived
            {
                destination = null;
                ccf.CenterCamera(currentMenu.corner1, currentMenu.corner2);
            }
            else
            {
                timer += Time.deltaTime / transitionSeconds;

                // Move
                Vector3 pos;
                pos.x = Mathf.Lerp(originPosition.x, destination.position.x, timer);
                pos.y = Mathf.Lerp(originPosition.y, destination.position.y, timer);
                pos.z = Mathf.Lerp(originPosition.z, destination.position.z, timer);
                transform.position = pos;

                // Rotate
                transform.rotation = Quaternion.identity;
                transform.Rotate(
                    Mathf.Lerp(originRotation.x, destination.rotation.eulerAngles.x, timer),
                    Mathf.Lerp(originRotation.y, destination.rotation.eulerAngles.y, timer),
                    Mathf.Lerp(originRotation.z, destination.rotation.eulerAngles.z, timer));
            }
        }
    }

    private void ChangeMenu(MenuReferences menu, float seconds = 1f)
    {
        originPosition = transform.position;
        originRotation = transform.rotation.eulerAngles;
        timer = 0f;
        transitionSeconds = seconds;
        currentMenu = menu;
        destination = currentMenu.cameraPos;
        ccf.DisableCameraCentering();
    }
}
