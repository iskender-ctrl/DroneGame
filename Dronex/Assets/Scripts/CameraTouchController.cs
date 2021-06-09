using UnityEngine;

public class CameraTouchController : MonoBehaviour
{
    private CameraFollowController cameraFollowController;

    void Start()
    {
        cameraFollowController = GetComponent<CameraFollowController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            cameraFollowController.changeZoom(difference * 0.01f * -1);

            cameraFollowController.changeRotation(Vector3.SignedAngle(touchOne.position - touchZero.position, touchOnePrevPos - touchZeroPrevPos, Vector3.forward) * -1);
        }
    }
}
