using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Button topViewButton;
    [SerializeField] Button playerViewButton;
    [SerializeField] Button nextMaterialButton;
    [SerializeField] Button previousMaterialButton;
    [SerializeField] GameObject miniMap;
    [SerializeField] GameObject materialSet;
    List<Material>currentMaterials;
    
    private void Start()
    {
        playerViewButton.onClick.AddListener(() =>
        {
            Manager.Instance.SetVirtualCam(VirtualCam.PlayerCam);
        });

        topViewButton.onClick.AddListener(() => {

            Manager.Instance.SetVirtualCam(VirtualCam.TopCam);
        });

        nextMaterialButton.onClick.AddListener(() => { SetMaterial(1); });
        previousMaterialButton.onClick.AddListener(() => { SetMaterial(-1); });
    }

    public void SetMaterial(int index)
    {
        Manager.Instance.SetItemMaterial(index);
    }

    private void OnEnable()
    {
        Manager.OnStateChanged += OnStateChangedListner;
    }

    private void OnStateChangedListner(VirtualCam cam)
    {
        materialSet.SetActive(cam == VirtualCam.ObjectCam);
        miniMap.SetActive(cam == VirtualCam.PlayerCam);
    }

    private void OnDisable()
    {
        Manager.OnStateChanged -= OnStateChangedListner;
    }
}
