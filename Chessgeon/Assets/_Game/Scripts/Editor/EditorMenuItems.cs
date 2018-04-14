﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DaburuTools;

public class EditorMenuItems
{
    private const string MENU_ITEM_PREFIX = "Chessgeon/";
    private const string SCENE_PATH_LOADING_SCREEN = "Assets/_Game/Scenes/SCENE_LoadingScreen.unity";
    private const string SCENE_PATH_DUNGEON = "Assets/_Game/Scenes/SCENE_Dungeon.unity";

    [MenuItem(MENU_ITEM_PREFIX + "Open Loading Screen Scene", false, 1)]
    private static void OpenLoadingScreenScene() { DTEditorUtility.OpenScene(SCENE_PATH_LOADING_SCREEN); }

    [MenuItem(MENU_ITEM_PREFIX + "Open Dungeon Scene", false, 2)]
    private static void OpenDungeonScene() { DTEditorUtility.OpenScene(SCENE_PATH_DUNGEON); }
}
