using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaburuTools;

public class EditorMenuItems
{
	private const string MENU_ITEM_PREFIX = "Chessgeon/";
	public const string SCENE_PATH_LOADING_SCREEN = "Assets/_Game/Scenes/SCENE_LoadingScreen.unity";
	public const string SCENE_PATH_DUNGEON = "Assets/_Game/Scenes/SCENE_Dungeon.unity";

	[MenuItem(MENU_ITEM_PREFIX + "Open Loading Screen Scene", false, 1)]
	private static void OpenLoadingScreenScene() { DTEditorUtility.OpenScene(SCENE_PATH_LOADING_SCREEN); }

	[MenuItem(MENU_ITEM_PREFIX + "Open Dungeon Scene", false, 2)]
	private static void OpenDungeonScene() { DTEditorUtility.OpenScene(SCENE_PATH_DUNGEON); }

#if UNITY_ANDROID
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG")]
    private static void BuildChessgeonDebug() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG); }
#elif UNITY_IOS
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG (Append)")]
    private static void BuildChessgeonDebugAppend() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG, true); }
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG (Replace)")]
    private static void BuildChessgeonDebugReplace() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG); }
#endif

	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon RELEASE")]
    private static void BuildChessgeonRelease() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.RELEASE); }
}
