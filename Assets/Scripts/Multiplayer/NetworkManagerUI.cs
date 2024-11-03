using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    private SpawnMP spawnMP;

    [SerializeField] public Button hostBtn;
    [SerializeField] public Button clientBtn;
    [SerializeField] public Button startBtn;

    public NetworkVariable<bool> hostGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> clientGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> startGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        spawnMP = GameObject.Find("SpawnManager").GetComponent<SpawnMP>();

        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            hostBtn.gameObject.SetActive(false);
            clientBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(true);
            hostGame.Value = true;
            Debug.Log("Client ID " + OwnerClientId);
            Debug.Log("IsServer " + IsServer);
        });

        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            clientBtn.gameObject.SetActive(false);
            hostBtn.gameObject.SetActive(false);
            clientGame.Value = true;
            Debug.Log("Client ID " + OwnerClientId);
            Debug.Log("IsServer " + IsServer);
        });

        startBtn.onClick.AddListener(() => {
            clientBtn.gameObject.SetActive(false);
            hostBtn.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(false);
            spawnMP.InvokeSpawn();
        });




    }
}
