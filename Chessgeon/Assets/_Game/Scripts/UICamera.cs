using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class UICamera : MonoBehaviour
{
	private static UICamera _instance = null;
	private Camera _UICamera = null;
	public static Camera Camera { get { return _instance._UICamera; } }

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			_UICamera = this.GetComponent<Camera>();
			Debug.Assert(_UICamera != null, "Could not grab reference to Camera component.");
			_UICamera.cullingMask = 1 << LayerMask.NameToLayer(Constants.LAYER_NAME_UI);
			Debug.Assert(_UICamera.depth == 1, "UICamera not set to correct depth. Depth should be higher than DungeonCamera.");
			Debug.Assert(_UICamera.clearFlags == CameraClearFlags.Depth, "UICamera clear flag should be set to Depth only.");

			float designHeight = Utils.GetDesignHeightFromDesignWidth(Constants.DESIGN_WIDTH);
			_UICamera.orthographicSize = designHeight / 200.0f;
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
