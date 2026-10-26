using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class TestConvertedNavMesh2017_Centroids
{
    [MenuItem("Tools/Navigation/Test Converted NavMesh - Triangle Centers")]
    private static void Test()
    {
        NavMeshData data = Selection.activeObject as NavMeshData;
        if (data == null)
        {
            Debug.LogError("Select the converted NavMeshData asset in the Project window first.");
            return;
        }

        NavMeshDataInstance instance = NavMesh.AddNavMeshData(data);
        if (!instance.valid)
        {
            Debug.LogError("NavMesh.AddNavMeshData rejected the selected asset.");
            return;
        }

        try
        {
            NavMeshTriangulation mesh = NavMesh.CalculateTriangulation();
            int triangleCount = mesh.indices.Length / 3;
            Debug.Log("Converted NavMesh: " + mesh.vertices.Length + " vertices, " + triangleCount + " triangulation triangles.");
            if (triangleCount == 0) return;

            Vector3 min = mesh.vertices[0];
            Vector3 max = mesh.vertices[0];
            foreach (Vector3 vertex in mesh.vertices)
            {
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }
            Debug.Log("Triangulation world-space bounds: min=" + min + ", max=" + max);

            int[] tested = new int[4];
            int[] found = new int[4];
            Vector3[] firstHit = new Vector3[4];
            Vector3[] secondHit = new Vector3[4];
            bool[] hasFirst = new bool[4];
            bool[] hasSecond = new bool[4];

            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 a = mesh.vertices[mesh.indices[i * 3]];
                Vector3 b = mesh.vertices[mesh.indices[i * 3 + 1]];
                Vector3 c = mesh.vertices[mesh.indices[i * 3 + 2]];
                Vector3 center = (a + b + c) / 3f;
                int tile = (center.x >= -17f ? 1 : 0) + (center.z >= 28.254961f ? 2 : 0);
                if (tested[tile] >= 64) continue;
                tested[tile]++;

                NavMeshHit hit;
                if (!NavMesh.SamplePosition(center, out hit, 2f, NavMesh.AllAreas)) continue;
                found[tile]++;
                if (!hasFirst[tile])
                {
                    hasFirst[tile] = true;
                    firstHit[tile] = hit.position;
                }
                else if (!hasSecond[tile] && Vector3.Distance(firstHit[tile], hit.position) > 2f)
                {
                    hasSecond[tile] = true;
                    secondHit[tile] = hit.position;
                }
            }

            int totalFound = 0;
            for (int tile = 0; tile < 4; tile++)
            {
                totalFound += found[tile];
                string message = "Tile " + tile + ": " + found[tile] + "/" + tested[tile] + " triangle-center samples found";
                if (hasFirst[tile]) message += ", first hit=" + firstHit[tile];
                if (hasSecond[tile])
                {
                    NavMeshPath path = new NavMeshPath();
                    bool calculated = NavMesh.CalculatePath(firstHit[tile], secondHit[tile], NavMesh.AllAreas, path);
                    message += ", path calculated=" + calculated + ", status=" + path.status + ", corners=" + path.corners.Length;
                }
                Debug.Log(message);
            }

            Debug.Log("Result: " + totalFound + " triangle-center samples succeeded. If all are zero despite nonzero triangulation, geometry is visible but navigation queries are not succeeding; the converted tile data needs more investigation.");
        }
        finally
        {
            instance.Remove();
        }
    }
}
