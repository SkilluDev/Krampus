using UnityEngine.UI;
using UnityEngine;

public class InventoryCard : MonoBehaviour {


    //2
    [SerializeField] private Image m_itemIcon;

    public void SetInfo(Sprite itemIcon) {
        m_itemIcon.sprite = itemIcon;
    }
}
