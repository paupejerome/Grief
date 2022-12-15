using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance { get; private set; } 

	[SerializeField]
	public GameObject _initialSpawnPoint;
	[SerializeField]
	public GameObject player;
	[SerializeField]
	Fader _fader;
	[SerializeField]
	int TestStartingScene;

	public int currentScene { get; private set; }

    private void Awake()
	{
		instance = this;
	}


	public void TransitScene(Vector3 newPosition, int nextScene, bool hidePlayer = false)
	{
        StartCoroutine(IETransitScene(newPosition, nextScene, hidePlayer));
    }
	IEnumerator IETransitScene(Vector3 newPosition, int nextScene, bool hidePlayer = false)
    {
		bool disableMov = player.GetComponent<CharacterMovement>().enabled;

		if(disableMov)
			player.GetComponent<CharacterMovement>().EnableMovement(false);

		_fader.FadeIn();
        yield return new WaitForSeconds(_fader.AnimLenght());

		player.transform.position = newPosition;

		if (hidePlayer)
			player.GetComponent<CharacterMovement>().HidePlayer(true);

		SceneManager.UnloadSceneAsync(currentScene);
		SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

		while (SceneManager.GetSceneByBuildIndex(currentScene).isLoaded || !SceneManager.GetSceneByBuildIndex(nextScene).isLoaded)
			yield return null;
		GameState.SetScene();

		if (!hidePlayer)
			player.GetComponent<CharacterMovement>().HidePlayer(false);

		_fader.FadeOut();
        yield return new WaitForSeconds(_fader.AnimLenght());

		if(disableMov && !EventManager.playingEvent)
			player.GetComponent<CharacterMovement>().EnableMovement(true);

		currentScene = nextScene;
	}
	public IEnumerator IEEventTransitScene(int spawnPointIdx, int sceneIdx)
	{
		Vector3 newPosition = transform.GetChild(spawnPointIdx).position;
		yield return StartCoroutine(IETransitScene(newPosition, sceneIdx));
	}

	public void StartGame()
    {
		StartCoroutine(IEStartGame());
	}
	IEnumerator IEStartGame()
    {
		_fader.FadeIn();
		yield return new WaitForSeconds(_fader.AnimLenght());

		SceneManager.LoadSceneAsync(TestStartingScene, LoadSceneMode.Additive);

		while (!SceneManager.GetSceneByBuildIndex(TestStartingScene).isLoaded)
			yield return null;
		GameState.SetScene();

		_fader.FadeOut();
		player.SetActive(true);
		yield return new WaitForSeconds(_fader.AnimLenght());

		currentScene = TestStartingScene;
	}
}
