using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
public class ClientNetworkAnimatora : NetworkAnimator
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

  
}
