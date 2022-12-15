using UnityEngine;

public class vcamFollow : MonoBehaviour
{
   
    public GameObject target;

    public bool MaxCoords;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (target)
        {
            transform.position = target.transform.position;

            if (MaxCoords)
            {
                float i;

                if (transform.position.x < minX)
                {
                    i = minX - transform.position.x;
                    transform.position += new Vector3(i, 0, 0);
                }
                else if (transform.position.x > maxX)
                {
                    i = transform.position.x - maxX;
                    transform.position -= new Vector3(i, 0, 0);
                }

                if (transform.position.y < minY)
                {
                    i = minY - transform.position.y;
                    transform.position += new Vector3(0, i, 0);
                }
                else if (transform.position.y > maxY)
                {
                    i = transform.position.y - maxY;
                    transform.position -= new Vector3(0, i, 0);
                }
            }
        }
    }

    public void setTarget(GameObject t)
    {
        target = t;
    }
}