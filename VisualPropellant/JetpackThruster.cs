using UnityEngine;

namespace VisualPropellant;

public class JetpackThruster
{
    static PlayerResources resources;
    static Material thrusterMat;
    static Color normalFlameColor;
    static Color normalLightColor;
    static Color strangeFlameColor;
    static Color strangeLightColor;
    static int propID_color;

    public float CurrentStrangeFuel { get; set; }

    // Threshold for strange color being used alone
    static float strangeThreshold;

    // Bias to account for the coloration of the flame texture (so it lines up with light color)
    static float strangeFlameBias;

    public JetpackThruster () {
        resources = Object.FindObjectOfType<PlayerResources>();
        propID_color = Shader.PropertyToID("_Color");

        strangeThreshold = 0.5f;
        strangeFlameBias = 2.0f;

        CurrentStrangeFuel = 0f;

        if (thrusterMat == null) {
            // Make jetpack new material so ship thrusters are not affected
            thrusterMat = resources._jetpackFlameColorSwapper._thrusterRenderers[0].material;
            normalFlameColor = thrusterMat.color;
            strangeFlameColor = new Color(0f, 2.252f, 7.507f, 0.5f);
            
            var swapper = resources._jetpackFlameColorSwapper;
            normalLightColor = swapper._baseLightColor;
            strangeLightColor = new Color(0f, 0.759f, 0.494f);
        }
        else {
            thrusterMat.SetColor(propID_color, normalFlameColor);
        }

        foreach (var item in resources._jetpackFlameColorSwapper._thrusterRenderers) {
            item.sharedMaterial = thrusterMat;
        }
    }

    public void UpdateFlameColor() {
        float strangeAmount = CurrentStrangeFuel / Mathf.Min(resources._currentFuel, PlayerResources._maxFuel * strangeThreshold);
        if (resources._currentFuel == 0f) {
            strangeAmount = 0f;
        }

        var flameColor = Color.Lerp(normalFlameColor, strangeFlameColor, Mathf.Clamp01(strangeAmount * strangeFlameBias));
        var lightColor = Color.Lerp(normalLightColor, strangeLightColor, Mathf.Clamp01(strangeAmount));

        thrusterMat.SetColor(propID_color, flameColor);

        var swapper = resources._jetpackFlameColorSwapper;
        for (int l = 0; l < swapper._thrusterLights.Length; l++)
        {
            swapper._thrusterLights[l].color = lightColor;
        }
    }
}