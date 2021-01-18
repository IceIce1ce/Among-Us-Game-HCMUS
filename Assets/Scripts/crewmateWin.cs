using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class crewmateWin : MonoBehaviourPun
{
    private float timeLeft;
    [SerializeField] private Text timeText;

    private void Start()
    {
        timeLeft = 300;
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timeText.text = ((int) timeLeft).ToString();
            if (timeLeft <= 0)
            {
                photonView.RPC("checkCrewmateWin", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void checkCrewmateWin()
    {
        PhotonNetwork.LoadLevel(5); //use photon network change scene when using global variable
    }
}