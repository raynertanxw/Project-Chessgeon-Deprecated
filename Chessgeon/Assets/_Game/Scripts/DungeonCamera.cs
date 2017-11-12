﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
	[SerializeField] private Dungeon _dungeon = null;

	private Camera _dungeonCamera = null;

	private readonly Quaternion _cameraYRotOffset = Quaternion.AngleAxis(30.0f, Vector3.up);

	// TODO: Make this a scroll sensitivity in options.
	private float _dragSpeed = 0.75f;
	private Vector3 _dragOrigin;
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
			transform.Translate(move, Space.World);
			RestrictCameraPosition();
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
		_camMinX = -2.5f;
		_camMinZ = -2.5f;

		_camMaxX = inFloor.Size.x - 2.5f;
		_camMaxZ = inFloor.Size.y - 7.5f;
	}
}
