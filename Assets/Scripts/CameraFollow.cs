using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // The target object the camera should look at
    public float smoothSpeed = 0.125f;  // Smoothing speed for the camera movement
    public Vector3 offset;  // Offset position from the target object

    void LateUpdate()
    {
        //Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = smoothedPosition;

        // Always look at the target
        transform.LookAt(target);
    }
}