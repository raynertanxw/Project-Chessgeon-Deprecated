using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
	private Camera _UICamera = null;

	void Awake()
	{
		_UICamera = this.GetComponent<Camera>();
		Debug.Assert(_UICamera != null, "Could not grab reference to Camera component.");
		_UICamera.cullingMask = 1 << LayerMask.NameToLayer(Constants.LAYER_NAME_UI);
		Debug.Assert(_UICamera.depth == 1, "UICamera not set to correct depth. Depth should be higher than DungeonCamera.");
		Debug.Assert(_UICamera.clearFlags == CameraClearFlags.Depth, "UICamera clear flag should be set to Depth only.");
	}
}
