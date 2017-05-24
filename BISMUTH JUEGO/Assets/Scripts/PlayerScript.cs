﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameController gc = null;

    // Movement Variables
    public float rotSpeed = 90f; // Degrees per second
    public float movSpeed = 3f; // units per second

    // Jumping Variables
    public float jumpForce = 5f;
    private Rigidbody rb;
    private bool onFloor;

    void Start ()
    {
        Debug.Log("PlayerScript START");
        rb = GetComponent<Rigidbody>();
	}


	void Update ()
    {
        if (gc == null)
            gc = GameObject.Find("GameController").GetComponent<GameController>();

        if (gc.GetState() == GameState.PLAY)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                transform.Rotate(transform.up * -rotSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                transform.Rotate(transform.up * rotSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                transform.Translate(Vector3.forward * movSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                transform.Translate(Vector3.forward * -movSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log(gc.GetState());
        }
    }

	void OnTriggerEnter(Collider other)
    {
        
    }


    // Jumping =============================================================================================================================

    void FixedUpdate()
    {
        if (onFloor && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            //onFloor = false;
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Terrain")
            onFloor = true;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "Terrain")
            onFloor = false;
    }
}
