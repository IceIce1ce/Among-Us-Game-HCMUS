using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DoorSystem : MonoBehaviourPun
{
    [SerializeField] Button DoorButton;
    [SerializeField] Image DoorButtonImage; //doorBtn black overlay
    [SerializeField] List<GameObject> Doors;
    bool isOpen = true; //check system

    void Awake()
    {
        OpenDoors();
        //DoorButton.onClick.AddListener(() => CloseDoors());
        DoorButton.onClick.AddListener(() => photonView.RPC("CloseDoors", RpcTarget.All));
    }
    
    [PunRPC]
    public void CloseDoors()
    {
        foreach (GameObject door in Doors) door.SetActive(true);
        //Disable the Sabotage button for 10sec
        FindObjectOfType<SabotageController>().DoorsClosed();
        isOpen = false;
        GetComponent<AudioSource>().Play();
        //Start the button CoolDown
        StartCoroutine(DoorsCoolDown(30f));
    }
    
    void OpenDoors()
    {
        foreach (GameObject door in Doors) door.SetActive(false);
        isOpen = true;
    }

    IEnumerator DoorsCoolDown(float coolDown)
    {
        DoorButton.interactable = false;
        float TimeLeft = coolDown;
        while (TimeLeft != 0)
        {
            DoorButtonImage.fillAmount = TimeLeft / coolDown;
            //When reaching half the timer open the doors
            if (TimeLeft < coolDown / 2 && !isOpen) OpenDoors();
            yield return new WaitForSeconds(1);
            TimeLeft--;
        }
        DoorButton.interactable = true;
        DoorButtonImage.fillAmount = 0;
    }

    //Called from the SabotageController to Diable the Door Buttons when a Sabotage is started
    public void DisableDoors(float coolDown)
    {
        DoorButton.interactable = false;
        StopAllCoroutines();
        StartCoroutine(DoorsCoolDown(coolDown));
    }
}