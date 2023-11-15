using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stat))]
public class Unit : MonoBehaviour
{
    public Stat nomalStat;
    public Stat attributeStat;

    //����
    [SerializeField]
    private int level;
    public int Level { get { return level; } }
    //���� ����ġ
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
    //�ʿ� ����ġ
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
