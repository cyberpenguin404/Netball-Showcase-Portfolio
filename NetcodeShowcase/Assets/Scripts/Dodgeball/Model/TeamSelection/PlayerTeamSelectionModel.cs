using MVP.Model;
using Unity.Netcode;

namespace Dodgeball.Model
{
	public class PlayerTeamSelectionModel : ModelBase
	{
		private bool _isCurrentPlayer;
		private bool _isReady;
		private PlayerColor _selectedColor;

		public ulong PlayerId { get; }
		public bool IsCurrentPlayer { get; }


		public bool IsReady
		{
			get { return _isReady; }
			set
			{
				if (_isReady == value) return;
				_isReady = value;
				OnPropertyChanged();
			}
		}

		public PlayerColor SelectedColor
		{
			get => _selectedColor;
			set
			{
				//if(_fieldName.Equals(value))
				if (_selectedColor == value)
					return;
				_selectedColor = value;
				OnPropertyChanged();
			}
		}

		public PlayerTeamSelectionModel(ulong playerId)
		{
			PlayerId = playerId;
			IsCurrentPlayer = NetworkManager.Singleton.LocalClientId == PlayerId;
        }
	}
}
