using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AU_Body : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodySprite;

    public void SetColor(Color newColor)
    {
        bodySprite.color = newColor;
    }

    private void OnEnable()
    {
        if (AU_PlayerController.allBodies != null)
        {
            AU_PlayerController.allBodies.Add(transform);
        }
    }

    public void Report()
    {
        Debug.Log("Reported");
        Destroy(gameObject);
    }
}