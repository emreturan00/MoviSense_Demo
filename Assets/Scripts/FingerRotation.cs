using System;
using System.IO;
using System.IO.Ports;
using System.Globalization;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class FingerRotation : MonoBehaviour
{
    public int port = 12345;  // Port to listen on
    UdpClient udpClient;
    public float sensitivity = 100;

    public string portName = "COM5"; // Update with your COM port
    public int baudRate = 38400;

    private string inoData;

    private SerialPort serialPort;
    private string receivedData;
    private Quaternion receivedQuaternion;
    private float[] receivedFlexData = new float[3]; // [thumb, index, middle]

    public GameObject thumb2;
    public GameObject thumb3;
    public GameObject index1;
    public GameObject index2;
    public GameObject index3;
    public GameObject middle1;
    public GameObject middle2;
    public GameObject middle3;
    
    public GameObject ring1;
    public GameObject ring2;
    public GameObject ring3;
    
    public GameObject pinky1;
    public GameObject pinky2;
    public GameObject pinky3;

    float thumb;
    float index;
    float middle;


    void Start()
    {
        
    }

    void Update()
    {
        inoData = inoDataController.Instance.receivedData;

        if (!string.IsNullOrEmpty(inoData))
        {
            ParseInoData(inoData);
            UpdateFingerRotation(receivedFlexData);
        }
    }

    private void ParseInoData(string data)
    {
        string[] qData = data.Split(',');

        if (qData.Length >= 7)
        {
            try
            {
                
                thumb = float.Parse(qData[4], System.Globalization.CultureInfo.InvariantCulture);
                index = float.Parse(qData[5], System.Globalization.CultureInfo.InvariantCulture);
                middle = float.Parse(qData[6], System.Globalization.CultureInfo.InvariantCulture);

                receivedFlexData[0] = Map(thumb, 550, 800, 0, 80);
                receivedFlexData[1] = Map(index, 585, 770, 10, 90);
                receivedFlexData[2] = Map(middle, 585, 760, 0, 80);
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
    

    void UpdateFingerRotation(float[] ReceivedData)
    {
        Quaternion thumbcurrentRotation2 = thumb2.transform.localRotation;
        Quaternion thumbcurrentRotation3 = thumb3.transform.localRotation;

        Quaternion indexcurrentRotation1 = index1.transform.localRotation;
        Quaternion indexcurrentRotation2 = index2.transform.localRotation;
        Quaternion indexcurrentRotation3 = index3.transform.localRotation;

        Quaternion middlecurrentRotation1 = middle1.transform.localRotation;
        Quaternion middlecurrentRotation2 = middle2.transform.localRotation;
        Quaternion middlecurrentRotation3 = middle3.transform.localRotation;
        
        Quaternion ringCurrentRotation1 = middle1.transform.localRotation;
        Quaternion ringCurrentRotation2 = middle2.transform.localRotation;
        Quaternion ringCurrentRotation3 = middle3.transform.localRotation;
        
        Quaternion pinkyCurrentRotation1 = middle1.transform.localRotation;
        Quaternion pinkyCurrentRotation2 = middle2.transform.localRotation;
        Quaternion pinkyCurrentRotation3 = middle3.transform.localRotation;

        Quaternion thumb2rotation = Quaternion.Euler(thumbcurrentRotation2.eulerAngles.x, thumbcurrentRotation2.eulerAngles.y, ReceivedData[0]);
        Quaternion thumb3rotation = Quaternion.Euler(thumbcurrentRotation3.eulerAngles.x, thumbcurrentRotation3.eulerAngles.y, ReceivedData[0]);

        Quaternion index1rotation = Quaternion.Euler(ReceivedData[1], indexcurrentRotation1.eulerAngles.y, indexcurrentRotation1.eulerAngles.z);
        Quaternion index2rotation = Quaternion.Euler(ReceivedData[1], indexcurrentRotation2.eulerAngles.y, indexcurrentRotation2.eulerAngles.z);
        Quaternion index3rotation = Quaternion.Euler(ReceivedData[1], indexcurrentRotation3.eulerAngles.y, indexcurrentRotation3.eulerAngles.z);

        Quaternion middle1rotation = Quaternion.Euler(ReceivedData[2], middlecurrentRotation1.eulerAngles.y, middlecurrentRotation1.eulerAngles.z);
        Quaternion middle2rotation = Quaternion.Euler(ReceivedData[2], middlecurrentRotation2.eulerAngles.y, middlecurrentRotation2.eulerAngles.z);
        Quaternion middle3rotation = Quaternion.Euler(ReceivedData[2], middlecurrentRotation3.eulerAngles.y, middlecurrentRotation3.eulerAngles.z);
        
        Quaternion ring1rotation = Quaternion.Euler( (ReceivedData[1]+ReceivedData[2])/2.1f, middlecurrentRotation1.eulerAngles.y, middlecurrentRotation1.eulerAngles.z);
        Quaternion ring2rotation = Quaternion.Euler((ReceivedData[1]+ReceivedData[2])/2.1f, middlecurrentRotation2.eulerAngles.y, middlecurrentRotation2.eulerAngles.z);
        Quaternion ring3rotation = Quaternion.Euler((ReceivedData[1]+ReceivedData[2])/2.1f, middlecurrentRotation3.eulerAngles.y, middlecurrentRotation3.eulerAngles.z);
        
        Quaternion pinky1rotation = Quaternion.Euler(ReceivedData[2]/1.5f, middlecurrentRotation1.eulerAngles.y, middlecurrentRotation1.eulerAngles.z);
        Quaternion pinky2rotation = Quaternion.Euler(ReceivedData[2]/1.5f, middlecurrentRotation2.eulerAngles.y, middlecurrentRotation2.eulerAngles.z);
        Quaternion pinky3rotation = Quaternion.Euler(ReceivedData[2]/1.5f, middlecurrentRotation3.eulerAngles.y, middlecurrentRotation3.eulerAngles.z);

        
        // Apply the quaternion rotation to the finger GameObject
        thumb2.transform.localRotation = thumb2rotation;
        thumb3.transform.localRotation = thumb3rotation;

        index1.transform.localRotation = index1rotation;
        index2.transform.localRotation = index2rotation;
        index3.transform.localRotation = index3rotation;

        middle1.transform.localRotation = middle1rotation;
        middle2.transform.localRotation = middle2rotation;
        middle3.transform.localRotation = middle3rotation;

        pinky1.transform.localRotation = ring1rotation;
        pinky2.transform.localRotation = ring2rotation;
        pinky3.transform.localRotation = ring3rotation;
        
        ring1.transform.localRotation = ring1rotation;
        ring2.transform.localRotation = ring2rotation;
        ring3.transform.localRotation = ring3rotation;
    }

    float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
    }
    
    
}