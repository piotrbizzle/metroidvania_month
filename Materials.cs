using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public enum Material {None, Air, Copper, Tin, Lead, Stone, Wood, Gold};

    public static Dictionary<Material, Color> MaterialToColor = new Dictionary<Material, Color>(){
	{Material.None, new Color(0.17f, 0.19f, 0.14f)},
	{Material.Air, new Color(0f, 0f, 0f, 0f)},
	{Material.Copper, new Color(0.6f, 0.3f, 0.0f)},
	{Material.Tin, new Color(0.6f, 0.6f, 0.6f)},
	{Material.Lead, new Color(0.18f, 0.13f, 0.25f)},
	{Material.Stone, new Color(0.2f, 0.3f, 0.2f)},
	{Material.Wood, new Color(0.8f, 0.6f, 0.4f)},
	{Material.Gold, new Color(0.9f, 0.6f, 0.0f)},
    };

    public static bool DoesFloat(Material material) {
	return material == Material.Wood;
    }
}
