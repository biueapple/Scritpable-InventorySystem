using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stat))]
public class Unit : MonoBehaviour
{
    public Unit enemy;
    public Stat stat;

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

    [SerializeField]
    private int skillPoint;
    public int SkillPoint { get { return skillPoint;} set { skillPoint += value; } }

    public Action SkillList = null;

    private void Awake()
    {
        stat.Init(100, 50, 0.1f, 0.1f, 10, 10, 10, 10, 1, 0, 1, 3);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(SkillList != null)
        {
            SkillList();
        }
    }


    public bool IsActionAlreadyRegistered(Action actionToCheck)
    {
        if (SkillList == null)
        {
            return false;
        }
        foreach (Delegate existingDelegate in SkillList.GetInvocationList())
        {
            if (existingDelegate == (Delegate)actionToCheck)
            {
                return true; // �̹� ��ϵ� ���
            }
        }
        return false;
    }

    public void Test()
    {
        enemy.stat.Be_Attacked(stat, 10, ATTACKTYPE.NOMAL, DAMAGETYPE.AD);
    }

}
