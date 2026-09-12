using UnityEngine;

namespace PD4.Singleton
{
	public abstract class MonobehaviourSingleton<T> : MonoBehaviour where T : MonobehaviourSingleton<T>
	{
		private static T _instance;
		private static bool _applicationQuit;
		public static T Instance
		{
			get
			{

				if (_instance == null) //Lazy instantiation
				{
//#if UNITY_EDITOR
//					if (_applicationQuit) return null;
//#endif

					GameObject gameObject = new GameObject(typeof(T).Name);
					_instance = gameObject.AddComponent<T>();
					DontDestroyOnLoad(gameObject);
					Debug.Log("Created new singleton gameobject");

					//Don't allow lazy instantiation when application is quitting
					Application.quitting += Application_quitting;
				}
				return _instance;
			}
		}

		private static void Application_quitting()
		{
			_applicationQuit = true;
		}

		protected virtual void Awake()
		{
			if (_instance != null && _instance != this) //prevent other instances (could be embedded in scene)
			{
				Destroy(this.gameObject);
				Debug.Log("Destroyed this gameobject");
				return;
			}
			DontDestroyOnLoad(this.gameObject);
		}
	}

}
