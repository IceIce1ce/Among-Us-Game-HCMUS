using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomSetup : MonoBehaviourPun
{
    private PhotonView myPV;
    [SerializeField] private float timeToStart;
    private float timerToStart;
    private bool readyToStart;
    [SerializeField] private Button StartButton;
    [SerializeField] private Text countdownDisplay;
    //display total player in droneship lobby
    [SerializeField] private Text totalPlayerLobby;
    //show public image exclude host
    [SerializeField] private Image public_img;

    private void Start()
    {
        myPV = GetComponent<PhotonView>();
        timerToStart = timeToStart;
        PhotonNetwork.Instantiate("Space_Player", Vector3.zero, Quaternion.identity);
        //show public image exclude host
        public_img.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }

    private void Update()
    {
        StartButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        if (readyToStart)
        {
            StartButton.interactable = false;
            timerToStart -= Time.deltaTime;
            countdownDisplay.text = ((int) timerToStart).ToString();
        }
        else
        {
            timerToStart = timeToStart;
            countdownDisplay.text = "";
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (timerToStart <= 0)
            {
                timerToStart = 100;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.LoadLevel(3);
            }
        }
        //interact button start with number player >= 2
        if (PhotonNetwork.PlayerList.Length <= 1)
        {
            StartButton.interactable = false;
            totalPlayerLobby.color = Color.red;
        }
        else
        {
            StartButton.interactable = true;
            totalPlayerLobby.color = Color.green;
        }
        //display total player in droneship lobby
        int current_totalplayer = PhotonNetwork.CurrentRoom.PlayerCount;
        totalPlayerLobby.text = current_totalplayer.ToString() + "/10";
    }

    public void Play()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myPV.RPC("RPC_Play", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_Play()
    {
        readyToStart = !readyToStart;
    }
}