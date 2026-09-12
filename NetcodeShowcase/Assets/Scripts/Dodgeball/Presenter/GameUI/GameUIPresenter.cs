using Dodgeball.Model;
using Dodgeball.Presenter;
using MVP.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// GameUIPresenter is a second presenter for the GameModel which handles the panel switching
/// </summary>
public class GameUIPresenter : PresenterMonobehaviour<GameModel>
{
	[SerializeField]
	private GamePresenter _gamePresenter;

	[SerializeField]
	private UIDocument
		_teamSelectUI,
		_countdownUI,
		_waitingForPlayersUI,
		_matchPlayingUI,
		_gameOverUI;

	private SelectTeamPresenter _teamSelectionUI;

	protected override void Start()
	{
		Model = _gamePresenter.Model;
		SetCurrentGameStateUI();
		base.Start();
	}



	protected override void OnModelPropertyChanged(string propertyName)
	{
		switch (propertyName)
		{
			case nameof(Model.CurrentGameState):
				SetCurrentGameStateUI();
				break;
		}

	}

	void SetCurrentGameStateUI()
	{
		_waitingForPlayersUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.WaitForOthers);
		_teamSelectUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.SelectTeam);
		_countdownUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.CountDown);
		_matchPlayingUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.Playing);
		_gameOverUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.GameOver);
	}
}
