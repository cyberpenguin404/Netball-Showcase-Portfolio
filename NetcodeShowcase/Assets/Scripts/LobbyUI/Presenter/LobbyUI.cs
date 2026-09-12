using Assets.Scripts.LobbyUI;
using MVP.Presenter;
using PD4.LobbySystem.Model;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PD4.LobbySystem.Presenter
{
	public class LobbyUI : PresenterMonobehaviour<LobbySystemModel>
	{

		[SerializeField]
		private UIDocument _lobbyUIDoc;

		[SerializeField]
		private UIDocument _waitingUIDoc;

		private TabView _lobbyTabView;
		private CreateLobbyUI _createPanel = null;
		private JoinLobbyUI _joinPanel = null;
		private WaitPanel _waitPanel = null;

		
		void Start()
		{
			_lobbyUIDoc.enabled = true;
			_waitingUIDoc.enabled = true;

			Model = new LobbySystemModel();
			_createPanel = new CreateLobbyUI(_lobbyUIDoc.rootVisualElement.Q("CreatePanel")) { Model = this.Model };
			_joinPanel = new JoinLobbyUI(_lobbyUIDoc.rootVisualElement.Q("JoinPanel")) { Model = this.Model };
			_waitPanel = new WaitPanel(_waitingUIDoc.rootVisualElement) { Model = this.Model };


            RefreshTabView();

		}
        private void OnEnable()
        {
            LobbyManager.Instance.SessionJoined += StartGameScene;
        }
        private void OnDisable()
        {
			if (LobbyManager.Instance != null)
            LobbyManager.Instance.SessionJoined -= StartGameScene;
        }

        private void StartGameScene(object sender, System.EventArgs e)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("gameplayScene", LoadSceneMode.Single);
            }
        }
        void RefreshTabView()
		{
			if (_lobbyTabView != null)//avoid subscribing multiple times
			{
				_lobbyTabView.activeTabChanged -= _lobbyTabView_activeTabChanged;
			}

			_lobbyTabView = _lobbyUIDoc.rootVisualElement.Q<TabView>("TabSelector");

			if (_lobbyTabView != null)
			{
				_lobbyTabView.activeTabChanged += _lobbyTabView_activeTabChanged;
			}
		}

		private void _lobbyTabView_activeTabChanged(Tab previous, Tab current)
		{
			if (current.tabHeader == _lobbyTabView.GetTabHeader(0))
				Model.CurrentMode = LobbySystemModel.Mode.JoinLobby;
			else
				Model.CurrentMode = LobbySystemModel.Mode.CreateLobby;
		}

		protected override void OnModelUpdated(LobbySystemModel previousModel)
		{
			UpdatePanelSelection();
		}
		protected override void OnModelPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(Model.CurrentMode))
			{
				Debug.Log($"switching to mode {Model.CurrentMode}");
				UpdatePanelSelection();
			}
		}

		private void UpdatePanelSelection()
		{

			switch (Model.CurrentMode)
			{
				case LobbySystemModel.Mode.CreateLobby:

					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
					_waitingUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
					RefreshTabView();
					_lobbyTabView.selectedTabIndex = 1;

		

					break;
				case LobbySystemModel.Mode.JoinLobby:
					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
					_waitingUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
					RefreshTabView();
					_lobbyTabView.selectedTabIndex = 0;
				

					break;
				default:
					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
					
					
					break;
			}
		}
	}
}
