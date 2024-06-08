using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandPointer : MonoBehaviour
{
    public Transform hand;  // The hand or wrist transform
    public Camera uiCamera; // A camera to render the UI, if necessary
    public Transform indexRotation;  // The IK target for the index finger
    public Transform middleRotation;  // The IK target for the middle finger
    public LineRenderer lineRenderer; // LineRenderer to visualize the raycast
    public GraphicRaycaster graphicRaycaster;  // The GraphicRaycaster component attached to the Canvas

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;

        if (graphicRaycaster == null)
        {
            graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        }
    }

    void Update()
    {
        // Create a ray from the hand's position
        Ray ray = new Ray(hand.position, hand.forward);
        RaycastHit hit;

        // Set the start position of the line renderer
        lineRenderer.SetPosition(0, hand.position);

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Set the end position of the line renderer
            lineRenderer.SetPosition(1, hit.point);

            // Check if the hit object has a UI component
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name);
                // Check if the hit object is a UI element
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = uiCamera.WorldToScreenPoint(hit.point);

                // Create a list to hold the results of the raycast
                List<RaycastResult> results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    // Check if the result is a UI button
                    Debug.Log(result.gameObject+ "66f");
                    Button button = result.gameObject.GetComponent<Button>();
                    if (button != null)
                    {
                        Debug.Log("ButtonClicked");
                        button.onClick.Invoke();
                    }
                }
            }
        }
        else
        {
            // If the raycast doesn't hit anything, set the end position far away
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
        }
    }

    bool IsHandClosed()
    {
        // Check if the x-rotation of both IK targets is greater than 160 degrees
        // When both index and middle finger is closed. Button Will be clicked.
        return indexRotation.localEulerAngles.x > 160f && middleRotation.localEulerAngles.x > 160f;
    }
}
