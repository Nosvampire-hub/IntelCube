using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

public class PlayerRelay : MonoBehaviour
{
	public async Task<string> CreateRelay()
	{
		try
		{
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			Debug.Log(joinCode);

			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

			NetworkManager.Singleton.StartHost();

			return joinCode;
		} 
		catch (RelayServiceException e)
		{
			Debug.Log(e);
		}

		return "null";
	}

	public async void JoinRelay(string joinCode)
	{
		Debug.Log("Joining Lobby with code: " + joinCode);
		try
		{
			Debug.Log("Joining Relay");

			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartClient();
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
		}
	}
}
