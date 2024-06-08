using System;
using System.IO;
using System.Globalization;
using UnityEngine;

public class RotFlexController : MonoBehaviour
{
    private string inoData;

    public GameObject _hand; // Assign in inspector

    private Quaternion receivedQuaternion;
    private float[] receivedFlexData = new float[3]; // [thumb, index, middle]

    public Transform _thumbTarget;
    public Transform _indexTarget;
    public Transform _middleTarget;
    public Transform _ringTarget;
    public Transform _pinkyTarget;
    
    float thumb;
    float index;
    float middle;
    
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
        inoData = inoDataController.Instance.receivedData;

        if (!string.IsNullOrEmpty(inoData))
        {
            ParseInoData(inoData);
            UpdateRotation();
            UpdateFingerRotation(receivedFlexData);
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

        if (qData.Length >= 7)
        {
            try
            {
                float w = float.Parse(qData[0], System.Globalization.CultureInfo.InvariantCulture);
                float x = float.Parse(qData[1], System.Globalization.CultureInfo.InvariantCulture); // Inverting X if needed
                float y = -float.Parse(qData[2], System.Globalization.CultureInfo.InvariantCulture);
                float z = -float.Parse(qData[3], System.Globalization.CultureInfo.InvariantCulture); // Inverting Z if needed

                thumb = float.Parse(qData[4], System.Globalization.CultureInfo.InvariantCulture);
                index = float.Parse(qData[5], System.Globalization.CultureInfo.InvariantCulture);
                middle = float.Parse(qData[6], System.Globalization.CultureInfo.InvariantCulture);

                // Adjust for Unity's left-handed coordinate system if necessary
                receivedQuaternion = new Quaternion(x, -y, -z, w);
                receivedFlexData[0] = Map(thumb, 550, 800, -25, 35);
                receivedFlexData[1] = Map(index, 550, 800, 120, 220);
                receivedFlexData[2] = Map(middle, 550, 800, 120, 220);
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

    

    void UpdateRotation()
    {
        Quaternion correctedQuaternion = GetCorrectedQuaternion(receivedQuaternion);
        _hand.transform.rotation = correctedQuaternion;
        
    }

    void UpdateFingerRotation(float[] ReceivedData)
    {
        // Assuming ReceivedData contains mapped flex sensor values
        // Adjust these values to the specific needs of your application

        // Create new rotations from scratch for clarity
        Quaternion thumbRotation = Quaternion.Euler(0, 0, ReceivedData[0]);
        Quaternion indexRotation = Quaternion.Euler(ReceivedData[1], 0, 0);
        Quaternion middleRotation = Quaternion.Euler(ReceivedData[2], 0, 0);
        Quaternion ringRotation = Quaternion.Euler((ReceivedData[1]+ ReceivedData[2])/2, 0, 0);
        Quaternion pinkyRotation = Quaternion.Euler((ReceivedData[1]+ReceivedData[2])/2, 0, 0);

        // Apply the quaternion rotation to the finger GameObject
        _thumbTarget.localRotation = thumbRotation;
        _indexTarget.rotation = indexRotation;
        _middleTarget.localRotation = middleRotation;

        if (middleRotation.eulerAngles.x > 175 && indexRotation.eulerAngles.x > 175)
        {
            _pinkyTarget.localRotation = pinkyRotation;
            _ringTarget.localRotation = ringRotation;
        }
        else
        {
            _pinkyTarget.localEulerAngles = new Vector3(120, 0, 0);
            _ringTarget.localEulerAngles = new Vector3(120, 0, 0);
        }

    }

  

    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
    }

    
}