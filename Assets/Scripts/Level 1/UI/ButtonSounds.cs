using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance?.PlaySound2D("Hover");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance?.PlaySound2D("Click");
    }
}