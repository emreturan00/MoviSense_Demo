using System;
using System.Globalization;
using UnityEngine;

public class MPU6050Handler : MonoBehaviour
{
    private string inoData; // Data received from the external source
    public GameObject _hand; // Assign the GameObject representing the hand in the inspector

    private Quaternion receivedQuaternion; // To store the received quaternion data

    [Header("Accelerometer Offset")]
    public float _x = -70f;
    public float _y = 165;
    public float _z = 180;
    
    // Initialize the offset based on the default quaternion values measured when the hand is in the default orientation
    Quaternion initialOrientation ; // Unity uses Euler angles in degrees

    void Start()
    {
        initialOrientation = Quaternion.Euler(_x, _y, _z); // Unity uses Euler angles in degrees
        // Invert the initialOffset to apply it as a correction factor
        // You need to set initialOffset based on a known good orientation when your application starts
        initialOrientation = Quaternion.Inverse(initialOrientation);
    }

    void Update()
    {
        inoData = inoDataController.Instance.receivedData; // Replace this with your actual data fetching logic

        if (!string.IsNullOrEmpty(inoData))
        {
            ParseInoData(inoData);
            ApplyRotation();
        }

    }

    Quaternion GetCorrectedQuaternion(Quaternion sensorQuaternion)
    {
        // Apply the initial offset correction to the quaternion from the sensor
        return initialOrientation * sensorQuaternion;
    }

    private void ParseInoData(string data)
    {
        string[] qData = data.Split(',');

        if (qData.Length >= 4) // Ensure we have at least 4 parts to construct a Quaternion
        {
            try
            {
                float w = float.Parse(qData[0], CultureInfo.InvariantCulture);
                float x = float.Parse(qData[1], CultureInfo.InvariantCulture);
                float y = -float.Parse(qData[2], CultureInfo.InvariantCulture); // Inverting Y if needed based on your sensor alignment
                float z = -float.Parse(qData[3], CultureInfo.InvariantCulture); // Inverting Z if needed based on your sensor alignment

                receivedQuaternion = new Quaternion(x, -y, -z, w);
            }
            catch (FormatException e)
            {
                Debug.LogError("Received inoData format is incorrect: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Received inoData does not have enough parts: " + data);
        }
    }

    private void ApplyRotation()
    {
        Quaternion correctedQuaternion = GetCorrectedQuaternion(receivedQuaternion);
        _hand.transform.rotation = correctedQuaternion;
    }
}
