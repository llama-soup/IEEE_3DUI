using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    public event System.Action<string> OnJoinCodeGenerated;
    private const int maxPlayers = 2;
    private const int maxRetries = 3;
    private string joinCode;

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

            NetworkManager.Singleton.StartHost();
            Debug.Log($"Host started. Join code: {joinCode}");

            // Trigger the event with the join code
            OnJoinCodeGenerated?.Invoke(joinCode);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay service error: {ex.Message}");
        }
    }

    public async void JoinGame(string providedJoinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(providedJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started");
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError($"Relay service error: {ex.Message}");
        }
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            TransitionToGame();
        }
    }

    private void TransitionToGame()
    {
        Debug.Log("Transitioning to the Main Scene...");
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}