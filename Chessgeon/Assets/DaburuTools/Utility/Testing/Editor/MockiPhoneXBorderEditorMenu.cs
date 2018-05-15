using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MockiPhoneXBorderEditorMenu
{
	[MenuItem("DaburuTools/Utility/Add Mock iPhoneX Border", false, 1)]
	private static void DT_Util_AddMockiPhoneXBorder()
	{
		Object MockiPhoneXBoderPrefab = AssetDatabase.LoadAssetAtPath("Assets/DaburuTools/Utility/Testing/Mock iPhoneX Borders.prefab", typeof(GameObject));
		GameObject mockiPhoneXBorder = PrefabUtility.InstantiatePrefab(MockiPhoneXBoderPrefab) as GameObject;
		Undo.RegisterCreatedObjectUndo(mockiPhoneXBorder, "Create Mock iPhoneX Border");
		// Remove the "(Clone)" of the name.
		//mockiPhoneXBorder.name = mockiPhoneXBorder.name.Remove(mockiPhoneXBorder.name.Length - 7);
		Debug.Log("Mock iPhoneX Border created successfully.");
	}
}