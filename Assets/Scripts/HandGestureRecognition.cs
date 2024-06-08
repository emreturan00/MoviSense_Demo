using UnityEngine;

public class HandGestureRecognition : MonoBehaviour
{
    public Transform hand;
    public Transform _thumbTarget;
    public Transform _indexTarget;
    public Transform _middleTarget;
    
    void Update()
    {
        if (IsFistGesture())
        {
           Debug.Log("Fist Made!!!!! DANGEROUS");
        }
        else if (IsPointingGesture())
        {
            Debug.Log("Pointing Gesture Recognized");
            Debug.Log("POINTING A GUNNNN!!!!!!");
        }

       // _head.rotation = _camera.transform.rotation;
    }

    bool IsFistGesture()
    {
            return (_thumbTarget.localEulerAngles.z> 0 && _indexTarget.localEulerAngles.x> 160 && _middleTarget.localEulerAngles.x > 160 );
    }

    bool IsPointingGesture()
    {
        return  (_thumbTarget.localEulerAngles.z> -20 && _indexTarget.localEulerAngles.x< 130 && _middleTarget.localEulerAngles.x > 160 );
    }
}