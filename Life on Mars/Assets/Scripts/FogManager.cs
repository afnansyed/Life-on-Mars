using UnityEngine;

public class FogManager : MonoBehaviour
{
    public static FogManager Instance { get; private set; }

    public Gradient fogColorOverOxygen;
    public AnimationCurve fogDensityOverOxygen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlanetManager.Instance.OnOxygenLevelChanged += UpdateFog;
        UpdateFog(PlanetManager.Instance.oxygenLevel);
    }

    void UpdateFog(float o2)
    {
        RenderSettings.fogColor = fogColorOverOxygen.Evaluate(o2);
        RenderSettings.fogDensity = fogDensityOverOxygen.Evaluate(o2);
    }
}
