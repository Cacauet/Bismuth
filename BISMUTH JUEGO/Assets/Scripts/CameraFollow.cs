using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameController gc = null;
    public GameObject target = null;

    void Start()
    {
        Debug.Log("CameraFollow START");
    }

	void Update ()
    {
        if(gc == null)
            gc = GameObject.Find("GameController").GetComponent<GameController>();

        if (gc.GetState() == GameState.PLAY) // Follow target when playing
        {
            // Translate
            Vector3 pos = target.transform.position;
            pos += target.transform.up * 1f;
            pos -= target.transform.forward * 1.5f;
            transform.position = pos;

            // Rotate
            transform.LookAt(target.transform);
        }
    }
}
