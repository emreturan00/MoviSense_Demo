using UnityEngine;

[System.Serializable]
public class VRMap1
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void Map()
    {

        ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class IKTargetFollowVRRigDriver : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap1 head;
    public VRMap1 leftHand;
    public VRMap1 rightHand;
    //public VRMap leftFoot;
    //public VRMap rightFoot;

    //public Transform spineIKTarget;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        //transform.position = new Vector3(transform.position.x,head.ikTarget.position.y + headBodyPositionOffset.y,transform.position.z);
        //transform.position = head.ikTarget.position + headBodyPositionOffset;
        

        //float yaw = head.vrTarget.eulerAngles.y;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z),turnSmoothness);
        
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
