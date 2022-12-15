using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour, IConditionalTrigger
{
	[SerializeField] public Event thisEvent;
	[SerializeField] GameObject promptPrefab;
	GameObject prompt;

	private void Start()
	{
		GameState.Register(this);
		gameObject.SetActive(thisEvent.active);
		GetComponent<Collider>().isTrigger = true;

        if (thisEvent.requireInput)
        {			
			prompt = Instantiate(promptPrefab, transform);
			if (!prompt)
				Debug.Log("Missing prompt prefab", gameObject);
			prompt.SetActive(false);
        }
	}

	public void CheckToEnable()
	{
		gameObject.SetActive(thisEvent.active);
	}

	public void CallStartEvent()
    {
		if(prompt)
			prompt.SetActive(false);
		gameObject.SetActive(false);
		EventManager.StartEvent(thisEvent);		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (EventManager.playingEvent || other.tag != "Player")
			return;

		if (!thisEvent.requireInput)
			CallStartEvent();
        else
			prompt.SetActive(true);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag != "Player")
			return;

		if (thisEvent.requireInput)
			prompt.SetActive(false);
	}

	private void OnDestroy()
	{
		GameState.Unregister(this);
	}
}

