
using UnityEditor;
using UnityEngine;

public static class LightProbeCheck
{
    [MenuItem("Tools/Check Legacy LightProbes")]
    private static void Check()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        LightProbes probes = AssetDatabase.LoadAssetAtPath<LightProbes>(path);

        if (probes == null)
        {
            Debug.LogWarning("Could not load LightProbes asset: " + path);
            return;
        }

        Debug.Log("Imported Light Probe Count: " + probes.count);
    }
}
