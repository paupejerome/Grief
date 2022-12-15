using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance { get; private set; }

    [SerializeField] Event lowMoodEnding;
    [SerializeField] Event highMoodEnding;
    [SerializeField] int moodForGoodEnding = 50;

    [Space]
    [SerializeField] Event[] mainEvents;
    [SerializeField] Event[] subEvents;
    [SerializeField] Event[] fixedEvents;

    [Space]
    [SerializeField] int minEventsMorning = 0;
    [SerializeField] int maxEventsMorning = 3;
    [SerializeField] int minEventsEvening = 0;
    [SerializeField] int maxEventsEvening = 3;

    [Space]
    [SerializeField] GameObject player;
    [SerializeField] DialogueBox dialogueBox;
    [SerializeField] ChoiceHandler choiceHandler;
    [SerializeField] AnimationHandler animationHandler;
    [SerializeField] AudioHandler audioHandler;
    [SerializeField] LevelManager levelManager;

    public static bool playingEvent { get; private set; }
    static int currentMain;
    Event currentEvent;
    int currentStep;

    void Awake()
    {
		if (!instance)
            instance = this;
        else 
        {
            Debug.LogError("There are multiple EventManagers, extra instance deleted: " + gameObject.name, gameObject);
            Destroy(gameObject);
        }

		foreach (Event ev in mainEvents)
            ev.Reset();
        foreach (Event ev in subEvents)
            ev.Reset();
        foreach (Event ev in fixedEvents)
            ev.Reset();

        playingEvent = false;
        currentMain = 0;
    }

	public static void ActivateEndEvent()
	{
        if (GameState.mood >= instance.moodForGoodEnding)
            instance.highMoodEnding.active = true;
        else
            instance.lowMoodEnding.active = true;
	}

	public static void UpdateEvents(bool nextMain)
    {
        instance.UpdateMain(nextMain);
        instance.UpdateFixed();
        instance.PickSubs();
	}

    void UpdateMain(bool nextMain)
    {
        if (instance.mainEvents.Length <= 0)
            return;

        Event ev = instance.mainEvents[currentMain];

        if (nextMain)
        {
            if (ev.active)
            {
                Debug.Log("Main event " + ev.name + " was not played!", ev);
                ev.active = false;
            }

            currentMain++;
            if (currentMain >= instance.mainEvents.Length)
            {
                Debug.LogError("Exhausted main events list, cannot activate a new one");
                return;
            }
            ev = instance.mainEvents[currentMain];
        }

        ev.active = CheckEvent(ev, true);
    }

    void UpdateFixed()
    {
		foreach (Event ev in fixedEvents)
            ev.active = CheckEvent(ev);
	}

	void PickSubs()
    {
        foreach (Event ev in subEvents)
            ev.active = false;

        int nbEvents;
        if (GameState.CheckCondition(Conditions.Morning))
            nbEvents = Random.Range(minEventsMorning, maxEventsMorning + 1);
        else if (GameState.CheckCondition(Conditions.Evening))
            nbEvents = Random.Range(minEventsEvening, maxEventsEvening + 1);
        else
        {
            nbEvents = 0;
            Debug.LogError("No time condition true for picking events!");
        }

        Debug.Log("Nombre d'events: " + nbEvents);
        Shuffle(subEvents);

        for (int i = 0; i < subEvents.Length; i++)
        {
            if (CheckEvent(subEvents[i]))
            {
                subEvents[i].active = true;
                nbEvents--;
                if (nbEvents == 0)
                    return;
            }
        }
    }

    bool CheckEvent(Event ev, bool isMain = false)
    {
        if(!isMain)
        {
            foreach (Event reqEv in ev.requiredEvents)
            {
                if (!reqEv.WasPlayed())
                    return false;
            }
		}

        for (int i = 0; i < (int)Conditions.count; i++)
        {
            if (ev.conditions[i] && !GameState.CheckCondition(i))
                return false;
        }

        return ev.CanBePlayed();
    }

    static void Shuffle<T>(T[] list)
    {
        T temp;
		for (int i = list.Length - 1; i > 0; i--)
		{
            int r = Random.Range(0, i);
            temp = list[i];
            list[i] = list[r];
            list[r] = temp;
		}
	}

    public static void StartEvent(Event ev)
    {
        playingEvent = true;
        instance.currentEvent = ev;
        instance.StartCoroutine(instance.PlayEvent());
	}

    private IEnumerator PlayEvent()
    {
        player.GetComponent<CharacterMovement>().EnableMovement(false);

        int moodChange = currentEvent.moodChange;
        int nextStep = 0;

        for (int i = 0; i < currentEvent.steps.Length; )
        {
            EventStep step = currentEvent.steps[i];
            currentStep = i + 1;
            yield return null;

            switch (step.stepType)
            {
            case StepType.Dialogue:
                yield return StartCoroutine(dialogueBox.Print(step.dialogues));
                break;

            case StepType.Choice:
                int selected = 0;
                Choice[] choices = step.choices;

                yield return StartCoroutine(choiceHandler.DoChoice(choices, result => selected = result));

                if (choices[selected].overrideMoodChange)
                    moodChange = choices[selected].moodChange;
                if (choices[selected].overrideNextStep)
                {
                    step.overrideNextStep = true;
                    step.nextStep = choices[selected].nextStep;
                }                    
				break;

			case StepType.Animation:
                yield return StartCoroutine(animationHandler.PlayAnimation(step.animations));
				break;

			case StepType.Sound:
                yield return StartCoroutine(audioHandler.PlayClips(step.sounds));
				break;

			case StepType.ChangeScene:
                yield return StartCoroutine(levelManager.IEEventTransitScene(step.changeScene.spawnIndex, step.changeScene.sceneIndex));
				break;

			case StepType.Custom:
                yield return StartCoroutine(step.customStep.Perform());
				break;

            default:
                Debug.LogError("Unknown StepType for Event '" + currentEvent.name + "' at step " + i, currentEvent);
                yield break;
			}

            if (step.overrideNextStep)
                nextStep = step.nextStep - 1;
            if (nextStep == i)
            {
                nextStep++;
                i++;
            }
            else
            {
                i = nextStep;
            }            
        }

        player.GetComponent<CharacterMovement>().EnableMovement(true);
        GameState.ChangeMood(moodChange);
        currentEvent.Complete();
        playingEvent = false;
    }


    // The step to stop the animation at
    public static void SetStopAnimation(int animator, int step)
    {
        instance.StartCoroutine(instance.StopAnimationAtStep(animator, step));
    }

    // Without parameter the animation will stop at the end of the Event
    public static void SetStopAnimation(int animator)
    {
        instance.StartCoroutine(instance.StopAnimationAtEnd(animator));
    }

    IEnumerator StopAnimationAtStep(int animator, int step)
    {
        while (playingEvent)
        {
            if (currentStep < step)
                yield return null;
            else
            {
                animationHandler.StopLoop(animator);
                yield break;
            }
        }

        animationHandler.StopLoop(animator);
        Debug.LogError("A looping animation was not stopped before the Event's end, step " + step + " never reached.", currentEvent);
    }

    IEnumerator StopAnimationAtEnd(int animator)
    {
        while (playingEvent)
            yield return null;

        animationHandler.StopLoop(animator);
    }


    // The step to stop the sound at
    public static void SetSoundStop(int step)
    {
        instance.StopCoroutine("StopSoundAtStep");
        instance.StopCoroutine("StopSoundAtEnd");
        instance.StartCoroutine(instance.StopSoundAtStep(step));
    }

    // Without parameter the sound will stop at the end of the Event
    public static void SetSoundStop()
    {
        instance.StopCoroutine("StopSoundAtStep");
        instance.StopCoroutine("StopSoundAtEnd");
        instance.StartCoroutine(instance.StopSoundAtEnd());
    }

    IEnumerator StopSoundAtStep(int step)
    {
        while (playingEvent)
        {
            if (currentStep < step)
                yield return null;
            else
            {
                audioHandler.StopLoop();
                yield break;
            }
        }

        audioHandler.StopLoop();
        Debug.LogError("A looping sound was not stopped before the Event's end, step " + step + " never reached.", currentEvent);
    }

    IEnumerator StopSoundAtEnd()
    {
        while (playingEvent)
            yield return null;

        audioHandler.StopLoop();
    }
}
