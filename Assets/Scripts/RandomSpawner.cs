using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject EndOfTrack;

    [SerializeField] private int LayerOfObjects = 0;

    [SerializeField] GameObject[] Spawnables;

    [SerializeField] float NormalSpeed;

    //How many per second
    [SerializeField] public float MinGenerationSpeed;
    [SerializeField] public float MaxGenerationSpeed;

    [SerializeField] bool Reverse;

    private List<GameObject> Instances;

    GameObject inst;

    private float timer;


    [HideInInspector] public bool RandomSpeed;
    [HideInInspector] public float MinSpeed = 0;
    [HideInInspector] public float MaxSpeed = 0;
    private List<float> Speed;

    [HideInInspector] public bool RandomY;
    [HideInInspector] public float MinY = 0;
    [HideInInspector] public float MaxY = 0;
    private Vector3 SpawnPos;


    private void Awake()
    {
        Instances = new List<GameObject>();
        Speed = new List<float>();
        inst = new GameObject();
    }

    void Start()
    {
        timer = RNG();

        SpawnPos = transform.position;
    }

    void Update()
    {
        Spawn();

        Advance();
    }

    void Spawn()
    {
        timer -= Time.deltaTime;
        if (timer<=0)
        {
            Instanciate();
            timer = RNG();
        }
    }

    void Advance()
    {
        for (int i = 0; i < Instances.Count; i++)
        {
            if (Instances[i] != null)
            {
                if (Reverse)
                {
                    Instances[i].transform.position += new Vector3(-Speed[i], 0, 0) * Time.deltaTime;

                    if (Instances[i].transform.position.x <= EndOfTrack.transform.position.x)
                    {
                        Remove(i);
                    } 
                }
                else
                {
                    Instances[i].transform.position += new Vector3(Speed[i], 0, 0) * Time.deltaTime;

                    if (Instances[i].transform.position.x >= EndOfTrack.transform.position.x)
                    {
                        Remove(i);
                        
                    }
                }
            }
        }
    }


    float RNG()
    {
        if (MinGenerationSpeed>MaxGenerationSpeed)
        {
            MinGenerationSpeed = MaxGenerationSpeed;
        }
        return Random.Range(MinGenerationSpeed, MaxGenerationSpeed);
    }

    void Instanciate()
    {
        if (RandomY)
        {
            SpawnPos.y = transform.position.y + Random.Range(MinY, MaxY);
        }

        inst = Instantiate(Spawnables[Random.Range(0, Spawnables.Length)], SpawnPos, Quaternion.identity);

        ChangeLayer(inst);

        if (Reverse)
        {
            inst.transform.localScale = new Vector3(-inst.transform.localScale.x, inst.transform.localScale.y, inst.transform.localScale.z);
        }


        inst.transform.parent = transform;
        Instances.Add(inst);


        float speed = NormalSpeed;
        if (RandomSpeed)
        {
            Speed.Add(Random.Range(MinSpeed, MaxSpeed));
        }
        else
        {
            Speed.Add(speed);
        }
    }

    void ChangeLayer(GameObject i)
    {
        if (i.GetComponent<SpriteRenderer>() != null)
        {
            i.GetComponent<SpriteRenderer>().sortingOrder = LayerOfObjects;
        }
        else
        {
            if (i.GetComponentInChildren<SpriteRenderer>() != null)
            {
                i.GetComponentInChildren<SpriteRenderer>().sortingOrder = LayerOfObjects;
            }
        }
    }

    private void Remove(int i)
    {
        Destroy(Instances[i]);
        Instances.Remove(Instances[i]);
        Speed.Remove(Speed[i]);
    }
}
