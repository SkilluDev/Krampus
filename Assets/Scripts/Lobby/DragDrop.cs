using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler {
    
    private Canvas canvas;
    private RectTransform RectTransform;
    private CanvasGroup m_canvasGroup;

	public void Awake() {
		RectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        m_canvasGroup = GetComponent<CanvasGroup>();
	}

	public void OnBeginDrag(PointerEventData eventData) {
        m_canvasGroup.blocksRaycasts = false;
    }
	public void OnDrag(PointerEventData eventData) {
        RectTransform.anchoredPosition +=  eventData.delta/canvas.scaleFactor;
    }

	public void OnDrop(PointerEventData eventData) 
    {

    }
	public void OnEndDrag(PointerEventData eventData) {
         m_canvasGroup.blocksRaycasts = true;
        
         
         RectTransform.localPosition = Vector2.zero;

    }
	public void OnPointerDown(PointerEventData eventData) => throw new System.NotImplementedException();
}
