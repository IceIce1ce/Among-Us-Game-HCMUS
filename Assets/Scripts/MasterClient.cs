using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MasterClient : MonoBehaviourPun
{
    [SerializeField] private GameObject _imposterWindow;
    [SerializeField] private Text _imposterText;
    
    public void Initialize()
    {
        StartCoroutine(PickImposter());
    }

    private IEnumerator PickImposter()
    {
        GameObject[] players;
        List<int> playerIndex = new List<int>();
        int tries = 0;
        int imposterNumber = 0;
        int imposterNumberFinal = 0;
        do
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            tries++;
            yield return new WaitForSeconds(0.25f);
        } while ((players.Length < PhotonNetwork.PlayerList.Length) && (tries < 5));
        for (int i = 0; i < players.Length; i++)
        {
            playerIndex.Add(i);
        }
        imposterNumber = players.Length < 5 ? 1 : 2;
        imposterNumberFinal = imposterNumber;
        while (imposterNumber > 0)
        {
            int pickedImposterIndex = playerIndex[Random.Range(0, playerIndex.Count)];
            playerIndex.Remove(pickedImposterIndex);
            PhotonView pv = players[pickedImposterIndex].GetComponent<PhotonView>();
            Debug.Log(pv.ViewID + " is imposter");
            pv.RPC("SetImposter", RpcTarget.All);
            imposterNumber--;
        }
        photonView.RPC("ImposterPicked", RpcTarget.All, imposterNumberFinal);
    }

    [PunRPC]
    public void ImposterPicked(int imposterNumber)
    {
        StartCoroutine(ShowImposterAnimation(imposterNumber));
    }

    private IEnumerator ShowImposterAnimation(int imposterNumber)
    {
        _imposterWindow.SetActive(true);
        _imposterText.gameObject.SetActive(true);
        _imposterText.text = "There " + (imposterNumber < 2 ? "is" : "are") + " " + "<color=red>" + imposterNumber 
                             +  " imposter" + (imposterNumber > 1 ? "s" : string.Empty) + "</color>" + " among us";
        yield return new WaitForSeconds(4);
        _imposterWindow.SetActive(false);
    }
}