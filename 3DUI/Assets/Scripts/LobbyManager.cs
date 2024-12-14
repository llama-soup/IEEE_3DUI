using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;

public class LobbyManager : MonoBehaviour
{
    public event System.Action<string> OnJoinCodeGenerated;
    private const int maxPlayers = 2;
    private const int maxRetries = 3;
    private string joinCode;
    private bool hasTransitioned = false;
    private bool isHosting = false;
    [SerializeField] private GameObject mainChatPrefab;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in successful");
                return;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError($"Authentication attempt {i + 1} failed: {ex.Message}");
            }
        }
        Debug.LogError($"Failed to sign in after {maxRetries} attempts");
    }

    public async void HostGame()
    {
        if (isHosting || NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host game already in progress or started.");
            return;
        }

        isHosting = true;

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log($"Host started. Join code: {joinCode}");
                OnJoinCodeGenerated?.Invoke(joinCode);
            }
            else
            {
                Debug.LogError("Failed to start host.");
                isHosting = false;
            }
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay service error: {ex.Message}");
            isHosting = false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error while hosting: {ex.Message}");
            isHosting = false;
        }
    }

    public async void JoinGame(string providedJoinCode)
    {
        Debug.Log($"Attempting to join game with code: {providedJoinCode}");
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(providedJoinCode);
            Debug.Log("Join allocation successful");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            Debug.Log("Relay server data set");

            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started");
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay service error: {ex.Message}.\nProvided Join Code: {providedJoinCode}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error: {ex.Message}.\nProvided Join Code: {providedJoinCode}");
        }
    }

    private void Update()
    {
        if (!hasTransitioned && NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            TransitionToGame();
            hasTransitioned = true;
        }
    }

    private void TransitionToGame()
    {
        Debug.Log("Transitioning to the Main Scene...");
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        Instantiate(mainChatPrefab);
    }
}