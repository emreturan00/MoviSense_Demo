using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Threading;

public class SaveController : MonoBehaviour
{
    public Transform handTransform;  // Attach the reference hand's transform component here in Unity Editor
    public Transform thumbTransform; // Attach the thumb's transform component here in Unity Editor
    public Transform indexTransform; // Attach the index finger's transform component here in Unity Editor
    public Transform middleTransform; // Attach the middle finger's transform component here in Unity Editor

    private string inoData;
    private string pyData;
    private StreamWriter writer;
    private float startTime;
    private string filePath;
    private string directoryPath = "Collected Data";

    // Start is called before the first frame update
    void Start()
    {
        
        // Set the current culture to US English
        CultureInfo ci = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = ci;
        Thread.CurrentThread.CurrentUICulture = ci;
        
        // Ensure directory exists
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Format file name with current date and time
        string formattedTime = DateTime.Now.ToString("MM-dd_HH-mm");
        filePath = Path.Combine(directoryPath, $"MoviSense_DATA_{formattedTime}.txt");

        // Initialize StreamWriter
        writer = new StreamWriter(filePath, true);

        // Write header if the file is new
        if (new FileInfo(filePath).Length == 0)
        {
            writer.WriteLine("SceneName;GyroQuaternion_W;GyroQuaternion_X;GyroQuaternion_Y;GyroQuaternion_Z;Flex Thumb;Flex Index;Flex Middle;HandPoz_X;HandPoz_Y;HandPoz_Z;HandRot_X;HandRot_Y;HandRot_Z;ThumbRot_Z;IndexRot_X;MiddleRot_X;Hand Condition;Time;Timestamp");
            writer.Flush();
        }

        // Initialize start time
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        pyData = inoDataController.Instance.receivedString;  // Expected to get "1.00,0.00,0.00,0.00,670,622,599"
        inoData = inoDataController.Instance.receivedData;    // This line could be redundant, adjust based on your actual data flow
        string allData = CombineSensorData();
        SaveData(allData);
    }

    private string CombineSensorData()
    {
        float elapsedTime = (Time.time - startTime);
        float timestamp = Time.time;
        string handCondition = DetermineHandCondition(); // Implement based on your criteria

        // Splitting inoData for processing
        string[] splitData = inoData.Split(',');
        string gyroData = string.Join(";", splitData, 0, 4); // Gyro data separated by semicolon
        string flexData = string.Join(";", splitData, 4, 3); // Flex data separated by semicolon

        // Position and rotation in string format
        string position = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", handTransform.position.x, handTransform.position.y, handTransform.position.z);
        string rotation = string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", handTransform.eulerAngles.x, handTransform.eulerAngles.y, handTransform.eulerAngles.z);
        string thumbRotation = thumbTransform.localEulerAngles.z.ToString(CultureInfo.InvariantCulture);
        string indexRotation = indexTransform.localEulerAngles.x.ToString(CultureInfo.InvariantCulture);
        string middleRotation = middleTransform.localEulerAngles.x.ToString(CultureInfo.InvariantCulture);
        startTime = Time.time;
        // Combine all data
        return $"SceneName;{gyroData};{flexData};{position};{rotation};{thumbRotation};{indexRotation};{middleRotation};{handCondition};{elapsedTime};{timestamp}";
    }

    private string DetermineHandCondition()
    {
        // Placeholder, implement your logic to determine the condition of the hand
        return "Normal";
    }

    private void SaveData(string allData)
    {
        writer.WriteLine(allData);
        writer.Flush();
    }

    void OnApplicationQuit()
    {
        if (writer != null)
        {
            writer.Close();
        }
    }

    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
        }
    }
}
