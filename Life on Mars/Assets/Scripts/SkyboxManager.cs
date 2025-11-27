using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public static SkyboxManager Instance { get; private set; }

    public Gradient skyTintOverOxygen;

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
        PlanetManager.Instance.OnOxygenLevelChanged += UpdateSky;
        UpdateSky(PlanetManager.Instance.oxygenLevel);
    }

    void UpdateSky(float o2)
    {
        RenderSettings.skybox.SetColor("_SkyTint", skyTintOverOxygen.Evaluate(o2));
    }
}
