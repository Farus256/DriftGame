using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private Button joinButton;

    private RoomInfo roomInfo;
    private PhotonUIManager launcher;

    public void SetRoomInfo(RoomInfo info, PhotonUIManager manager)
    {
        roomInfo = info;
        launcher = manager;
        roomNameText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        if (roomInfo == null) return;
        launcher.JoinRoomByName(roomInfo.Name);
    }
}