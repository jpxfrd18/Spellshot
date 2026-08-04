using UnityEngine;
using UnityEditor;

public class RockMaterialRandomizer
{
    [MenuItem("Tools/Rocks/Randomize Rock Materials")]
    static void RandomizeRockMaterials()
    {
        // Load your materials (drag them into Resources or use AssetDatabase)
        Material materialA = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/grey.mat");
        Material materialB = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Dark Grey.mat");

        if (materialA == null || materialB == null)
        {
            Debug.LogError("Materials not found. Check your paths.");
            return;
        }

        // Find all rocks by tag
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

        int countA = 0;
        int countB = 0;

        foreach (GameObject rock in rocks)
        {
            MeshRenderer mesh = rock.GetComponentInChildren<MeshRenderer>();
            if (mesh == null) continue;

            bool useA = Random.value > 0.5f;
            mesh.sharedMaterial = useA ? materialA : materialB;

            if (useA) countA++; else countB++;
        }

        Debug.Log($"Rock materials randomized. A:{countA} B:{countB}");
    }
}
