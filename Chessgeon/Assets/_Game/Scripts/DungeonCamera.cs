using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaburuTools;

public class DungeonCamera : MonoBehaviour
{
	private static DungeonCamera _instance = null;

	[SerializeField] private Dungeon _dungeon = null;

	private Camera _dungeonCamera = null;

	private readonly Quaternion _cameraYRotOffset = Quaternion.AngleAxis(30.0f, Vector3.up);

	// TODO: Make this a scroll sensitivity in options.
	private float _dragSpeed = 10.0f;
	private Vector2 _prevFramPos;
	private float _camMinX;
	private float _camMaxX;
	private float _camMinZ;
	private float _camMaxZ;

	private bool _isFocusingOnTile = false;
	private bool _isShaking = false;

	private void Awake()
	{
		if (_instance == null)
		{
			Debug.Assert(_dungeon != null, "_dungeon is not assigned.");
			_instance = this;
			_dungeonCamera = this.GetComponent<Camera>();
			int cullMaskANDLayerUI = (_dungeonCamera.cullingMask & (1 << LayerMask.NameToLayer(Constants.LAYER_NAME_UI)));
			Debug.Assert(cullMaskANDLayerUI == 0, "Dungeon Cam should not have UI in it's culling mask.");
			_dungeon.OnFloorGenerated += CalcCameraBounds;

			BoardScroller.OnDrag += OnDrag;
			BoardScroller.OnBeginDrag += OnBeginDrag;
		}
		else if (_instance != this)
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private Vector2 ConvertPointerPosToPercentage(Vector2 inPointerPos)
	{
		inPointerPos.x = inPointerPos.x / Screen.width;
		inPointerPos.y = inPointerPos.y / Screen.height;

		return inPointerPos;
	}

	private void OnBeginDrag(Vector2 inPointerPos)
	{
		_prevFramPos = ConvertPointerPosToPercentage(inPointerPos);
	}

	private void OnDrag(Vector2 inPointerPos)
	{
		Vector2 pos = ConvertPointerPosToPercentage(inPointerPos);
		Vector2 diff = _prevFramPos - pos;
		_prevFramPos = pos;

		if (!_isFocusingOnTile && !_isShaking)
		{
			Vector3 move = new Vector3(diff.x * _dragSpeed, 0.0f, diff.y * _dragSpeed);
			move = _cameraYRotOffset * move;
			transform.Translate(move, Space.World);
			RestrictCameraPosition();
		}
	}

	private Vector3 RestrictToCameraBounds(Vector3 inPos)
	{
		Vector3 restrictedCamPos = inPos;
		if (restrictedCamPos.x > _camMaxX) restrictedCamPos.x = _camMaxX;
		else if (restrictedCamPos.x < _camMinX) restrictedCamPos.x = _camMinX;

		if (restrictedCamPos.z > _camMaxZ) restrictedCamPos.z = _camMaxZ;
		else if (restrictedCamPos.z < _camMinZ) restrictedCamPos.z = _camMinZ;

		return restrictedCamPos;
	}

	private void RestrictCameraPosition()
	{
		transform.position = RestrictToCameraBounds(transform.position);
	}

	private void CalcCameraBounds()
	{
		_camMinX = -5.5f;
		_camMinZ = -10.5f;

		_camMaxX = _dungeon.CurrentFloor.Size.x - 4.5f;
		_camMaxZ = _dungeon.CurrentFloor.Size.y - 9.5f;
	}

	public static void FocusCameraToTile(Vector2Int inPos, float inDuration, DTJob.OnCompleteCallback inOnComplete = null) { FocusCameraToTile(inPos.x, inPos.y, inDuration, inOnComplete); }
	public static void FocusCameraToTile(int inX, int inY, float inDuration, DTJob.OnCompleteCallback inOnComplete = null)
	{
		// Note: Assumes that the y and x euler degrees are acute angles.
		Vector3 tileTransformPos = _instance._dungeon.TileManager.GetTileTransformPosition(inX, inY);
		float hyp = (_instance.transform.position.y - tileTransformPos.y) / Mathf.Tan(_instance.transform.eulerAngles.x * Mathf.Deg2Rad);
		float diffX = hyp * Mathf.Sin(_instance.transform.eulerAngles.y * Mathf.Deg2Rad);
		float diffZ = hyp * Mathf.Cos(_instance.transform.eulerAngles.y * Mathf.Deg2Rad);
		Vector3 targetPos = new Vector3(
			tileTransformPos.x - diffX,
			_instance.transform.position.y,
			tileTransformPos.z - diffZ);
		targetPos = _instance.RestrictToCameraBounds(targetPos);

		MoveToAction moveToFocus = new MoveToAction(_instance.transform, targetPos, inDuration, Utils.CurveSmoothStep);
		moveToFocus.OnActionStart += () => { _instance._isFocusingOnTile = true; };
		moveToFocus.OnActionFinish += () =>
		{
			_instance._isFocusingOnTile = false;
			if (inOnComplete != null) inOnComplete();
		};
		ActionHandler.RunAction(moveToFocus);
	}

	public static void CameraShake(int inNumShakes, float inIntensity, float inDuration)
	{
		ShakeAction camShake = new ShakeAction(_instance.transform, inNumShakes, inIntensity, Utils.CurveInverseLinear);
		camShake.SetShakeByDuration(inDuration, inNumShakes);
		camShake.OnActionStart += () => { _instance._isShaking = true; };
		camShake.OnActionFinish += () => { _instance._isShaking = false; };
		ActionHandler.RunAction(camShake);
	}
}
