using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace VisualPropellant;

public class VisualPropellant : ModBehaviour
{
	public static VisualPropellant Instance;

	public JetpackThruster JetpackThruster { get; private set; }

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"{nameof(VisualPropellant)} is loaded.", MessageType.Success);

		new Harmony("Throckmorpheus.VisualPropellant").PatchAll(Assembly.GetExecutingAssembly());

		LoadManager.OnCompleteSceneLoad += OnLoadScene;
	}

	void OnLoadScene(OWScene currentScene, OWScene newScene) {
		if (newScene == OWScene.SolarSystem || newScene == OWScene.EyeOfTheUniverse) {
			JetpackThruster = new JetpackThruster();
		}
	}
}

