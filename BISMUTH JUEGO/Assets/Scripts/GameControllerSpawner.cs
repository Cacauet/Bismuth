using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerSpawner : MonoBehaviour
{
    private GameController gc = null;

    public GameObject gcPrefab;
    public int currentScene;
    public GameState initState = GameState.PLAY;

    void Awake()
    {
        Debug.Log("GameControllerSpawner AWAKE");
        GameObject gameController = GameObject.Find("GameController");

        if (gameController == null)
        {
            Debug.Log("GameController NOT FOUND");
            gameController = Instantiate(gcPrefab);
            gameController.transform.parent = transform;
            gameController.name = "GameController";

            gc = gameController.GetComponent<GameController>();
            gc.SetState(initState);
        }
        else
        {
            Debug.Log("GameController FOUND");
        }
    }

    void Update()
    {
        if(gc.GetState() == GameState.PLAY)
        {
            GameObject.Find("Main Camera").GetComponent<CameraCutSceneController>().StartCutscene(Cutscene.INTRO);
        }
    }
}
