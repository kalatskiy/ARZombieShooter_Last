using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using System.Linq;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private bool m_IsQuitting = false;
    public GameObject _zombie;
    public GameObject _respawnPoint;
    private float _gameStartDelay = 10f;
    public bool _isStarted;
    private DetectedPlane _detectedPlane;
    public Camera FirstPersonCamera;
    private DetectedPlaneGenerator _planeGen;
    private DetectedPlaneVisualizer _planeVis;
    private const float k_ModelRotation = 180.0f;

    public static GameController Instance { get; private set; }


    public void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
    }
    void Start()
    {
        QuitOnConnectionErrors();
        //_planeGen.GetComponent<DetectedPlaneGenerator>();
        _planeGen = GetComponent<DetectedPlaneGenerator>();
        _planeVis = GetComponent<DetectedPlaneVisualizer>();
        //_detectedPlane = GetComponent<DetectedPlane>();

    }
    void Update()
    {

        _UpdateApplicationLifecycle();
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose the Andy model for the Trackable that got hit.
                GameObject prefab;
                if (hit.Trackable is FeaturePoint)
                {
                    prefab = _respawnPoint;
                    _isStarted = true;

                }
                else if (hit.Trackable is DetectedPlane)
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    if (detectedPlane.PlaneType == DetectedPlaneType.Vertical)
                    {
                        prefab = _respawnPoint;
                        _isStarted = true;
                    }
                    else
                    {
                        prefab = _respawnPoint;
                        _isStarted = true;
                    }
                }
                else
                {
                    prefab = _respawnPoint;
                    _isStarted = true;
                }
                // Instantiate Andy model at the hit pose.
                var RespawnPoint = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                RespawnPoint.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make Andy model a child of the anchor.
                RespawnPoint.transform.parent = anchor.transform;
            }
        }
    }
    void QuitOnConnectionErrors()
    {
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>
                ("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
    private void _UpdateApplicationLifecycle()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        if (m_IsQuitting)
        {
            return;
        }
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }
    public void OnApplicationQuit()
    {
        Application.Quit();
    }
}

