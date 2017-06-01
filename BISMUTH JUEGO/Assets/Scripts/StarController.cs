using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    public float maxHeight;
    public float speed;

    private bool unlocked;
    private float currentHeight;
    private Vector3 originPos;

    void Start ()
    {
        currentHeight = 0;
        unlocked = false;
        originPos = transform.position;
    }
	
	void Update ()
    {
        if (unlocked && currentHeight < maxHeight)
        {
            float inc = Time.deltaTime * speed;
            transform.position += transform.up * inc;
            currentHeight += inc;
        }
    }

    public void Reset()
    {
        currentHeight = 0;
        unlocked = false;
        transform.position = originPos;
    }

    public void Unlock()
    {
        unlocked = true;
    }

    public bool Unlocked()
    {
        return unlocked;
    }
}
