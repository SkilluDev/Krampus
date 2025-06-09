using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/LevelModifier/Deadline", fileName = "Deadline")]
public class Deadline : LevelModifier {

    [SerializeField] private float m_startTime;

    public override void UpdateLevel() {


        Game.MainGameInfo.Timer.SetGameTime(30);
        Debug.Log("Siema: czas reduced");
    }

}
