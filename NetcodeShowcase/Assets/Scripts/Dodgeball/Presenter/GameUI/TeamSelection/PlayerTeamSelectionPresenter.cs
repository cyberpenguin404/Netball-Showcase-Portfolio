using Dodgeball.Model;
using MVP.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dodgeball.Presenter
{

	public class PlayerTeamSelectionPresenter : PresenterBase<PlayerTeamSelectionModel>
	{
		private VisualElement _redImageElement;
		private VisualElement _blueImageElement;
		private VisualElement _readyElement;
		private VisualElement _currentPlayerElement;

		public PlayerTeamSelectionPresenter(VisualElement rowElement)
		{
			_currentPlayerElement = rowElement.Q("localPlayerIndicator");
			_readyElement = rowElement.Q<Label>("Ready");
			_blueImageElement = rowElement.Q("Blue");
			_redImageElement = rowElement.Q("Red");
		}

		protected override void OnModelUpdated(PlayerTeamSelectionModel previousModel)
		{
			UpdateIsCurrentPlayer();
			UpdateSelection();
			UpdateIsReady();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.IsCurrentPlayer):
					UpdateIsCurrentPlayer();
					break;
				case nameof(Model.SelectedColor):
					UpdateSelection();
					break;
				case nameof(Model.IsReady):
					UpdateIsReady();
					break;
			}

		}


		private void UpdateIsCurrentPlayer()
		{
			_currentPlayerElement.style.visibility = Model.IsCurrentPlayer ? Visibility.Visible : Visibility.Hidden;
		}
		private void UpdateSelection()
		{
			_redImageElement.style.unityBackgroundImageTintColor = Model.SelectedColor == PlayerColor.Red ? Color.white : Color.black;
			_blueImageElement.style.unityBackgroundImageTintColor = Model.SelectedColor == PlayerColor.Blue ? Color.white : Color.black;

		}
		private void UpdateIsReady()
		{
			_readyElement.style.visibility = Model.IsReady ? Visibility.Visible : Visibility.Hidden;
		}
	}
}
