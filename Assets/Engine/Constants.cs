using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */ 
public sealed class Constants
{
	 
	// Yellow
	public static Color PATH_COLOR = new Color (1.0f, 1.0f, 0.0f);
	// Red
	public static Color TERRAIN_COLOR = new Color (1.0f, 0.0f, 0.0f);
	// Green
	public static Color TOWER_SPAWN_COLOR = new Color (0.0f, 1.0f, 0.0f);
	// Blue
	public static Color TARGET_SPAWN_COLOR = new Color (0.0f, 0.0f, 1.0f);
	// Purple
	public static Color ENEMY_SPAWN_COLOR = new Color (1.0f, 0.0f, 1.0f);
	// Used in the noise generator algorithm to clamp the playable zone
	public static float INITIAL_PLAYABLE_HEIGHT = 0.1f;

	public static float NOISE_HEIGHT_MULTIPLIER = 100.0f;

	public static float VERTEX_SPACING = 2.5f;
	public static float VERTEX_SPACING_32 = 5.0f;
	public static float VERTEX_SPACING_16 = 10.0f;

	public static string SAVE_GAME_RELATIVE_PATH = "/saveGame.dat";

	public static float GAME_TIME_SCALE_CACHE = 1.0f;

	public static float Y_CORRECTION = 0.0f;
}
