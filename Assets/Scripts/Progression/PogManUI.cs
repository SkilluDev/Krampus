using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class PogManUI : MonoBehaviour {


    [SerializeField] private int m_CurrentLevel;
    [SerializeField] private int maxLevel = 5;


    [Header("======[Map]=====")]
    [BoxGroup("Map")][SerializeField] private RectTransform m_mapContainer;
    [BoxGroup("Map")][SerializeField] private Button m_mapButtonPref;
    [BoxGroup("Map")][SerializeField] private Sprite m_currentLevelSprite;
    [BoxGroup("Map")][SerializeField] private Sprite m_doneLevelSprtie;
    [BoxGroup("Map")][SerializeField] private Sprite m_futureLevelSprtie;


    [Header("======[Modifiers]=====")]
    [BoxGroup("Modifiers")][SerializeField] private LevelModifiersPool m_pool;
    [BoxGroup("Modifiers")][SerializeField] private ChallangeModifierUI[] m_challangeModifierUIs;

    private LevelModifier[] m_levelModifiers;
    private LevelModifier m_selectedModifier;


    [BoxGroup("Start")][SerializeField] private Button m_startButton;

    private void Start() {
        GenerateMap();
        SetRandomModifiers();
        m_selectedModifier = null;
        m_startButton.gameObject.SetActive(false);
    }

    public void GenerateMap() {

        for (int i = 1; i < m_CurrentLevel; i++) {

            Button b = Instantiate(m_mapButtonPref);
            b.transform.SetParent(m_mapContainer);
            b.GetComponent<Image>().sprite = m_doneLevelSprtie;
            b.enabled = false;

        }
        Button cc = Instantiate(m_mapButtonPref);
        cc.transform.SetParent(m_mapContainer);
        cc.GetComponent<Image>().sprite = m_currentLevelSprite;

        for (int j = m_CurrentLevel + 1; j <= maxLevel; j++) {
            Button b = Instantiate(m_mapButtonPref);
            b.transform.SetParent(m_mapContainer);
            b.GetComponent<Image>().sprite = m_futureLevelSprtie;
            b.enabled = false;
        }


    }

    public void SelectCard(int i) {
         if (!m_startButton.gameObject.activeSelf) { m_startButton.gameObject.SetActive(true); }
        foreach (var a in m_challangeModifierUIs) {

            a.Deselect();
        }
        m_challangeModifierUIs[i].Select();
        m_selectedModifier = m_levelModifiers[i];
    }

    public void LoadGameScene() {

        
        Game.PogMan.SetNextLevelModifier(m_selectedModifier);
        IEnumerator Internal() {

            yield return new WaitForSecondsRealtime(1.2f);
            Game.LoadState(Game.State.MainGame);
        }

        StartCoroutine(Internal());
    }

    void SetRandomModifiers() {


       
        m_levelModifiers = m_pool.getRandom(m_challangeModifierUIs.Length,m_CurrentLevel);


        for (int i = 0; i < m_levelModifiers.Length; i++) {
            var t = m_levelModifiers[i];
            m_challangeModifierUIs[i].SetInfo(t.ModifierName, t.Description);
         }
     }
}
