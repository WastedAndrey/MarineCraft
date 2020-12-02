using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelColor : MonoBehaviour
{
    [SerializeField]
    GameObject rendererObj;
    [SerializeField]
    List<Material> editedMaterials = new List<Material>();

    public void SetColor(Color color)
    {
        var materials = rendererObj.GetComponent<Renderer>().materials;
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < editedMaterials.Count; j++)
            {
                if (materials[i].GetColor("_Color") == editedMaterials[j].GetColor("_Color"))
                {
                    materials[i].SetColor("_Color", color);
                }
            }
        }
    }
}
