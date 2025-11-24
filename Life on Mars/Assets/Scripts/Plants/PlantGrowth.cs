using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public TreeSeed treeSeed;

    private int currentIteration = 0;
    private float timer = 0f;
    private MeshFilter mf;
    private MeshRenderer mr;

    private bool isFullyGrown = false;
    private bool isRegistered = false;

    // For GPU instancing
    private Matrix4x4[] leafMatrices;
    private MaterialPropertyBlock leafPropertyBlock;

    // Planet-influenced properties
    private float sizeMultiplier = 1f;
    private float growthRateMultiplier = 1f;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        if (treeSeed != null)
        {
            Random.InitState(treeSeed.randomSeed);
            mr.material = treeSeed.barkMaterial;
        }

        leafPropertyBlock = new MaterialPropertyBlock();
        UpdatePlanetInfluence();
        RebuildMesh();
    }

    private void UpdatePlanetInfluence()
    {
        if (PlanetManager.Instance != null)
        {
            growthRateMultiplier = PlanetManager.Instance.GetGrowthRateMultiplier();
            sizeMultiplier = PlanetManager.Instance.GetTreeSizeMultiplier();
        }
    }

    private void RegisterWithPlanet()
    {
        if (PlanetManager.Instance != null && !isRegistered)
        {
            PlanetManager.Instance.RegisterMatureTree(treeSeed.oxygenMulitplier);
            isRegistered = true;
        }
    }

    private void Update()
    {
        if (treeSeed == null) return;

        UpdatePlanetInfluence();

        float iterationDuration = (treeSeed.growthDurationSeconds / treeSeed.maxIterations) / growthRateMultiplier;
        timer += Time.deltaTime;

        if (timer >= iterationDuration && currentIteration < treeSeed.maxIterations)
        {
            timer = 0f;
            currentIteration++;
            RebuildMesh();

            // register as oxygen producer when fully grown
            if (currentIteration >= treeSeed.maxIterations && !isFullyGrown)
            {
                isFullyGrown = true;
                RegisterWithPlanet();
            }
        }

        // render leaves
        if (leafMatrices != null && leafMatrices.Length > 0 && treeSeed.leafPrefab != null)
        {
            MeshFilter leafMF = treeSeed.leafPrefab.GetComponent<MeshFilter>();
            MeshRenderer leafMR = treeSeed.leafPrefab.GetComponent<MeshRenderer>();

            if (leafMF != null && leafMR != null)
            {
                Graphics.DrawMeshInstanced(
                    leafMF.sharedMesh,
                    0,
                    leafMR.sharedMaterial,
                    leafMatrices,
                    leafMatrices.Length,
                    leafPropertyBlock
                );
            }
        }
    }

    private void RebuildMesh()
    {
        if (treeSeed == null) return;

        // generate L-System
        string lString = LSystemGenerator.Generate(
            treeSeed.axiom,
            treeSeed.rules,
            currentIteration);

        // Debug.Log($"Iteration {currentIteration}: {lString}");

        float effectiveStepLength = treeSeed.stepLength * sizeMultiplier;
        float effectiveBaseRadius = treeSeed.baseRadius * sizeMultiplier;

        // build geometry
        var (segments, leaves) = TurtleMeshBuilder.Build(
            lString,
            effectiveStepLength,
            treeSeed.angle,
            effectiveBaseRadius,
            treeSeed.leafDensity);

        // generate branch mesh
        mf.mesh = BuildMeshFromSegments(segments, treeSeed.branchSides);

        leafMatrices = new Matrix4x4[leaves.Count];
        float effectiveLeafScale = treeSeed.leafScale * sizeMultiplier;
        for (int i = 0; i < leaves.Count; i++)
        {
            leafMatrices[i] = Matrix4x4.TRS(
                transform.TransformPoint(leaves[i].position),
                transform.rotation * leaves[i].rotation,
                Vector3.one * effectiveLeafScale
            );
        }
    }

    private Mesh BuildMeshFromSegments(List<TurtleMeshBuilder.Segment> segs, int sides)
    {
        List<Vector3> verts = new();
        List<int> tris = new();

        foreach (var s in segs)
        {
            Vector3 dir = (s.b - s.a).normalized;
            Vector3 right = Vector3.Cross(dir, Vector3.forward).normalized;

            int startIndex = verts.Count;

            for (int i = 0; i < sides; i++)
            {
                float t = (i / (float)sides) * Mathf.PI * 2f;
                Vector3 offset = Quaternion.AngleAxis(i * (360f / sides), dir) * right * s.thickness;

                verts.Add(s.a + offset);
                verts.Add(s.b + offset);

                int baseIdx = startIndex + i * 2;
                int nextIdx = startIndex + ((i + 1) % sides) * 2;

                // side quad
                tris.Add(baseIdx);
                tris.Add(nextIdx);
                tris.Add(baseIdx + 1);

                tris.Add(baseIdx + 1);
                tris.Add(nextIdx);
                tris.Add(nextIdx + 1);
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();
        return m;
    }
}