using UnityEngine;

public class HandGrab : MonoBehaviour
{
    public Transform hand;  // The hand or wrist transform
    private GameObject heldObject = null;
    public Transform _indexFinger;
    public Transform _middleFinger;
    //public Transform _thumbFinger;
    

    void Update()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = new Vector3(hand.position.x,hand.position.y-0.1f,hand.position.z-0.1f);  // Update the position to the hand's position
            //heldObject.transform.position = hand.position;  // Update the position to the hand's position

            heldObject.transform.rotation = hand.rotation;  // Update the rotation to the hand's rotation
            
            if (_indexFinger.localEulerAngles.x < 150f || _middleFinger.localEulerAngles.x < 150f) // Use Space key to release the object
            {
                ReleaseObject();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (heldObject == null && other.CompareTag("Grabbable") && IsHandClosed())
        {
            GrabObject(other.gameObject);
            Debug.Log("Grabbed");
        }
    }

    void GrabObject(GameObject objectToGrab)
    {
        heldObject = objectToGrab;
        heldObject.GetComponent<Rigidbody>().isKinematic = true;  // Make the object kinematic while holding
    }

    void ReleaseObject()
    {
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false;  // Make the object non-kinematic when released
            heldObject = null;
        }
    }

    bool IsHandClosed()
    {
        // Add your logic to determine if the hand is closed based on flex sensor data
        // For demonstration, return true to simulate grabbing
        return _indexFinger.localEulerAngles.x > 160f && _middleFinger.localEulerAngles.x > 160f;
    }
}