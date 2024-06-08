using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using System.IO;
using System;

public class TestingController : MonoBehaviour
{
    public TMP_Text taskDisplayText;
    public Transform gloveTransform;
    public GameObject xzCamera;
    public GameObject yCamera;
    // Position Parameters
    private float[] xPositions = { -50, -30, -10, 10, 30, 50 };
    private float[] yPositions = { -30, -20, -10, 10, 20, 30 };
    private float[] zPositions = { 10, 30, 50 };
    // Rotation Parameters
    private float[] xRotations = { -90, 0, 90 };  // Corrected as per your requirements
    private float[] yRotations = { -30, 0, 30 };  // Added this line for Y-axis rotations
    private float[] zRotations = { -80, 0, 80 };  // Corrected as per your requirements


    private int currentTestIndex = 0;
    private string dataFilePath;
    private int testPhase = 0; // 0-2 for positions, 3 for X-axis rotations, 4 for Y-axis rotations
    private float startTime;

    void Start()
    {
        xzCamera.SetActive(true);
        yCamera.SetActive(false);

        string recorderDirectory = Application.dataPath + "/Recorder";
        Directory.CreateDirectory(recorderDirectory);

        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        dataFilePath = $"{recorderDirectory}/glove_data_{timeStamp}.txt";
        File.WriteAllText(dataFilePath, "MovementAxis;TestType;DesiredValue;HandPosition_X;HandPosition_Y;HandPosition_Z;Rotation_X;Rotation_Y;Rotation_Z;Time\n");

        startTime = Time.time;
        UpdateTaskDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData();
            NextTask();
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CalibrateGlove();
        }*/
    }

    void UpdateTaskDisplay()
    {
        string axis = "Position";
        float currentValue = 0;
        string units = "cm";  // Default unit for position tests

        if (testPhase < 3)
        {  // Position tests
            axis = testPhase == 0 ? "Position X" : testPhase == 1 ? "Position Y" : "Position Z";
            currentValue = (testPhase == 0 ? xPositions[currentTestIndex] :
                (testPhase == 1 ? yPositions[currentTestIndex] : zPositions[currentTestIndex]));
            units = "cm";  // Using centimeters for position
        }
        else
        {  // Rotation tests
            axis = (testPhase == 3) ? "Rotation X" : (testPhase == 4) ? "Rotation Y" : "Rotation Z";
            currentValue = (testPhase == 3 ? xRotations[currentTestIndex] :
                testPhase == 4 ? yRotations[currentTestIndex] :
                zRotations[currentTestIndex]);
            units = "degrees"; // Using degrees for rotation
        }

        taskDisplayText.text = $"Performance Test \n {axis}: {currentValue} {units}";
        UpdateCameras();
    }

    void UpdateCameras()
    {
        yCamera.SetActive(testPhase == 1);
        xzCamera.SetActive(testPhase != 1);
    }

    void SaveData()
    {
        // Create a CultureInfo object for invariant culture
        CultureInfo culture = CultureInfo.InvariantCulture;

        Vector3 actualPosition = gloveTransform.position * 100;  // Convert to cm for position tests
        Vector3 actualRotation = gloveTransform.eulerAngles;  // Rotation already in degrees

        string axis = (testPhase < 3) ? (testPhase == 0 ? "X" : (testPhase == 1 ? "Y" : "Z")) :
            (testPhase == 3 ? "X" : (testPhase == 4 ? "Y" : "Z"));
        string type = (testPhase < 3) ? "Position" : "Rotation";
        float desiredValue = (testPhase < 3) ? (testPhase == 0 ? xPositions[currentTestIndex] :
                (testPhase == 1 ? yPositions[currentTestIndex] : zPositions[currentTestIndex])) :
            (testPhase == 3 ? xRotations[currentTestIndex] :
                testPhase == 4 ? yRotations[currentTestIndex] :
                zRotations[currentTestIndex]);
        float actualValue = (testPhase < 3) ? (testPhase == 0 ? actualPosition.x :
                (testPhase == 1 ? actualPosition.y : actualPosition.z)) :
            (testPhase == 3 ? actualRotation.x : actualRotation.z);  // Use z for rotation

        float elapsedTime = Time.time - startTime;

        string dataLine = string.Format(culture, "{0};{1};{2};{3:0.##};{4:0.##};{5:0.##};{6:0.##};{7:0.##};{8:0.##};{9:0.##}\n",
            axis, type, desiredValue, actualPosition.x, actualPosition.y, actualPosition.z, actualRotation.x, actualRotation.y, actualRotation.z, elapsedTime);

        File.AppendAllText(dataFilePath, dataLine);
    }

    public void ResetPosition()
    {
        gloveTransform.position = Vector3.zero;
        //gloveTransform.rotation = Quaternion.identity;
        Debug.Log("Glove calibrated to origin.");
    }

    public void ResetRotation()
    {
        gloveTransform.rotation = Quaternion.Euler(0, 0, 180);
        //gloveTransform.rotation = Quaternion.identity;
        Debug.Log("Glove calibrated to origin.");
    }

    void NextTask()
    {
        currentTestIndex++;
        if ((testPhase < 3 && currentTestIndex >= (testPhase == 0 ? xPositions.Length :
                (testPhase == 1 ? yPositions.Length : zPositions.Length))) ||
            (testPhase >= 3 && currentTestIndex >= xRotations.Length))
        {
            currentTestIndex = 0;
            testPhase++;
            if (testPhase > 5)  // Now goes up to 5 since we have added Y rotation tests
            {
                Debug.Log("Testing complete!");
                return;
            }
        }
        UpdateTaskDisplay();
    }

}
