using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum MenuBtAnimation
{
    None,
    Pisca
}
public class MenuBt : MonoBehaviour
{
    public TMP_Text btLabel;
    public TMP_Text notificationLabel;
    public Image btIcon;
    public Image notification;
    public Button button;
    public int intValue = -1;
    public Action onSelect, onUnselect;
    public bool marker;
    public bool ActiveAndInteractable()
    {
        return gameObject.activeInHierarchy && button.interactable;
    }
    public void SetDisabled()
    {
        button.interactable = false;
    }
    public void SetEnabled()
    {
        button.interactable = true;
    }
    public void ResetState()
    {
        button.interactable = true;
    }
    public void SetNotificationText(string text)
    {
        if (text != null)
        {
            if (notificationLabel) notificationLabel.text = text;
            if (notification) notification.gameObject.SetActive(true);
        }
        else
        {
            if (notification) notification.gameObject.SetActive(false);
        }
    }
}
