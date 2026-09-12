using MVP.Presenter;
using PD4.LobbySystem.Model;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace PD4.LobbySystem.Presenter
{
    public class CreateLobbyUI : PresenterBase<LobbySystemModel>
    {
        private TextField _lobbyNameField;
        private Button _createButton;

        public CreateLobbyUI(VisualElement panelRoot)
        {
            _lobbyNameField = panelRoot.Q<TextField>();
            _createButton = panelRoot.Q<Button>();

            _lobbyNameField.RegisterValueChangedCallback(NameValueChanged);

            //if _createButton_clicked is async, use this line
            //_createButton.clicked += async () => _createButton_clicked();
            _createButton.clicked += _createButton_clicked;

        }

        private async void _createButton_clicked()
        {
            Debug.Log($"Creating lobby {Model.LobbyName}");
            await Model.CreateLobbyAsync();
        }

        private void NameValueChanged(ChangeEvent<string> nameChangeEvent)
        {
            Model.LobbyName = nameChangeEvent.newValue;
        }

        protected override void OnModelPropertyChanged(string propertyName)
        {
            if (propertyName == nameof(Model.LobbyName))
            {
                _lobbyNameField.value = Model.LobbyName;
                _createButton.SetEnabled(!string.IsNullOrWhiteSpace(Model.LobbyName));
            }
        }
    }
}
