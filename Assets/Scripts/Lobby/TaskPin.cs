using System;
using UnityEngine;

public class TaskPin : MonoBehaviour
{
   public Task m_task {get;set;}


    public void ShowDetails() {
            Game.Lobbyinfo.UI.ShowTaskDetails(m_task);
    }

  
}
