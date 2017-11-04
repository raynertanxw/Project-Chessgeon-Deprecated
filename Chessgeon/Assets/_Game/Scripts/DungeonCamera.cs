using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;

	private Camera _dungeonCamera = null;

	private readonly Quaternion _cameraYRotOffset = Quaternion.AngleAxis(30.0f, Vector3.up);

	// TODO: Make this a scroll sensitivity in options.
	private float _dragSpeed = 0.5f;
	private Vector3 _dragOrigin;

	private void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_dungeonCamera = this.GetComponent<Camera>();
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_dragOrigin = Input.mousePosition;
		}
		else if (!Input.GetMouseButton(0))
		{
			return;
		}
		else
		{
			Vector3 pos = _dungeonCamera.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
			Vector3 move = new Vector3(pos.x * _dragSpeed, 0.0f, pos.y * _dragSpeed);
			move = _cameraYRotOffset * move;
			// TODO: Restrict the camera movement based on the dungeon size.

			transform.Translate(move, Space.World);
		}
	}
}
