using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Main controller for the login UI system.
/// Manages panel switching and coordinates between login and registration flows.
/// </summary>
public class LoginUI : MonoBehaviour
{
	#region Fields
	private LoginPanelView _loginPanelView;
	private RegistrationPanelView _registrationPanelView;
	#endregion

	#region Lifecycle
	private void OnEnable()
	{
		InitializePanels();
		RegisterCallbacks();
		ShowLoginPanel();
	}

	private void OnDisable()
	{
		UnregisterCallbacks();
	}
	#endregion

	#region Initialization
	private void InitializePanels()
	{
		var root = GetComponent<UIDocument>().rootVisualElement;

		_loginPanelView = new LoginPanelView(root);
		_registrationPanelView = new RegistrationPanelView(root);
	}

	private void RegisterCallbacks()
	{
		_loginPanelView.OnLoginRequested += OnLoginRequested;
		_loginPanelView.OnRegisterRequested += OnLoginToRegisterRequested;
		_registrationPanelView.OnRegisterRequested += OnRegisterRequested;
		_registrationPanelView.OnBackRequested += OnRegistrationBackRequested;

		_loginPanelView.RegisterCallbacks();
		_registrationPanelView.RegisterCallbacks();
	}

	private void UnregisterCallbacks()
	{
		_loginPanelView.OnLoginRequested -= OnLoginRequested;
		_loginPanelView.OnRegisterRequested -= OnLoginToRegisterRequested;
		_registrationPanelView.OnRegisterRequested -= OnRegisterRequested;
		_registrationPanelView.OnBackRequested -= OnRegistrationBackRequested;

		_loginPanelView.UnregisterCallbacks();
		_registrationPanelView.UnregisterCallbacks();
	}
	#endregion

	#region Panel Management
	private void ShowLoginPanel()
	{
		_loginPanelView.Show();
		_registrationPanelView.Hide();
	}

	private void ShowRegistrationPanel()
	{
		_loginPanelView.Hide();
		_registrationPanelView.Show();
	}
	#endregion

	#region Event Handlers
	/// Handles login request, validates input, and triggers PlayFab login.
	private void OnLoginRequested(object sender, EventArgs e)
	{
		string email = _loginPanelView.Email;
		string password = _loginPanelView.Password;

		if (!ValidateInputs(email, password))
		{
			_loginPanelView.ShowError("Email and password required");
			return;
		}

		_loginPanelView.HideError();
		Debug.Log($"Login attempt: {email}");

		PlayFabClientAPI.LoginWithEmailAddress(
			new PlayFab.ClientModels.LoginWithEmailAddressRequest()
			{
				Email = email,
				Password = password
			},
			OnLoginSuccess,
			OnError
			);
	}

    private void OnError(PlayFabError error)
    {
		Debug.Log(error.GenerateErrorReport());
		_loginPanelView.ShowError($"{error.ErrorMessage} Please try again.");
    }

    private void OnLoginSuccess(LoginResult result)
    {
        PlayfabPlayer.Instance.StorePlayFabID(result.PlayFabId);
		PlayfabPlayer.Instance.FetchDisplayName(async () =>
		{ 
			await AuthenticateUnityServicesAsync();

			SwapToNextScene();
			});
    }

    private async Task AuthenticateUnityServicesAsync()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed into UGS. PlayerID: {AuthenticationService.Instance.PlayerId}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unity Services Authentication failed: {ex.Message}");
        }
    }

    private static void SwapToNextScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    /// Handles registration request, validates input, and triggers PlayFab registration.
    private void OnRegisterRequested(object sender, EventArgs e)
	{
		string email = _registrationPanelView.Email;
		string password = _registrationPanelView.Password;
		string displayName = _registrationPanelView.DisplayName;
		string username = displayName;

		if (!ValidateInputs(email, password))
		{
			_registrationPanelView.ShowError("Username and password required");
			return;
		}

		Debug.Log($"Register attempt: {email}");
		_registrationPanelView.HideError();
		// TODO: Call PlayFab register method

		PlayFabClientAPI.RegisterPlayFabUser(
			 new RegisterPlayFabUserRequest()
             {
                 Email = email,
                 Username = username,
                 DisplayName = displayName,
                 Password = password
             },
			 OnregisterSuccess,
			 OnError);

	}

    private void OnregisterSuccess(RegisterPlayFabUserResult result)
    {
		Debug.Log($"Register success: {result}");
    }

    //Register buttton pressed in Login panel
    private void OnLoginToRegisterRequested(object sender, EventArgs e)
	{
		ShowRegistrationPanel();
	}

	//Backbutton pressed in Register panel
	private void OnRegistrationBackRequested(object sender, EventArgs e)
	{
		ShowLoginPanel();
	}
	#endregion

	#region Validation
	private bool ValidateInputs(string username, string password)
	{
		return !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password);
	}
	#endregion
}
