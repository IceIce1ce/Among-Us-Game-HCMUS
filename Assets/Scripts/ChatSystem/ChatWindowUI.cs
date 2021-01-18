using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatWindowUI : MonoBehaviourPun
{
    [SerializeField] private ChatItemUI _chatItemPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private InputField _inputText;
    public PlayerInfo _playerInfo;
    
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SendMessage();
        }           
    }

    private void OnEnable()
    {
        _inputText.text = string.Empty; //clean chat for first time enable
        _inputText.ActivateInputField();
    }

    private void SendMessage()
    {
        if (string.IsNullOrEmpty(_inputText.text)) return;
        if (_playerInfo == null) return;
        photonView.RPC("ReceiveMessageRPC", RpcTarget.All, _inputText.text, _playerInfo.CurrentColor().r, 
            _playerInfo.CurrentColor().g, _playerInfo.CurrentColor().b);
        _inputText.text = string.Empty;
        _inputText.ActivateInputField();
    }

    private void InstantiateChatItem(string text, Color color)
    {
        ChatItemUI newChatItem = Instantiate(_chatItemPrefab);
        newChatItem.transform.SetParent(_container);
        newChatItem.transform.position = Vector3.zero;
        newChatItem.transform.localScale = Vector3.one;
        newChatItem.Initialize(text, color);
    }

    [PunRPC]
    public void ReceiveMessageRPC(string text, float red, float green, float blue)
    {
        InstantiateChatItem(text, new Color(red, green, blue));
    }

    public void OnSendButtonPress()
    {
        SendMessage();
    }
}