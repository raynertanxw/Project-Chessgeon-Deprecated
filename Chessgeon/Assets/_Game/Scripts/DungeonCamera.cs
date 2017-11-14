using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;

	private Camera _dungeonCamera = null;

	private readonly Quaternion _cameraYRotOffset = Quaternion.AngleAxis(30.0f, Vector3.up);

	// TODO: Make this a scroll sensitivity in options.
	private float _dragSpeed = 10.0f;
	private Vector3 _prevFramPos;
	private float _camMinX;
	private float _camMaxX;
	private float _camMinZ;
	private float _camMaxZ;

	private void Awake()
	{
		Debug.Assert(_dungeon != null, "_dungeon is not assigned.");

		_dungeonCamera = this.GetComponent<Camera>();
		_dungeon.OnFloorGenerated.AddListener(CalcCameraBounds);
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			Vector3 pos = _dungeonCamera.ScreenToViewportPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown(0)) _prevFramPos = pos;

			Vector3 diff = _prevFramPos - pos;
			_prevFramPos = pos;

			Vector3 move = new Vector3(diff.x * _dragSpeed, 0.0f, diff.y * _dragSpeed);
			move = _cameraYRotOffset * move;
			transform.Translate(move, Space.World);
			RestrictCameraPosition();
		}
		else
		{
			return;
		}
	}

	private void RestrictCameraPosition()
	{
		Vector3 restrictedCamPos = transform.position;
		if (transform.position.x > _camMaxX) restrictedCamPos.x = _camMaxX;
		else if (transform.position.x < _camMinX) restrictedCamPos.x = _camMinX;

		if (transform.position.z > _camMaxZ) restrictedCamPos.z = _camMaxZ;
		else if (transform.position.z < _camMinZ) restrictedCamPos.z = _camMinZ;

		transform.position = restrictedCamPos;
	}

	private void CalcCameraBounds(Floor inFloor)
	{
		_camMinX = -5.5f;
		_camMinZ = -5.5f;

		_camMaxX = inFloor.Size.x - 4.5f;
		_camMaxZ = inFloor.Size.y - 9.5f;
	}
}
