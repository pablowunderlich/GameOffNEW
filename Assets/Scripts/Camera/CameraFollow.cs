using ScriptableObjectArchitecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3Reference cameraOffset;
    public FloatReference smoothTime;
    private Vector3 velocity = Vector3.zero;
    public Camera sceneCamera;
    public FloatReference cameraOrthoSize;
    // Start is called before the first frame update

    [SerializeField] private Transform target;
    private void Awake()
    {
        sceneCamera = GetComponent<Camera>();
        

        Vector3 targetPosition = target.position + cameraOffset.Value;
        transform.position = targetPosition;
        
    }
    // Update is called once per frame
    void Update()
    {
        sceneCamera.orthographicSize = cameraOrthoSize.Value;
        Vector3 targetPosition = target.position + cameraOffset.Value;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime.Value);
    }
}
