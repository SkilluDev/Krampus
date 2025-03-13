using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    private static bool m_showTutorials = true;

    public static void SetShowTutorials(bool state) {
        m_showTutorials = state;
    }

    public static bool GetShowTutorials() {
        return m_showTutorials;
    }
}
