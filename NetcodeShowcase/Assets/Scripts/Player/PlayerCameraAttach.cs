using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraAttach : MonoBehaviour
{
	[SerializeField]
	private Transform _cameraTarget;
	[SerializeField]
	private Transform _cameraLookat;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void OnEnable()
	{
		CinemachineCamera cinemachineCamera = FindAnyObjectByType<CinemachineThirdPersonFollow>().GetComponent<CinemachineCamera>();
		cinemachineCamera.Target = new CameraTarget()
		{
			TrackingTarget = _cameraTarget,
			LookAtTarget = _cameraLookat
		};
		cinemachineCamera.enabled = true;

	}

	// Update is called once per frame
	void Update()
	{

	}
}
