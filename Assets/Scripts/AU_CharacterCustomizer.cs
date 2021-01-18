using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AU_CharacterCustomizer : MonoBehaviour
{
    [SerializeField] Color[] allColors;

    public void SetColor(int colorIndex)
    {
        //AU_PlayerController.localPlayer.SetColor(allColors[colorIndex]);
    }

    public void NextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
