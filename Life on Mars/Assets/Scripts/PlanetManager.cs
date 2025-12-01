using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PlanetManager : MonoBehaviour
{
    public static PlanetManager Instance { get; private set; }

    [Header("Atmospheric Properties")]
    [Range(0f, 1f)]
    [Tooltip("0 = no oxygen, 1 = terraformed")]
    public float oxygenLevel = 0.1f;

    [Range(0f, 1f)]
    public float atmosphereQuality = 0.2f;

    [Header("Oxygen Generation")]
    [Tooltip("Oxygen added per grown tree per second")]
    public float oxygenPerTreePerSecond = 0.001f;

    [Header("Player")]
    public float basePlayerOxygenLoss = 5f;
    public float minPlayerOxygenLoss = 0.5f;

    // events for other systems to subscribe to
    public event Action<float> OnOxygenLevelChanged;
    public event Action<float> OnAtmosphereQualityChanged;

    public TMP_Text oxygenText;

    private float activeMatureTrees = 0;

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

    private void Update()
    {
        // trees gradually increase oxygen
        if (activeMatureTrees > 0)
        {
            float oxygenGain = activeMatureTrees * oxygenPerTreePerSecond * Time.deltaTime;
            ModifyOxygenLevel(oxygenGain);
        }

        if (oxygenLevel > atmosphereQuality)
        {
            float qualityGain = (oxygenLevel - atmosphereQuality) * 0.05f * Time.deltaTime;
            ModifyAtmosphereQuality(qualityGain);
        }
        oxygenText.text = "Mars O2: " + oxygenLevel;
    }

    public void RegisterMatureTree(float amount)
    {
        activeMatureTrees += amount;
    }

    public void ModifyOxygenLevel(float amount)
    {
        float oldLevel = oxygenLevel;
        oxygenLevel = Mathf.Clamp01(oxygenLevel + amount);

        if (!Mathf.Approximately(oldLevel, oxygenLevel))
        {
            // invoke any actions watching OnOxygenLevelChanged event (could use for UI)
            OnOxygenLevelChanged?.Invoke(oxygenLevel);
        }
    }

    public void ModifyAtmosphereQuality(float amount)
    {
        float oldQuality = atmosphereQuality;
        atmosphereQuality = Mathf.Clamp01(atmosphereQuality + amount);

        if (!Mathf.Approximately(oldQuality, atmosphereQuality))
        {
            OnAtmosphereQualityChanged?.Invoke(atmosphereQuality);
        }
    }

    // for player oxygen loss calculation
    public float GetPlayerOxygenLossRate()
    {
        return Mathf.Lerp(basePlayerOxygenLoss, minPlayerOxygenLoss, oxygenLevel);
    }

    // growth rate multiplier for plants based on atmosphere quality (0.5x to 2x)
    public float GetGrowthRateMultiplier()
    {
        return Mathf.Lerp(0.5f, 2f, atmosphereQuality);
    }

    // size multiplier for trees
    public float GetTreeSizeMultiplier()
    {
        return Mathf.Lerp(0.6f, 1.8f, atmosphereQuality);
    }

    // could use for UI / skybox ?
    public Color GetAtmosphereColor()
    {
        return Color.Lerp(new Color(0.8f, 0.4f, 0.3f), new Color(0.5f, 0.8f, 0.9f), atmosphereQuality);
    }
}
