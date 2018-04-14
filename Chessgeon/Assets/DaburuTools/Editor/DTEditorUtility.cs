using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using System.Collections;
using System.IO;

namespace DaburuTools
{
	public class DTEditorUtility
	{
		/// <summary>
		/// Shrinks the passed in Rect by one unit on all sides. Maintains its position.
		/// Please pass in the Rect by ref.
		/// i.e. ShrinkRectByOne(ref myRect);
		/// </summary>
		public static void ShrinkRectByOne(ref Rect _rect)
		{
			_rect.min += Vector2.one;
			_rect.max -= Vector2.one;
		}

		public static void DestroyImmediateAndAllChildren(GameObject _gameObject)
		{
			while (_gameObject.transform.childCount > 0)
			{
				// Call this method on its children.
				DestroyImmediateAndAllChildren(_gameObject.transform.GetChild(0).gameObject);
			}

			// Finally Destroy itself.
			GameObject.DestroyImmediate(_gameObject);
			return;
		}

		public static string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if ( !string.IsNullOrEmpty(path) && File.Exists(path) ) 
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}

        /// <summary>
		/// Opens the destination path scene. Will prompt to save changes in any opened scenes if needed.
		/// </summary>
		/// <param name="inPath"></param>
        public static void OpenScene(string inPath)
        {
            // TODO: Check if the path exits.
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(inPath, OpenSceneMode.Single);
            }
        }

        /// <summary>
        /// Creates a scriptable object of type T where T : UnityEngine.ScriptableObject. Takes in a destination path and the name for the newly created asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inPath"></param>
        /// <param name="inNewAssetName"></param>
        public static void CreateScriptableObject<T>(string inPath, string inNewAssetName) where T : UnityEngine.ScriptableObject
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(typeof(T));

            if (!Directory.Exists(inPath)) Directory.CreateDirectory(inPath);
            AssetDatabase.CreateAsset(asset, Path.Combine(inPath, inNewAssetName));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
