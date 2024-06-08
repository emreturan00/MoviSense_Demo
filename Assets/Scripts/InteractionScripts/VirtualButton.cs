using UnityEngine;

public class VirtualButton : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glove")) // Make sure your glove object has the tag "Glove"
        {
            Debug.Log("Button Pressed");
            // Add functionality for what happens when the button is pressed
            // For example, turning on a light or starting a machine
        }
    }
}