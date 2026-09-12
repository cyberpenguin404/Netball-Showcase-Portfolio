using Dodgeball.Model;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
	[SerializeField]
	private PlayerColor _color;
	public PlayerColor Color => _color;


	[SerializeField]
	private float _width, _depth;

	public Vector3 GetRandomVector()
	{
		Vector3 randomOffset = new Vector3(Random.Range(-.5f, .5f) * _width, 0f, Random.Range(-.5f, .5f) * _depth);
		return transform.TransformPoint(randomOffset);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = _color == PlayerColor.Red ? UnityEngine.Color.red : UnityEngine.Color.blue;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(_width, 1f, _depth));
	}

}
