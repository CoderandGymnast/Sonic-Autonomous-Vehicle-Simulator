using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStable : MonoBehaviour
{

	public GameObject car;
	public float carX;
	public float carY;
	public float carZ;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		carX = car.transform.eulerAngles.x; // NOTE: Euler angles: Rotation angles.
		carY = car.transform.eulerAngles.y;
		carZ = car.transform.eulerAngles.z;

		transform.eulerAngles = new Vector3(0, carY, 0); // NOTE: Rotation angles with respect to Unity 3D coordinates system.
	}
}
