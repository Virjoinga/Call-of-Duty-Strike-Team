using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// Runs without baking or changing scenes. Each asset is removed before the next.
public static class NavMeshRecoveryDiagnostics
{
    [Serializable]
    public class Result
    {
        public string asset;
        public string error;
        public int triangles;
        public int[] tested = new int[4];
        public int[] found = new int[4];
        public int[] first64Tested = new int[4];
        public int[] first64Found = new int[4];
        public string[] withinTilePaths = new string[4];
        public List<string> crossTilePaths = new List<string>();
        public int adjacencyTested;
        public int adjacencyComplete;
        public int portalTested;
        public int portalComplete;
        public List<string> adjacencyFailures = new List<string>();
    }

    [Serializable]
    public class Probe
    {
        public Vector3 start;
        public Vector3 end;
        public bool crossTile;
        public string label;
    }

    [Serializable]
    public class Probes
    {
        public Probe[] probes;
    }

    [Serializable]
    public class Report
    {
        public string unityVersion;
        public List<Result> results = new List<Result>();
    }

    [MenuItem("Tools/Navigation/Run Recovery Grid Diagnostics")]
    public static void Run()
    {
        if (Application.unityVersion != "2017.4.40f1")
            throw new InvalidOperationException("Run recovery diagnostics with Unity 2017.4.40f1.");
        if (NavMesh.CalculateTriangulation().indices.Length != 0)
            throw new InvalidOperationException("Open an empty scene with no loaded NavMeshes before running diagnostics.");

        string[] paths = AssetDatabase.FindAssets("t:NavMeshData", new string[] { "Assets/NavMeshRecoveryDiagnostics" });
        if (paths.Length == 0)
            throw new InvalidOperationException("No diagnostic assets in Assets/NavMeshRecoveryDiagnostics.");
        Array.Sort(paths, delegate(string a, string b) { return string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)); });
        Report report = new Report();
        report.unityVersion = Application.unityVersion;
        foreach (string guid in paths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Result result = Test(path);
            report.results.Add(result);
            Debug.Log(Path.GetFileName(path) + ": triangles=" + result.triangles + ", found=[" + string.Join(",", Array.ConvertAll(result.found, delegate(int n) { return n.ToString(); })) + "] / tested=[" + string.Join(",", Array.ConvertAll(result.tested, delegate(int n) { return n.ToString(); })) + "] " + result.error);
        }
        string output = Path.GetFullPath("NavMeshRecoveryResults.json");
        File.WriteAllText(output, JsonUtility.ToJson(report, true));
        Debug.Log("Recovery diagnostics saved to " + output);
    }

    private static string PathResult(Vector3 start, Vector3 end)
    {
        NavMeshPath path = new NavMeshPath();
        bool calculated = NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path);
        return "calculated=" + calculated + ", status=" + path.status + ", corners=" + path.corners.Length;
    }

    private static Result Test(string path)
    {
        Result result = new Result();
        result.asset = path;
        NavMeshData data = AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
        if (data == null)
        {
            result.error = "Could not load NavMeshData";
            return result;
        }
        NavMeshDataInstance instance = NavMesh.AddNavMeshData(data);
        try
        {
            if (!instance.valid)
                throw new InvalidOperationException("AddNavMeshData rejected asset");
            NavMeshTriangulation mesh = NavMesh.CalculateTriangulation();
            result.triangles = mesh.indices.Length / 3;
            Vector3[] first = new Vector3[4];
            Vector3[] farthest = new Vector3[4];
            float[] distances = new float[4];
            for (int i = 0; i < result.triangles; i++)
            {
                Vector3 center = (mesh.vertices[mesh.indices[i * 3]] + mesh.vertices[mesh.indices[i * 3 + 1]] + mesh.vertices[mesh.indices[i * 3 + 2]]) / 3f;
                int tile = (center.x >= -17f ? 1 : 0) + (center.z >= 28.254961f ? 2 : 0);
                result.tested[tile]++;
                bool first64 = result.first64Tested[tile] < 64;
                if (first64) result.first64Tested[tile]++;
                NavMeshHit hit;
                if (!NavMesh.SamplePosition(center, out hit, 2f, NavMesh.AllAreas)) continue;
                result.found[tile]++;
                if (first64) result.first64Found[tile]++;
                if (result.found[tile] == 1) first[tile] = hit.position;
                float distance = Vector3.Distance(first[tile], hit.position);
                if (distance > distances[tile])
                {
                    distances[tile] = distance;
                    farthest[tile] = hit.position;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                result.withinTilePaths[i] = distances[i] > 2f ? PathResult(first[i], farthest[i]) : "No separated sample pair";
                for (int j = 0; j < 4; j++)
                {
                    if (i != j && result.found[i] > 0 && result.found[j] > 0)
                        result.crossTilePaths.Add(i + "->" + j + ": " + PathResult(first[i], first[j]));
                }
            }
            string probesFile = "Assets/NavMeshRecoveryDiagnostics/AdjacencyProbes.json";
            if (File.Exists(probesFile))
            {
                Probes probes = JsonUtility.FromJson<Probes>(File.ReadAllText(probesFile));
                foreach (Probe probe in probes.probes)
                {
                    result.adjacencyTested++;
                    if (probe.crossTile) result.portalTested++;
                    NavMeshHit start;
                    NavMeshHit end;
                    NavMeshPath navPath = new NavMeshPath();
                    bool complete = NavMesh.SamplePosition(probe.start, out start, 2f, NavMesh.AllAreas) && NavMesh.SamplePosition(probe.end, out end, 2f, NavMesh.AllAreas) && NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, navPath) && navPath.status == NavMeshPathStatus.PathComplete;
                    if (complete)
                    {
                        result.adjacencyComplete++;
                        if (probe.crossTile) result.portalComplete++;
                    }
                    else
                    {
                        result.adjacencyFailures.Add(probe.label);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            result.error = exception.ToString();
        }
        finally
        {
            if (instance.valid) instance.Remove();
        }
        return result;
    }
}
