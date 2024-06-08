using UnityEngine;
using System.IO;
using System.Globalization;
using System;
using TMPro;

public class TransformationRecorder : MonoBehaviour
{
    public Transform objectToTrack;
    public TMP_Text tmp_text;

    private float[] xPositions = { -50, -25, -10, -5, 0, 5, 10, 25, 50 };
    private float[] yPositions = { -30, -15, -5, 0, 5, 15, 30 };
    private float[] zPositions = { -10, -5, 0, 5, 10 };
    private float[] xRotations = { -80, 0, 80 };  // Adjusted to specified values
    private float[] zRotations = { -15, 0, 15 };  // Adjusted to specified values

    private int currentPositionIndex = 0;
    private int currentTestPhase = 0;  // 0: X pos, 1: Y pos, 2: Z pos, 3: X rot, 4: Z rot
    private bool isRecording = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private string dataFilePath;

    void Start()
    {
        SetupDataFile();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isRecording)
            {
                StartRecording();
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (isRecording)
            {
                StopRecording();
                PrepareNextTask();
            }
        }
    }

    void StartRecording()
    {
        startPosition = objectToTrack.position;
        startRotation = objectToTrack.rotation;
        isRecording = true;
        UpdateTaskDisplay("Recording... Press 'B' to stop.");
    }

    void StopRecording()
    {
        Vector3 endPosition = objectToTrack.position;
        Quaternion endRotation = objectToTrack.rotation;
        isRecording = false;
        WriteData("End", endPosition, endRotation);
        CalculateAndWriteDifference(startPosition, startRotation, endPosition, endRotation);
    }

    void PrepareNextTask()
    {
        currentPositionIndex++;
        float[] currentArray = GetCurrentArray();

        if (currentPositionIndex >= currentArray.Length)
        {
            currentPositionIndex = 0;
            currentTestPhase++;
            if (currentTestPhase > 4) currentTestPhase = 0;  // Loop back to the first phase or end testing
        }

        string message = "Ready for next recording. Press 'A' to start.";
        UpdateTaskDisplay(message);
    }

    float[] GetCurrentArray()
    {
        switch (currentTestPhase)
        {
            case 0: return xPositions;
            case 1: return yPositions;
            case 2: return zPositions;
            case 3: return xRotations;
            case 4: return zRotations;
            default: return xPositions;
        }
    }

    string GetCurrentTaskDescription()
    {
        string type = (currentTestPhase < 3) ? "Position" : "Rotation";
        string axis = (currentTestPhase == 0) ? "X" :
                      (currentTestPhase == 1) ? "Y" :
                      (currentTestPhase == 2) ? "Z" :
                      (currentTestPhase == 3) ? "X" : "Z";
        return $"{type} - {axis} Axis";
    }

    void SetupDataFile()
    {
        string recorderDirectory = Application.dataPath + "/TransformationData";
        Directory.CreateDirectory(recorderDirectory);
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        dataFilePath = $"{recorderDirectory}/transformation_data_{timeStamp}.txt";
        File.WriteAllText(dataFilePath, "Timestamp;Event;PosX;PosY;PosZ;RotX;RotY;RotZ;RotW\n");
        UpdateTaskDisplay("Press 'A' to start recording the first task.");
    }

    void UpdateTaskDisplay(string message)
    {
        if (tmp_text != null)
        {
            string taskValue = GetCurrentTaskValue();  // Get the current task value
            tmp_text.text = $"{message} Current task: {GetCurrentTaskDescription()} at {taskValue} units.";
        }
        Debug.Log(message);
    }
    
    string GetCurrentTaskValue()
    {
        float[] currentArray = GetCurrentArray();
        if (currentPositionIndex < currentArray.Length)
        {
            return currentArray[currentPositionIndex].ToString(CultureInfo.InvariantCulture);
        }
        return "No Task";
    }
    
    void WriteData(string eventDescription, Vector3 position, Quaternion rotation)
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("yyyy-MM-dd HH:mm:ss.fff", culture);  // More precise timestamp

        Vector3 positionDifference = position - startPosition;
        float positionMagnitude = positionDifference.magnitude;  // Magnitude of position change
        float rotationMagnitude = Quaternion.Angle(startRotation, rotation);  // Magnitude of rotation change

        string dataLine = string.Format(culture, "{0};{1};{2:0.##};{3:0.##};{4:0.##};{5:0.##};{6:0.##};{7:0.##};{8:0.##};{9:0.##};{10:0.##}\n",
            formattedTime, eventDescription, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w,
            positionMagnitude, rotationMagnitude);

        File.AppendAllText(dataFilePath, dataLine);
    }

    void CalculateAndWriteDifference(Vector3 start, Quaternion startRot, Vector3 end, Quaternion endRot)
    {
        Vector3 positionDifference = end - start;
        Quaternion rotationDifference = endRot * Quaternion.Inverse(startRot);
        float positionMagnitude = positionDifference.magnitude;
        float rotationMagnitude = Quaternion.Angle(startRot, endRot);

        string summaryLine = $"Summary;Difference;{positionDifference.x:0.##};{positionDifference.y:0.##};{positionDifference.z:0.##};{rotationDifference.eulerAngles.x:0.##};{rotationDifference.eulerAngles.y:0.##};{rotationDifference.eulerAngles.z:0.##};{positionMagnitude:0.##};{rotationMagnitude:0.##}\n";
        File.AppendAllText(dataFilePath, summaryLine);
    }
}
