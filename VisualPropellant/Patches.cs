using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace VisualPropellant;

[HarmonyPatch]
public sealed class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.StartRefillResources))]
    public static void PlayerResources_StartRefillResources_Prefix(PlayerResources __instance, bool fuel, bool health, bool dlcFuelTank) {
        if (fuel && dlcFuelTank) {
            VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel += Mathf.Max(0f,
                PlayerResources._maxFuel - __instance._currentFuel - VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.StartRefillResources))]
    public static void PlayerResources_StartRefillResources_Postfix() {
        VisualPropellant.Instance.JetpackThruster.UpdateFlameColor();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerResources), nameof(PlayerResources.UpdateFuel))]
    public static void PlayerResources_UpdateFuel_Postfix(PlayerResources __instance) {
        if (VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel > 0f) {
            float magnitude = __instance._jetpackThruster.GetLocalAcceleration().magnitude;
            if (!__instance._isRefueling && magnitude > 0f && (!__instance._fluidDetector.InFluidType(FluidVolume.Type.WATER) || __instance._jetpackThruster.IsBoosterFiring())) {
                float thrust = Mathf.Min(magnitude, __instance._jetpackThruster.GetMaxTranslationalThrust() * 2f) * 0.1f;
                if (__instance._invincible || PlayerState.IsInsideTheEye()) {
                    thrust = 0f;
                }
                // Possibly strange fuel should still fade when in the eye?

                float fuelExpended = thrust * Time.deltaTime;
                VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel -= fuelExpended;
                if (VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel < 0f) {
                    VisualPropellant.Instance.JetpackThruster.CurrentStrangeFuel = 0f;
                }
                VisualPropellant.Instance.JetpackThruster.UpdateFlameColor();
            }
        }

        if (__instance._usingOxygenAsPropellant) {
            VisualPropellant.Instance.ModHelper.Console.WriteLine("Using oxygen as propellant.");
        }
    }
}