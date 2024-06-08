using UnityEngine;

public class VirtualDial : MonoBehaviour
{
    public Transform gloveTransform; // Assign the glove's transform in the inspector
    private bool isGrabbed = false;

    void Update()
    {
        if (isGrabbed)
        {
            RotateDial(gloveTransform.position);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glove"))
        {
            isGrabbed = true;
            Debug.Log("Dial Grabbed");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Glove"))
        {
            isGrabbed = false;
            Debug.Log("Dial Released");
        }
    }

    private void RotateDial(Vector3 glovePosition)
    {
        // Calculate the angle using the position of the glove relative to the dial
        Vector3 direction = glovePosition - transform.position;
        direction = transform.InverseTransformDirection(direction); // Convert to local space
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Apply the rotation
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}