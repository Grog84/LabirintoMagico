using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBase : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position.Set(Random.Range(-10, 10), Random.Range(-10, 10), 2);	
	}
}
