using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCornerFocus : MonoBehaviour
{
    // Corners
    public Transform corner1 = null;
    public Transform corner2 = null;

    // General Values
    private Camera cam;
    private Vector2 screenDimensions;
    private bool centerCam;
    private bool centered;

    // Persperctive variables
    private float timer;
    private float originFOV;
    private float destFOV;
    private const float CENTER_DURATION = 1f;
    private const float FOV_MARGIN = 5.0f;

    // Smooth damping references
    private float m_ZoomSpeed;
    private Vector3 m_MoveVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
        screenDimensions.x = Screen.width;
        screenDimensions.y = Screen.height;
        centered = true;
        centerCam = false;

        if (corner1 != null && corner2 != null)
            CenterCamera(corner1, corner2);
    }

    void FixedUpdate()
    {
        if (centerCam && cam.orthographic)
        {
            // Move camera
            Vector3 averagePos = (corner1.position + corner2.position) / 2;
            averagePos.y = transform.position.y;

            transform.position = Vector3.SmoothDamp(transform.position, averagePos, ref m_MoveVelocity, 0.2f);

            // Adjust zoom
            float size = 0f;
            Vector3 desiredLocalPos = transform.InverseTransformPoint(averagePos);

            Vector3 desiredPosToTarget = transform.InverseTransformPoint(corner1.position) - desiredLocalPos;
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / cam.aspect);

            desiredPosToTarget = transform.InverseTransformPoint(corner2.position) - desiredLocalPos;
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / cam.aspect);

            // Camera limits
            size = Mathf.Max(size, 2f);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, size, ref m_ZoomSpeed, 0.2f);
        }
    }

    void Update()
    {
        if (!centerCam)
            return;
        
        if (screenDimensions.x != Screen.width || screenDimensions.y != Screen.height)
        {
            screenDimensions.x = Screen.width;
            screenDimensions.y = Screen.height;
            CenterCamera(corner1, corner2);
        }

        if(!centered) // change camera FOV if "aspect ratio has changed" or "not centered yet"
        {
            if(timer > CENTER_DURATION)
            {
                centered = true;
            }
            else
            {
                timer += Time.deltaTime / CENTER_DURATION;

                if (!cam.orthographic)
                    cam.fieldOfView = Mathf.Lerp(originFOV, destFOV, timer);
            }
        }
    }

    public void CenterCamera(Transform c1, Transform c2)
    {
        corner1 = c1;
        corner2 = c2;
        centerCam = true;
        centered = false;
        timer = 0f;

        if (!cam.orthographic)
        {
            // Calculate the new FOV -> Source: http://stackoverflow.com/questions/22015697/how-to-keep-2-objects-in-view-at-all-time-by-scaling-the-field-of-view-or-zy
            Vector3 middlePoint = corner1.position + 0.5f * (corner2.position - corner1.position);
            float distanceBetweenCorners = (corner2.position - corner1.position).magnitude;
            float distanceFromMiddlePoint = (cam.transform.position - middlePoint).magnitude;
            float aspectRatio = screenDimensions.x / screenDimensions.y;
            destFOV = (2.0f * Mathf.Rad2Deg * Mathf.Atan((0.5f * distanceBetweenCorners) / (distanceFromMiddlePoint * (aspectRatio)))) + FOV_MARGIN;
            originFOV = cam.fieldOfView;
        }
    }

    public void DisableCameraCentering()
    {
        centerCam = false;
    }
}
