using UnityEngine;
using TMPro;

public class RotateObject : MonoBehaviour
{
    public Transform objectToRotate;
    public Transform rotationPivot;
    public TMP_Text rotationText;
    public float rotationStep = 10.0f;

    // Function to increase the x-axis rotation
    public void IncreaseRotation()
    {
        RotatingObject(rotationStep);
    }

    // Function to decrease the x-axis rotation
    public void DecreaseRotation()
    {
        RotatingObject(-rotationStep);
    }

    // Helper function to apply rotation and update text
    private void RotatingObject(float angle)
    {
        objectToRotate.Rotate(0, 0, angle, Space.World);
        UpdateRotationText();
    }

    // Update the rotation amount in TMP_Text
    private void UpdateRotationText()
    {
        rotationText.text = "Rotation: " + rotationPivot.rotation.x.ToString("F2") + "°";
    }
}