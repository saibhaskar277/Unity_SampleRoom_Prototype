using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectMaterialMapper", menuName = "Game Data/ObjectMaterialMapper")]
public class ObjectMaterialMapper : ScriptableObject
{
    public List<ObjectMaterial> materials;

    public ObjectMaterial GetObjectMaterial(int itemID)
    {
        var material = materials.Find(x=>x.ItemID==itemID); 
        return material;
    }
}

[System.Serializable]
public class ObjectMaterial
{
    public int ItemID;
    public List<Material>material;   
}