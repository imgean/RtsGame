using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 1f; // Time after which the particle will be destroyed
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
   
}
