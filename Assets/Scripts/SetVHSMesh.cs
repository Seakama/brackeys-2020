using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVHSMesh : MonoBehaviour
{

    public List<Mesh> VHSMeshes = new List<Mesh>();

    void Start()
    {
        MeshFilter VHSMesh = GetComponentInChildren<MeshFilter>();
        VHSMesh.mesh = VHSMeshes[Mathf.RoundToInt(Random.Range(0, VHSMeshes.Count - 1))];
    }
}
