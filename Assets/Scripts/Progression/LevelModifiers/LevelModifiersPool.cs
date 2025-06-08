using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LevelModifierPoolElement {
    [SerializeField] private int m_minLevel;

    [SerializeField] private LevelModifier m_levelModifier;


    public LevelModifier GetLevelModifier() {
        return m_levelModifier;
    }

    public int GetMinLevel() {
        return m_minLevel;
     }


}

[CreateAssetMenu(menuName = "Game/LevelModifierPool", fileName = "NewPool")]
public class LevelModifiersPool : ScriptableObject {


    public LevelModifier nonLevelModifier;
    public LevelModifierPoolElement[] levelModifiers;


    public LevelModifier[] getRandom(int size, int currentLevel) {

      
        List<int> indexes = new List<int>();

        for (int i = 0; i < levelModifiers.Length; i++) {

            if (currentLevel >= levelModifiers[i].GetMinLevel()) {
                 indexes.Add(i);
            }
           

        
        }

        int[] pos =  Shuffle(indexes.ToArray());
        LevelModifier[] results = new LevelModifier[size];

        for (int j = 0; j < size; j++) {
            if (j >= pos.Length) {
                results[j] = nonLevelModifier;
            } else {
                 results[j] = levelModifiers[pos[j]].GetLevelModifier();
             }

        }

        return results;
    }


    int[] Shuffle(int[] el) {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < el.Length; t++) {
            int tmp = el[t];
            int r = UnityEngine.Random.Range(t, el.Length);
            el[t] = el[r];
            el[r] = tmp;
        }

        return el;
    }
}
