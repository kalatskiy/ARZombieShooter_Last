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
    [SerializeField]
    private GameObject _respawnPoint;

    private bool _isStarted;
    private DetectedPlane _detectedPlane;
    [SerializeField]
    private Camera FirstPersonCamera;
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
        _planeGen = GetComponent<DetectedPlaneGenerator>();
        _planeVis = GetComponent<DetectedPlaneVisualizer>();
    }
    void Update()
    {
        _UpdateApplicationLifecycle();
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }        
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
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
                var RespawnPoint = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                RespawnPoint.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

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
}

