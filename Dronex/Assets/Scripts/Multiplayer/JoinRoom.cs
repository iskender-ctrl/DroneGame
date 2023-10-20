using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviourPunCallbacks
{
    [Header("Room List Panel")]
    [SerializeField] private Transform RoomListContent;

    [Header("Room Info Prefab")]
    [SerializeField] private GameObject roomInfoPrefab;
 
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListings(); // Önceki listeyi temizle

        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject roomListItem = Instantiate(roomInfoPrefab, RoomListContent);
            roomListItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = roomInfo.Name;
            roomListItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers;

            Button joinButton = roomListItem.transform.GetChild(2).GetComponent<Button>();
            // Butona bir dinleyici eklemek için onClick.AddListener kullanılır.
            joinButton.onClick.AddListener(() =>
            {
                // Odaya katılmak için kullanmak istediğiniz fonksiyonu burada çağırabilirsiniz.
                // Örneğin:
                PhotonNetwork.JoinRoom(roomInfo.Name);
            });
        }
    }

    void ClearRoomListings()
    {
        for (int i = RoomListContent.childCount - 1; i >= 0; i--)
        {
            Destroy(RoomListContent.GetChild(i).gameObject);
        }
    }
}
