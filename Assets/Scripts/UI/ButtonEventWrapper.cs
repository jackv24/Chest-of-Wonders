using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Wraps buttons event methods into events handlers to be assigned by other scripts
/// </summary>
public class ButtonEventWrapper : MonoBehaviour, ISelectHandler
{
    public delegate void NormalEvent();
    public event NormalEvent onSelect;

    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null)
            onSelect();
    }

    public static void CopyEvents(ref ButtonEventWrapper original, ref ButtonEventWrapper source)
    {
        source.onSelect = original.onSelect;
    }
}
