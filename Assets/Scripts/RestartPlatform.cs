using UnityEngine;
using Unity.Netcode;

public class RestartPlatform : NetworkBehaviour
{
    [SerializeField] private Vector3 spawn = Vector3.zero;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsOwner && Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("teleport zone.");
                TeleportServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportServerRpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                TeleportClientRpc(playerObject.NetworkObjectId, spawn);
            }
        }
    }

    [ClientRpc]
    private void TeleportClientRpc(ulong playerNetworkObjectId, Vector3 spawn)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerNetworkObjectId, out var playerObject))
        {
            var characterController = playerObject.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false; //reset velocity
                playerObject.transform.position = spawn;
                characterController.enabled = true;
            }
            else
            {
                playerObject.transform.position = spawn; // sometimes it doesnt work?
            }
        }
    }
}
