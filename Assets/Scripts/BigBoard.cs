using UnityEngine;
using TMPro;
using Unity.Netcode;
public class BigBoard : MonoBehaviour
{
    [SerializeField] private TextMeshPro bestTimeTextP1;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
        UpdateBestTimeDisplay();
    }

    private void UpdateBestTimeDisplay()
    {
        var player1 = FindPlayerByClientId(0);
        var player2 = FindPlayerByClientId(1);
        if (player1 != null && player1.bestTime.Value != 0 && player1.bestTime.Value < player2.bestTime.Value)
        {
            bestTimeTextP1.text = "Best time: " + player1.bestTime.Value.ToString("F2") + "s";
        }
        if (player2 != null && player2.bestTime.Value!=0 && player2.bestTime.Value < player1.bestTime.Value)
        {
            bestTimeTextP1.text = "Best time: " + player2.bestTime.Value.ToString("F2") + "s";
        }

    }
    private Player FindPlayerByClientId(ulong clientId)
    {
        foreach (var player in NetworkManager.Singleton.ConnectedClients.Values)
        {
            if (player.ClientId == clientId)
            {
                return player.PlayerObject.GetComponent<Player>();
            }
        }

        return null;
    }
}
