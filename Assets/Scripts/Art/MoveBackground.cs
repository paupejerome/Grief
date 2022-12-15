using UnityEngine;

public class MoveBackground : MonoBehaviour 
{
	public float speed;
	private float x;
	public float end;
	public float start;


	void Update () 
	{
		x = transform.position.x;
		x += speed * Time.deltaTime;
		transform.position = new Vector3 (x, transform.position.y, transform.position.z);


		if (x <= end)
		{
			x = start;
			transform.position = new Vector3 (x, transform.position.y, transform.position.z);
		}
	}
}
