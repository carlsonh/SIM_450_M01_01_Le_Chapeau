using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public bool bGameEnded = false; // has the game ended?
    public float fTimeToWin; // time a player needs to hold the hat to win
    public float fInvincibleDuration; // how long after a player gets the hat, are they invincible
    private float fHatPickupTime; // the time the hat was picked up by the current holder

    [Header("Players")]
    public string strPlayerPrefabLocation; // path in Resources folder to the Player prefab
    public Transform[] spawnPoints; // array of all available spawn points
    public PlayerController[] players; // array of all the players
    public int iPlayerWithHat; // id of the player with the hat
    private int iPlayersInGame; // number of players in the game 

    // instance
    public static GameManager instance;



    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //Set instance
        instance = this;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("JoinedGame", RpcTarget.All);
    }



    #region Game Join / Player Spawn
    /// <summary>
    /// Called when all players are in the game
    /// </summary>
    [PunRPC]
    void JoinedGame()
    {
        iPlayersInGame++;

        if (iPlayersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    /// <summary>
    /// Spawns a player and instantiates it to other players in the server
    /// </summary>
    void SpawnPlayer()
    {
        //Instantiate player across network
        GameObject goPlayerObj = PhotonNetwork.Instantiate(strPlayerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        //Get the player script
        PlayerController playerScript = goPlayerObj.GetComponent<PlayerController>();

        //Init the player script
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
    #endregion



    #region GetPlayer by ID/GO
    /// <summary>
    /// Returns a player by their ID
    /// </summary>
    public PlayerController GetPlayer(int iPlayerId)
    {
        return players.First(x => x.id == iPlayerId);
    }

    /// <summary>
    /// Returns a player by their GO
    /// </summary>
    public PlayerController GetPlayer(GameObject goPlayerObject)
    {
        return players.First(x => x.gameObject == goPlayerObject);
    }
    #endregion



    #region Hat Interactions

    /// <summary>
    /// Tag! When a player hits someone with a hat, steal it
    /// </summary>
    [PunRPC]
    public void GiveHat(int iPlayerId, bool bIsInitialGive)
    {
        //Remove the hat from target
        if (!bIsInitialGive)
        {
            GetPlayer(iPlayerWithHat).SetHat(false);
        }

        //Give hat to new player
        iPlayerWithHat = iPlayerId;
        GetPlayer(iPlayerId).SetHat(true);
        fHatPickupTime = Time.time;
    }

    /// <summary>
    /// Check if the player can steal the hat (invincibleDuration has passed since pickup)
    /// </summary>
    public bool CanGetHat()
    {
        if (Time.time > fHatPickupTime + fInvincibleDuration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion



    #region Game State
    /// <summary>
    /// Called when a player has won the game (Has hat for long enough)
    /// </summary>
    [PunRPC]
    public void WinGame(int iPlayerId)
    {
        bGameEnded = true;
        PlayerController player = GetPlayer(iPlayerId);

        GameUI.instance.SetWinText(player.plPhotonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    /// <summary>
    /// Sends players back to menu
    /// </summary>
    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("MenuScene");
    }
    #endregion


}
