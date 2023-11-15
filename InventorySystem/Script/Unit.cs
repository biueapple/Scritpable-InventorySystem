using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stat))]
public class Unit : MonoBehaviour
{
    public Stat nomalStat;
    public Stat attributeStat;

    //레벨
    [SerializeField]
    private int level;
    public int Level { get { return level; } }
    //현재 경험치
    [SerializeField]
    private float exp;
    public float Exp
    {
        get { return exp; }
        set
        {
            exp += value;
            if (exp >= demandExp)
            {
                level++;
                exp -= demandExp;
                demandExp += 10;
            }
        }
    }
    //필요 경험치
    [SerializeField]
    private float demandExp;
    public float DemandExp { get { return demandExp; } }


    private void Awake()
    {
        //nomalStat.Init(100, 50, 0.1f, 0.1f, 10, 10, 10, 10, 3);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
