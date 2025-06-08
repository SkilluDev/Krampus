using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelModifier/LegPower", fileName = "LegPower")]
public class LegPower : LevelModifier {


    [SerializeField] private float m_bonusMovemenetSpeed;

    public override void UpdateLevel() {

        foreach (var n in Game.MainGameInfo.Nuns) {
            n.SetRunSpeed(n.RunSpeed * (1 + m_bonusMovemenetSpeed));
		 }
     }
}
