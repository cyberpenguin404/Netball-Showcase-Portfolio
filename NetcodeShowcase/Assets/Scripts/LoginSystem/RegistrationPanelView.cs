using System;
using UnityEngine.UIElements;

public class RegistrationPanelView
{
	#region Events
	public event EventHandler OnRegisterRequested;
	public event EventHandler OnBackRequested;
	#endregion

	#region Properties
	public string Email => _usernameInput.value;
	public string Password => _passwordInput.value;
	public string DisplayName => _displayNameInput.value;
	#endregion

	#region Fields
	private VisualElement _registrationPanel;
	private TextField _usernameInput;
	private TextField _passwordInput;
	private TextField _displayNameInput;
	private Button _registerButton;
	private Button _backButton;
	private Label _errorText;
	#endregion

	public RegistrationPanelView(VisualElement root)
	{
		_registrationPanel = root.Q<VisualElement>("RegistrationPanel");
		_usernameInput = _registrationPanel.Q<TextField>("usernameInput");
		_passwordInput = _registrationPanel.Q<TextField>("passwordInput");
		_displayNameInput = _registrationPanel.Q<TextField>("displayNameInput");
		_errorText = _registrationPanel.Q<Label>("ErrorText");
		_registerButton = _registrationPanel.Q<Button>("RegisterButton");
		_backButton = _registrationPanel.Q<Button>("BackButton");

		HideError();
	}

	public void RegisterCallbacks()
	{
		_registerButton.clicked += OnRegisterButtonClicked;
		_backButton.clicked += OnBackButtonClicked;
		_usernameInput.RegisterValueChangedCallback(OnInputChanged);
		_passwordInput.RegisterValueChangedCallback(OnInputChanged);
		_displayNameInput.RegisterValueChangedCallback(OnInputChanged);

		UpdateButtonStates();
	}

	public void UnregisterCallbacks()
	{
		_registerButton.clicked -= OnRegisterButtonClicked;
		_backButton.clicked -= OnBackButtonClicked;
		_usernameInput.UnregisterValueChangedCallback(OnInputChanged);
		_passwordInput.UnregisterValueChangedCallback(OnInputChanged);
		_displayNameInput.UnregisterValueChangedCallback(OnInputChanged);
	}

	#region Panel Display
	public void Show()
	{
		_registrationPanel.style.display = DisplayStyle.Flex;
		ClearInputs();
		HideError();
	}

	public void Hide()
	{
		_registrationPanel.style.display = DisplayStyle.None;
	}
	#endregion



	#region Error Management
	public void ShowError(string errorMessage)
	{
		_errorText.text = errorMessage;
		_errorText.style.display = DisplayStyle.Flex;
	}

	public void HideError()
	{
		_errorText.style.display = DisplayStyle.None;
	}
	#endregion

	#region Input Management
	public void ClearInputs()
	{
		_usernameInput.value = string.Empty;
		_passwordInput.value = string.Empty;
		_displayNameInput.value = string.Empty;
	}
	#endregion

	#region Button Event Handlers
	private void OnInputChanged(ChangeEvent<string> evt)
	{
		UpdateButtonStates();
	}

	private void UpdateButtonStates()
	{
		bool isValid = !string.IsNullOrWhiteSpace(_usernameInput.value) &&
					   !string.IsNullOrWhiteSpace(_passwordInput.value) &&
					   !string.IsNullOrWhiteSpace(_displayNameInput.value);
		_registerButton.SetEnabled(isValid);
	}

	private void OnRegisterButtonClicked()
	{
		OnRegisterRequested?.Invoke(this, EventArgs.Empty);
	}

	private void OnBackButtonClicked()
	{
		OnBackRequested?.Invoke(this, EventArgs.Empty);
	}
	#endregion
}
