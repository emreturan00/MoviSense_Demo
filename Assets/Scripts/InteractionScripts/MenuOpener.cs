using UnityEngine;

public class MenuOpener : MonoBehaviour, IInteractable
{
    public GameObject menu; // Assign this in the Unity Editor to the menu you want to toggle

    public void Interact()
    {
        // Toggle the visibility of a menu or perform other actions
        menu.SetActive(!menu.activeSelf);
        Debug.Log("Menu toggled");
    }
}