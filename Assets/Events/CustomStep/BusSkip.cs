using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusSkip : CustomStep
{
    [SerializeField] GameObject bus;
    public override IEnumerator Perform()
    {
        GameObject.Find("__LEVEL__").transform.GetChild(3).GetChild(3).GetComponent<RandomSpawner>().MinGenerationSpeed = 999;
        GameObject.Find("__LEVEL__").transform.GetChild(3).GetChild(3).GetComponent<RandomSpawner>().MaxGenerationSpeed = 999;
        GameObject currentBus = Instantiate<GameObject>(bus, new Vector3(70, -3.7f, 0), Quaternion.identity, GameObject.Find("__CHANGEMENT DE SCENE__").transform);
        GameManager.GetInstance()._player.GetComponent<CharacterMovement>().SwitchDirection();
        yield return new WaitForSeconds(0.25f);
        while (currentBus.transform.position.x > 20)
        {
            currentBus.transform.position += new Vector3(-75,0,0) * Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        GameManager.GetInstance()._player.GetComponent<CharacterMovement>().SwitchDirection();
        while (currentBus.transform.position.x > -30)
        {
            currentBus.transform.position += new Vector3(-75, 0, 0) * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        FindObjectOfType<BusSpawning>().changingScene = true;
        BusSpawning.isPickingUpCharacter = !BusSpawning.isPickingUpCharacter;
        //throw new System.NotImplementedException();
    }
}
