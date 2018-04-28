using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    private const float CameraPanSpeed = 3f;
    private const float CameraRotateIncrement = 90f;
    private const float CameraLerpTime = 0.35f;
    
    public const float CameraZoomSpeed = 1f;
    private const float CameraSizeMin = 4;
    private const float CameraSizeMax = 24;


    private bool m_cameraAnimating;
    private Quaternion m_desiredRotation;
    private Vector3 m_desiredPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
    }

    /// <summary>
    /// Take player input and move the camera.
    /// </summary>
    private void HandleInput()
    {
        // Pan the camera
        if(Input.GetKey(KeyCode.W))
        {
            PanCamera(new Vector2(0, 1));
        }
        if (Input.GetKey(KeyCode.S))
        {
            PanCamera(new Vector2(0, -1));
        }
        if (Input.GetKey(KeyCode.A))
        {
            PanCamera(new Vector2(-1, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            PanCamera(new Vector2(1, 0));
        }

        // Rotate the camera
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(-1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(1);
        }

        // Handle scrolling
        ZoomCamera(-Input.mouseScrollDelta.y * GameCamera.CameraZoomSpeed);
    }

    /// <summary>
    /// Move the camera across the screen based on movement speed.
    /// </summary>
    private void PanCamera(Vector2 amount)
    {
        transform.position += transform.up * amount.y * CameraPanSpeed * Time.deltaTime;
        transform.position += transform.right * amount.x * CameraPanSpeed * Time.deltaTime;
    }

    private void RotateCamera(int direction)
    {
        if(m_cameraAnimating)
        {
            return;
        }

        float angle = CameraRotateIncrement * Mathf.Sign(direction);

        // Raycast to hit a tile.
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 100f, LayerMask.GetMask("MapTile")))
        {
            // Rotate around the hit point.
            Quaternion oldRotation = transform.rotation;
            Vector3 oldPosition = transform.position;
            transform.RotateAround(hit.point, Vector3.up, angle);

            // Keep the camera looking at the point
            transform.LookAt(hit.point);

            // If the camera is too close to the point, move it back so it doesn't clip into the level
            if(Vector3.Distance(transform.position, hit.point) < 10)
            {
                transform.position += transform.forward * -10;
            }

            // Start a coroutine to rotate the camera instead of snapping it.
            m_desiredRotation = transform.rotation;
            m_desiredPosition = transform.position;
            transform.rotation = oldRotation;
            transform.position = oldPosition;
            StartCoroutine(LerpCameraToRotation());
        }
        else
        {
            // Rotate the camera manually by euler angles and keep it isometric.
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + CameraRotateIncrement * Mathf.Sign(direction),
                transform.rotation.eulerAngles.z);
        }
    }

    /// <summary>
    /// Coroutine to move the camera into place over time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpCameraToRotation()
    {
        m_cameraAnimating = true;

        float timer = 0;

        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;

        while(timer < CameraLerpTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / CameraLerpTime;

            transform.rotation = Quaternion.Lerp(startRotation, m_desiredRotation, ratio);
            transform.position = Vector3.Lerp(startPosition, m_desiredPosition, ratio);

            yield return new WaitForEndOfFrame();
        }

        m_cameraAnimating = false;
    }

    /// <summary>
    /// Scroll to zoom
    /// </summary>
    private void ZoomCamera(float zoomAmount)
    {
        float currentSize = GetComponent<Camera>().orthographicSize;
        currentSize += zoomAmount;

        GetComponent<Camera>().orthographicSize = Mathf.Clamp(currentSize, CameraSizeMin, CameraSizeMax);
    }
}
