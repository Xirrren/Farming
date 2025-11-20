using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class BtnEffect : MonoBehaviour , IPointerEnterHandler , IPointerExitHandler ,IPointerClickHandler
{
    private float scaleFactor = 1.2f;
    private float duration = 0.2f;
    
    private Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale*scaleFactor, duration).SetUpdate(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioMgr.instance.PlaySFX("Button-Click");
    }
}
