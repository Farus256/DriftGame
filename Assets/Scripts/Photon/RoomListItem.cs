using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Button joinButton;

    private RoomInfo _roomInfo;
    private PhotonUIManager _launcher;

    public void SetRoomInfo(RoomInfo info, PhotonUIManager launcher)
    {
        _roomInfo = info;
        _launcher = launcher;

        // Пример текста: "MyRoom (1/2)"
        roomNameText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() => OnJoinButtonClicked());
    }

    private void OnJoinButtonClicked()
    {
        if (_roomInfo == null) return;
        // Вызов метода из LauncherUI
        _launcher.JoinRoomByName(_roomInfo.Name);
    }
}