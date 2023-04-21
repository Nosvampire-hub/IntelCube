using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerLobby : MonoBehaviour
{
	private Lobby hostLobby;
	private Lobby joinedLobby;
    private GameObject relayService;
	private float heartBeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;
    private string relayCode;
	// Start is called before the first frame update
	private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
		playerName = "Coroutine" + UnityEngine.Random.Range(10000, 99999);
		Debug.Log(playerName);

        relayService = GameObject.Find("NetworkManager");
    
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();

	}

    IEnumerator StartPlayingOnline(float halt)
    {
		yield return new WaitForSeconds(halt);
        JoinLobby();
	}

    private async void HandleLobbyPollForUpdates()
    {
		if (joinedLobby != null)
		{
			lobbyUpdateTimer -= Time.deltaTime;
			if (lobbyUpdateTimer < 0f)
			{
				float lobbyUpdateTimerMax = 1.1f;
				lobbyUpdateTimer = lobbyUpdateTimerMax;

				Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
		}
	}

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartBeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby()
    {
        try 
        {
            relayCode = await relayService.GetComponent<PlayerRelay>().CreateRelay();
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                    }
                },
                Data = new Dictionary<string, DataObject>
                {
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                }
            };

			Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("IntelCube", 4, createLobbyOptions);

            hostLobby = lobby;

            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
            
            Debug.Log("Lobby Created");

		}
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void MatchMaking()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
				Count = 25,
				Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0" ,QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
			};

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("Lobbies found:" + queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }

            if (queryResponse.Results.Count > 0)
            {
				JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
				{
					Player = new Player
					{
						Data = new Dictionary<string, PlayerDataObject>
					{
						{"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
					}
					}
				};

				Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, joinLobbyByIdOptions);

				joinedLobby = lobby;

				string relayCode = joinedLobby.Data["RelayCode"].Value;

				Debug.Log(" Joining Lobby with " + relayCode);

				relayService.GetComponent<PlayerRelay>().JoinRelay(relayCode);

				PrintPlayers(joinedLobby);
			}
			else
            {
                CreateLobby();
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);   
        }
    }

    public async void JoinLobby()
    {
        try
        {
			QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                    }
                }
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id,joinLobbyByIdOptions);

            joinedLobby = lobby;

            string relayCode = joinedLobby.Data["RelayCode"].Value;
			
            Debug.Log( " Joining Lobby with " + relayCode);

			relayService.GetComponent<PlayerRelay>().JoinRelay(relayCode);

			//PrintPlayers(joinedLobby);
		}
        catch (LobbyServiceException e)
		{
            Debug.Log(e);
        }
        
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby" + lobby.Name + "" + lobby.Data["RelayCode"].Value);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }
    private void PrintPLayers1()
    {
        PrintPlayers(joinedLobby);
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}
	}

    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

		}
		catch (LobbyServiceException e)
		{
			Debug.Log(e);
		}

	}
}
