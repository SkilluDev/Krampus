using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using LitMotion;

public class InventoryCard : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {


    //2
    [SerializeField] private Image m_itemIcon;

    [SerializeField] private GameObject m_descBox;
    [SerializeField] private RectTransform m_descBackground;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_descriptionText;

    [SerializeField] private float m_baseHeight;

    [SerializeField] private Animator m_animator;




 

    private void Start() {
        m_descBox.SetActive(false);


    }
    public void OnPointerEnter(PointerEventData eventData) {
        m_descBox.SetActive(true);
        m_animator.SetTrigger("Highlighted");
     
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        Hide();
     }


    public void Hide() {
         m_descBox.SetActive(false);
        m_animator.SetTrigger("Normal");
     }

    public void SetInfo(Item item) {
        m_itemIcon.sprite = item.ItemIcon;
        m_titleText.text = item.ItemName;
        m_descriptionText.text = item.Description;

        m_descriptionText.ForceMeshUpdate();
        float height = m_descriptionText.preferredHeight;

        m_descBackground.sizeDelta = new Vector2(m_descBackground.sizeDelta.x, m_baseHeight + height);
    }
}
