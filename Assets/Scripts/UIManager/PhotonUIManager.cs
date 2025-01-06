using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Пример скрипта, управляющего подключением к Photon,
/// созданием/входом в комнаты, а также показом списка доступных комнат.
/// </summary>
public class PhotonUIManager : MonoBehaviourPunCallbacks
{
    public string SceneName;
    
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;     // Поле ввода имени комнаты
    [SerializeField] private TMP_Text statusText;              // Вывод статуса
    [SerializeField] private Transform roomListContainer;      // Контейнер (LayoutGroup) для списка комнат
    [SerializeField] private RoomListItem roomListItemPrefab;  // Префаб UI-элемента для комнаты
    [SerializeField] private Button startGameButton;           // Кнопка "Start Game" (можем активировать по необходимости)

    [SerializeField] private GameObject multiplayerPanel;
    
    // Храним список комнат в кэше
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();


    public void OnMultiplayerButtonPressed()
    {
        ShowMultiplayerMenu();
        //Connect();
    }

    private void ShowMultiplayerMenu()
    {
        multiplayerPanel.SetActive(true);
    }
    private void Start()
    {
        statusText.text = "Connecting to Photon...";
        // Подключаемся к Photon Cloud (AppId & Region в PhotonServerSettings)
        PhotonNetwork.ConnectUsingSettings();
        
        // OPTIONAL: мы можем чаще вызывать обновление списка комнат
        // по таймеру, если хотим. Но обычно Photon сам шлёт через OnRoomListUpdate.
        PhotonNetwork.AutomaticallySyncScene = true;
       // InvokeRepeating(nameof(RequestRoomList), 5f, 5f);
    }

    /// <summary>
    /// Принудительно запрашивать обновлённый список комнат (не всегда нужно).
    /// </summary>
    private void RequestRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            // В PUN 2 нет прямого метода "RequestRoomList", 
            // Photon автоматически обновляет, но для Fusion — есть. 
            // В PUN 2 мы обычно rely on OnRoomListUpdate.  
            // Можно ре-join лобби, но это грубо. 
            statusText.text = "Requesting room list (PUN auto-updates though)...";
        }
        else
        {
            statusText.text = "Not in lobby, can't request list!";
        }
    }

    // =================================================================================
    //                       PHOTON PUN CALLBACKS
    // =================================================================================

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected to Master. Joining Lobby...";
        // Присоединяемся к лобби, чтобы видеть список комнат
        PhotonNetwork.JoinLobby();

        // Разрешаем синхронную загрузку сцен (если хотим в будущем LoadLevel)
        //PhotonNetwork.AutomaticallySyncScene = false;

        // Скрываем/деактивируем кнопку Start Game, пока не вошли в комнату
        if (startGameButton != null) startGameButton.gameObject.SetActive(false);
    }

    // Когда мы действительно в лобби
    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby. You can create or join a room...";
        ClearRoomList();
    }
        
    // Когда мы **вышли** из лобби (или потеряли подключение)
    public override void OnLeftLobby()
    {
        statusText.text = "Left Lobby.";
        ClearRoomList();
    }
    
    // Если не удалось подключиться к лобби
    public void OnJoinLobbyFailed(short returnCode, string message)
    {
        statusText.text = $"Join Lobby Failed: {message}";
    }

    // Список комнат в лобби обновился (каждый раз, когда меняется список)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    // Когда мы создаём комнату и она успешно создана
    public override void OnCreatedRoom()
    {
        statusText.text = $"Room '{PhotonNetwork.CurrentRoom.Name}' created. Waiting for others...";
    }

    // Если создать комнату не удалось (например, уже существует)
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Create Room Failed: {message}";
    }

    // Когда мы **вошли** в комнату (после CreateRoom или JoinRoom)
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        statusText.text = $"Joined Room: {PhotonNetwork.CurrentRoom.Name}. Players: {PhotonNetwork.CurrentRoom.PlayerCount}";

        // Если сцена уже загружается (например, хост уже вызвал LoadLevel), клиент догрузит её
        if (PhotonNetwork.IsMasterClient && startGameButton != null)
        {
            startGameButton.gameObject.SetActive(true);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log($"[PhotonUIManager] Client will sync scene automatically if needed.");
        }
    }


    // Если не удалось зайти в комнату
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = $"Join Room Failed: {message}";
    }

    // Если нас выкинуло
    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Disconnected: {cause}";
        ClearRoomList();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"[PhotonUIManager] Player joined: {newPlayer.NickName}, count: {PhotonNetwork.CurrentRoom.PlayerCount}");
    
        // Обновляем текст
        if (statusText != null)
        {
            statusText.text = 
                $"Room: {PhotonNetwork.CurrentRoom.Name}. Players: {PhotonNetwork.CurrentRoom.PlayerCount}";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"[PhotonUIManager] Player left: {otherPlayer.NickName}, count: {PhotonNetwork.CurrentRoom.PlayerCount}");
    
        // Обновляем текст
        if (statusText != null)
        {
            statusText.text = 
                $"Room: {PhotonNetwork.CurrentRoom.Name}. Players: {PhotonNetwork.CurrentRoom.PlayerCount}";
        }
    }
    
    // =================================================================================
    //                   UI BUTTON HANDLERS
    // =================================================================================

    /// <summary>
    /// Нажатие кнопки "Create Room"
    /// </summary>
    public void OnCreateRoomButton()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Room name is empty!";
            return;
        }
        statusText.text = $"Creating room '{roomNameInput.text}' ...";

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2, // например, 2
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomNameInput.text, options, TypedLobby.Default);
    }

    /// <summary>
    /// Нажатие кнопки "Join Room By Name"
    /// (если хотим вручную вводить имя и подключаться)
    /// </summary>
    public void OnJoinRoomButton()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            statusText.text = "Room name is empty!";
            return;
        }
        statusText.text = $"Joining room '{roomNameInput.text}' ...";
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }

    /// <summary>
    /// Нажатие кнопки "Start Game" - грузим следующую сцену
    /// </summary>
    public void OnStartGameButton()
    {
        // if (!PhotonNetwork.InRoom)
        // {
        //     statusText.text = "Not in a room, can't start!";
        //     return;
        // }
        //
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     statusText.text = "Only MasterClient can start the game!";
        //     return;
        // }
        
        multiplayerPanel.SetActive(false);
        
        Debug.Log($"MasterClient? {PhotonNetwork.IsMasterClient}, " + 
                  $"InRoom? {PhotonNetwork.InRoom}, " +
                  $"AutomaticallySyncScene? {PhotonNetwork.AutomaticallySyncScene}");
        
        statusText.text = "Loading GameScene...";
        PhotonNetwork.LoadLevel(SceneName);
    }

    // =================================================================================
    //                   ROOM LIST UPDATING
    // =================================================================================

    /// <summary>
    /// Обновляем список доступных комнат (OnRoomListUpdate)
    /// </summary>
    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        // 1) Обновляем кэш
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList || info.PlayerCount == info.MaxPlayers)
            {
                // Удаляем из кэша
                int index = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (index != -1)
                {
                    cachedRoomList.RemoveAt(index);
                }
            }
            else
            {
                // Добавляем или обновляем
                int index = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (index == -1)
                {
                    // Новая
                    cachedRoomList.Add(info);
                }
                else
                {
                    cachedRoomList[index] = info;
                }
            }
        }
        // 2) Обновляем UI
        RefreshRoomListUI();
    }

    /// <summary>
    /// Очищаем UI-список комнат
    /// </summary>
    private void ClearRoomList()
    {
        // Удаляем все объекты в container
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }
        cachedRoomList.Clear();
    }

    /// <summary>
    /// Перерисовываем UI-список комнат из cachedRoomList
    /// </summary>
    private void RefreshRoomListUI()
    {
        // Сначала очищаем ScrollView
        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        // Для каждой комнаты в списке создаём UI-элемент
        foreach (RoomInfo info in cachedRoomList)
        {
            RoomListItem item = Instantiate(roomListItemPrefab, roomListContainer);
            item.SetRoomInfo(info, this);
        }
    }

    /// <summary>
    /// Метод, который вызывается RoomListItem,
    /// когда пользователь нажимает "Join" на конкретной комнате.
    /// </summary>
    public void JoinRoomByName(string roomName)
    {
        statusText.text = $"Joining room: {roomName}";
        PhotonNetwork.JoinRoom(roomName);
    }
}
