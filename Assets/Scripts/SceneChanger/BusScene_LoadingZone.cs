using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusScene_LoadingZone : MonoBehaviour
{
    [SerializeField]
    float sceneDuration = 5;

    [SerializeField]
    int _idxSceneRoadsideHome;
    [SerializeField]
    GameObject _spawnPointSceneRoadsideHome;
    [SerializeField]
    int _idxSceneRoadsideOffice;
    [SerializeField]
    GameObject _spawnPointSceneRoadsideOffice;

    int _sceneWeCameFrom;

    private void Awake()
    {
        _sceneWeCameFrom = LevelManager.instance.currentScene;
    }

    void Start()
    {
        StartCoroutine(IELeaveBusScene());
    }


    IEnumerator IELeaveBusScene()
    {
        yield return new WaitForSeconds(sceneDuration);

        if (_sceneWeCameFrom == _idxSceneRoadsideOffice)
        {
            LevelManager.instance.TransitScene(_spawnPointSceneRoadsideHome.transform.position, _idxSceneRoadsideHome);
        }
        else if (_sceneWeCameFrom == _idxSceneRoadsideHome)
        {
            LevelManager.instance.TransitScene(_spawnPointSceneRoadsideOffice.transform.position, _idxSceneRoadsideOffice);
        }
        else
        {
            Debug.LogError("Look at your BusScene_LoadingZone.cs, you noob.");
        }

    }

}
