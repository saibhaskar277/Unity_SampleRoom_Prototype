using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    [SerializeField] CameraManager cameraManager;
    [SerializeField] ObjectManager objectManager;
    [SerializeField]ObjectMaterialMapper objectMaterialMapper;
    List<VirtualCamera> cameras = new List<VirtualCamera>();

    public static event Action<VirtualCam> OnStateChanged;

    public VirtualCam CurrentState { get; private set; } = VirtualCam.PlayerCam;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetCamList(List<VirtualCamera> cams)
    {
        cameras = cams;
    }
    Object currentObj;
    public void ItemClicked(Object item)
    {
        currentObj = item;
    }

    public void SetItemMaterial(int index)
    {
        objectManager.SetMaterialTarget(currentObj,index);
    }

    public void SetVirtualCam(VirtualCam cam, Transform taregt=null)
    {
        foreach (var item in cameras)
        {
            item.vCam.Priority = 0;
        }

        var currentCam = cameras.Find(x => x.cam == cam);
        switch (cam)
        {
            case VirtualCam.PlayerCam:
                currentCam.vCam.Priority = 1;
                break;
            case VirtualCam.TopCam:
                currentCam.vCam.Priority = 1;
                break;
            case VirtualCam.ObjectCam:
                currentCam.vCam.Priority = 1;
                currentCam.vCam.Follow = taregt;
                currentCam.vCam.LookAt = taregt;
                break;
        }

        SetCameraState(cam);

    }

    void SetCameraState(VirtualCam state)
    {
        if (CurrentState == state)
            return;

        CurrentState = state;
        OnStateChanged?.Invoke(CurrentState);
    }
}



