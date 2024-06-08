using System;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public float maxRotationAngle = 150f; // Maximum rotation angle in degrees

    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        //uaternion targetRotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        //kTarget.rotation = Quaternion.RotateTowards(ikTarget.rotation, targetRotation, maxRotationAngle);

    }
}

[System.Serializable]
public class VRMapFeet
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;
    public VRMapFeet leftFoot;
    public VRMapFeet rightFoot;
    //public VRMap spine;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;
    
    // Body Position
    public Transform _leftFootPosition;
    public Transform _rightFootPosition;
    private Rigidbody rb;

    //public Transform _spineRotation;
    
    private Vector3 middlePoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 leftFootPosition = _leftFootPosition.position;
        Vector3 rightFootPosition = _rightFootPosition.position;
    }

    void FixedUpdate()
    {
        // Centering the body between right and left feet.

        middlePoint = (_leftFootPosition.position + _rightFootPosition.position) / 2f;

        middlePoint.y = transform.position.y; // Set y position to original y position
        //transform.position = Vector3.Lerp(transform.position,middlePoint,0.5f);      
        float yaw = head.vrTarget.eulerAngles.y;

        // Calculating the velocity required to reach the target position in the specified duration.
        Vector3 velocity = (middlePoint - transform.position) / 0.15f;

        // Applying the velocity to the rigidbody.
        rb.velocity = velocity;
        
        //_spineRotation.rotation = Quaternion.Lerp(_spineRotation.rotation,Quaternion.Euler(_spineRotation.eulerAngles.x, yaw, _spineRotation.eulerAngles.z),turnSmoothness);

//
        //// Set the forward direction of the body to the normalized direction vector.
        //transform.forward = feetDirection.normalized;
//
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = new Vector3(transform.position.x,head.ikTarget.position.y + headBodyPositionOffset.y,transform.position.z);
        //transform.position = head.ikTarget.position + headBodyPositionOffset;

        float yaw = head.vrTarget.eulerAngles.y;
        // Calculate the direction vector between the feet.

        // Apply the rotation to the body.
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);
        
        //_spineRotation.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
        leftFoot.Map();
        rightFoot.Map();
        //spine.Map();
    }
}
