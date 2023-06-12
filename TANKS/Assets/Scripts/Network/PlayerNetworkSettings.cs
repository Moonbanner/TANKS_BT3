using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerNetworkSettings : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNumber;
    private NetworkVariable<FixedString128Bytes> networkPlayerNumber = new NetworkVariable<FixedString128Bytes>
        ("Player 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        networkPlayerNumber.Value = "Player " + (OwnerClientId + 1);
        playerNumber.text = networkPlayerNumber.Value.ToString();
    }
}
