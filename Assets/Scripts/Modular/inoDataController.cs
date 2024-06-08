using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class inoDataController : MonoBehaviour
{
    private static inoDataController instance = null;
    public static inoDataController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<inoDataController>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("inoDataController");
                    instance = obj.AddComponent<inoDataController>();
                }
            }
            return instance;
        }
    }

    public string receivedData { get; set; }
    public string receivedString { get; set; }

    public int port = 12345;  // Port to listen on
    UdpClient udpClient;

    public string portName = "COM6"; // Update with your COM port
    public int baudRate = 38400;

    private SerialPort serialPort;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        udpClient = new UdpClient(port);
        udpClient.BeginReceive(ReceiveCallback, null);

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

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                receivedData = serialPort.ReadLine();
                Debug.Log("Received from serial port: " + receivedData);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read data from serial port: " + e.Message);
            }
        }

        Debug.Log("Serial Data: " + receivedData + " UDP Data: " + receivedString);
    }

    void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            IPEndPoint source = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedBytes = udpClient.EndReceive(result, ref source);
            receivedString = Encoding.ASCII.GetString(receivedBytes);
            Debug.Log("Received from UDP: " + receivedString);

            // Listen for the next message
            udpClient.BeginReceive(ReceiveCallback, null);
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
        }
    }
}