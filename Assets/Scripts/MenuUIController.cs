using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private GameObject networkAddressPanel;

    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private InputField networkAddressInputField;

    [SerializeField] private NetworkRoomManager networkRoomManager;

    private void OnEnable()
    {
        landingPagePanel.SetActive(true);
        networkAddressPanel.SetActive(false);
        joinButton.SetActive(true);
        cancelButton.SetActive(false);
    }

    private void Start()
    {
        networkAddressInputField.onValueChanged.AddListener(UpdateNetworkAddress);
    }

    public void JoinLobby()
    {
        landingPagePanel.SetActive(false);
        networkAddressPanel.SetActive(true);
        joinButton.SetActive(true);
        cancelButton.SetActive(false);
    }

    public void LeaveLobby()
    {
        landingPagePanel.SetActive(true);
        networkAddressPanel.SetActive(false);
    }

    public void StartClient()
    {
        joinButton.SetActive(false);
        cancelButton.SetActive(true);
        networkRoomManager.StartClient();
    }

    public void StopClient()
    {
        joinButton.SetActive(true);
        cancelButton.SetActive(false);
        networkRoomManager.StopClient();
    }

    private void UpdateNetworkAddress(string value)
    {
        networkRoomManager.networkAddress = value;
    }
}