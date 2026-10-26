using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public static class AttachSceneNavMesh
{
    [MenuItem("Tools/Navigation/Attach Selected NavMesh To Scene")]
    public static void AttachSelected()
    {
        NavMeshData data = Selection.activeObject as NavMeshData;
        if (data == null)
        {
            Debug.LogError("Select the recovered NavMeshData asset in the Project window first.");
            return;
        }
        Attach(data);
    }

    public static void Attach(NavMeshData data)
    {
        if (Application.isPlaying || EditorSceneManager.sceneCount != 1)
            throw new InvalidOperationException("Exit Play mode and open only the target scene before assigning its NavMesh.");
        if (data == null || string.IsNullOrEmpty(AssetDatabase.GetAssetPath(data)))
            throw new ArgumentException("Use a saved NavMeshData asset.");

        Scene scene = SceneManager.GetActiveScene();
        // Unity 2017 exposes the scene settings through this editor-only API.
        PropertyInfo property = typeof(UnityEditor.AI.NavMeshBuilder).GetProperty("navMeshSettingsObject", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (property == null) throw new InvalidOperationException("This Unity version does not expose the scene NavMesh settings object.");
        UnityEngine.Object target = property.GetValue(null, null) as UnityEngine.Object;
        if (target == null) throw new InvalidOperationException("No active scene NavMesh settings object.");
        SerializedObject settings = new SerializedObject(target);
        SerializedProperty reference = settings.FindProperty("m_NavMeshData");
        if (reference == null) throw new InvalidOperationException("The scene has no m_NavMeshData property.");

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Attach Scene NavMesh");
        Undo.RecordObject(target, "Attach Scene NavMesh");
        reference.objectReferenceValue = data;
        settings.ApplyModifiedProperties();

        int removed = 0;
        foreach (MonoBehaviour loader in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
        {
            if (loader == null || loader.GetType().FullName != "RecoveredNavMeshLoader" || loader.gameObject.scene != scene) continue;
            GameObject holder = loader.gameObject;
            // Preserve objects that also hold other components or children.
            if (holder.GetComponents<Component>().Length == 2 && holder.transform.childCount == 0)
                Undo.DestroyObjectImmediate(holder);
            else
                Undo.DestroyObjectImmediate(loader);
            removed++;
        }
        EditorSceneManager.MarkSceneDirty(scene);
        Undo.CollapseUndoOperations(group);
        Debug.Log("Assigned " + AssetDatabase.GetAssetPath(data) + " directly to scene " + scene.name + "; removed " + removed + " recovery loaders. Save and reopen the scene to verify native loading. Do not bake.", data);
    }
}
