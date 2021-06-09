using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;

    public Transform target;

    public float smoothSpeed = 0.125f;

    public Vector3 position = new Vector3(3, 3, 3);

    [Range(1.0f, 5.0f)]
    public float zoom = 1.5f;

    [Range(1, 179)]
    public int fieldOfView = 60;

    public enum SmoothAlgorithm
    {
        SmoothDamp,
        Lerp
    }

    public SmoothAlgorithm smoothAlgorithm = SmoothAlgorithm.SmoothDamp;

    public float maxZoom = 5.0f;

    public float minZoom = 1.0f;

    public float zoomSensitivity = 3.0f;

    [Range(0, 360)]
    public float orbitRotation = 180;

    public void Update()
    {
        Camera cam = GetComponent<Camera>();
        cam.fieldOfView = fieldOfView;

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            changeZoom(Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * -1);
        }

        Vector3 desiredPosition = target.position + position * zoom;

        // Calculate with orientation again
        desiredPosition = Quaternion.Euler(0, orbitRotation, 0) * (desiredPosition - target.position) + target.position;

        switch (smoothAlgorithm)
        {
            case SmoothAlgorithm.SmoothDamp:
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
                break;
            case SmoothAlgorithm.Lerp:
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                break;
        }

        transform.LookAt(target);
    }

    public void changeZoom(float changeValue)
    {
        zoom = Mathf.Clamp(zoom + changeValue, minZoom, maxZoom);
    }

    public void changeRotation(float changeValue)
    {
        orbitRotation += changeValue;
    }

}
