using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour    
{
    [SerializeField] private Button serverbtn;
    [SerializeField] private Button hostbtn;
    [SerializeField] private Button clientbtn;
    [SerializeField] private TextMeshProUGUI player_NumOfText;

    private NetworkVariable<int> numOfPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake()
    {
        serverbtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        }
            );
        hostbtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        }
            );
        clientbtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        }
            );
    }

    private void Update()
    {
        player_NumOfText.text = numOfPlayers.Value.ToString() + " CONNECTED PLAYERS";
        if (!IsServer) return;
        numOfPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}
