using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class AU_Interactable : MonoBehaviourPun
{
    [SerializeField] private GameObject miniGame;
    private GameObject highLight;
    private Button btnUse;

    private void Start()
    {
        btnUse = GameObject.Find("UseBtn").GetComponent<Button>();
    }

    private void OnEnable()
    {
        highLight = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9) return; //prevent ghost from interacting object
        if (other.gameObject.layer == 11) return; //prevent imposter from interacting object
        PhotonView myPV = other.GetComponent<PhotonView>();
        if (other.tag == "Player" && myPV.IsMine)
        {
            highLight.SetActive(true);
            btnUse.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9) return; //prevent ghost from interacting object
        if (other.gameObject.layer == 11) return; //prevent imposter from interacting object
        PhotonView myPV = other.GetComponent<PhotonView>();
        if (other.tag == "Player" && myPV.IsMine)
        {
            highLight.SetActive(false);
            btnUse.interactable = false;
        }
    }

    public void PlayMiniGame()
    {
        miniGame.SetActive(true);
    }
}