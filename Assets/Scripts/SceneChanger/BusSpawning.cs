using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BusSpawning : MonoBehaviour
{
    [SerializeField]
    GameObject spawnPoint;

    [SerializeField]
    int nextIndex;




    [SerializeField]
    public static bool isPickingUpCharacter = false; 

    [SerializeField]
    GameObject bus;

    [SerializeField]
    GameObject endOfTrack;

    [SerializeField]
    GameObject trueEndOfTrack;

    [SerializeField]
    float busSpeedX = 1;

    [SerializeField]
    GameObject spawner;

    [SerializeField]
    float WaitForSeconds = 2;

    bool busIsComing;

    Vector3 busSpeed;

    bool busGoingAway = false;


    float oldGenSpeedMin;
    float oldGenSpeedMax;

    [SerializeField]
    GameObject charDisplacer;

    GameObject cameraTarget;
    GameObject character;


    //Station specific
    [SerializeField]
    GameObject keyPress;

    public bool changingScene;


    void Start()
    {
        changingScene = false;


        cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget");

        character = GameObject.FindGameObjectWithTag("Player");

        busSpeed = new Vector3(busSpeedX, 0, 0);

        keyPress.SetActive(false);

        isPickingUpCharacter = !isPickingUpCharacter;

        if (isPickingUpCharacter)
        {
            busIsComing = false;
        }
        else
        {
            oldGenSpeedMin = spawner.GetComponent<RandomSpawner>().MinGenerationSpeed;
            oldGenSpeedMax = spawner.GetComponent<RandomSpawner>().MaxGenerationSpeed;

            spawner.GetComponent<RandomSpawner>().MinGenerationSpeed = 999;
            spawner.GetComponent<RandomSpawner>().MaxGenerationSpeed = 999;
        }
    }

    void Update()
    {
        if (changingScene)
        {
            return;
        }

        if (isPickingUpCharacter)
        {
            if (busIsComing)
            {
                if (bus.transform.position.x > endOfTrack.transform.position.x)
                {
                    bus.transform.position -= busSpeed * Time.deltaTime;
                }
                else
                {
                    StartCoroutine(BusIsAtStation());
                }
            }
        }
        else
        {
            if (bus.transform.position.x > endOfTrack.transform.position.x)
            {
                bus.transform.position -= busSpeed * Time.deltaTime;

                StartCoroutine(GoGoLaterAutobus());

                StartMovingCharacter();
            }
        }

        if (busGoingAway && bus.transform.position.x > trueEndOfTrack.transform.position.x)
        {
            bus.transform.position -= busSpeed * Time.deltaTime;


            if (isPickingUpCharacter)
            {
                StartMovingCharacter();
            }
            else
            {
                StartCoroutine(EndMovingCharacterTimer());
            }
        }

        if (bus.transform.position.x <= trueEndOfTrack.transform.position.x && isPickingUpCharacter)
        {
            changingScene = true;
            LevelManager.instance.TransitScene(spawnPoint.transform.position, nextIndex);
        }
    }




    private void OnTriggerEnter(Collider other)
    {
        if (isPickingUpCharacter)
        {
            keyPress.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        keyPress.SetActive(false);
    }

    public void pressE(InputAction.CallbackContext context)
    {
        if (keyPress.activeInHierarchy)
        {
            CallBus();

            keyPress.SetActive(false);
        }
    }

    void CallBus()
    {
        spawner.GetComponent<RandomSpawner>().MinGenerationSpeed = 999;
        spawner.GetComponent<RandomSpawner>().MaxGenerationSpeed = 999;

        StartCoroutine(GoGoAutobus());

        character.GetComponent<CharacterMovement>().EnableMovement(false);
    }


    IEnumerator BusIsAtStation()
    {
        yield return new WaitForSeconds(WaitForSeconds);

        busGoingAway = true;
    }




    IEnumerator GoGoAutobus()
    {
        yield return new WaitForSeconds(WaitForSeconds);

        busIsComing = true;
    }




    IEnumerator GoGoLaterAutobus()
    {
        yield return new WaitForSeconds(WaitForSeconds);

        spawner.GetComponent<RandomSpawner>().MinGenerationSpeed = oldGenSpeedMin;
        spawner.GetComponent<RandomSpawner>().MaxGenerationSpeed = oldGenSpeedMax;

        busGoingAway = true;
    }


   


    void StartMovingCharacter()
    {
        cameraTarget.GetComponent<vcamFollow>().setTarget(charDisplacer);

        character.GetComponent<CharacterMovement>().EnableMovement(true);

        character.GetComponent<CharacterMovement>().HidePlayer(true);
    }

    void EndMovingCharacter()
    {
        character.GetComponent<CharacterMovement>().HidePlayer(false);

        cameraTarget.GetComponent<vcamFollow>().setTarget(character);
    }

    IEnumerator EndMovingCharacterTimer()
    {
        yield return new WaitForSeconds(1);

        EndMovingCharacter();
    }
}
