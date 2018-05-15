using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaburuTools;

public class EditorMenuItems
{
	public const string MENU_ITEM_PREFIX = "Chessgeon/";
	public const string SCENE_PATH_LOADING_SCREEN = "Assets/_Game/Scenes/SCENE_LoadingScreen.unity";
	public const string SCENE_PATH_DUNGEON = "Assets/_Game/Scenes/SCENE_Dungeon.unity";

	[MenuItem(MENU_ITEM_PREFIX + "Open Loading Screen Scene", false, 1)]
	private static void OpenLoadingScreenScene() { DTEditorUtility.OpenScene(SCENE_PATH_LOADING_SCREEN); }

	[MenuItem(MENU_ITEM_PREFIX + "Open Dungeon Scene", false, 2)]
	private static void OpenDungeonScene() { DTEditorUtility.OpenScene(SCENE_PATH_DUNGEON); }

#if UNITY_ANDROID
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG", false, 101)]
    private static void BuildChessgeonDebug() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG); }
#elif UNITY_IOS
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG (Append)", false, 101)]
    private static void BuildChessgeonDebugAppend() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG, true); }
	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon DEBUG (Replace)", false, 102)]
    private static void BuildChessgeonDebugReplace() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.DEBUG); }
#endif

	[MenuItem(MENU_ITEM_PREFIX + "Build Chessgeon RELEASE", false, 110)]
    private static void BuildChessgeonRelease() { ChessgeonBuildScript.BuildChessgeon(eBuildScheme.RELEASE); }
}



[InitializeOnLoad] // NOTE: Needed to call the static consturctor.
public static class EditorMenuCheckmarkItems
{
    static EditorMenuCheckmarkItems()
    {
		if (!EditorPrefs.HasKey(ChessgeonBuildScript.RUN_IN_IOS_SIMULATOR_KEY))
		{
			EditorPrefs.SetBool(ChessgeonBuildScript.RUN_IN_IOS_SIMULATOR_KEY, false);
		}
		_runIniOSSimulator = EditorPrefs.GetBool(ChessgeonBuildScript.RUN_IN_IOS_SIMULATOR_KEY);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            ToggleRunIniOSSimulator(_runIniOSSimulator);
        };
    }



	private static bool _runIniOSSimulator;

	private const string RUN_IOS_IN_SIMULATOR_MENU_CHECKMARK = EditorMenuItems.MENU_ITEM_PREFIX + "RUN iOS In Simulator";
	[MenuItem(RUN_IOS_IN_SIMULATOR_MENU_CHECKMARK, false, 121)]
    private static void ToggleAction()
    {
        ToggleRunIniOSSimulator(!_runIniOSSimulator);
    }

	public static void ToggleRunIniOSSimulator(bool inRunIniOSSimulator)
    {
		_runIniOSSimulator = inRunIniOSSimulator;
		EditorPrefs.SetBool(ChessgeonBuildScript.RUN_IN_IOS_SIMULATOR_KEY, inRunIniOSSimulator);
		UnityEditor.Menu.SetChecked(RUN_IOS_IN_SIMULATOR_MENU_CHECKMARK, inRunIniOSSimulator);
    }
}