using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{

    private const float CameraPanSpeed = 3f;
    private const float CameraRotateIncrement = 90f;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCamera(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCamera(1);
        }
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
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + CameraRotateIncrement * Mathf.Sign(direction),
            transform.rotation.eulerAngles.z);
    }
}
