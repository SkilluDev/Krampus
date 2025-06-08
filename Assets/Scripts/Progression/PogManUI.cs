using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PogManUI : MonoBehaviour {


    [BoxGroup("Map")][SerializeField] private RectTransform m_mapContainer;
    [BoxGroup("Map")][SerializeField] private Sprite m_currentLevelSprite;
    [BoxGroup("Map")][SerializeField] private Sprite m_doneLevelSprtie;
    [BoxGroup("Map")][SerializeField] private Sprite m_futureLevelSprtie;
}
