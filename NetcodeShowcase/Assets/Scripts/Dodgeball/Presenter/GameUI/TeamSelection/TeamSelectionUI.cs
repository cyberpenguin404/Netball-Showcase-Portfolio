using Assets.Scripts.Dodgeball.Network;
using Dodgeball.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamSelectionUI : MonoBehaviour
{
	[SerializeField]
	private GameUIPresenter _gameUIPresenter;

	private SelectTeamPresenter _teamSelection;

	[SerializeField]
	private SelectTeamSync _networkSync;

	private void Start()
	{
		var uiDoc = GetComponent<UIDocument>();
		_teamSelection = new SelectTeamPresenter(uiDoc, _gameUIPresenter.Model.TeamSelection, _networkSync);
		_teamSelection.RegisterCallbacks();
	}
	private void OnEnable()
	{
		_teamSelection?.RegisterCallbacks();
	}
	private void OnDisable()
	{
		_teamSelection?.UnregisterCallbacks();
	}
}
