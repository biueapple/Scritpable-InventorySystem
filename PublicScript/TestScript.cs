using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        ObjectPooling.Active.Add(player);
        ObjectPooling.Active.Add(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, enemy.transform.position);
    }
}
