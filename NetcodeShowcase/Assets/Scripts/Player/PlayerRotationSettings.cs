using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerRotationSettings
{
	[field: SerializeField]
	public float MinLookAngleX { get; set; }

	[field: SerializeField]
	public float MaxLookAngleX { get; set; }

	[field: SerializeField]
	public float LookRotSpeedVert { get; set; }

	[field: SerializeField]
	public float LookRotSpeedHor { get; set; }
}
