using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogMan : MonoBehaviour {

    [SerializeField] private int m_CurrentLevel = 0;
    public int CurrentLevel => m_CurrentLevel;



    public List<Item> m_KrampusItems = new List<Item>();




    public void ResetProgress() {

        m_CurrentLevel = 0;
        m_KrampusItems.Clear();

    }

    public void AddItem(Item item) {
        m_KrampusItems.Add(item);        
     }
		
	


}
