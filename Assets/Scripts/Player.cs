using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Player : NetworkBehaviour
{
    public NetworkVariable<float> currentTime = new(0f);
    public NetworkVariable<float> bestTime = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<bool> isTimerRunning = new(false);
    [SerializeField] private CinemachineVirtualCamera vc;
    [SerializeField] private AudioListener audioListener;
    private CharacterController characterController;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        characterController = GetComponent<CharacterController>();

        if (IsOwner)
        {
            UpdateBestTimeServerRpc(0f);
            audioListener.enabled = true;
            vc.Priority = 1;
        }
        else
        {
            vc.Priority = 0;
            audioListener.enabled = false;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsOwner) return;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isTimerRunning.Value && IsOwner)
        {
            StartTimerServerRpc();
            TryStartGame();
        }

        if (isTimerRunning.Value)
        {
            if (IsServer)
            {
                currentTime.Value += Time.deltaTime;
                UpdateCurrentTimeServerRpc(currentTime.Value);
            }
        }
        
            CheckStopTimerCollision();
        
    }

    void StopTimer()
    {
        if (isTimerRunning.Value)
        {
            StopTimerServerRpc();
        }
    }

    void TryStartGame()
    {
        var ground = FindObjectsByType<Floor>(FindObjectsSortMode.None);
        if (ground.Length > 0)
        {
            var foundGround = ground[0];
            foundGround.GroundDisappearServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.Log("No Ground found");
        }
    }

    private void CheckStopTimerCollision()
    {
        float detectionRadius = 2f;
        Collider[] hitColliders = Physics.OverlapSphere(characterController.bounds.center, detectionRadius);

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Finish"))
            {
                StopTimer();
               
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateCurrentTimeServerRpc(float newTime)
    {
        currentTime.Value = newTime;
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateBestTimeServerRpc(float playerCurrentTime)
    {
        
        
        if (bestTime.Value == 99f || playerCurrentTime < bestTime.Value && bestTime.Value!=0f)
        {
            bestTime.Value = playerCurrentTime;
         
        }
        if (bestTime.Value == 0f)
        {
            bestTime.Value = 99f;
           
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void StartTimerServerRpc()
    {
        if (!isTimerRunning.Value)
        {
            isTimerRunning.Value = true;
            ResetTimerServerRpc();
            StartTimerClientRpc();
        }
    }

    [ClientRpc]
    void StartTimerClientRpc()
    {
        isTimerRunning.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    void StopTimerServerRpc()
    {
        isTimerRunning.Value = false;
        StopTimerClientRpc();
        UpdateBestTimeServerRpc(currentTime.Value);
    }
    [ClientRpc]
    void StopTimerClientRpc()
    {
        isTimerRunning.Value = false;
    }
    [ServerRpc(RequireOwnership = false)]
    void ResetTimerServerRpc()
    {
        currentTime.Value = 0f;
        ResetTimerClientRpc();
    }
    [ClientRpc]
    void ResetTimerClientRpc()
    {
        currentTime.Value = 0f;
    }
}