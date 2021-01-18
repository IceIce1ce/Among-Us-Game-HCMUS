using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button findMatchBtn;
    [SerializeField] private GameObject searchingPanel;
    [SerializeField] private InputField nameInputField;
    private Canvas host_canvas;
    [SerializeField] private Button backMenuBtn;
    [SerializeField] private Text versionText;

    private void Start()
    {
        nameInputField.gameObject.SetActive(false);
        host_canvas = GameObject.Find("Host_Canvas").GetComponent<Canvas>();
        host_canvas.gameObject.SetActive(false);
        findMatchBtn.gameObject.SetActive(false);
        searchingPanel.SetActive(false);
        backMenuBtn.gameObject.SetActive(false);
        versionText.enabled = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    //set nickname and check valid findMatchBtn
    public void OnClick_SetName()
    {
        PhotonNetwork.NickName = nameInputField.text;
    }

    public void OnTFChange()
    {
        if (nameInputField.text.Length > 2) findMatchBtn.interactable = true;
        else findMatchBtn.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to " + PhotonNetwork.CloudRegion + " server");
        PhotonNetwork.AutomaticallySyncScene = true;
        findMatchBtn.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(true);
        host_canvas.gameObject.SetActive(true);
        findMatchBtn.interactable = false; //require input name player
        backMenuBtn.gameObject.SetActive(true);
        versionText.enabled = true;
    }

    public void findMatch()
    {
        nameInputField.gameObject.SetActive(false);
        host_canvas.gameObject.SetActive(false);
        searchingPanel.SetActive(true);
        findMatchBtn.gameObject.SetActive(false);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Search for a game");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Couldn't find a room - create a new room");
        MakeRoom();
    }

    void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 4); 
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 4, //max 20 CCU
            CleanupCacheOnLeave = true //clean when a user leave room
        };
        PhotonNetwork.CreateRoom("Roomname_" + randomRoomName, roomOptions, null);
        Debug.Log("Room created, waiting for another player");
        //PhotonNetwork.CreateRoom("Roomname_" + randomRoomName, new RoomOptions() {MaxPlayers = 2}, null);
    }
    
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /* matchmaking for fixed number player
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + "/2 Starting Game");
            PhotonNetwork.LoadLevel(1);
        }
    }
    */
    
    //matchmaking for 2-10 players
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Client successfully joined the room");
            PhotonNetwork.LoadLevel(2);
        }
    }

    public void StopSearch()
    {
        nameInputField.gameObject.SetActive(true);
        host_canvas.gameObject.SetActive(true);
        searchingPanel.SetActive(false);
        findMatchBtn.gameObject.SetActive(true);
        PhotonNetwork.LeaveRoom();
        Debug.Log("Stopped, come back to menu");
    }
    
    //Back to mmenu
    public void backToMenu()
    {
        PhotonNetwork.Disconnect(); //disconnect before load scene
        SceneManager.LoadScene("MainMenu");
    }
}