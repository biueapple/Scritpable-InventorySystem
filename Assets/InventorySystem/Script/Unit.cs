using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Stat stat;

    // Start is called before the first frame update
    void Start()
    {
        stat.Init(100, 50, 0.1f, 0.1f, 10, 10, 10, 10, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
