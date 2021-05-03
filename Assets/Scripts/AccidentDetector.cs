using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccidentDetector : MonoBehaviour

{

	public bool isCollided { get; private set; } = false;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}

	public bool isFalling()
	{
		return gameObject.transform.position.y < 0 ? true : false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("OnCollisionEnter");
		isCollided = true;
	}
}
