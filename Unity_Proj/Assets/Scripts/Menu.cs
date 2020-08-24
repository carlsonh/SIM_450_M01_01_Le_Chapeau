using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;



public class Menu : MonoBehaviourPunCallbacks
{


    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    // Start is called before the first frame update
    void Start()
    {///On Start, disable buttons until connected to director server
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster ()
    {///Have connected to director, allow access to server controls
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }



    void SetScreen (GameObject screen)
    {
        //Deactivate all screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //Enable the requested screen
        screen.SetActive(true);
    }


    #region MainScreen Buttons
    public void OnCreateRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public void OnJoinRoomButton (TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }
    #endregion MainScreen Buttons




    #region LobbyScreen Buttons
    public void OnLeaveLobbyButton ()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton ()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameScene");
    }
    #endregion LobbyScreen Buttons



    public void OnPlayerNameUpdate (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
        Debug.Log(playerNameInput.text);
    }

    public override void OnPlayerLeftRoom (Photon.Realtime.Player otherPlayer)
    {//Called when any player leaves the room, OnJoinRoom only called on joiner
        UpdateLobbyUI();
    }

    public override void OnJoinedRoom ()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateLobbyUI ()
    {
        playerListText.text = "";

       //Disp. all players currently in lobby
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        if(PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        } else
        {
            startGameButton.interactable = false;
        }
    }

}
