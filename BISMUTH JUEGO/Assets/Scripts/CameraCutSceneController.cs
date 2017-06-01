using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Cutscene
{
    INTRO,
    WALL_SHOW,
    FINAL
}

public class CameraCutSceneController : MonoBehaviour
{
    private GameController gc = null;

    private Cutscene currentCutscene = Cutscene.INTRO;
    private float transitionTimer;
    private int transitionCounter;

    // Camera traveling transforms
    public Transform[] introTransforms;
    public float[] introTransitionDurations;

    public Transform[] wallTransforms;
    public float[] wallTransitionDurations;

    public Transform[] finalTransforms;
    public float[] finalTransitionDurations;


    void Update ()
    {
        if (gc == null)
            gc = GameObject.Find("GameController").GetComponent<GameController>();

        if (gc.GetState() == GameState.CUTSCENE)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                gc.SetState(GameState.TRANSITION_TO_PLAY);
            }
            else
            {
                switch (currentCutscene)
                {
                    case Cutscene.INTRO:
                        {
                            if (!HandleCutscene(introTransforms, introTransitionDurations))
                            {
                                gc.SetState(GameState.PLAY);
                            }

                            break;
                        }
                    case Cutscene.WALL_SHOW:
                        {
                            if (!HandleCutscene(wallTransforms, wallTransitionDurations))
                                gc.SetState(GameState.PLAY);

                            break;
                        }
                    case Cutscene.FINAL:
                        {
                            if (!HandleCutscene(finalTransforms, finalTransitionDurations))
                                gc.SetState(GameState.PLAY);

                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }

    // returns true if cutscene is still going
    private bool HandleCutscene(Transform[] transforms, float[] durations)
    {
        bool ret = true;

        if(transitionCounter == transforms.Length - 1) // transition finished
        {
            Debug.Log("CAMERA MOVEMENT FINISHED: " + currentCutscene);
            ret = false;
        }
        else
        {
            if(transitionTimer > 1f)
            {
                transitionCounter++;
                transitionTimer = 0f;
            }
            else
            {
                transitionTimer += Time.deltaTime / durations[transitionCounter];

                // Move
                Vector3 pos;
                pos.x = Mathf.Lerp(transforms[transitionCounter].position.x, transforms[transitionCounter + 1].position.x, transitionTimer);
                pos.y = Mathf.Lerp(transforms[transitionCounter].position.y, transforms[transitionCounter + 1].position.y, transitionTimer);
                pos.z = Mathf.Lerp(transforms[transitionCounter].position.z, transforms[transitionCounter + 1].position.z, transitionTimer);
                transform.position = pos;

                // Rotate
                transform.rotation = Quaternion.identity;
                transform.Rotate(
                    Mathf.Lerp(transforms[transitionCounter].rotation.eulerAngles.x, transforms[transitionCounter + 1].rotation.eulerAngles.x, transitionTimer),
                    Mathf.Lerp(transforms[transitionCounter].rotation.eulerAngles.y, transforms[transitionCounter + 1].rotation.eulerAngles.y, transitionTimer),
                    Mathf.Lerp(transforms[transitionCounter].rotation.eulerAngles.z, transforms[transitionCounter + 1].rotation.eulerAngles.z, transitionTimer));
            }
        }

        return ret;
    }


    public void StartCutscene(Cutscene cutscene, GameController _gc)
    {
        gc = _gc;
        transitionTimer = 0f;
        transitionCounter = 0;
        currentCutscene = cutscene;
        gc.SetState(GameState.CUTSCENE);

        switch (currentCutscene)
        {
            case Cutscene.INTRO:
                {
                    transform.position = introTransforms[0].position;
                    transform.rotation = introTransforms[0].rotation;

                    break;
                }
            case Cutscene.WALL_SHOW:
                {
                    transform.position = wallTransforms[0].position;
                    transform.rotation = wallTransforms[0].rotation;

                    break;
                }
            case Cutscene.FINAL:
                {
                    transform.position = finalTransforms[0].position;
                    transform.rotation = finalTransforms[0].rotation;

                    break;
                }
            default:
                break;
        }
    }
}
