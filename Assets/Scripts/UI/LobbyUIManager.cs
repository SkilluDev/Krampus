using TMPro;
using UnityEngine;

public class LobbyUIManager : NewUIManager
{
         public enum Panel {
                None =0,
                TaskPanel= 1,
                EquPanel = 2,
                ShopPanel = 3,


        }


        public bool isPanelOpen => currentPanel != Panel.None;
        public RectTransform[] m_panels;
        [SerializeField] private TaskPanel m_TaskPanel;
        [SerializeField] private EquPanel m_EquPanel;

        [SerializeField] private ShopPanel m_shopPanel;
        public ShopPanel ShopPanel => m_shopPanel;
        private Panel currentPanel = Panel.None;


        [SerializeField] private TextMeshProUGUI m_goldDisplay;


        


        void Start() {
                        HideBlackBars();


                
        }

        public void UpdateGoldValue() {
        m_goldDisplay.text = Game.PogMan.GoldAmount.ToString();
    }

        protected override void Ready() {
                base.Ready();
                UpdateGoldValue();
                UIElementsEntryAnimation();
                ExitPanel();
                m_shopPanel.UpdateShop();
        }

        public void ExitPanel() {
                foreach(var a in m_panels) {
                        a.gameObject.SetActive(false);
                }
                currentPanel = Panel.None;
        }

        public void OpenPanel(Panel panel) {
                m_panels[(int)panel-1].gameObject.SetActive(true);
                currentPanel = panel;
                if((int)panel == 3) {
            m_shopPanel.UpdateShop();
        }
                
        }
        public void  PlacePins(int taskNumber) {
                m_TaskPanel.PlacePins(taskNumber);
        
    }

        public void ShowTaskDetails(Task task) {
                m_TaskPanel.ShowDetails(task);  
    }

    public void  UpdateEqu() {
        m_EquPanel.UpdateEqu();
    }
}
