using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Tasks/Task", fileName = "NewTask")]
public class Task : ScriptableObject
{
   public string name;
   public string m_description;

   public LevelSet levelSet;

   public  int goldAmount;
}
