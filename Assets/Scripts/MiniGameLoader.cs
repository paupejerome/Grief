using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MiniGameLoader : MonoBehaviour, IConditionalTrigger
{
	[SerializeField] int miniGameIndex = 1;
	[SerializeField] int winMoodChange;
	[SerializeField] int lossMoodChange;
	[SerializeField] bool morning;
	[SerializeField] bool evening;
	[SerializeField] GameObject promptPrefab;

	GameObject prompt;
	static MiniGameLoader instance;
	static Transform player;
	static Vector3 playerPos;
	static int currentScene;
	static int moodChange;

	private void Awake()
	{
		instance = this;
		GetComponent<Collider>().isTrigger = true;
	}

	private void Start()
	{
		GameState.Register(this);
		CheckToEnable();

		prompt = Instantiate(promptPrefab, transform);
		prompt.transform.localPosition += new Vector3(0, 2, 8);
		if (!prompt)
			Debug.Log("Missing prompt prefab", gameObject);
		prompt.SetActive(false);
	}

	public void CheckToEnable()
	{
		gameObject.SetActive( (morning && GameState.CheckCondition(Conditions.Morning)) || (evening && GameState.CheckCondition(Conditions.Evening)) );
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			prompt.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			prompt.SetActive(false);
		}
	}

	public void StartGame()
	{
		GameManager.GetInstance()._taskBox.SetActive(false);
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerPos = player.position;
		currentScene = LevelManager.instance.currentScene;
		moodChange = 0;

		LevelManager.instance.TransitScene(Vector3.zero, miniGameIndex, true);		
	}

	public static void EndGame(bool result)
	{
		LevelManager.instance.TransitScene(playerPos, currentScene, false);
		moodChange = result ? instance.winMoodChange : instance.lossMoodChange;
		GameManager.GetInstance().EnableTaskBar("Go to sleep");
	}

	private void OnDestroy()
	{
		GameState.Unregister(this);
		GameState.ChangeMood(moodChange);
		moodChange = 0;
	}	
}
