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

    [SerializeField] private TextMeshProUGUI m_ChalangeText;
    [SerializeField] private TextMeshProUGUI m_ChalangeRewardText;


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
        if(task.Challanges != null && task.Challanges.Length >0 ) 
        {
            Challange a = task.Challanges[0];
            m_ChalangeRewardText.text = a.GoldReward.ToString();
            m_ChalangeText.text = a.Description + " " + a.Value + " times";
        }else {

            m_ChalangeRewardText.text = "";
            m_ChalangeText.text = "";
        
        }
        
    }

    public void StartTask() {
        if(highlightedTask != null) {
              Game.PogMan.StartNewGame(highlightedTask);
        }
    }


   
}
