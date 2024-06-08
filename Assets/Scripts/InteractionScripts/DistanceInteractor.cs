using UnityEngine;

public class DistanceInteractor : MonoBehaviour
{
    public Transform fingertip; // Assign the fingertip transform in the Unity Editor
    public float interactDistance = 5f; // Maximum distance to interact with objects
    public LayerMask interactableLayer; // Layer mask to filter which objects can be interacted with

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Simulate pinch gesture using key P
        {
            RaycastInteract();
        }
    }

    void RaycastInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(fingertip.position, fingertip.forward, out hit, interactDistance, interactableLayer))
        {
            Debug.Log("Hit " + hit.collider.name);
            // Perform interaction
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
        else
        {
            Debug.Log("No interactable object in range");
        }
    }
}