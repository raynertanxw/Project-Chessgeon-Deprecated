using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
	public const int SCENE_LOADINGSCREEN = 0;
	public const int SCENE_DUNGEON = 1;
	public const string SCENE_NAME_LOADINGSCREEN = "SCENE_LoadingScreen";
	public const string SCENE_NAME_DUNGEON = "SCENE_Dungeon";

	public const string LAYER_NAME_UI = "UI";
    public const string LAYER_NAME_DUNGEON_INTERACTABLE = "Dungeon Interactable";

    public static readonly LayerMask LAYER_MASK_DUNGEON_INTERACTABLE = 1 << LayerMask.NameToLayer(LAYER_NAME_DUNGEON_INTERACTABLE);
}
