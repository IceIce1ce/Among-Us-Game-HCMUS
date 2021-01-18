using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    private int numColor = 4;
    public SpriteRenderer playerBodyColor;
    public SpriteRenderer playerEyeColor;
    private int colorIdx;

    //random color chat
    public Color CurrentColor()
    {
        if(colorIdx == 0) return Color.cyan;
        else if(colorIdx == 1) return Color.blue;
        else if(colorIdx == 2) return Color.green;
        else if(colorIdx == 3) return Color.red;
        else return Color.magenta;
    }

    private void Awake()
    {
        if (photonView.IsMine) colorIdx = Random.Range(0, numColor); //need check random not duplicate color
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(colorIdx);
        }
        else
        {
            colorIdx = (int) stream.ReceiveNext();
        }
    }
    
    private void Update()
    {
        switch (colorIdx)
        {
            case 0:
                playerBodyColor.color = Color.cyan;
                playerEyeColor.color = Color.cyan;
                break;
            case 1:
                playerBodyColor.color = Color.blue;
                playerEyeColor.color = Color.blue;
                break;
            case 2:
                playerBodyColor.color = Color.green;
                playerEyeColor.color = Color.green;
                break;
            case 3:
                playerBodyColor.color = Color.red;
                playerEyeColor.color = Color.red;
                break;
            case 4:
                playerBodyColor.color = Color.magenta;
                playerEyeColor.color = Color.magenta;
                break;
        }
    }
}