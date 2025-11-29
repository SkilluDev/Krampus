using System.Collections.Generic;
using System.Linq;
using KrampUtils;
using TMPro;
using UnityEngine;

public class TaskPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_mapPanel;
    public  Vector2[] m_pinPlaces;
    [SerializeField] private TaskPin m_taskPinPref;

    private Task highlightedTask;

    [Header("DETAILS")]
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_DescText;
    [SerializeField] private TextMeshProUGUI m_AText;

    [SerializeField] private TextMeshProUGUI m_GoldText;


    private void Start() {
     
    }
    public void PlacePins( int howMany) {

       
        Vector2[] pinPoses =  m_pinPlaces.UnityShuffle().Take(howMany).ToArray();

        for(int i =0; i<howMany;i++) {
            Vector2 pos =  pinPoses[i];
            RectTransform newPin = Instantiate(m_taskPinPref).GetComponent<RectTransform>();
            newPin.GetComponent<TaskPin>().m_task = Game.Lobbyinfo.m_tasks[i];  
            newPin.SetParent(m_mapPanel,false);
            newPin.anchoredPosition = pos;
        }
      
    }
    public void ShowDetails(Task task) {
        m_TitleText.text = task.name;
        m_DescText.text = task.m_description;
        m_GoldText.text = task.goldAmount.ToString();
        highlightedTask = task;
        
    }

    public void StartTask() {
        if(highlightedTask != null) {
              Game.PogMan.StartNewGame(highlightedTask);
        }
    }


   
}
