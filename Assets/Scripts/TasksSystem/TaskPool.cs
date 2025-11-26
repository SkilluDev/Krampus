using System.Linq;
using KrampUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tasks/TaskPool", fileName = "TaskPool")]
public class TaskPool : ScriptableObject
{
     public Task[] tasks;

    public Task[] RandomTasksForKrampus(int howMany) =>
        tasks.UnityShuffle().Take(howMany).ToArray();
}
