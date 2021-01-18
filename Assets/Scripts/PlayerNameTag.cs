using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private Text nameText;
    //sync text transform
    //[SerializeField] private Transform targetTransform;
    //private Vector3 targetPosition = Vector3.zero;

    private void Start()
    {
        if (!photonView.IsMine) return;
        //PlayerPrefs.DeleteAll();
        //sync text transform
        /*
        photonView.RPC("SetName", RpcTarget.All);
        if (photonView.IsMine)
        {
            GetComponent<CameraWork>().OnStartFollowing();
        }
        */
        nameText.text = photonView.Owner.NickName;
    }
    /*
    [PunRPC]
    void SetName()
    {
        nameText.text = photonView.Owner.NickName;
    }

    private void LateUpdate()
    {
        targetPosition = targetTransform.position;
        nameText.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + new Vector3(2f, 40f, 0f);
    }
    */
}