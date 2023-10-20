using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnConnectedToMaster()
    {
        print("Connected to the server.");

        // Rastgele bir isim alın
        string playerName = GenerateRandomPlayerName();

        // Kontrol edin ve eşsiz bir isim olduğundan emin olun
        playerName = EnsureUniquePlayerName(playerName);

        // Oyuncu ismini ayarlayın
        PhotonNetwork.NickName = playerName;

        GetComponent<CreateRoom>().canCreateRoom = true;
        PhotonNetwork.JoinLobby();
    }

    string GenerateRandomPlayerName()
    {
        // Örnek bir isim listesi
        string[] randomNames = { "Player1", "Player2", "Player3", "Player4", "Player5" };

        // Rastgele bir isim seçin
        string randomName = randomNames[Random.Range(0, randomNames.Length)];

        return randomName;
    }

    string EnsureUniquePlayerName(string playerName)
    {
        int suffix = 1;
        string originalName = playerName;

        // İsim eşsiz olana kadar döngü
        while (PlayerNameExists(playerName))
        {
            playerName = originalName + suffix;
            suffix++;
        }

        return playerName;
    }

    bool PlayerNameExists(string playerName)
    {
        // Oyuncu ismi mevcut mu kontrolünü burada yapabilirsiniz
        // Eğer isim zaten kullanılmışsa true dönmelidir
        // Aksi takdirde false dönmelidir
        // Örnek: return CheckIfNameExistsInDatabase(playerName);

        return false; // Varsayılan olarak isim mevcut değil olarak döner
    }
}
