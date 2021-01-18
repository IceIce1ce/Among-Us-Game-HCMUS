using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Network : MonoBehaviourPunCallbacks
{
    public MasterClient masterClient;
    //chat system by player body color
    public ChatWindowUI chatWindowUI;
    //display and hide chat
    public UIControl uiControl;
    //random spawn player
    public Transform[] spawnPoints;
    /* use these setting without matchmaking
    private void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;
        //PhotonNetwork.NickName = "AU_Player" + Random.Range(0, 5000);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinOrCreateRoom("Room" + Random.Range(1, 4), new RoomOptions() {MaxPlayers = 4}, null);
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions() {MaxPlayers = 10}, null);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("AU_Player", new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0),
            Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            masterClient.Initialize();
        }
    }
    */
    private void Start()
    {
        //random spawn player
        Player[] players = PhotonNetwork.PlayerList;
        if (photonView.IsMine)
        {
            for (int i = 0; i < players.Length; i++)
            {
                photonView.RPC("RPCStartGameSpawn", players[i], spawnPoints[i].position, spawnPoints[i].rotation);
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            masterClient.Initialize(); //random imposter
        }
    }

    [PunRPC]
    void RPCStartGameSpawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        //PhotonNetwork.Instantiate("AU_Player", new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0), Quaternion.identity);
        //GameObject newPlayer = PhotonNetwork.Instantiate("AU_Player", new Vector3(5, 5, 0), Quaternion.identity);
        GameObject newPlayer = PhotonNetwork.Instantiate("AU_Player", spawnPosition, spawnRotation, 0); //spawn position
        chatWindowUI._playerInfo = newPlayer.GetComponent<PlayerInfo>(); //chat system by player body color
        newPlayer.GetComponent<AU_PlayerController>()._uiControl = uiControl; //display and hide chat
    }
}