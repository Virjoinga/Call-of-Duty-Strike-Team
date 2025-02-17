using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class UnstaticObject : EditorWindow
{
    [MenuItem("AntiRetardation/Menu")]
    static void GetMe()
    {
        GetWindow<UnstaticObject>();
    }

    void OnGUI()
    {
        GameObject[] realList = FindObjectsOfType<GameObject>();



        GUILayout.Label("GameObjects: " + realList.Length);

        if (GUILayout.Button("AntiRetard Meshes"))
        {
            int count = 0;

            foreach (GameObject go in realList)
            {
                if (go.isStatic && (go.GetComponent(typeof(SpriteRoot)) != null || go.GetComponent(typeof(SpriteText)) != null))
                {
                    count++;
                    go.isStatic = false;
                }
            }

            EditorUtility.DisplayDialog("E", "Unretarded " + count.ToString() + " Meshes With SpriteRoot", "ok");
            EditorSceneManager.MarkAllScenesDirty();
        }
    }

    List<GameObject> GetChilds(GameObject obj)
    {
        List<GameObject> toReturn = new List<GameObject>();

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            toReturn.Add(obj);
            toReturn.AddRange(GetChilds(obj.transform.GetChild(i).gameObject));
        }

        return toReturn;
    }
}
