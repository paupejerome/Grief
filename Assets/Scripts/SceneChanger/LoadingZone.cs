using UnityEngine;

public class LoadingZone : MonoBehaviour, IConditionalTrigger
{
	[SerializeField]
	int _nextIndex;
	[SerializeField]
	GameObject _spawnPoint;

	[Space]
	[SerializeField] bool morning;
	[SerializeField] bool evening;

	private void Start()
	{
		GameState.Register(this);
		CheckToEnable();
	}

	public void CheckToEnable()
	{
		gameObject.SetActive( (morning && GameState.CheckCondition(Conditions.Morning)) || (evening && GameState.CheckCondition(Conditions.Evening)) );
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			LevelManager.instance.TransitScene(_spawnPoint.transform.position, _nextIndex);
		}
	}

	private void OnDestroy()
	{
		GameState.Unregister(this);
	}
}
