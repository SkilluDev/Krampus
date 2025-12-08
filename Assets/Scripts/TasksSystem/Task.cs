using System;
using System.Security.Claims;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Tasks/Task", fileName = "NewTask")]
public class Task : ScriptableObject
{
   public string name;
   public string m_description;

   public LevelSet levelSet;

   public  int goldAmount;

   [SerializeField] private Challange[] challanges;

   public Challange[] Challanges => challanges;


   public void ClaimRewards() {
        Game.PogMan.AddGold(goldAmount);
        foreach(Challange c in challanges) {
            c.Claim();
        }
    }
}
