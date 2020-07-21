using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AddMeshColliders : EditorWindow
{
    [MenuItem("GameObject/Add MeshColliders To All Children", false, 48)]
    static void AddColliders(MenuCommand command)
    {
        Transform[] parent = Selection.transforms;
        List<MeshRenderer> meshes = new List<MeshRenderer>();

        for(int i = 0; i < parent.Length; i++)
        {
            //if (parent[i].TryGetComponent(out MeshRenderer r)) meshes.Add(r); Unnecessary, GetInChildren includes this
            List<MeshRenderer> results = new List<MeshRenderer>();
            parent[i].GetComponentsInChildren(true, results);
            meshes.AddRange(results);
        }

        foreach(MeshRenderer renderer in meshes)
        {
            if(!renderer.GetComponent<MeshCollider>())
                Undo.AddComponent<MeshCollider>(renderer.gameObject);
        }
    }

    [MenuItem("GameObject/Add MeshColliders To All Children", true, 48)]
    static bool ValidateObject(MenuCommand command)
    {
        return Selection.activeObject is GameObject;
    }
}