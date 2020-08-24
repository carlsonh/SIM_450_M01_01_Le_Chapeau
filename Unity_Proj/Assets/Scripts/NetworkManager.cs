using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public static NetworkManager instance;

 
    void Awake()
    {
        if (instance != null && instance != this)
        {///Is there an existing NetMan that's not this one
            gameObject.SetActive(false);///If so, this one isn't needed
        } else
        {
            //This instance should become the NetMan
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom (string strRoomName)
    {///Try to create a room w/name ^
        PhotonNetwork.CreateRoom(strRoomName);
    }

    public void JoinRoom (string strRoomName)
    {///Try to join a room w/name ^
        PhotonNetwork.JoinRoom(strRoomName);
    }

    [PunRPC]
    public void ChangeScene (string strSceneName)
    {///Change active scene to the scene ^
        PhotonNetwork.LoadLevel(strSceneName);
    }


    public override void OnConnectedToMaster()
    {///
        Debug.Log("Connected to director server");
        //CreateRoom("initRoom");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room: " + PhotonNetwork.CurrentRoom.Name);
    }
    
}
