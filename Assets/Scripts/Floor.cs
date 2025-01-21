using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Floor : NetworkBehaviour
{
    [SerializeField] GameObject groundPrefab;
    private GameObject ground;
    private bool isGroundSpawned = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            SpawnGround();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GroundDisappearServerRpc(ulong clientId)
    {
        Debug.Log("GroundDisappearServerRpc called by Client ID: " + clientId);
        if (ground != null && isGroundSpawned)
        {
            ground.GetComponent<NetworkObject>().Despawn();
            isGroundSpawned = false;

            StartCoroutine(RespawnGround(3f));
        }
    }

    IEnumerator RespawnGround(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpawnGround();
        Debug.Log("Ground Respawned");
    }

    private void SpawnGround()
    {
        ground = Instantiate(groundPrefab, transform);
        ground.GetComponent<NetworkObject>().Spawn();
        isGroundSpawned = true;
    }
}
