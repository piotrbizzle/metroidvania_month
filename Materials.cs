using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public static float bounceSpeed = 16.0f;
    
    public enum Material {None, Air, Copper, Tin, Lead, Stone, Wood, Gold, Pumice, Goo};

    public static Dictionary<Material, Color> MaterialToColor = new Dictionary<Material, Color>(){
	{Material.None, new Color(0.17f, 0.19f, 0.14f)},
	{Material.Air, new Color(0f, 0f, 0f, 0f)},
	{Material.Copper, new Color(0.6f, 0.3f, 0.0f)},
	{Material.Tin, new Color(0.6f, 0.6f, 0.6f)},
	{Material.Lead, new Color(0.25f, 0.13f, 0.25f)},
	{Material.Stone, new Color(0.25f, 0.25f, 0.3f)},
	{Material.Wood, new Color(0.8f, 0.6f, 0.4f)},
	{Material.Gold, new Color(0.9f, 0.6f, 0.0f)},
	{Material.Pumice, new Color(1.0f, 0.95f, 0.7f)},
	{Material.Goo, new Color(0.0f, 0.4f, 0.1f)},
    };

    public static bool DoesFloat(Material material) {
	return material == Material.Wood || material == Material.Pumice || material == Material.Goo;
    }

    public static void Bounce(Material material, Player player) {
	if (material != Material.Goo) {
	    return;
	}
	player.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(player.gameObject.GetComponent<Rigidbody2D>().velocity.x, bounceSpeed);

    }
}
