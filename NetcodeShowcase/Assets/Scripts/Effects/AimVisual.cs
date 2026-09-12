using Dodgeball.Presenter;
using UnityEngine;

public class AimVisual : MonoBehaviour
{
	[SerializeField]
	private float _lookAheadTime = 3f;

	[SerializeField]
	private int _numSegments = 20;

	[SerializeField]
	private PlayerThrow _throwScript;

	private LineRenderer _lineRenderer;
	Vector3[] _segmentPositions;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_segmentPositions = new Vector3[_numSegments + 1];
		_lineRenderer.positionCount = _segmentPositions.Length;
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (_segmentPositions == null) return;
		if (_lineRenderer == null) return;
		if (_numSegments + 1 != _segmentPositions.Length)
		{
			_segmentPositions = new Vector3[_numSegments + 1];
			_lineRenderer.positionCount = _segmentPositions.Length;
		}
	}
#endif

	// Update is called once per frame
	void LateUpdate()
	{
		Vector3 velocity = _throwScript.CalculateAimVelocity();
		Vector3 startPos = _throwScript.GetGrabPosition();

		float timeStep = _lookAheadTime / (_segmentPositions.Length - 1);

		for (int idx = 0; idx < _segmentPositions.Length; ++idx)
		{
			_segmentPositions[idx] = CalculateAimPoint(startPos, velocity, timeStep * idx);
		}
		_lineRenderer.SetPositions(_segmentPositions);
	}


	static Vector3 CalculateAimPoint(Vector3 pos, Vector3 vel, float aheadTime)
	{
		return pos + vel * aheadTime + 0.5f * Physics.gravity * aheadTime * aheadTime;
	}
}
