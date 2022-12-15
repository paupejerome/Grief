using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Day { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
public enum DayTime { Morning, /*Noon,*/ Evening }
public enum Conditions { Weekday, Weekend, Morning, /*Noon,*/ Evening, Sunny, Raining, count }

public interface IConditionalTrigger { public void CheckToEnable(); }


public class GameState : MonoBehaviour
{
	static GameState instance;
	[SerializeField] GameObject player;
	[SerializeField] Fader fader;
	[SerializeField] int startingMood = 40;
	[SerializeField] Day startingDay = Day.Monday;
	[SerializeField] DayTime startingTime = DayTime.Morning;
	[Tooltip("The time when the day resets, changing main event")]
	[SerializeField] DayTime resetTime = DayTime.Morning;
	[Tooltip("Chance of rain every DayTime, 0 to 1")]
	[SerializeField] float baseRainChance = 0.2f;
	[Tooltip("Chance to keep raining, 0 to 1")]
	[SerializeField] float rainChanceIfRaining = 0.6f;

	public static int mood { get; private set; }
	static int day;
	static public int time;
	static float rain;
	static bool[] conditions;
	static List<IConditionalTrigger> triggers;

	private void Awake()
	{
		instance = this;
		
		mood = startingMood;
		day = (int)startingDay;
		time = (int)startingTime;
		baseRainChance = Mathf.Clamp(baseRainChance, 0f, 1f);
		rainChanceIfRaining = Mathf.Clamp(rainChanceIfRaining, 0f, 1f);
		rain = baseRainChance;

		conditions = new bool[(int)Conditions.count];
		triggers = new List<IConditionalTrigger>();		

		UpdateConditions();
		EventManager.UpdateEvents(false);
	}

	public static void AdvanceTime()
	{
		time++;
		if (time == 2)
		{
			time = 0;
			day++;
			if (day == 7)
				day = 0;
		}

		instance.UpdateGameState();
		Debug.Log((Day)day + " , " + (DayTime)time);
	}

	void UpdateGameState()
	{
		UpdateConditions();
		StartCoroutine(Fade());
		EventManager.UpdateEvents(time == (int)resetTime);
		if (time == (int)DayTime.Morning)
			FindObjectOfType<SoundManager>().ChangeTrack();

		foreach (IConditionalTrigger trigger in triggers)
			trigger.CheckToEnable();
	}

	static void UpdateConditions()
	{
		rain = conditions[5] ? instance.rainChanceIfRaining : instance.baseRainChance;

		conditions[0] = day <= 4;				// weekday
		conditions[1] = day > 4;				// weekend
		conditions[2] = time == 0;				// morning
		//conditions[3] = time == 1;
		conditions[3] = time == 1;				// evening
		conditions[4] = rain < Random.value;	// sunny
		conditions[5] = !conditions[4];			// raining
	}

	IEnumerator Fade()
	{
		bool playerActive = player.activeSelf;
		if(playerActive)
			player.GetComponent<CharacterMovement>().EnableMovement(false);
		fader.FadeIn();
		yield return new WaitForSeconds(fader.AnimLenght());

		SetScene();

		fader.FadeOut();
		yield return new WaitForSeconds(fader.AnimLenght());
		if(playerActive && !EventManager.playingEvent)
			player.GetComponent<CharacterMovement>().EnableMovement(true);
	}

	public static void SetScene()
	{
		string s = string.Empty;

		GameObject day = GameObject.Find("Day");
		GameObject night = GameObject.Find("Night");
		if (day && night)
		{
			night.transform.GetChild(0).gameObject.SetActive(conditions[3]);
			day.transform.GetChild(0).gameObject.SetActive(!conditions[3]);

			s += conditions[3] ? "Night, " : "Day, ";
		}
		else
			s += "No Time GameObject, ";

		GameObject rain = GameObject.Find("Rain");
		if (rain)
		{
			foreach(Transform obj in rain.GetComponentInChildren<Transform>())
				obj.gameObject.SetActive(conditions[5]);

			s += conditions[5] ? "Raining" : "Sunny";
		}
		else
			s += "No Weather Object";

		Debug.Log(s);
	}

	public static void ToggleRain(bool set)
    {
		GameObject rain = GameObject.Find("Rain");
		if (rain)
		{
			foreach (Transform obj in rain.GetComponentInChildren<Transform>())
				obj.gameObject.SetActive(set);
		}
		conditions[4] = !conditions[4];
		conditions[5] = !conditions[5];
	}

	public static void Register(IConditionalTrigger trigger)
	{
		if (!triggers.Contains(trigger))
			triggers.Add(trigger);
		else
			Debug.Log("Adding a trigger that's already in list...");
	}

	public static void Unregister(IConditionalTrigger trigger)
	{
		if (triggers.Contains(trigger))
			triggers.Remove(trigger);
		else
			Debug.Log("Removing a trigger that's not in list...");
	}


	public static bool CheckCondition(Conditions condition)
	{
		return conditions[(int)condition];
	}

	public static bool CheckCondition(int condition)
	{
		return conditions[condition];
	}

	public static void ChangeMood(int value)
	{
		mood = Mathf.Clamp(mood + value, 0, 100);
		Debug.Log("Mood : "+  mood);
	}
}
