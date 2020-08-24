using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]


    public int id;

    [Header("Info")]
    public float fMoveSpeed;
    public float fJumpForce;
    public GameObject goHatObject;

    [HideInInspector]
    public float fCurHatTime;

    [Header("Components")]
    public Rigidbody rbRig;
    public Player plPhotonPlayer;
    public PhotonView photonView; ///Missed this somewhere along the way




    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }


        //Does this go here? Probably
        if (PhotonNetwork.IsMasterClient)
        {
            if (fCurHatTime >= GameManager.instance.fTimeToWin && !GameManager.instance.bGameEnded)
            {
                GameManager.instance.bGameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }


        // Track hat worn time
        if (goHatObject.activeInHierarchy)
        {
            fCurHatTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Check for Movement input, and apply if present
    /// </summary>
    void Move()
    {
        float x = Input.GetAxis("Horizontal") * fMoveSpeed;
        float z = Input.GetAxis("Vertical") * fMoveSpeed;

        rbRig.velocity = new Vector3(x, rbRig.velocity.y, z);
    }


    /// <summary>
    /// Check if near the ground, jump if true
    /// </summary>
    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 0.7f))
        {
            rbRig.AddForce(Vector3.up * fJumpForce, ForceMode.Impulse);
        }
    }


    /// <summary>
    /// Called when player obj is instantiated, init's players and their rigidBodies
    /// </summary>
    [PunRPC]
    public void Initialize(Player plPlayer)
    {
        plPhotonPlayer = plPlayer;
        id = plPlayer.ActorNumber;

        GameManager.instance.players[id - 1] = this;


        //Give first player the hat
        if (id == 1) //>:C
        {
            GameManager.instance.GiveHat(id, true);
        }

        //If this player being created isn't the local user, disable physics as that's calculated locally and synced by server
        if (photonView.IsMine)
        {
            rbRig.isKinematic = true;
        }
    }



    #region Hat Interactions

    /// <summary>
    /// Set player has hat or not
    /// </summary>
    public void SetHat(bool bHasHat)
    {
        goHatObject.SetActive(bHasHat);
    }



    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    ///
    /// On Collision, check if there is a hat to steal, and do so if possible
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine)
        {//If following interaction of another player(?), ignore it?
            return;
        }

        if (other.gameObject.CompareTag("Player") && GameManager.instance.GetPlayer(other.gameObject).id == GameManager.instance.iPlayerWithHat)
        {//If hit another player ^               && They have the hat ^

            if (GameManager.instance.CanGetHat())
            {//Can we steal the hat?

                //Steal said hat
                GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
            }
        }
    }

    #endregion


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(fCurHatTime);
        }
        else if (stream.IsReading)
        {
            fCurHatTime = (float)stream.ReceiveNext();
        }
    }


}
