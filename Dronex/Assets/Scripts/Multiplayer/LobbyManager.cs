using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby Panel")]
    [SerializeField] GameObject lobbyPanel;

    [Header("Lobby Content")]
    [SerializeField] Transform lobbyContent;

    [Header("Player Name Prefab")]
    [SerializeField] GameObject playerNamePrefab;

    [Header("Play Button")]
    [SerializeField] Button playButton;

    [Header("Loading Scene Name")]
    [SerializeField] private string loadingSceneName;
    private void Start()
    {
        playButton.onClick.AddListener(() => LoadScene(loadingSceneName));
    }
    public override void OnCreatedRoom()
    {
        lobbyPanel.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            playButton.gameObject.SetActive(true);
        }
    }
    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
    }
    private void UpdatePlayerList()
    {
        // Oyundaki oyuncu listesini güncellemek için bu fonksiyonu kullanabilirsiniz.
        // lobbyContent içindeki oyuncu isimlerini temizleyip yeniden ekleyebilirsiniz.
        lobbyPanel.SetActive(true);

        ClearLobbyList();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerNameObject = Instantiate(playerNamePrefab, lobbyContent);
            TextMeshProUGUI playerNameText = playerNameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerNameText.text = player.NickName;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Bir oyuncu odaya katıldığında bu fonksiyon çalışır.
        // Yeni oyuncuyu oyuncu listesine eklemek için burada gerekli işlemleri yapabilirsiniz.

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Bir oyuncu odadan ayrıldığında bu fonksiyon çalışır.
        // Ayrılan oyuncuyu oyuncu listesinden çıkartmak için burada gerekli işlemleri yapabilirsiniz.

        UpdatePlayerList();
    }

    private void ClearLobbyList()
    {
        // Mevcut lobby listesini temizleyin
        foreach (Transform child in lobbyContent)
        {
            Destroy(child.gameObject);
        }
    }
    public void LoadScene(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Sahneyi yüklemeden önce diğer oyunculara yükleme işlemini iletmek için Photon ağı üzerinden senkronizasyon yapın.
            // PhotonMasterClient sahneyi yükler ve diğer oyuncuların senkronize olmasını bekler.
            PhotonNetwork.LoadLevel(loadingSceneName);
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(1); // İsteğe bağlı bir bekleme süresi ekleyebilirsiniz.

        // Diğer sahneye geçiş işlemini gerçekleştirin.
        PhotonNetwork.LoadLevel(sceneName);
    }
}
