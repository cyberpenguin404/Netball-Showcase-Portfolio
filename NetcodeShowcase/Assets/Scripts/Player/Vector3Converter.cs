namespace Dodgeball.Presenter
{
	public static class Vector3Converter
	{
		public static UnityEngine.Vector3 ToUnityVector(this System.Numerics.Vector3 vector)
		{
			return new UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
		}
		public static System.Numerics.Vector3 ToNumericsVector(this UnityEngine.Vector3 vector)
		{
			return new System.Numerics.Vector3(vector.x, vector.y, vector.z);
		}
	}
}
