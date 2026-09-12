using Dodgeball.Model;
using MVP.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dodgeball.Presenter
{
	public class CountdownUI : PresenterMonobehaviour<GameModel>
	{
		[SerializeField]
		private GameUIPresenter _gameUIPresenter;

		private Label _countdownLabel;
		protected override void Start()
		{
			Model = _gameUIPresenter.Model;
		}

		private void OnEnable()
		{
			_countdownLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>();

		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.CountDownTime):
					UpdateCountdownTime();
					break;
			}

		}

		void UpdateCountdownTime()
		{
			if (Model.CountDownTime >= 1f)
			{
				_countdownLabel.text = $"{(int)Model.CountDownTime}";

			}
			else
			{
				_countdownLabel.text = "GO!";
			}
		}

	}
}
