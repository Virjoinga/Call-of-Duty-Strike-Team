using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class StaticMeshSeparation
{
    private const string RootFolder = "Assets/StaticMesh";

    private struct StaticBatchInfo
    {
        public int firstSubMesh;
        public int subMeshCount;
    }

    private class Conversion
    {
        public MeshRenderer renderer;
        public MeshFilter filter;
        public MeshCollider collider;
        public Mesh mesh;
        public int lightmapIndex;
        public Vector4 lightmapScaleOffset;
        public int realtimeLightmapIndex;
        public Vector4 realtimeLightmapScaleOffset;
    }

    [MenuItem("Static Mesh Separation/Split Scene")]
    public static void SplitScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (!scene.IsValid() || string.IsNullOrEmpty(scene.path))
        {
            Debug.LogError("Static Mesh Separation: open and save a scene first.");
            return;
        }

        MeshRenderer[] renderers = UnityEngine.Object.FindObjectsOfType<MeshRenderer>();
        Dictionary<Mesh, Mesh[]> cache = new Dictionary<Mesh, Mesh[]>();
        List<Conversion> conversions = new List<Conversion>();
        HashSet<Mesh> sourceMeshes = new HashSet<Mesh>();
        int skipped = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer renderer = renderers[i];
            StaticBatchInfo batch;
            if (!TryGetStaticBatchInfo(renderer, out batch) || batch.subMeshCount <= 0)
                continue;

            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            if (filter == null)
            {
                Debug.LogWarning("Static Mesh Separation: no MeshFilter on " + renderer.name);
                skipped++;
                continue;
            }

            Mesh combinedMesh = filter.sharedMesh;
            if (combinedMesh == null)
            {
                Debug.LogWarning("Static Mesh Separation: no shared mesh on " + renderer.name);
                skipped++;
                continue;
            }

            if (batch.firstSubMesh < 0 || batch.firstSubMesh + batch.subMeshCount > combinedMesh.subMeshCount)
            {
                Debug.LogError("Static Mesh Separation: invalid batch range on " + renderer.name + " (first=" + batch.firstSubMesh + ", count=" + batch.subMeshCount + ", subMeshes=" + combinedMesh.subMeshCount + ").");
                skipped++;
                continue;
            }

            Mesh[] splitMeshes;
            if (!cache.TryGetValue(combinedMesh, out splitMeshes))
            {
                splitMeshes = MeshManager.SplitMesh(combinedMesh);
                cache.Add(combinedMesh, splitMeshes);
                sourceMeshes.Add(combinedMesh);
            }

            int lightmapIndex = renderer.lightmapIndex;
            Vector4 lightmapScaleOffset = renderer.lightmapScaleOffset;
            int realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            Vector4 realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;

            Mesh output;
            if (batch.subMeshCount == 1)
                output = MeshManager.CopyMesh(splitMeshes[batch.firstSubMesh]);
            else
            {
                Mesh[] parts = new Mesh[batch.subMeshCount];
                for (int j = 0; j < parts.Length; j++)
                    parts[j] = splitMeshes[batch.firstSubMesh + j];
                output = MeshManager.CombineMesh(parts);
            }

            if (lightmapIndex >= 0 && !IsIdentity(lightmapScaleOffset))
                MeshManager.UndoLightmapScaleOffset(output, lightmapScaleOffset);

            if (realtimeLightmapIndex >= 0 && !IsIdentity(realtimeLightmapScaleOffset))
                MeshManager.UndoLightmapScaleOffset(output, realtimeLightmapScaleOffset);

            MeshManager.TransformWorldSpaceDataToLocal(output, renderer.transform);
            output.name = MakeSafeName(renderer.name + "_Separated");
            output.RecalculateBounds();

            if (output.normals == null || output.normals.Length != output.vertexCount)
                output.RecalculateNormals();

            MeshCollider collider = renderer.GetComponent<MeshCollider>();
            if (collider != null && collider.sharedMesh != combinedMesh)
                collider = null;

            conversions.Add(new Conversion
            {
                renderer = renderer,
                filter = filter,
                collider = collider,
                mesh = output,
                lightmapIndex = lightmapIndex,
                lightmapScaleOffset = lightmapScaleOffset,
                realtimeLightmapIndex = realtimeLightmapIndex,
                realtimeLightmapScaleOffset = realtimeLightmapScaleOffset
            });
        }

        if (conversions.Count == 0)
        {
            Debug.LogError("Static Mesh Separation: no static-batched MeshRenderers were found.");
            return;
        }

        string sceneFolder = RootFolder + "/" + MakeSafeName(scene.name);
        EnsureDirectory(RootFolder);
        EnsureDirectory(sceneFolder);

        try
        {
            AssetDatabase.StartAssetEditing();
            for (int i = 0; i < conversions.Count; i++)
            {
                string path = AssetDatabase.GenerateUniqueAssetPath(sceneFolder + "/" + conversions[i].mesh.name + ".asset");
                AssetDatabase.CreateAsset(conversions[i].mesh, path);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Static Mesh Separation: asset creation failed. The scene was not modified.");
            return;
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        for (int i = 0; i < conversions.Count; i++)
        {
            Conversion c = conversions[i];
            GameObject go = c.filter.gameObject;

            Undo.RecordObject(c.filter, "Separate Static Mesh");
            c.filter.sharedMesh = c.mesh;
            EditorUtility.SetDirty(c.filter);

            if (c.collider != null)
            {
                Undo.RecordObject(c.collider, "Separate Static Mesh Collider");
                c.collider.sharedMesh = c.mesh;
                EditorUtility.SetDirty(c.collider);
            }

            Undo.RecordObject(go, "Clear Static Batching Flag");
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(go);
            flags &= ~StaticEditorFlags.BatchingStatic;
            GameObjectUtility.SetStaticEditorFlags(go, flags);
            EditorUtility.SetDirty(go);

            Undo.RecordObject(c.renderer, "Restore Lightmap Data");
            c.renderer.lightmapIndex = c.lightmapIndex;
            c.renderer.lightmapScaleOffset = c.lightmapScaleOffset;
            c.renderer.realtimeLightmapIndex = c.realtimeLightmapIndex;
            c.renderer.realtimeLightmapScaleOffset = c.realtimeLightmapScaleOffset;
            EditorUtility.SetDirty(c.renderer);

            ClearStaticBatchInfo(c.renderer);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log("Static Mesh Separation complete. Converted " + conversions.Count + " renderers, skipped " + skipped + ", using " + sourceMeshes.Count + " combined meshes.");
    }

    private static bool TryGetStaticBatchInfo(MeshRenderer renderer, out StaticBatchInfo result)
    {
        result = new StaticBatchInfo();
        SerializedObject so = new SerializedObject(renderer);
        SerializedProperty info = so.FindProperty("m_StaticBatchInfo");
        if (info == null)
            return false;

        SerializedProperty first = info.FindPropertyRelative("firstSubMesh");
        SerializedProperty count = info.FindPropertyRelative("subMeshCount");
        if (first == null || count == null)
            return false;

        result.firstSubMesh = first.intValue;
        result.subMeshCount = count.intValue;
        return true;
    }

    private static void ClearStaticBatchInfo(MeshRenderer renderer)
    {
        try
        {
            MethodInfo method = typeof(Renderer).GetMethod("SetStaticBatchInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(renderer, new object[] { 0, 0 });
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Static Mesh Separation: SetStaticBatchInfo failed on " + renderer.name + ": " + ex.Message);
        }

        SerializedObject so = new SerializedObject(renderer);
        SerializedProperty info = so.FindProperty("m_StaticBatchInfo");
        if (info == null)
            return;

        SerializedProperty first = info.FindPropertyRelative("firstSubMesh");
        SerializedProperty count = info.FindPropertyRelative("subMeshCount");
        if (first != null) first.intValue = 0;
        if (count != null) count.intValue = 0;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(renderer);
    }

    private static bool IsIdentity(Vector4 value)
    {
        return Mathf.Abs(value.x - 1f) < 0.000001f && Mathf.Abs(value.y - 1f) < 0.000001f && Mathf.Abs(value.z) < 0.000001f && Mathf.Abs(value.w) < 0.000001f;
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    private static string MakeSafeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "StaticMesh";

        char[] invalid = Path.GetInvalidFileNameChars();
        for (int i = 0; i < invalid.Length; i++)
            name = name.Replace(invalid[i], '_');
        return name;
    }
}

public static class MeshManager
{
    public static Mesh CopyMesh(Mesh source)
    {
        Mesh mesh = new Mesh();
        mesh.name = source.name;
        mesh.indexFormat = source.indexFormat;
        CopyVertexStreams(source, mesh, 0, source.vertexCount);
        mesh.subMeshCount = source.subMeshCount;

        for (int i = 0; i < source.subMeshCount; i++)
            mesh.SetTriangles(source.GetTriangles(i, true), i, false);

        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh[] SplitMesh(Mesh input)
    {
        Mesh[] result = new Mesh[input.subMeshCount];

        for (int subMeshIndex = 0; subMeshIndex < input.subMeshCount; subMeshIndex++)
        {
            int[] triangles = input.GetTriangles(subMeshIndex, true);
            Mesh mesh = new Mesh();
            mesh.name = input.name + "_SubMesh_" + subMeshIndex;
            mesh.indexFormat = input.indexFormat;

            if (triangles.Length == 0)
            {
                mesh.vertices = new Vector3[0];
                mesh.subMeshCount = 1;
                mesh.SetTriangles(new int[0], 0, false);
                result[subMeshIndex] = mesh;
                continue;
            }

            int baseVertex = triangles[0];
            int maxVertex = triangles[0];
            
            for (int i = 1; i < triangles.Length; i++)
            {
                if (triangles[i] < baseVertex) baseVertex = triangles[i];
                if (triangles[i] > maxVertex) maxVertex = triangles[i];
            }
            
            int vertexCount = maxVertex - baseVertex + 1;
            for (int i = 0; i < triangles.Length; i++)
                triangles[i] -= baseVertex;

            CopyVertexStreams(input, mesh, baseVertex, vertexCount);
            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles, 0, false);
            mesh.RecalculateBounds();
            result[subMeshIndex] = mesh;
        }

        return result;
    }

    public static Mesh CombineMesh(Mesh[] subMeshes)
    {
        Mesh mesh = new Mesh();
        if (subMeshes == null || subMeshes.Length == 0)
            return mesh;

        int totalVertexCount = 0;
        bool hasNormals = true;
        bool hasTangents = true;
        bool hasColors = true;
        bool hasUv = true;
        bool hasUv2 = true;
        bool hasUv3 = true;
        bool hasUv4 = true;

        for (int i = 0; i < subMeshes.Length; i++)
        {
            Mesh subMesh = subMeshes[i];
            totalVertexCount += subMesh.vertexCount;
            hasNormals &= subMesh.normals != null && subMesh.normals.Length == subMesh.vertexCount;
            hasTangents &= subMesh.tangents != null && subMesh.tangents.Length == subMesh.vertexCount;
            hasColors &= subMesh.colors32 != null && subMesh.colors32.Length == subMesh.vertexCount;
            hasUv &= subMesh.uv != null && subMesh.uv.Length == subMesh.vertexCount;
            hasUv2 &= subMesh.uv2 != null && subMesh.uv2.Length == subMesh.vertexCount;
            hasUv3 &= subMesh.uv3 != null && subMesh.uv3.Length == subMesh.vertexCount;
            hasUv4 &= subMesh.uv4 != null && subMesh.uv4.Length == subMesh.vertexCount;
        }

        if (totalVertexCount > 65535)
            mesh.indexFormat = IndexFormat.UInt32;

        List<Vector3> vertices = new List<Vector3>(totalVertexCount);
        List<Vector3> normals = hasNormals ? new List<Vector3>(totalVertexCount) : null;
        List<Vector4> tangents = hasTangents ? new List<Vector4>(totalVertexCount) : null;
        List<Color32> colors = hasColors ? new List<Color32>(totalVertexCount) : null;
        List<Vector2> uv = hasUv ? new List<Vector2>(totalVertexCount) : null;
        List<Vector2> uv2 = hasUv2 ? new List<Vector2>(totalVertexCount) : null;
        List<Vector2> uv3 = hasUv3 ? new List<Vector2>(totalVertexCount) : null;
        List<Vector2> uv4 = hasUv4 ? new List<Vector2>(totalVertexCount) : null;

        for (int i = 0; i < subMeshes.Length; i++)
        {
            Mesh subMesh = subMeshes[i];
            vertices.AddRange(subMesh.vertices);
            if (hasNormals) normals.AddRange(subMesh.normals);
            if (hasTangents) tangents.AddRange(subMesh.tangents);
            if (hasColors) colors.AddRange(subMesh.colors32);
            if (hasUv) uv.AddRange(subMesh.uv);
            if (hasUv2) uv2.AddRange(subMesh.uv2);
            if (hasUv3) uv3.AddRange(subMesh.uv3);
            if (hasUv4) uv4.AddRange(subMesh.uv4);
        }

        mesh.vertices = vertices.ToArray();
        if (hasNormals) mesh.normals = normals.ToArray();
        if (hasTangents) mesh.tangents = tangents.ToArray();
        if (hasColors) mesh.colors32 = colors.ToArray();
        if (hasUv) mesh.uv = uv.ToArray();
        if (hasUv2) mesh.uv2 = uv2.ToArray();
        if (hasUv3) mesh.uv3 = uv3.ToArray();
        if (hasUv4) mesh.uv4 = uv4.ToArray();

        mesh.subMeshCount = subMeshes.Length;
        int vertexOffset = 0;
        for (int i = 0; i < subMeshes.Length; i++)
        {
            int[] triangles = subMeshes[i].GetTriangles(0, true);
            for (int j = 0; j < triangles.Length; j++)
                triangles[j] += vertexOffset;
            mesh.SetTriangles(triangles, i, false);
            vertexOffset += subMeshes[i].vertexCount;
        }

        mesh.RecalculateBounds();
        return mesh;
    }

    public static void UndoLightmapScaleOffset(Mesh mesh, Vector4 scaleOffset)
    {
        Vector2[] uv2 = mesh.uv2;
        if (uv2 == null || uv2.Length != mesh.vertexCount)
            return;

        if (Mathf.Abs(scaleOffset.x) < 0.000001f || Mathf.Abs(scaleOffset.y) < 0.000001f)
        {
            Debug.LogWarning("Static Mesh Separation: cannot undo lightmap scale/offset " + scaleOffset);
            return;
        }

        if (IsIdentity(scaleOffset))
            return;

        for (int i = 0; i < uv2.Length; i++)
            uv2[i] = new Vector2((uv2[i].x - scaleOffset.z) / scaleOffset.x, (uv2[i].y - scaleOffset.w) / scaleOffset.y);

        mesh.uv2 = uv2;
    }

    public static void TransformWorldSpaceDataToLocal(Mesh mesh, Transform target)
    {
        Matrix4x4 worldToLocal = target.worldToLocalMatrix;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = worldToLocal.MultiplyPoint3x4(vertices[i]);
        mesh.vertices = vertices;

        Vector3[] normals = mesh.normals;
        if (normals != null && normals.Length == mesh.vertexCount)
        {
            Matrix4x4 normalMatrix = worldToLocal.transpose;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = normalMatrix.MultiplyVector(normals[i]);
                if (normals[i].sqrMagnitude > 0.0000001f)
                    normals[i].Normalize();
            }
            mesh.normals = normals;
        }

        Vector4[] tangents = mesh.tangents;
        if (tangents != null && tangents.Length == mesh.vertexCount)
        {
            bool mirrored = worldToLocal.determinant < 0f;
            for (int i = 0; i < tangents.Length; i++)
            {
                Vector4 tangent = tangents[i];
                Vector3 direction = worldToLocal.MultiplyVector(new Vector3(tangent.x, tangent.y, tangent.z));
                if (direction.sqrMagnitude > 0.0000001f)
                    direction.Normalize();
                tangents[i] = new Vector4(direction.x, direction.y, direction.z, mirrored ? -tangent.w : tangent.w);
            }
            mesh.tangents = tangents;
        }
    }

    private static void CopyVertexStreams(Mesh source, Mesh destination, int start, int count)
    {
        destination.vertices = Slice(source.vertices, start, count);
        if (source.normals != null && source.normals.Length == source.vertexCount) destination.normals = Slice(source.normals, start, count);
        if (source.tangents != null && source.tangents.Length == source.vertexCount) destination.tangents = Slice(source.tangents, start, count);
        if (source.colors32 != null && source.colors32.Length == source.vertexCount) destination.colors32 = Slice(source.colors32, start, count);
        if (source.uv != null && source.uv.Length == source.vertexCount) destination.uv = Slice(source.uv, start, count);
        if (source.uv2 != null && source.uv2.Length == source.vertexCount) destination.uv2 = Slice(source.uv2, start, count);
        if (source.uv3 != null && source.uv3.Length == source.vertexCount) destination.uv3 = Slice(source.uv3, start, count);
        if (source.uv4 != null && source.uv4.Length == source.vertexCount) destination.uv4 = Slice(source.uv4, start, count);
    }

    private static T[] Slice<T>(T[] source, int start, int count)
    {
        T[] result = new T[count];
        Array.Copy(source, start, result, 0, count);
        return result;
    }

    private static bool IsIdentity(Vector4 value)
    {
        return Mathf.Abs(value.x - 1f) < 0.000001f && Mathf.Abs(value.y - 1f) < 0.000001f && Mathf.Abs(value.z) < 0.000001f && Mathf.Abs(value.w) < 0.000001f;
    }
}
