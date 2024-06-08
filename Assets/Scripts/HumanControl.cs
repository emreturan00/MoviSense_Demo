using UnityEngine;

public class HumanControl : MonoBehaviour
{
    public Transform leftLegIKTarget;
    public Transform rightLegIKTarget;
    public Transform rightArmIKTarget;
    public Transform characterIK;

    void Start()
    {
        // Set initial positions and rotations for each IK target
        // Values are taken from the screenshots you provided

        // Left Leg IK Target
        leftLegIKTarget.localPosition = new Vector3(-0.09f, 0.35f, 1f);
        leftLegIKTarget.localRotation = Quaternion.Euler(-60, -180, 0);

        // Right Leg IK Target
        rightLegIKTarget.localPosition = new Vector3(0.09f, 0.35f, 1f);
        rightLegIKTarget.localRotation = Quaternion.Euler(-60, -180, 0);

        // Right Arm IK Target
        rightArmIKTarget.localPosition = new Vector3(0.25f, 1f, 0.175f);
        rightArmIKTarget.localRotation = Quaternion.Euler(90, -100, -120);

        // Character IK (main object)
        characterIK.position = new Vector3(0f, 0.9f, 0.05f);
        characterIK.rotation = Quaternion.identity; // Assuming no initial rotation

        // Log to console that the positions and rotations have been set
        Debug.Log("Initial IK targets and character positions/rotations set.");
    }

    void Update()
    {
        // Here you can add any real-time adjustments or controls if needed
    }
}