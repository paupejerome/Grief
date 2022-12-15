using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum StepType { Dialogue, Choice, Animation, Sound, ChangeScene, Custom };

[Serializable]
public class EventStep
{
	public StepType stepType;
	public Dialogue[] dialogues;
	public Choice[] choices;
	public Animation[] animations;
	public Sound[] sounds;
	public ChangeScene changeScene;
	public CustomStep customStep;

	[Tooltip("Should the Event's next step be overriden?")]
	public bool overrideNextStep;
	[Tooltip("The Event's next step")]
	public int nextStep;
}


[Serializable]
public struct Dialogue
{
	public string text;
	[Tooltip("Display the text as italic, to represent the player's thoughts")]
	public bool italic;
}

[Serializable]
public struct Choice
{
	[Tooltip("The choice's text")]
	public string choiceText;
	[Tooltip("The minimum mood required for this choice to be available")]
	public int minMood;
	[Tooltip("The maximum mood required for this choice to be available")]
	public int maxMood;
	[Tooltip("Should the event's 'moodChange' be overriden by the choice?")]
	public bool overrideMoodChange;
	[Tooltip("Override the event's 'moodChange' value (only the last override is applied)")]
	public int moodChange;
	[Tooltip("Should the event's next step be overriden by the choice?")]
	public bool overrideNextStep;
	[Tooltip("The Event's next step")]
	public int nextStep;
}

[Serializable]
public struct Animation
{
	[Tooltip("Is this animation performed by the player character?")]
	public bool mainCharacter;
	[Tooltip("The clip to be played")]
	public AnimationClip animationClip;
	//[Tooltip("The index of the AnimationHandler's child Transform where the animation should be performed")]
	//public int childTransformIndex;
	[Tooltip("The world position where the animation should be played if it's not on main character")]
	public Vector3 position;
	[Tooltip("Make the animation face the other way")]
	public bool flip;

	[Tooltip("Move as the animation is playing, also applies looping")]
	public bool applyMotion;
	[Tooltip("The position where to stop moving")]
	public Vector3 destination;
	[Tooltip("The time it will take to reach the desination")]
	public float travelTime;

	[Tooltip("Should the animation loop?")]
	public bool loop;
	[Tooltip("Stop the animation at the end of the Event")]
	public bool stopAtEnd;
	[Tooltip("Step at which the animation will stop plating")]
	public int stopAtStep;

	[Tooltip("The animation's playback speed")]
	public float playbackSpeed;
	[Tooltip("Should the animation or motion complete before continuing?")]
	public bool waitForEnd;
}

[Serializable]
public struct Sound
{
	public AudioClip audioClip;
	[Tooltip("Volume 0 to 1")]
	public float volume;
	[Tooltip("Playing a looping sound will stop any previous looping sound")]
	public bool loop;
	[Tooltip("Stop the looping clip at the end of the Event")]
	public bool stopAtEnd;
	[Tooltip("Step at which the sound will stop playing")]
	public int stopAtStep;
	[Tooltip("Should the clip complete before continuing?")]
	public bool waitForEnd;
}

[Serializable]
public struct ChangeScene
{
	[Tooltip("The index of the scene to load")]
	public int sceneIndex;
	[Tooltip("Index of the LevelManager's child whose transform is the new player position (DON'T USE 0)")]
	public int spawnIndex;
}

[DisallowMultipleComponent]
public abstract class CustomStep : MonoBehaviour
{
	public abstract IEnumerator Perform();
}
