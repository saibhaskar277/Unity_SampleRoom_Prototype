using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]Transform target;

    public float rotationSpeed = 150f;
    public float smoothTime = 0.05f;
    public float minPitch = 20f;
    public float maxPitch = 30f;

    private CinemachineVirtualCamera topVcam;
    public float zoomSpeed = 10f;
    public float zoomLerpSpeed = 10f;
    public float minScroll = 60f;
    public float maxScroll = 100f;

    private float yaw;
    private float pitch;

    private CinemachineTransposer transposer;
    private float targetFOV;
    private Coroutine zoomCoroutine;

    [SerializeField] List<VirtualCamera> camList;

    

    private void Awake()
    {
        var camera = camList.Find(x=>x.cam==VirtualCam.TopCam);
        topVcam = camera.vCam;
        if (topVcam != null)
        {
            transposer = topVcam.GetCinemachineComponent<CinemachineTransposer>();
            targetFOV = topVcam.m_Lens.FieldOfView;
        }
    }

    private void Start()
    {
        Manager.Instance.SetCamList(camList);
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * rotationSpeed * Time.deltaTime;
            pitch -= mouseY * rotationSpeed * Time.deltaTime; 
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            target.rotation = Quaternion.Euler(0f, yaw, 0f);
            transposer.m_FollowOffset.y = pitch;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetFOV -= scroll * zoomSpeed;
            targetFOV = Mathf.Clamp(targetFOV, minScroll, maxScroll);

            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(ZoomCoroutine(targetFOV));
        }
    }

    private IEnumerator ZoomCoroutine(float newTargetFOV)
    {
        while (Mathf.Abs(topVcam.m_Lens.FieldOfView - newTargetFOV) > 0.05f)
        {
            topVcam.m_Lens.FieldOfView = Mathf.Lerp(
                topVcam.m_Lens.FieldOfView,
                newTargetFOV,
                Time.deltaTime * zoomLerpSpeed
            );
            yield return null;
        }

        topVcam.m_Lens.FieldOfView = newTargetFOV;
    }
}

[System.Serializable]
public class VirtualCamera
{
    public VirtualCam cam;
    public CinemachineVirtualCamera vCam;
}

public enum VirtualCam
{
    ObjectCam,
    PlayerCam,
    TopCam
}
