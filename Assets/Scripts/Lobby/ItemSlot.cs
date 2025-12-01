using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    private RectTransform m_rectTransform;

    

	public void Awake() {
		m_rectTransform = GetComponent<RectTransform>();
	}
	public void OnDrop(PointerEventData eventData) 
    {
        Debug.Log("DROP");  
       if( eventData.pointerDrag != null) {
        RectTransform drop= eventData.pointerDrag.GetComponent<RectTransform>();

            drop.SetParent(m_rectTransform,false); 
              drop.localPosition = Vector2.zero;
            

            
            
           
                
        



    }

	
}}
