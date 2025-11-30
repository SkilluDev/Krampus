using UnityEngine;
using UnityEngine.EventSystems;

public class ResetSpace :  MonoBehaviour,IDropHandler
{
    private RectTransform m_rectTransform;
    [SerializeField] private RectTransform m_backArea;

	public void Awake() {
		m_rectTransform = GetComponent<RectTransform>();
	}

    public void OnDrop(PointerEventData eventData) 
    {
        Debug.Log("COME BODY");  
       if( eventData.pointerDrag != null) {
        RectTransform drop= eventData.pointerDrag.GetComponent<RectTransform>();

            drop.SetParent(m_backArea,false); 
            drop.localPosition = Vector2.zero;
                
        }
    }
}
