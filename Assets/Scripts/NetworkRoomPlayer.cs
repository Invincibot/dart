using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayer : Mirror.NetworkRoomPlayer
{
    private static Color _darkGreen = Color.Lerp(Color.black, Color.green, 0.5f);
    private static Color _darkRed = Color.Lerp(Color.black, Color.red, 0.5f);

    [SerializeField] private GameObject roomPlayerUIPrefab;

    private GameObject _roomPlayerUI;
    private Button _roomPlayerButton;
    private Text _roomPlayerText;
    private Image _roomPlayerImage;
    private Transform _playersPanel;

    public override void OnStartClient()
    {
        NetworkRoomManager networkRoomManager = ((NetworkRoomManager) NetworkManager.singleton);
        _playersPanel = networkRoomManager.playersPanel;
        networkRoomManager.startButton.gameObject.SetActive(isServer);
        
        if (!_roomPlayerUI) _roomPlayerUI = Instantiate(roomPlayerUIPrefab, _playersPanel);
        _roomPlayerButton = _roomPlayerUI.GetComponentInChildren<Button>();
        _roomPlayerImage = _roomPlayerUI.GetComponent<Image>();
        _roomPlayerText = _roomPlayerButton.GetComponentInChildren<Text>();
        _roomPlayerButton.interactable = isLocalPlayer;
        if (isLocalPlayer) _roomPlayerButton.onClick.AddListener(Ready);
        
        ReadyStateChanged(false, readyToBegin);
    }

    public override void OnStopClient()
    {
        if (_roomPlayerUI) Destroy(_roomPlayerUI);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        _roomPlayerImage.color = newReadyState ? _darkGreen : _darkRed;

        string text = "Ready";
        if (isLocalPlayer && newReadyState) text = "Cancel";
        if (!isLocalPlayer && !newReadyState) text = "Idle";
        _roomPlayerText.text = text;
    }

    private void Ready()
    {
        Debug.Log("ready" + readyToBegin);
        CmdChangeReadyState(!readyToBegin);
    }
}