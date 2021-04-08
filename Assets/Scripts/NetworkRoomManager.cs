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
    [SerializeField] private GameObject gameSpawnerPrefab;
    [SerializeField] private Text networkAddressText;
    
    private GameSpawner _gameSpawner;

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
        if (sceneName != GameplayScene) return;
        
        _gameSpawner = Instantiate(gameSpawnerPrefab).GetComponent<GameSpawner>();
        _gameSpawner.SpawnBounds();
        _gameSpawner.SpawnMeteors();
    }

    public override void OnRoomClientSceneChanged(NetworkConnection conn)
    {
        if (networkSceneName != GameplayScene) return;
        
        roomPanel.SetActive(false);
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        return _gameSpawner.SpawnPlayer();
    }

    public override void OnRoomServerPlayersReady()
    {
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
}