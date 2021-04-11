using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("")]
public class NetworkRoomManager : Mirror.NetworkRoomManager
{
    public Transform playersPanel;
    public Button startButton;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject roomPanel;

    [SerializeField] private GameObject startPanel;
    private StartPanelController _startPanelController;
    [Range(1, 10)] public float freezeTime;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private Text winText;
    [SerializeField] private GameObject gameSpawnerPrefab;
    [SerializeField] private Text networkAddressText;

    private Dictionary<int, GameObject> _alivePlayers;
    private GameSpawner _gameSpawner;
    [SerializeField] private GameObject cameraPrefab;

    #region Room

    public override void Start()
    {
        base.Start();
        _startPanelController = startPanel.GetComponent<StartPanelController>();
    }

    public override void OnRoomClientConnect(NetworkConnection conn)
    {
        menuPanel.SetActive(false);
        roomPanel.SetActive(true);
        networkAddressText.text = networkAddress;
        startButton.onClick.AddListener(StartGame);
        startButton.interactable = false;
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        if (networkSceneName != GameplayScene) return;

        _alivePlayers = new Dictionary<int, GameObject>();
        _gameSpawner = Instantiate(gameSpawnerPrefab).GetComponent<GameSpawner>();
        _gameSpawner.SpawnBounds();
        _gameSpawner.SpawnMeteors();
        
        Invoke(nameof(EnablePlayers), freezeTime);
    }

    private void EnablePlayers()
    {
        foreach (GameObject player in _alivePlayers.Values)
        {
            player.GetComponent<PlayerController>().controlsEnabled = true;
        }
    }

    public override void OnRoomClientSceneChanged(NetworkConnection conn)
    {
        roomPanel.SetActive(networkSceneName == RoomScene);
        winPanel.SetActive(false);

        if (networkSceneName != GameplayScene) return;
        Instantiate(cameraPrefab);

        startPanel.SetActive(true);
        _startPanelController.SetEnemyText(numPlayers - 1);

        Invoke(nameof(DisableStartScreen), freezeTime);
    }

    private void DisableStartScreen()
    {
        startPanel.SetActive(false);
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        GameObject player = _gameSpawner.SpawnPlayer();
        NetworkRoomPlayer networkRoomPlayer = roomPlayer.GetComponent<NetworkRoomPlayer>();
        int index = networkRoomPlayer.index;
        _alivePlayers.Add(index, player);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.controlsEnabled = false;
        playerController.index = index;
        
        return player;
    }

    public override void OnRoomServerPlayersReady()
    {
        if (numPlayers < minPlayers) return;
        startButton.interactable = true;
    }

    public override void OnRoomServerPlayersNotReady()
    {
        startButton.interactable = false;
    }

    private void StartGame()
    {
        if (!allPlayersReady) return;
        ServerChangeScene(GameplayScene);
    }

    public void ExitLobby()
    {
        menuPanel.SetActive(true);
        roomPanel.SetActive(false);
        StopClient();
        StopServer();
    }

    #endregion

    #region Game

    [Server]
    public void PlayerDead(int playerIndex)
    {
        _alivePlayers.Remove(playerIndex);
        if (_alivePlayers.Count != 1) return;

        DisplayGameWinScreen(true);
        Invoke(nameof(LoadRoomScene), 5f);
    }

    [Server]
    private void LoadRoomScene()
    {
        DisplayGameWinScreen(false);
        ServerChangeScene(RoomScene);
    }

    [Client]
    private void DisplayGameWinScreen(bool display)
    {
        winPanel.SetActive(display);
        winText.text = "Player " + (_alivePlayers[0].GetComponent<PlayerController>().index + 1) + " Wins!";
    }

    #endregion
}