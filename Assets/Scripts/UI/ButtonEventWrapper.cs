using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Wraps buttons event methods into events handlers to be assigned by other scripts
/// </summary>
public class ButtonEventWrapper : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{
    public delegate void SelectEvent();
    public event SelectEvent OnSelected;

	public delegate void DeselectEvent();
	public event DeselectEvent OnDeselected;

	public delegate void SubmitEvent();
    public event SubmitEvent OnSubmitted;

    public static void CopyEvents(ref ButtonEventWrapper original, ref ButtonEventWrapper source)
    {
        source.OnSelected = null;
        source.OnSubmitted = null;
		source.OnDeselected = null;

        source.OnSelected = original.OnSelected;
        source.OnSubmitted = original.OnSubmitted;
		source.OnDeselected = original.OnDeselected;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (OnSelected != null)
            OnSelected();
    }

	public void OnDeselect(BaseEventData eventData)
	{
		if (OnDeselected != null)
			OnDeselected();
	}

	public void OnSubmit(BaseEventData eventData)
    {
        if (OnSubmitted != null)
            OnSubmitted();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnSubmitted != null)
            OnSubmitted();
    }
}
