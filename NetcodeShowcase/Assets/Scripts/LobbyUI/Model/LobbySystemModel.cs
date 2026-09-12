
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using MVP.Model;
using Assets.Scripts.LobbyUI;

namespace PD4.LobbySystem.Model
{
    public class LobbySystemModel : ModelBase
    {
        #region Enums
        public enum Mode
        {
            JoinLobby,
            CreateLobby,
            Ready
        }
        #endregion

        #region Fields
        private Mode _currentMode;
        private string _lobbyName;
        private Lobby _selectedLobby;
        #endregion

        #region Properties
        public Mode CurrentMode
        {
            get
            {
                return _currentMode;
            }
            set
            {
                if (_currentMode == value) return;
                _currentMode = value;
                OnPropertyChanged();
            }
        }

        public string LobbyName
        {
            get => _lobbyName;
            set
            {
                if (_lobbyName == value)
                    return;
                _lobbyName = value;
                OnPropertyChanged();
            }
        }

        public List<Lobby> AllLobbies { get; private set; } = new List<Lobby>();

        public Lobby SelectedLobby
        {
            get => _selectedLobby;
            set
            {
                if (_selectedLobby == value)
                    return;
                _selectedLobby = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public LobbySystemModel()
        {
            AllLobbies = new List<Lobby>();
            CurrentMode = Mode.JoinLobby;
        }
        #endregion

        #region Lobby Management Methods
        public async Task CreateLobbyAsync()
        {
            if (LobbyManager.Instance != null)
            {
                await LobbyManager.Instance.CreateLobbyAsync(_lobbyName);
            }

            CurrentMode = Mode.Ready;
        }

        public async Task RefreshListAsync()
        {
            AllLobbies.Clear();

            AllLobbies = await LobbyManager.Instance.QueryLobbiesAsync();

            OnPropertyChanged(nameof(AllLobbies));
        }

        public async Task JoinLobbyAsync()
        {
            CurrentMode = Mode.Ready;

            await LobbyManager.Instance.JoinLobbyAsync(_selectedLobby);
        }

        public async Task LeaveLobbyAsync()
        {
            await LobbyManager.Instance.LeaveLobbyAsync();

            CurrentMode = Mode.CreateLobby;
        }
        #endregion
    }
}