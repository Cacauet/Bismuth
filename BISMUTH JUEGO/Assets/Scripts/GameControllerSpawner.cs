using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerSpawner : MonoBehaviour
{
    private GameController gc = null;
    public GameObject gcPrefab;
    public int currentScene;

    // Level Specs
    public StarController[] stars;
    public PortalController[] portals;
    public GameObject[] keys;

    public Transform player;
    private Vector3 playerPos;
    private Quaternion playerRot;

    void Awake()
    {
        Debug.Log("GameControllerSpawner AWAKE");
        GameObject gameController = GameObject.Find("GameController");

        if (gameController == null)
        {
            Debug.Log("GameController NOT FOUND");
            gameController = Instantiate(gcPrefab);
            gameController.transform.parent = transform;
            gameController.tag = gameController.name = "GameController";
        }
        else
        {
            Debug.Log("GameController FOUND");
        }

        gc = gameController.GetComponent<GameController>();
        gc.SetLevelSpecs(stars.Length, portals);
        gc.StartCutScene(Cutscene.INTRO);

        playerPos = player.position;
        playerRot = player.rotation;
    }

    public void ResetLevel()
    {
        // Reset objects
        foreach (StarController sc in stars)
            sc.Reset();

        foreach (PortalController pc in portals)
            pc.Reset();

        foreach (GameObject key in keys)
            key.SetActive(true);

        // Reset Player
        player.position = playerPos;
        player.rotation = playerRot;
    }
}
