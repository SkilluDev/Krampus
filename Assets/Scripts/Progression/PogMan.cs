using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogMan : MonoBehaviour {

    [SerializeField] private int m_currentLevel = 0;
    public int CurrentLevel => m_currentLevel;
    private List<Item> m_krampusItems;
    public IReadOnlyList<Item> KrampusItems => m_krampusItems;

    private LevelModifier m_NextLevelModifer;


    public void ResetProgress() {
        m_currentLevel = 0;
        m_krampusItems = null;
    }

    // those essentially move the list in and out without copying it and making sure no reference lives too long.
    public void Store(ref List<Item> items) {
        m_krampusItems = items;
        items = null;
    }

    public void Unpack(ref List<Item> items) {
        if (m_krampusItems == null) items = new List<Item>();
        else items = m_krampusItems;
        m_krampusItems = null;
    }

    public void SetNextLevelModifier(LevelModifier lm) {
        m_NextLevelModifer = lm;
     }

}
