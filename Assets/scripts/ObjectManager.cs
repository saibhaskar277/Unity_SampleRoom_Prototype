using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectManager : MonoBehaviour
{
    [SerializeField] ObjectMaterialMapper materialMapper;
    Object currentObj;
    List<Material> currentMaterials;
    int currentMaterialIndex;

    private void OnEnable()
    {
        Manager.OnStateChanged += OnStateChangedListner;
    }

    private void OnStateChangedListner(VirtualCam cam)
    {
        if (cam==VirtualCam.ObjectCam)
        {
            currentMaterialIndex = 0;
        }
    }

    public void SetMaterialTarget(Object currentItem,int index)
    {
        currentObj = currentItem;
        currentMaterialIndex += index;
        var objMaterial = materialMapper.GetObjectMaterial(currentObj.itemID);


        if (currentMaterialIndex<0)
        {
            currentMaterialIndex=objMaterial.material.Count-1;
        }
        if (currentMaterialIndex>=objMaterial.material.Count)
        {
            currentMaterialIndex = 0;
        }

        ApplyMaterial(objMaterial);
    }

    void ApplyMaterial(ObjectMaterial objMaterial)
    {
        currentObj.ApplyMaterial(objMaterial.material[currentMaterialIndex]);
    }

    private void OnDisable()
    {
        Manager.OnStateChanged -= OnStateChangedListner;
    }  
}