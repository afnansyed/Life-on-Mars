using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Procedural/TreeSeed")]
public class TreeSeed : ScriptableObject
{
    [Header("L-System")]
    public string axiom = "F";
    public List<LRule> rules = new List<LRule>();

    [Header("Geometry")]
    public float angle = 25f;
    public float stepLength = 0.75f;
    public float baseRadius = 0.12f;
    public float radiusFalloff = 0.7f; // How much thinner branches get per depth
    public int maxIterations = 5;
    public int branchSides = 8;

    [Header("Materials")]
    public Material barkMaterial;
    public Material leafMaterial;

    [Header("Leaves")]
    public GameObject leafPrefab;
    public float leafScale = 1f;
    public int leafDensity = 3; // num iterations before leaves appear

    [Header("Impact")]
    public float oxygenMulitplier = 1f;

    [Header("Growth Animation")]
    public float growthDurationSeconds = 30f;
    public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public int randomSeed = 42;
}