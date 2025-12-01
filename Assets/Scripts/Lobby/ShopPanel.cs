using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_shopContent;


    [Header("Description")]
    [SerializeField] private Image m_descIcon;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_descText;
    [SerializeField] private TextMeshProUGUI m_priceText;



	public void UpdateShop() {
		
	}
}
