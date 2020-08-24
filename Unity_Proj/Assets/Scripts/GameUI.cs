using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public PlayerUIContainer[] playerContainers; //controller?
    public TextMeshProUGUI winText;

    public static GameUI instance;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //Singleton, kind-of?
        instance = this;
    }



    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        InitializePlayerUI();
    }
    void Update()
    {
        UpdatePlayerUI();//What
    }

    void InitializePlayerUI()
    {
        //Loop through player name/hatTime containers

        // foreach (PlayerUIContainer container in playerContainers)
        // {
        //     if
        // }
        for (int containerIter = 0; containerIter < playerContainers.Length; ++containerIter)
        {
            PlayerUIContainer container = playerContainers[containerIter];

            if (containerIter < PhotonNetwork.PlayerList.Length)
            {
                container.obj.SetActive(true);
                container.nameText.text = PhotonNetwork.PlayerList[containerIter].NickName;
                container.hatTimeSlider.maxValue = GameManager.instance.fTimeToWin;
            }
            else
            {
                container.obj.SetActive(false);
            }
        }
    }

    void UpdatePlayerUI()
    {
        //Update UI for each player
        for (int playerIter = 0; playerIter < GameManager.instance.players.Length; ++playerIter)
        {
            if (GameManager.instance.players[playerIter] != null)
            {
                playerContainers[playerIter].hatTimeSlider.value = GameManager.instance.players[playerIter].fCurHatTime;
            }
        }
    }



    public void SetWinText(string strWinnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = strWinnerName + " wins!";
    }
}

[System.Serializable]
public class PlayerUIContainer
{
    public GameObject obj;
    public TextMeshProUGUI nameText;
    public Slider hatTimeSlider;
}