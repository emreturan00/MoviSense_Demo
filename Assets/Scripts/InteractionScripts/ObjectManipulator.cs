using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    private bool isHoldingObject = false;
    public Transform holdPoint; // Assign a point where objects will be held
    private GameObject currentObject = null;

    void Update()
    {
        if (isHoldingObject && Input.GetKeyUp(KeyCode.D)) // Release object
        {
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            currentObject.transform.parent = null;
            isHoldingObject = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isHoldingObject && Input.GetKeyDown(KeyCode.A) && other.gameObject.CompareTag("Pickup")) // Pickup object
        {
            currentObject = other.gameObject;
            currentObject.GetComponent<Rigidbody>().isKinematic = true;
            currentObject.transform.position = holdPoint.position;
            currentObject.transform.parent = transform;
            isHoldingObject = true;
        }
    }
}