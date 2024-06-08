    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionController : MonoBehaviour
{
    private string pyData;
    private Vector3 newPosition;
    public GameObject ReferencedHand; // Assign in inspector



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pyData = inoDataController.Instance.receivedString;
        if (!string.IsNullOrEmpty(pyData))
        {
            ParsePyData(pyData);
            UpdatePosition(newPosition);
        }

    }

    void ParsePyData(string pyData)
    {
        // Parse received data (assuming it's in the format "x,y,z")
        string[] parts = pyData.Split(',');
        if (parts.Length == 3)
        {
            try
            {
                float x = Map(float.Parse(parts[2]), 10, 60, -0.5f, 0.5f);
                float y = Map(float.Parse(parts[1]), 0, 450, -0.5f, 0.5f);
                float z = Map(float.Parse(parts[0]), 0, 650, -0.3f, -0.3f);

                // Update object position based on received data
                newPosition = new Vector3(x, -y, -z);
                Debug.Log("Cube position updated: " + newPosition);
            }
            catch (FormatException e)
            {
                Debug.LogError("Received pyData format is incorrect: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Received pyData does not have enough parts: " + pyData);
        }
    }

    void UpdatePosition(Vector3 position)
    {
        ReferencedHand.transform.localPosition = position;
    }

    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
    }
}