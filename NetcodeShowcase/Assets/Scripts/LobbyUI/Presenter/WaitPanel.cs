using MVP.Presenter;
using PD4.LobbySystem.Model;
using System;
using UnityEngine.UIElements;

public class WaitPanel : PresenterBase<LobbySystemModel>
{
    public event EventHandler LeftSession;
    private Label _playerCountLabel;
    private Button _leaveButton;
    public WaitPanel(VisualElement panelRoot)
    {
        _playerCountLabel = panelRoot.Q<Label>("txt_playerCount");
        _leaveButton = panelRoot.Q<Button>();
        _leaveButton.clicked += _leaveButton_clicked;
    }

    private async void _leaveButton_clicked()
    {
        await Model.LeaveLobbyAsync();
    }

    protected override void OnModelPropertyChanged(string propertyName)
    {

    }
}
