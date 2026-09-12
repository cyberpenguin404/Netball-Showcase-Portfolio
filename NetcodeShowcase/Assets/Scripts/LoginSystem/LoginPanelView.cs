using System;
using UnityEngine.UIElements;

public class LoginPanelView
{
	#region Events
	public event EventHandler OnLoginRequested;
	public event EventHandler OnRegisterRequested;
	#endregion

	#region Properties
	public string Email => _usernameInput.value;
	public string Password => _passwordInput.value;
	#endregion

	#region Fields
	private VisualElement _loginPanel;
	private TextField _usernameInput;
	private TextField _passwordInput;
	private Button _loginButton;
	private Button _registerButton;
	private Label _errorText;
	#endregion



	public LoginPanelView(VisualElement root)
	{
		_loginPanel = root.Q<VisualElement>("LoginPanel");
		_usernameInput = _loginPanel.Q<TextField>("usernameInput");
		_passwordInput = _loginPanel.Q<TextField>("passwordInput");
		_errorText = _loginPanel.Q<Label>("ErrorText");
		_loginButton = _loginPanel.Q<Button>("LoginButton");
		_registerButton = _loginPanel.Q<Button>("RegisterButton");

		HideError();
	}

	public void RegisterCallbacks()
	{
		_loginButton.clicked += OnLoginButtonClicked;
		_registerButton.clicked += OnRegisterButtonClicked;
		_usernameInput.RegisterValueChangedCallback(OnInputChanged);
		_passwordInput.RegisterValueChangedCallback(OnInputChanged);

		UpdateButtonStates();
	}

	public void UnregisterCallbacks()
	{
		_loginButton.clicked -= OnLoginButtonClicked;
		_registerButton.clicked -= OnRegisterButtonClicked;
		_usernameInput.UnregisterValueChangedCallback(OnInputChanged);
		_passwordInput.UnregisterValueChangedCallback(OnInputChanged);
	}

	#region Panel Display
	public void Show()
	{
		_loginPanel.style.display = DisplayStyle.Flex;
		ClearInputs();
		HideError();
	}

	public void Hide()
	{
		_loginPanel.style.display = DisplayStyle.None;
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
					   !string.IsNullOrWhiteSpace(_passwordInput.value);
		_loginButton.SetEnabled(isValid);
		//_registerButton.SetEnabled(isValid);
	}

	private void OnLoginButtonClicked()
	{
		OnLoginRequested?.Invoke(this, EventArgs.Empty);
	}

	private void OnRegisterButtonClicked()
	{
		OnRegisterRequested?.Invoke(this, EventArgs.Empty);
	}
	#endregion
}
