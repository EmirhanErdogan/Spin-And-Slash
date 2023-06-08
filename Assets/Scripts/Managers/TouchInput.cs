using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Emir;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler,
    IPointerUpHandler
{
    #region Private Fields

    private Vector2 DeltaPos;
    private GameObject PointerObj;

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerObj = null;
        PointerObj = eventData.pointerCurrentRaycast.gameObject;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerObj = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Dokunan parmak hareket etmeye başladı: " + eventData.pointerId + " " + eventData.position);
        PointerObj = null;
        PointerObj = eventData.pointerCurrentRaycast.gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("Parmak hareket ediyor: " + eventData.pointerId + " " + eventData.delta);
        DeltaPos = eventData.delta;
        // PointerObj = null;
        PointerObj = eventData.pointerCurrentRaycast.gameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Parmak dokunmayı kesti: " + eventData.pointerId + " " + eventData.position);
        DeltaPos = Vector2.zero;
        // PointerObj = null;
        PointerObj = eventData.pointerCurrentRaycast.gameObject;
        // DOVirtual.DelayedCall(0.05f, () =>
        // {
        //     PointerObj = null;
        // });
    }

    private void Update()
    {
        if (TouchManager.Instance.IsTouchUp())
        {
            DOVirtual.DelayedCall(0.05f, () => { PointerObj = null; });
        }
    }

    public Vector2 GetDeltaPos()
    {
        return DeltaPos;
    }

    public void SetDeltaPos(Vector2 value)
    {
        DeltaPos = value;
    }

    public GameObject GetPointerObject()
    {
        return PointerObj;
    }

    public void SetPointerObject(GameObject obj)
    {
        PointerObj = obj;
    }
}