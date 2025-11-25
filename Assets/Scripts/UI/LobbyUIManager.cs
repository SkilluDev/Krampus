using UnityEngine;

public class LobbyUIManager : NewUIManager
{

        public bool isPanelOpen => currentPanel != Panel.None;
        public enum Panel {
                        None =0,
                        TaskPanel= 1,
        }

        public RectTransform[] m_panels;
        private Panel currentPanel = Panel.None;


        void Start() {
                        HideBlackBars();


                
        }

        protected override void Ready() {
                base.Ready();
                UIElementsEntryAnimation();
                ExitPanel();
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
        }
}
