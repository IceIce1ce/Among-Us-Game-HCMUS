using UnityEngine;

public class PlayMusicController : MonoBehaviour
{
    //walking sound
    [SerializeField] AudioSource audioSourceWalk;

    public void PlayWalkingSound()
    {
        if(!audioSourceWalk.isPlaying) audioSourceWalk.Play();
    }

    public void StopWalking()
    {
        audioSourceWalk.Stop();
    }
    
    //vent sound
    [SerializeField] private AudioSource audioVent;

    public void PlayVent()
    {
        audioVent.Play();
    }
    
    //kill sound
    [SerializeField] private AudioSource audioKill;

    public void PlayKillSound()
    {
        audioKill.Play();
    }
}