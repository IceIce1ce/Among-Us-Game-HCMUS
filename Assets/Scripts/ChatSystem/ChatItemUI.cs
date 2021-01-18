using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private Image _image;

    public void Initialize(string text, Color color)
    {
        _text.text = text;
        _image.color = color;
    }
}