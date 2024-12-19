using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private static bool showTutorials = true;

    public static void setShowTutorials(bool state)
    {
        showTutorials = state;
    }

    public static bool getShowTutorials()
    {
        return showTutorials;
    }
}
