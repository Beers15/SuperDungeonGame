using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Camera follow variables.
    Transform target_transform;
    public Vector3 offset = new Vector3(2f, -2f, 2f);
    public float pitch = 2f;
    Vector3 lastoffset;
    bool isTopDownView = false;
	Camera camera;

    // Zoom variables.
    public static float currentZoom = 9f;
    public float zoomSpeed = 4f;
    public float minZoom = 1f;
    public static float maxZoom = 50f;

    public void SetTarget(GameObject target)
    {
		//Debug.Log("set target");
		camera = GetComponent<Camera>();
        target_transform = target.transform;
    }

    void Update()
    {
        // Mousewheel event; zooms in and out.
        // Get the mousewheel axis and calculate and set zoon. Zoom is clamped
        // between min and max zoom.
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 'P' char event; toggles top down view.
        // Toggles the x and z coodinates to change between normal and top-down view.
        if (Input.GetKeyDown("p"))
        {
            if (!isTopDownView)
            {
                lastoffset = offset;
                offset.Set(0f, offset.y, 0f);
                isTopDownView = true;
            }
            else
            {
                offset = lastoffset;
                isTopDownView = false;
            }
        }
    }

    void LateUpdate()
    {
        // Basic camera follow; updates the camera transform a certain position
        // away from the player.
		if (target_transform == null) {
			Debug.Log("No target!");
			return;
		}
		
		Vector3 target_pos = new Vector3(target_transform.position.x, 0, target_transform.position.z);
		if (target_transform != null && !camera.orthographic) {
			transform.position = target_pos - offset * currentZoom;
			transform.LookAt(target_pos + Vector3.up * pitch);
		}
		else if (camera.orthographic) {
			transform.position = target_pos - offset * 10;
			transform.LookAt(target_pos + Vector3.up * pitch);
			camera.orthographicSize = currentZoom;
		}
    }
}
