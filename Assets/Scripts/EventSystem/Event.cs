using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Event : ScriptableObject
{
    [System.NonSerialized] public bool active;
    public EventStep[] steps;
    public int moodChange;
    public bool requireInput;
    [SerializeField] int nbTimes = 1;       // two values are necessary to be able to serialize the value
    [System.NonSerialized] int played = 0;  // in editor while not modifying it during testing
    public Event[] requiredEvents;
    public bool[] conditions;

    public void Reset()
    {
        active = false;
        played = 0;
	}

    public void Complete()
    {
        active = false;
        played++;
	}

    public bool WasPlayed()
    {
        return played > 0;
	}

    public bool CanBePlayed()
    {
        return played < nbTimes;
	}
}