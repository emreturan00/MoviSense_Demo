using System;
using System.IO.Ports;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Required for interacting with UI components

public class SerialController : MonoBehaviour
{
    public string portName = "COM5"; // Update with your COM port
    public int baudRate = 38400;
    public GameObject rotatableObject; // Assign in inspector
    public TMP_Text dataText; // Assign in inspector

    private SerialPort serialPort;
    private string receivedData;
    private Quaternion receivedQuaternion;
    private float yaw, pitch, roll;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }
    }
    
    // Yaw Pitch Roll
    /*
    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                receivedData = serialPort.ReadLine();
                ParseData(receivedData);
                UpdateRotation();
                UpdateText();
            }
            catch (System.Exception)
            {
                Debug.Log("Failed to read data from serial port");
            }
        }
    }

    private void ParseData(string data)
    {
        string[] tmp = data.Split(',');
        if (tmp.Length == 3)
        {
            try
            {
                yaw = float.Parse(tmp[0], System.Globalization.CultureInfo.InvariantCulture);
                pitch = float.Parse(tmp[1], System.Globalization.CultureInfo.InvariantCulture);
                roll = float.Parse(tmp[2], System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                Debug.LogError("Received data format is incorrect: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Received data does not have three parts: " + data);
        }
    }*/

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();
                if (data.StartsWith("Q:"))
                {
                    ParseQuaternionData(data.Substring(2));
                    UpdateText(receivedQuaternion);
                    UpdateRotation(receivedQuaternion);
                }
            }
            catch (TimeoutException) // Catch timeout errors separately
            {
                // Timeout errors are normal, just ignore them
            }
            catch (Exception e)
            {
                Debug.Log("Failed to read data from serial port: " + e.Message);
            }
        }
    }

    private void ParseQuaternionData(string data)
    {
        string[] qData = data.Split(',');
        if (qData.Length == 4)
        {
            float w = float.Parse(qData[0], System.Globalization.CultureInfo.InvariantCulture);
            float x = float.Parse(qData[1], System.Globalization.CultureInfo.InvariantCulture); // Inverting X if needed
            float y = -float.Parse(qData[2], System.Globalization.CultureInfo.InvariantCulture);
            float z = -float.Parse(qData[3], System.Globalization.CultureInfo.InvariantCulture); // Inverting Z if needed

            // Adjust for Unity's left-handed coordinate system if necessary
            //receivedQuaternion = new Quaternion(x, y, z, w);

            // If further axis swapping is needed, do so here
            // Example for swapping y and z:
            receivedQuaternion = new Quaternion(y, z, x, w);
        }
    }
    
    
    
    void UpdateText(Quaternion quaternion)
    {
        // Convert quaternion to Euler angles
        Vector3 eulerAngles = quaternion.eulerAngles;

        // Format the text with the quaternion and Euler angles data
        dataText.text = $"Quaternion:\nw: {quaternion.w:F2}, x: {quaternion.x:F2}, y: {quaternion.y:F2}, z: {quaternion.z:F2}\n" +
                        $"Euler Angles (degrees):\nPitch: {eulerAngles.x:F2}, Yaw: {eulerAngles.y:F2}, Roll: {eulerAngles.z:F2}";
    }
    
    void UpdateRotation(Quaternion quaternion)
    {
        // Apply the received quaternion to the rotatable object.
        rotatableObject.transform.localRotation = quaternion;
    }

    
   

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed");
        }
    }
}
