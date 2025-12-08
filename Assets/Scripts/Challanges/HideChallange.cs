using UnityEngine;


[CreateAssetMenu(menuName = "Game/Challange/HideChallange", fileName = "HideChallange")]
public class HideChallange : Challange 
{

   


	public override void Register() {

		Game.roundInfo.Krampus.KrampusEvents.onKrampusFoundByNun.AddListener(Advance);
        Game.roundInfo.UI.UpdateChallangeText(m_desc + " (" + currentValue + "/" + value +" )", true);
    }   

    public void Advance(Krampus krampus, Nun nun) {
            
        currentValue += 1;
        
        if(currentValue >= value) {
                Fail();
                Debug.Log("FAILLLLED...");

                
        }
         Game.roundInfo.UI.UpdateChallangeText(m_desc + " (" + currentValue + "/" + value +" )", m_stillActive);
       

    }

    

}
