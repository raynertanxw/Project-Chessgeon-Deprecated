using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class MenuUICamera : MonoBehaviour
{
	private static MenuUICamera _instance = null;
	private Camera _menuUICamera = null;
	public static Camera Camera { get { return _instance._menuUICamera; } }

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			_menuUICamera = this.GetComponent<Camera>();
			Debug.Assert(_menuUICamera != null, "Could not grab reference to Camera component.");
			_menuUICamera.cullingMask = 1 << LayerMask.NameToLayer(Constants.LAYER_NAME_UI);
			Debug.Assert(_menuUICamera.depth == 2, "MenuUICamera not set to correct depth. Depth should be higher than UICamera.");
			Debug.Assert(_menuUICamera.clearFlags == CameraClearFlags.Depth, "MenuUICamera clear flag should be set to Depth only.");

			float designHeight = Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH);
			_menuUICamera.orthographicSize = designHeight / 200.0f;
		}
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}