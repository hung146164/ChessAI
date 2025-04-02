using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEngine;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    public GameManager gameManager;

    public MenuManager lobby;

    public MenuManager game;
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private TMP_InputField findRoomNameInput;
    [SerializeField] private TMP_Dropdown teamSelectOnline,teamSelectOffline;

    [Header("")]
    [SerializeField] private Transform StartGameButton;

    [SerializeField] private PlayerListContentString playerListContent;

    [SerializeField] private TMP_Text roomName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConnectToServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            lobby.OpenMenu("loading");
            PhotonNetwork.NickName = $"Player {Random.Range(1000, 9999)}";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            lobby.OpenMenu("online");
        }

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        lobby.OpenMenu("online");
    }

    public void CreateRoom()
    {
        string roomName = createRoomNameInput.text;

        lobby.OpenMenu("loading");

        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions
            {
                MaxPlayers = 2,
                IsVisible = true,
                IsOpen = true,
                EmptyRoomTtl = 0
            });
        }
        else
        {
            lobby.OpenMenu("createroom");
        }
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Faild");
        lobby.OpenMenu("createroom");
    }

    public void FindRoom()
    {
        PhotonNetwork.JoinRoom(findRoomNameInput.text);
        lobby.OpenMenu("loading");
        
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Find Fail!");
        lobby.OpenMenu("findroom");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        lobby.OpenMenu("room");

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        playerListContent.ClearListContent();
        playerListContent.UpdateListContent(PhotonNetwork.PlayerList.Select(player => player.NickName).ToList());
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerListContent.AddItem(newPlayer.NickName);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerListContent.RemoveItem(otherPlayer.NickName);
    }
    public override void OnLeftRoom()
    {
        lobby.OpenMenu("online");
    }
    public void ExitRoom()
    {
        lobby.OpenMenu("loading");
        PhotonNetwork.LeaveRoom();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC("SetTeam", RpcTarget.Others, Mathf.Abs(teamSelectOnline.value - 1));
            SetTeam(teamSelectOnline.value);

            photonView.RPC("StartGameOnline", RpcTarget.All);

        }
    }
    [PunRPC]
    private void StartGameOnline()
    {
        lobby.gameObject.SetActive(false);
        gameManager.chessBoard.gameObject.SetActive(true);
        gameManager.gameObject.SetActive(true);

        gameManager.isOnline = true;
        game.gameObject.SetActive(true);

    }
    [PunRPC]
    private void SetTeam(int team)
    {
        gameManager.SetTeam(team==1?TeamColor.Black:TeamColor.White);
    }
    public void StartGameOffline()
    {
        lobby.gameObject.SetActive(false);
        gameManager.chessBoard.gameObject.SetActive(true);
        gameManager.gameObject.SetActive(true);

        gameManager.isOnline = false;
        gameManager.SetTeam(teamSelectOffline.value == 0 ? TeamColor.White : TeamColor.Black);
        game.gameObject.SetActive(true);
    }

}