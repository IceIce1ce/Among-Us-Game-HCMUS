using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class SabotageController : MonoBehaviourPun
{
    [SerializeField] List<Button> sabotageButtons;
    [SerializeField] List<Image> sabotageButtonsImages; //doorBtn black overlay
    bool buttonsSabotaged = false;
    [SerializeField] private Light2D lightSabo; //blink light when using sabotage
    
    IEnumerator Flashing()
    {
        lightSabo.gameObject.SetActive(true);
        float timeLeft = 8;
        while (timeLeft != 0)
        {
            yield return new WaitForSeconds(1f);
            lightSabo.enabled = !lightSabo.enabled;
            timeLeft--;
        }
        lightSabo.gameObject.SetActive(false);
    }

    [PunRPC]
    void StartFlashing()
    {
        StartCoroutine(Flashing());
    }

    void Awake()
    {
        ActivateSabotages();
    }
    
    public void SabotageO2()
    {
        GetComponent<AudioSource>().Play();
        //blink light when using O2Btn
        photonView.RPC("StartFlashing", RpcTarget.All);
        Debug.Log("O2 sabotage was executed");
        DisableSabotages(30);
    }
    
    public void SabotageComms()
    {
        GetComponent<AudioSource>().Play();
        //blink light when using CommsBtn
        photonView.RPC("StartFlashing", RpcTarget.All);
        Debug.Log("Communications sabotage was executed");
        DisableSabotages(30);
    }
    
    public void SabotageReactor()
    {
        GetComponent<AudioSource>().Play();
        //blink light when using ReactorBtn
        photonView.RPC("StartFlashing", RpcTarget.All);
        Debug.Log("Reactor sabotage was executed");
        DisableSabotages(30);
    }
    
    public void SabotageLights()
    {
        GetComponent<AudioSource>().Play();
        //blink light when using ElectricBtn
        photonView.RPC("StartFlashing", RpcTarget.All);
        Debug.Log("Electric sabotage was executed");
        DisableSabotages(30);
    }
    
    public void DisableSabotages(float coolDownAmount)
    {
        if (!buttonsSabotaged)
        {
            buttonsSabotaged = true;
            //Disable the Doors buttons for 10 sec after the Sabotage
            foreach (DoorSystem doorSystem in FindObjectsOfType<DoorSystem>()) doorSystem.DisableDoors(10);
            StartCoroutine(SabotagingButtons(coolDownAmount));
        }
    }

    //Called when a Door is closed to diactivate the sabotage buttons for 10s
    public void DoorsClosed()
    {
        if (!buttonsSabotaged)
        {
            StopAllCoroutines();
            StartCoroutine(SabotagingButtons(10));
        }
    }

    //Called to activate the sabotage buttons
    void ActivateSabotages()
    {
        buttonsSabotaged = false;
        foreach (Button button in sabotageButtons) button.interactable = true;
    }
    
    IEnumerator SabotagingButtons(float coolDownAmount)
    {
        foreach (Button button in sabotageButtons) button.interactable = false;
        float TimeLeft = coolDownAmount;
        while (TimeLeft != 0)
        {
            foreach (Image img in sabotageButtonsImages) img.fillAmount = TimeLeft / coolDownAmount;
            yield return new WaitForSeconds(1);
            TimeLeft--;
        }
        ActivateSabotages();
        foreach (Image img in sabotageButtonsImages) img.fillAmount = 0;
    }
}