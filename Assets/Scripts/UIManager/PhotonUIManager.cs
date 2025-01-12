using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PhotonUIManager : MonoBehaviourPunCallbacks
{
    public static PhotonUIManager Instance { get; private set; }
    
    public string SceneName;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Transform roomListContainer;
    [SerializeField] private RoomListItem roomListItemPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private MenuCarSpawner menuCarSpawner;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void OnMultiplayerButtonPressed()
    {
        CarStats chosenCar = menuCarSpawner.GetCurrentCarStats();
    
        if (chosenCar != null)
            CarSelection.SelectedCarId = chosenCar.ID;
        
        bool isNowActive = !multiplayerPanel.activeInHierarchy;
        multiplayerPanel.SetActive(isNowActive);
    
        if (isNowActive)
        {
            if (!PhotonNetwork.IsConnected)
                ConnectToPhoton();
        }
        else
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
    }


    private void ConnectToPhoton()
    {
        statusText.text = "Connecting to Photon...";
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected to Master. Joining Lobby...";
        PhotonNetwork.JoinLobby();
        if (startGameButton) startGameButton.gameObject.SetActive(false);
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby.";
        ClearRoomList();
    }

    public override void OnLeftLobby()
    {
        statusText.text = "Left Lobby.";
        ClearRoomList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    public override void OnCreatedRoom()
    {
        statusText.text = $"Room '{PhotonNetwork.CurrentRoom.Name}' created.";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Create Room Failed: {message}";
    }

    public override void OnJoinedRoom()
    {
        statusText.text = $"Joined Room: {PhotonNetwork.CurrentRoom.Name}";
        if (PhotonNetwork.IsMasterClient && startGameButton)
            startGameButton.gameObject.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Join Room Failed: {message}";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Disconnected: {cause}";
        ClearRoomList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (statusText)
            statusText.text = $"Room: {PhotonNetwork.CurrentRoom.Name} | Players: {PhotonNetwork.CurrentRoom.PlayerCount}";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (statusText)
            statusText.text = $"Room: {PhotonNetwork.CurrentRoom.Name} | Players: {PhotonNetwork.CurrentRoom.PlayerCount}";
    }

    public void OnCreateRoomButton()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Room name is empty!";
            return;
        }
        RoomOptions options = new RoomOptions { MaxPlayers = 2, IsVisible = true, IsOpen = true };
        PhotonNetwork.CreateRoom(roomNameInput.text, options, TypedLobby.Default);
    }

    public void OnJoinRoomButton()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Room name is empty!";
            return;
        }
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }

    public void OnStartGameButton()
    {
        multiplayerPanel.SetActive(false);
        statusText.text = "Loading GameScene...";
        PhotonNetwork.LoadLevel(SceneName);
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList || info.PlayerCount == info.MaxPlayers)
            {
                int index = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (index != -1) cachedRoomList.RemoveAt(index);
            }
            else
            {
                int index = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (index == -1) cachedRoomList.Add(info);
                else cachedRoomList[index] = info;
            }
        }
        RefreshRoomListUI();
    }

    private void ClearRoomList()
    {
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }
        cachedRoomList.Clear();
    }

    private void RefreshRoomListUI()
    {
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (RoomInfo info in cachedRoomList)
        {
            RoomListItem item = Instantiate(roomListItemPrefab, roomListContainer);
            item.SetRoomInfo(info, this);
        }
    }

    public void JoinRoomByName(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
