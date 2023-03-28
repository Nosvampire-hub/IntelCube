using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;

public class UnityAuthentication : MonoBehaviour
{

	public bool loginSuccess = false;
	public string userName = "";

	private async void Start()
	{
		await UnityServices.InitializeAsync();

		Debug.Log(UnityServices.State);

		await SignInAnonymouslyAsync();
		
	}

	// Setup authentication event handlers if desired
	void SetupEvents()
	{
		AuthenticationService.Instance.SignedIn += () => {
			// Shows how to get a playerID
			Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

			// Shows how to get an access token
			Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

		};

		AuthenticationService.Instance.SignInFailed += (err) => {
			Debug.LogError(err);
		};

		AuthenticationService.Instance.SignedOut += () => {
			Debug.Log("Player signed out.");
		};

		AuthenticationService.Instance.Expired += () =>
		{
			Debug.Log("Player session could not be refreshed and expired.");
		};

	}

	async Task SignInAnonymouslyAsync()
	{
		try
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			Debug.Log("Sign in anonymously succeeded!");

			// Shows how to get the playerID
			Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

			userName = $"PlayerID: {AuthenticationService.Instance.PlayerId}";

			GameObject Username = GameObject.Find("NameTag");
			TextMeshPro userText = Username.GetComponent<TextMeshPro>();
			userName = Social.localUser.userName;
			userText.SetText(userName);

		}
		catch (AuthenticationException ex)
		{
			// Compare error code to AuthenticationErrorCodes
			// Notify the player with the proper error message
			Debug.LogException(ex);
		}
		catch (RequestFailedException ex)
		{
			// Compare error code to CommonErrorCodes
			// Notify the player with the proper error message
			Debug.LogException(ex);
		}
	}
}
