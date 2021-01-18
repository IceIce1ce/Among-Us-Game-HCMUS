using UnityEngine;

public class UIControl : MonoBehaviour
{
    //display and hide chat system
    public GameObject ChatWindowUI;

    public void OnChatButtonPressed()
    {
        ChatWindowUI.SetActive(!ChatWindowUI.activeInHierarchy);
    }

    public bool isChatWindowActive()
    {
        return ChatWindowUI.activeInHierarchy;
    }
}