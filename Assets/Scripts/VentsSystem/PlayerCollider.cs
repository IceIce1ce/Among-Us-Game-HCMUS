using UnityEngine;

public class PlayerCollider: MonoBehaviour
{
    AU_PlayerController au_player;

    private void Awake()
    {
        au_player = GetComponent<AU_PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vent")
        {
            if (au_player.IsInVent())
            {
                other.gameObject.GetComponent<Vent>().EnableVent(au_player);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vent")
        {
            if (au_player.IsInVent())
            {
                other.gameObject.GetComponent<Vent>().DisableVent();
            }
        }
    }
}