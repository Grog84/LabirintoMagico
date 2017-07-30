using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSortLayerScript : MonoBehaviour {

    void Update ()
    {
        this.GetComponent<Renderer>().sortingLayerName = "Particle";
    }

}
