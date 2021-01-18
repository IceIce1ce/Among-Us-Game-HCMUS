using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class imposterWin : MonoBehaviourPun
{
    private void Update()
    {
        //count number of imposter by layer
        int countImposter = 0;
        GameObject[] gob = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gob)
        {
            if (go.layer == 11) countImposter++;
        }
        int total_player = PhotonNetwork.CurrentRoom.PlayerCount - countImposter; //exclude number of imposter
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        if (total_player > 0)
        {
            foreach (GameObject go in gos)
            {
                if (go.layer == 9 && go.CompareTag("Player")) total_player--;
            }
            if (total_player <= 0)
            {
                photonView.RPC("checkImposterWin", RpcTarget.All);
                //for(int i = 0; i < gos.Length; i++) Destroy(gos[i]); //clean rpc
            }
        }
    }

    [PunRPC]
    private void checkImposterWin()
    {
        //PhotonNetwork.LoadLevel(4);
        SceneManager.LoadScene(4, LoadSceneMode.Single); //use scene manager change scene when using fixed variable in function
    }
}