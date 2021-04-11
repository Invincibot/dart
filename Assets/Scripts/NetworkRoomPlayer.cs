using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayer : Mirror.NetworkRoomPlayer
{
    private static readonly Color DarkGreen = Color.Lerp(Color.black, Color.green, 0.5f);
    private static readonly Color DarkRed = Color.Lerp(Color.black, Color.red, 0.5f);

    [SerializeField] private GameObject roomPlayerUIPrefab;

    private GameObject _roomPlayerUI;
    private Button _roomPlayerButton;
    private Text _roomPlayerText;
    private Image _roomPlayerImage;
    private Transform _playersPanel;

    public override void OnStartClient()
    {
        NetworkRoomManager networkRoomManager = (NetworkRoomManager) NetworkManager.singleton;
        _playersPanel = networkRoomManager.playersPanel;
        
        if (!_roomPlayerUI) _roomPlayerUI = Instantiate(roomPlayerUIPrefab, _playersPanel);
        _roomPlayerButton = _roomPlayerUI.GetComponentInChildren<Button>();
        _roomPlayerImage = _roomPlayerUI.GetComponent<Image>();
        _roomPlayerText = _roomPlayerButton.GetComponentInChildren<Text>();
        _roomPlayerButton.interactable = isLocalPlayer;
        if (!isLocalPlayer) return;
        _roomPlayerButton.onClick.AddListener(Ready);
    }

    public override void OnClientEnterRoom()
    {
        NetworkRoomManager networkRoomManager = (NetworkRoomManager) NetworkManager.singleton;
        networkRoomManager.startButton.gameObject.SetActive(isServer);
        _roomPlayerUI.GetComponentInChildren<Text>().text = "Player " + (index + 1);
        ReadyStateChanged(false, readyToBegin);
    }

    public override void OnStopClient()
    {
        if (_roomPlayerUI) Destroy(_roomPlayerUI);
    }

    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        _roomPlayerImage.color = newReadyState ? DarkGreen : DarkRed;

        string text = "Ready";
        if (isLocalPlayer && newReadyState) text = "Cancel";
        if (!isLocalPlayer && !newReadyState) text = "Idle";
        _roomPlayerText.text = text;
    }

    private void Ready()
    {
        CmdChangeReadyState(!readyToBegin);
    }
}