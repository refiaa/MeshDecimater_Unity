using UnityEngine;
using UnityMeshSimplifier;

/*
using UnityMeshSimplifier from github.com/Whinarn/UnityMeshSimplifier
*/

public static class MeshDecimaterUtility
{
    public static void DecimateMesh(Mesh originalMesh, Mesh decimatedMesh, float decimateLevel, bool isSkinnedMeshRenderer, int[] originalSubmeshCount)
    {
        MeshSimplifier meshSimplifier = new MeshSimplifier();
        meshSimplifier.Initialize(originalMesh);

        meshSimplifier.PreserveBorderEdges = true;
        meshSimplifier.PreserveUVSeamEdges = true;

        meshSimplifier.SimplifyMesh(decimateLevel);

        Mesh simplifiedMesh = meshSimplifier.ToMesh();

        decimatedMesh.Clear();
        decimatedMesh.vertices = simplifiedMesh.vertices;
        decimatedMesh.normals = simplifiedMesh.normals;
        decimatedMesh.uv = simplifiedMesh.uv;
        decimatedMesh.tangents = simplifiedMesh.tangents;

        decimatedMesh.subMeshCount = originalMesh.subMeshCount;
        for (int i = 0; i < originalMesh.subMeshCount; i++)
        {
            int[] triangles = simplifiedMesh.GetTriangles(i);
            int targetTriangleCount = Mathf.CeilToInt(originalSubmeshCount[i] * decimateLevel);
            if (triangles.Length > targetTriangleCount * 3)
            {
                System.Array.Resize(ref triangles, targetTriangleCount * 3);
            }
            decimatedMesh.SetTriangles(triangles, i);
        }

        if (isSkinnedMeshRenderer)
        {
            if (originalMesh.bindposes != null && originalMesh.boneWeights != null && originalMesh.boneWeights.Length == originalMesh.vertexCount)
            {
                decimatedMesh.bindposes = originalMesh.bindposes;
                decimatedMesh.boneWeights = CopyBoneWeights(originalMesh, simplifiedMesh);
            }

            CopyBlendShapes(originalMesh, simplifiedMesh, decimatedMesh);
        }
        else
        {
            CopyBlendShapes(originalMesh, simplifiedMesh, decimatedMesh);
        }

        decimatedMesh.RecalculateBounds();
        decimatedMesh.RecalculateNormals();
        decimatedMesh.RecalculateTangents();
    }

    private static void CopyBlendShapes(Mesh sourceMesh, Mesh simplifiedMesh, Mesh targetMesh)
    {
        Vector3[] originalVertices = sourceMesh.vertices;
        Vector3[] simplifiedVertices = simplifiedMesh.vertices;
        int[] closestVertexMap = new int[simplifiedVertices.Length];

        for (int i = 0; i < simplifiedVertices.Length; i++)
        {
            closestVertexMap[i] = FindClosestVertex(originalVertices, simplifiedVertices[i]);
        }

        for (int i = 0; i < sourceMesh.blendShapeCount; i++)
        {
            string shapeName = sourceMesh.GetBlendShapeName(i);
            int frameCount = sourceMesh.GetBlendShapeFrameCount(i);

            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                float frameWeight = sourceMesh.GetBlendShapeFrameWeight(i, frameIndex);
                Vector3[] deltaVertices = new Vector3[simplifiedVertices.Length];
                Vector3[] deltaNormals = new Vector3[simplifiedVertices.Length];
                Vector3[] deltaTangents = new Vector3[simplifiedVertices.Length];

                SimplifyBlendShapeFrame(sourceMesh, i, frameIndex, closestVertexMap, deltaVertices, deltaNormals, deltaTangents);
                targetMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
            }
        }
    }

    private static void SimplifyBlendShapeFrame(Mesh originalMesh, int shapeIndex, int frameIndex, int[] closestVertexMap, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
    {
        Vector3[] originalDeltaVertices = new Vector3[originalMesh.vertexCount];
        Vector3[] originalDeltaNormals = new Vector3[originalMesh.vertexCount];
        Vector3[] originalDeltaTangents = new Vector3[originalMesh.vertexCount];

        originalMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, originalDeltaVertices, originalDeltaNormals, originalDeltaTangents);

        for (int i = 0; i < closestVertexMap.Length; i++)
        {
            int closestVertexIndex = closestVertexMap[i];
            deltaVertices[i] = originalDeltaVertices[closestVertexIndex];
            deltaNormals[i] = originalDeltaNormals[closestVertexIndex];
            deltaTangents[i] = originalDeltaTangents[closestVertexIndex];
        }
    }

    private static int FindClosestVertex(Vector3[] vertices, Vector3 targetVertex)
    {
        int closestIndex = 0;
        float closestDistance = Vector3.Distance(vertices[0], targetVertex);

        for (int i = 1; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], targetVertex);
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    private static BoneWeight[] CopyBoneWeights(Mesh originalMesh, Mesh simplifiedMesh)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        Vector3[] simplifiedVertices = simplifiedMesh.vertices;
        BoneWeight[] originalBoneWeights = originalMesh.boneWeights;
        BoneWeight[] simplifiedBoneWeights = new BoneWeight[simplifiedVertices.Length];

        for (int i = 0; i < simplifiedVertices.Length; i++)
        {
            int closestVertexIndex = FindClosestVertex(originalVertices, simplifiedVertices[i]);
            simplifiedBoneWeights[i] = originalBoneWeights[closestVertexIndex];
        }

        return simplifiedBoneWeights;
    }
}
