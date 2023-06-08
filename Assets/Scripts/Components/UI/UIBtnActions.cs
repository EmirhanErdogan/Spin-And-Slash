using Emir;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBtnActions : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // do stuff on Pointer down
        GameManager.Instance.StartGame();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // do stuff on pointer up
    }
}