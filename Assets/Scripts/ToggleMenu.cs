using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class ToggleMenu : MonoBehaviour
{
    public GameObject _demoButtons; // Assign this in the inspector

    void Start()
    {
        Button btn = this.GetComponent<Button>(); // Get the Button component
        btn.onClick.AddListener(ToggleObject); // Add a listener for when the button is clicked
    }

    void ToggleObject()
    {
        // Toggle the active state of the GameObject
        if (_demoButtons != null)
        {
            _demoButtons.SetActive(!_demoButtons.activeSelf);
        }
    }
}