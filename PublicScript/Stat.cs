using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public enum DAMAGETYPE
{
    AD,
    AP,
    TRUE
}

public enum ATTACKTYPE
{
    NONE,
    NOMAL,
    SPECIAL
}

public enum ATTRIBUTES
{
    HP,
    MP,
    DEFENCE,
    RESISTANCE,
    AD,
    AP,
    ATTACKSPEED,
    CRITICAL,
    CRITICALMULTIPLIER,
    SPEED,
}

//shield
//Barrier
//protective

[System.Serializable]
public class Stat : MonoBehaviour
{
    //ü��
    [SerializeField]
    private float hp;
    public float HP { get { return  hp; } }
    [SerializeField]
    private float maxHp;
    public float MAXHP { get { return maxHp; } set { maxHp += value; hp += value; } }
    //ü�� �ڿ� ȸ����
    private float naturalHP;

    private List<Barrier> barrier;
    public float barrierFigure 
    {
        get
        {
            float f = 0;
            if (barrier == null)
                return f;
            for (int i = 0; i < barrier.Count; i++)
                f += barrier[i].figure;
            return f;
        }
    }
    public Barrier Barrier 
    { 
        set
        {
            if (barrier == null)
                barrier = new List<Barrier>();
            barrier.Add(value);
            if (barrierCoroutine == null)
                barrierCoroutine = StartCoroutine(BarrierCoroutine());
        }
    }
    private Coroutine barrierCoroutine = null;

    //����
    [SerializeField]
    private float mp;
    public float MP { get { return mp; } }
    [SerializeField]
    private float maxMp;
    public float MAXMP { get { return maxMp; } set { maxMp += value; mp += value; } }
    private float naturalMP;

    //����
    [SerializeField]
    private float defence;
    public float DEFENCE { get { return defence; } set { defence += value; } }
    
    //���׷�
    [SerializeField]
    private float resistance;
    public float RESISTANCE { get { return resistance; } set { resistance += value; } }

    //���ݷ�
    [SerializeField]
    private float ad;
    public float AD { get { return ad; } set { ad += value; } }

    //�ֹ���
    [SerializeField]
    private float ap;
    public float AP { get { return ap; } set { ap += value; } }

    //���ݼӵ�
    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed += value; } }

    //ġ��Ÿ Ȯ��
    [SerializeField]
    private float critical;
    public float Critical { get { return critical; } set { critical += value; } }

    //ġ��Ÿ ����
    [SerializeField]
    private float criticalMultiplier;
    public float CriticalMultiplier { get { return criticalMultiplier; } set { criticalMultiplier += value; } }

    //�̵��ӵ�
    [SerializeField]
    private float speed;
    public float SPEED { get { return speed; } set { speed += value; } }


    //Natural_Recovery_HP
    private Coroutine nrhp = null;
    //Natural_Recovery_MP
    private Coroutine nrmp = null;
    //Dot_Poison
    private Coroutine dot_poison = null;
    private Coroutine dot_burn = null;
    private Coroutine dot_shock = null;
    private Coroutine dot_bleeding = null;

    //����ü�� ��� ���� �Ѵ� ���ϼ��� ���� �¾����� attack hit �Ѵ� ȣ���ϴ� �ɷ�
    //�������� �ߵ��ϴ� ��������Ʈ�� return�̳� out���� ������� ������ �ϴ� ����� �����ߴµ�
    //out Ű����� �Ұ����ϰ� return�� ���������� �� �Լ��� ���ϸ� �ޱ⶧����
    //list�� �ϳ��� ȣ���ϱ�� ��

    //Func�� ��� ������ ������ ��ġ ���� ��
    //Action�� ��� ������ ������ ��ġ ��

    //�⺻ ���� �� ���� �ߵ�
    private List<Func<Stat, Stat, float, float>> nomalAttackBefore = null;

    //�⺻ ���� �� �Ŀ� �ߵ�
    private Action<Stat, Stat, float> nomalAttackAfter = null;

    //�� ���� ���� ���� �ߵ�
    private List<Func<Stat, Stat, float, float>> specialAttackBefore = null;
    // �Ŀ� �ߵ�
    private Action<Stat, Stat, float> specialAttackAfter = null;

    //�⺻���� �ޱ� ���� �ߵ�
    private List<Func<Stat, Stat, float, float>> hitNomalBefore = null;
    // �Ŀ� �ߵ�
    private Action<Stat, Stat, float> hitNomalAfter = null;

    //�� ���� ������ �ޱ� ���� �ߵ�
    private List<Func<Stat, Stat, float, float>> hitSpecialBefore = null;
    // �Ŀ� �ߵ�
    private Action<Stat, Stat, float> hitSpecialAfter = null;

    //���� ������ ȸ�� ��Ű�� ���� (hp) �ڽ� ����
    private List<Func<Stat, Stat, float, float>> healOtherBefore = null;
    // �Ŀ� �ߵ�
    private Action<Stat, Stat, float> healOtherAfter = null;

    //color��
    private Color red = new Color(0.5660378f, 0, 0);
    private Color green = new Color(0, 0.5647059f, 0);
    private Color blue = new Color(0, 0.2810156f, 1);

    //�����̻��� �޴��� �ȹ޴���
    public bool poison = true;
    public bool burn = true;
    public bool shock = true;
    public bool bleeding = true;


    //��Ʈ���̳� �ڿ� ȸ���� ���ʿ� �ѹ� �Ͼ��
    public float dotTime = 2;
    public float naturalTime = 2;

    public string SetText()
    {
        string str = "ü�� = " + Mathf.Floor(hp * 100f) / 100f + " / " + Mathf.Floor(maxHp * 100f) / 100f + "\n" +
                    "���� = " + Mathf.Floor(mp * 100f) / 100f + " / " + Mathf.Floor(maxMp * 100f) / 100f + "\n" +
                    "ü����� = " + naturalHP + "\n" +
                    "������� = " + naturalMP + "\n" +
                    "���� = " + defence + "\n" +
                    "���׷� = " + resistance + "\n" +
                    "���ݷ� = " + ad + "\n" +
                    "�ֹ��� = " + ap + "\n" +
                    "�ӵ� = " + speed;
        return str;
    }

    public void Init(float _hp, float _mp,float _nh,float _nm, float _de, float _re, float _ad,float _ap, float _attackSpeed, float _critical, float _critacalMultiple, float _speed)
    {
        hp = _hp;
        maxHp = _hp;
        mp = _mp;
        maxMp = _mp;
        naturalHP = _nh;
        naturalMP = _nm;
        defence = _de;
        resistance = _re;
        ad = _ad;
        ap = _ap;
        attackSpeed = _attackSpeed;
        critical = _critical;
        criticalMultiplier = _critacalMultiple;
        speed = _speed;
    }

    public void MinusHp(float figure)
    {
        figure = BarrierAbsorption(figure);
        hp -= figure;
        if(nrhp == null )
        {
            nrhp = StartCoroutine(Natural_Recovery_HP());
        }
        Debug.Log($"{name} ���� ü�� {hp}");
    }

    private IEnumerator BarrierCoroutine()
    {
        while(barrier.Count != 0)
        {
            for(int i = 0; i < barrier.Count; i++)
            {
                barrier[i].timer -= Time.deltaTime;
                if(barrier[i].timer <= 0)
                {
                    barrier.RemoveAt(i);
                    i--;
                }
            }
            yield return null;
        }
        barrierCoroutine = null;
    }

    public void MinusMp(float figure) 
    { 
        mp -= figure;
        if (mp < 0)
            mp = 0;
        if (nrmp == null)
        {
            nrmp = StartCoroutine(Natural_Recovery_MP());
        }
    }

    public void RecoveryHP(Stat perpetrator, float figure)
    {
        figure = CallHealBefore(perpetrator, this, figure);

        if (hp + figure > maxHp)
            figure = maxHp - hp;
        hp += figure;

        CallHealAfter(perpetrator, this, figure);
    }

    public void RecoveryMP(Stat perpetrator, float figure)
    {
        if (mp + figure > maxMp)
            figure = maxMp - mp;
        mp += figure;
    }

    public float BarrierAbsorption(float f)
    {
        if (barrier == null)
            return f;
        for(int i = 0; i < barrier.Count; i++)
        {
            if (barrier[i].figure <= f)
            {
                f -= barrier[i].figure;
                barrier.RemoveAt(i);
                i--;
            }
            else
            {
                barrier[i].figure -= f;
                f = 0;
                break;
            }
            if (f <= 0)
                return 0;
        }
        return f;
    }

    /// <summary>
    /// ad ������� ���¿� �°� ��������
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�� ����� (����) </param>
    /// <param name="per">�� ����� (�ۼ�Ʈ)</param>
    /// <returns>���°� ������� ���İ� �����</returns>
    public float Halved_AD(float figure, float penetration, float per)
    {
        float defence = this.defence - penetration;
        defence -= defence * per * 0.01f;
        return (100 / (100 + defence) * figure);
    }
    /// <summary>
    /// ap ������� ���¿� �°� ��������
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">���׷� ����� (����) </param>
    /// <param name="per">���׷� ����� (�ۼ�Ʈ)</param>
    /// <returns>���׷°� ������� ���İ� �����</returns>
    public float Halved_AP(float figure, float penetration, float per)
    {
        float resistance = this.resistance - penetration;
        resistance -= resistance * per * 0.01f;
        return (100 / (100 + resistance) * figure);
    }

    public float Be_Attacked(Stat perpetrator, float figure, ATTACKTYPE attack, DAMAGETYPE damage)
    {
        switch(attack)
        {
            case ATTACKTYPE.NONE:
                {
                    figure = CallHitSpecialBefore(perpetrator, this, figure);
                    switch (damage)
                    {
                        case DAMAGETYPE.AD:
                            Be_Attacked_AD(figure, perpetrator);
                            break;
                        case DAMAGETYPE.AP:
                            Be_Attacked_AP(figure, perpetrator);
                            break;
                        case DAMAGETYPE.TRUE:
                            Be_Attacked_TRUE(figure, perpetrator);
                            break;
                    }
                    CallHitSpecialAfter(perpetrator, this, figure);
                }
                break;
            case ATTACKTYPE.NOMAL:
                {
                    figure = perpetrator.CallNomalAttackBefore(perpetrator, this, figure);
                    figure = CallHitNomalBefore(perpetrator, this, figure);
                    switch (damage)
                    {
                        case DAMAGETYPE.AD:
                            Be_Attacked_AD(figure, perpetrator);
                            break;
                        case DAMAGETYPE.AP:
                            Be_Attacked_AP(figure, perpetrator);
                            break;
                        case DAMAGETYPE.TRUE:
                            Be_Attacked_TRUE(figure, perpetrator);
                            break;
                    }
                    CallHitNomalAfter(perpetrator, this, figure);
                    perpetrator.CallNomalAttackAfter(perpetrator, this, figure);
                }
                break;
            case ATTACKTYPE.SPECIAL:
                {
                    figure = perpetrator.CallSpecialAttackBefore(perpetrator, this, figure);
                    figure = CallHitSpecialBefore(perpetrator, this, figure);
                    switch (damage)
                    {
                        case DAMAGETYPE.AD:
                            Be_Attacked_AD(figure, perpetrator);
                            break;
                        case DAMAGETYPE.AP:
                            Be_Attacked_AP(figure, perpetrator);
                            break;
                        case DAMAGETYPE.TRUE:
                            Be_Attacked_TRUE(figure, perpetrator);
                            break;
                    }
                    CallHitSpecialAfter(perpetrator, this, figure);
                    perpetrator.CallSpecialAttackAfter(perpetrator, this, figure);
                }
                break;
        }

        return figure;
    }

    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�����</param>
    /// <param name="per">����� (�ۼ�Ʈ)</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    private float Be_Attacked_AD(float figure, Stat perpetrator)
    {
        float damage = Halved_AD(figure, 0, 0);
        MinusHp(damage);

        return damage;
    }
    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�����</param>
    /// <param name="per">����� (�ۼ�Ʈ)</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    private float Be_Attacked_AP(float figure, Stat perpetrator)
    {
        float damage = Halved_AP(figure, 0, 0);
        MinusHp(damage);

        return damage;
    }
    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    private float Be_Attacked_TRUE(float figure, Stat perpetrator)
    {
        MinusHp(figure);

        return figure;
    }

    //�ڿ� ȸ��, max���� �������� ����
    private IEnumerator Natural_Recovery_HP()
    {
        float t = 0;
        while(hp < maxHp)
        {
            t += Time.deltaTime;

            if(t > naturalTime)
            {
                hp += naturalHP;
                t = 0;
            }

            yield return null;
        }
    }
    private IEnumerator Natural_Recovery_MP()
    {
        float t = 0;
        while (mp < maxMp)
        {
            t += Time.deltaTime;

            if (t > naturalTime)
            {
                mp += naturalMP;
                t = 0;
            }

            yield return null;
        }
    }

    //��Ʈ������� ������ �̹� �ߵ����� ��Ʈ������� ���� �� �ߵ���
    //  �� : �����̳� ���׷� ������ ���� ���� attackDelegate ������ ����
    /// <summary>
    /// �� ��Ʈ�� 1�ʸ��� ������� ��
    /// </summary>
    /// <param name="duration">�ߵ� Ƚ��</param>
    /// <param name="figure">1ȸ�� �����</param>
    /// <param name="attack">���ݽ� �ߵ��ϴ� delegate</param>
    /// <param name="perpetrator">���� �ߴ���</param>
    public void Be_Attacked_Poison(int duration, float figure, Action<Stat, float> attack, Stat perpetrator)
    {
        if(poison)
        {
            if (dot_poison != null)
            {
                StopCoroutine(dot_poison);
            }
            dot_poison = StartCoroutine(Dot_Poison(duration, figure, attack));
        }
    }
    private IEnumerator Dot_Poison(int duration, float figure, Action<Stat, float> attack)
    {
        int du = 0;
        float t = 0;
        while (du <= duration)
        {
            yield return null;

            t += Time.deltaTime;

            if(t >= dotTime)
            {
                MinusHp(figure);
                attack(this, figure);
                du++;
                t = 0;
            }
        }
        dot_poison = null;
    }
    //ȭ�� : �����̳� ���׷� ������ ���� ���� hitDelegate������ ����
    /// <summary>
    /// ȭ�� ��Ʈ�� 1�ʸ��� ������� ��
    /// </summary>
    /// <param name="duration">�ߵ� Ƚ��</param>
    /// <param name="figure">1ȭ�� �����</param>
    /// <param name="perpetrator">���� �ߴ���</param>
    public void Be_Attacked_Burn(int duration, float figure, Stat perpetrator)
    {
        if(burn)
        {
            if (dot_burn != null)
            {
                StopCoroutine(dot_burn);
            }
            dot_burn = StartCoroutine(Dot_Burn(duration, figure, perpetrator));
        }
    }
    private IEnumerator Dot_Burn(int duration, float figure, Stat perpetrator)
    {
        int du = 0;
        float t = 0;
        while (du <= duration)
        {
            yield return null;

            t += Time.deltaTime;

            if (t >= dotTime)
            {
                Be_Attacked_TRUE(figure, perpetrator);
                du++;
                t = 0;
            }
        }
        dot_burn = null;
    }
    //���� : ap�� ���׷� ������ ���� hitDelegate������ ����
    /// <summary>
    /// ���� ��Ʈ�� 1�ʸ��� ������� ��
    /// </summary>
    /// <param name="duration">�ߵ� Ƚ��</param>
    /// <param name="figure">1ȸ�� �����</param>
    /// <param name="penetration">�� �����(+)</param>
    /// <param name="per">�� �����(%)</param>
    /// <param name="perpetrator">���� �ߴ���</param>
    public void Be_Attacked_Shock(int duration, float figure, float penetration, float per, Stat perpetrator)
    {
        if(shock)
        {
            if (dot_shock != null)
            {
                StopCoroutine(dot_shock);
            }
            dot_shock = StartCoroutine(Dot_Shock(duration, figure, penetration, per, perpetrator));
        }
    }
    private IEnumerator Dot_Shock(int duration, float figure, float penetration, float per, Stat perpetrator)
    {
        int du = 0;
        float t = 0;    
        while (du <= duration)
        {
            yield return null;

            t += Time.deltaTime;

            if(t >= dotTime)
            {
                Be_Attacked_AP(figure, perpetrator);
                du++;
                t = 0;
            }
        }
        dot_shock = null;
    }
    //���� : ad�� ���� ������ ���� attackDelegate ������ ����
    /// <summary>
    /// ���� ��Ʈ�� 1�ʸ��� ������� ��
    /// </summary>
    /// <param name="duration">�ߵ� Ƚ��</param>
    /// <param name="figure">1ȸ�� �����</param>
    /// <param name="penetration">�� �����(+)</param>
    /// <param name="per">�� �����(%)</param>
    /// <param name="attack">���ݽ� �ߵ��ϴ� delegate</param>
    /// <param name="perpetrator">���� �ߴ���</param>
    public void Be_Attacked_Bleeding(int duration, float figure, float penetration, float per, Action<Stat, float> attack, Stat perpetrator)
    {
        if(bleeding)
        {
            if (dot_bleeding != null)
            {
                StopCoroutine(dot_bleeding);
            }
            dot_bleeding = StartCoroutine(Dot_Bleeding(duration, figure, penetration, per, attack));
        }
    }
    private IEnumerator Dot_Bleeding(int duration, float figure, float penetration, float per, Action<Stat, float> attack)
    {
        int du = 0;
        float t = 0;
        float damage;
        while (du <= duration)
        {
            yield return null;

            t += Time.deltaTime;

            if(t >= dotTime)
            {
                damage = Halved_AD(figure, penetration, per);
                MinusHp(damage);
                attack(this, figure);
                du++;
                t = 0;
            }
        }
        dot_bleeding = null;
    }


    //�׽�Ʈ �غ� ���, �߰��� �ߺ������� �ؾ������� ���°��� ������� null�϶� ���� ���� �ȳ���
    //�ߺ��� �˾Ƽ� �ϳ��� ����
    //������ ȣ���Ҷ��� null�̸� ������

    //�⺻ ���� ����
    //�⺻ ���� �� ȣ�� �߰�
    //�ߺ������� ���� �ʴ°ɷ� 
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void AddNomalAttackBefore(Func<Stat, Stat, float, float> action)
    {
        if (nomalAttackBefore == null)
            nomalAttackBefore = new List<Func<Stat, Stat, float, float>>();
        //�ߺ�����
        //if (!nomalAttackBefore.Contains(action))
            nomalAttackBefore.Add(action);
    }
    //�⺻ ���� �� ȣ�� �߰�
    public void AddNomalAttackAfter(Action<Stat, Stat, float> action)
    {
        if(nomalAttackAfter == null)
            nomalAttackAfter += action;
        //�ߺ�����
        else //if (!nomalAttackAfter.GetInvocationList().Contains(action))
            nomalAttackAfter += action;
    }
    //�⺻ ���� �� ȣ�� ����
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void RemoveNomalAttackBefore(Func<Stat, Stat, float, float> action)
    {
        if (nomalAttackBefore == null)
            return;
        nomalAttackBefore.Remove(action);
    }
    //�⺻ ���� �� ȣ�� ����
    public void RemoveNomalAttackAfter(Action<Stat, Stat, float> action)
    {
        nomalAttackAfter -= action;
    }

    //Ư�� ���� ����
    //Ư�� ���� �� ȣ�� �߰�
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void AddSpecialAttackBefore(Func<Stat, Stat, float, float> action)
    {
        if (specialAttackBefore == null)
            specialAttackBefore = new List<Func<Stat, Stat, float, float>>();
        //if (!specialAttackBefore.Contains(action))
            specialAttackBefore.Add(action);
    }
    //Ư�� ���� �� ȣ�� �߰�
    public void AddSecialAttackAfter(Action<Stat, Stat, float> action)
    {
        if(specialAttackAfter == null)
            specialAttackAfter += action;
        //�ߺ�����
        else// if (!specialAttackAfter.GetInvocationList().Contains(action))
            specialAttackAfter += action;
    }
    //Ư�� ���� �� ȣ�� ����
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void RemoveSpecialAttackBefore(Func<Stat, Stat, float, float> action)
    {
        if (specialAttackBefore == null)
            return;
        specialAttackBefore.Remove(action);
    }
    //Ư�� ���� �� ȣ�� ����
    public void RemoveSecialAttackAfter(Action<Stat, Stat, float> action)
    {
        specialAttackAfter -= action;
    }

    //�⺻ ���� ����
    //�⺻ ���� �ޱ� ���� ȣ�� �߰�
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void AddHitNomalBefore(Func<Stat, Stat, float, float> action)
    {
        if (hitNomalBefore == null)
            hitNomalBefore = new List<Func<Stat, Stat, float, float>>();
        //if (!hitNomalBefore.Contains(action))
            hitNomalBefore.Add(action);
    }
    //�⺻ ���� ���� �� ȣ�� �߰�
    public void AddHitNomalAfter(Action<Stat, Stat, float> action)
    {
        if (hitNomalAfter == null)
            hitNomalAfter += action;
        //�ߺ�����
        else //if (!hitNomalAfter.GetInvocationList().Contains(action))
            hitNomalAfter += action;
    }
    //�⺻ ���� �ޱ� �� ȣ�� ����
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void RemoveHitNomalBefore(Func<Stat, Stat, float, float> action)
    {
        if (hitNomalBefore == null)
            return;
        hitNomalBefore.Remove(action);
    }
    //�⺻ ���� ���� �� ȣ�� ����
    public void RemoveHitNomalAfter(Action<Stat, Stat, float> action)
    {
        hitNomalAfter -= action;
    }

    //Ư�� ���� ����
    //Ư�� ���� �ޱ� �� ȣ�� �߰�
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void AddHitSpecialBefore(Func<Stat, Stat, float, float> action)
    {
        if(hitSpecialBefore == null)
            hitSpecialBefore = new List<Func<Stat, Stat, float, float>> ();
        //if (!hitSpecialBefore.Contains(action))
            hitSpecialBefore.Add(action);
    }
    //Ư�� ���� ���� �� ȣ�� �߰�
    public void AddHitSpecialAfter(Action<Stat, Stat, float> action)
    {
        if(hitSpecialAfter == null)
            hitSpecialAfter += action;
        //�ߺ�����
        else// if (!hitSpecialAfter.GetInvocationList().Contains(action))
            hitSpecialAfter += action;
    }
    //Ư�� ���� �ޱ� �� ȣ�� �߰�
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void RemoveHitSpecialBefore(Func<Stat, Stat, float, float> action)
    {
        if(hitSpecialBefore == null)
            return;
        hitSpecialBefore.Remove(action);
    }
    //Ư�� ���� ���� �� ȣ�� ����
    public void RemoveHitSecial(Action<Stat, Stat, float> action)
    {
        hitSpecialAfter -= action;
    }

    //�� ����
    //�� �ޱ� �� ȣ�� �߰�
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void AddHealBefore(Func<Stat, Stat, float, float> action)
    {
        if (healOtherBefore == null)
            healOtherBefore = new List<Func<Stat, Stat, float, float>>();
        //if (!healOtherBefore.Contains(action))
            healOtherBefore.Add(action);
    }
    //�� ���� �� ȣ�� �߰�
    public void AddHealAfter(Action<Stat, Stat, float> action)
    {
        if(healOtherAfter == null)
            healOtherAfter += action;
        //�ߺ�����
        else //if (!healOtherAfter.GetInvocationList().Contains(action))
            healOtherAfter += action;
    }
    //�� �ޱ� �� ȣ�� ����
    /// <summary>
    /// ������ ������ ��ġ ���ϰ� ��
    /// </summary>
    /// <param name="action">������ ������ ��ġ ���ϰ� ��</param>
    public void RemoveHealBefore(Func<Stat, Stat, float, float> action)
    {
        if(healOtherBefore == null)
            return;
        healOtherBefore.Remove(action);
    }
    //�� ���� �� ȣ�� ����
    public void RemoveHealAfter(Action<Stat, Stat, float> action)
    {
        healOtherAfter -= action;
    }


    //��������Ʈ ȣ��
    //�⺻ ���� �� ȣ��
    private float CallNomalAttackBefore(Stat perpetrator, Stat victim, float figure)
    {
        if (nomalAttackBefore == null)
            return figure;
       
        for(int i = 0; i < nomalAttackBefore.Count; i++)
        {
            figure = nomalAttackBefore[i](perpetrator, victim, figure);
        }
        return figure;
    }
    //�⺻ ���� �� ȣ��
    private void CallNomalAttackAfter(Stat perpetrator, Stat victim, float figure)
    {
        if (nomalAttackAfter != null) nomalAttackAfter(perpetrator, victim, figure);
    }

    //Ư�� ���� �� ȣ��
    private float CallSpecialAttackBefore(Stat perpetrator, Stat victim, float figure)
    {
        if (specialAttackBefore == null)
            return figure;
        for (int i = 0; i < specialAttackBefore.Count; i++)
        {
            figure = specialAttackBefore[i](perpetrator, victim, figure);
        }
        return figure;
    }
    //Ư�� ���� �� ȣ��
    private void CallSpecialAttackAfter(Stat perpetrator, Stat victim, float figure)
    {
        if (specialAttackAfter != null) specialAttackAfter(perpetrator, victim, figure);
    }

    //�⺻ ���� �ޱ� �� ȣ��
    private float CallHitNomalBefore(Stat perpetrator, Stat victim, float figure)
    {
        if (hitNomalBefore == null)
            return figure;
        for (int i = 0; i < hitNomalBefore.Count; i++)
        {
            figure = hitNomalBefore[i](perpetrator, victim, figure);
        }
        return figure;
    }
    //�⺻ ���� ���� �� ȣ��
    private void CallHitNomalAfter(Stat perpetrator, Stat victim, float figure)
    {
        if (hitNomalAfter != null) hitNomalAfter(perpetrator, victim, figure);
    }

    //Ư�� ���� �ޱ� �� ȣ��
    private float CallHitSpecialBefore(Stat perpetrator, Stat victim, float figure)
    {
        if (hitSpecialBefore == null)
            return figure;
        for (int i = 0; i < hitSpecialBefore.Count; i++)
        {
            figure = hitSpecialBefore[i](perpetrator, victim, figure);
        }
        return figure;
    }
    //Ư�� ���� ���� �� ȣ��
    private void CallHitSpecialAfter(Stat perpetrator, Stat victim, float figure)
    {
        if (hitSpecialAfter != null) specialAttackAfter(perpetrator, victim, figure);
    }
    //�� �ޱ� �� ȣ��
    private float CallHealBefore(Stat perpetrator, Stat victim, float figure)
    {
        if (healOtherBefore == null)
            return figure;
        for (int i = 0; i < healOtherBefore.Count; i++)
        {
            figure = healOtherBefore[i](perpetrator, victim, figure);
        }
        return figure;
    }
    //�� ���� �� ȣ��
    private void CallHealAfter(Stat perpetrator, Stat victim, float figure)
    {
        if (healOtherAfter != null) healOtherAfter(perpetrator, victim, figure);
    }






    //�ߺ�ó��
    public bool AlreadyNomalBefore(Func<Stat, Stat, float, float> action)
    {
        if(nomalAttackBefore == null)
            return false;
        if (nomalAttackBefore.Contains(action))
            return true;
        return false;
    }
    public bool AlreadyNomalAfter(Action<Stat, Stat, float> action)
    {
        if (nomalAttackAfter.GetInvocationList().Contains(action))
            return true;
        return false;
    }
    public bool AlreadySpecialBefore(Func<Stat, Stat, float, float> action)
    {
        if (specialAttackBefore == null)
            return false;
        if (specialAttackBefore.Contains(action))
            return true;
        return false;
    }
    public bool AlreadySpecialAfter(Action<Stat, Stat, float> action)
    {
        if (specialAttackAfter.GetInvocationList().Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHitNomalBefore(Func<Stat, Stat, float, float> action)
    {
        if (hitNomalBefore == null)
            return false;
        if (hitNomalBefore.Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHitNomalAfter(Action<Stat, Stat, float> action)
    {
        if (hitNomalAfter.GetInvocationList().Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHitSpecialBefore(Func<Stat, Stat, float, float> action)
    {
        if (hitSpecialBefore == null)
            return false;
        if (hitSpecialBefore.Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHitSpecialAfter(Action<Stat, Stat, float> action)
    {
        if (hitSpecialAfter.GetInvocationList().Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHealBefore(Func<Stat, Stat, float, float> action)
    {
        if (healOtherBefore == null)
            return false;
        if (healOtherBefore.Contains(action))
            return true;
        return false;
    }
    public bool AlreadyHealAfter(Action<Stat, Stat, float> action)
    {
        if (healOtherAfter.GetInvocationList().Contains(action))
            return true;
        return false;
    }
}


public class Barrier
{
    public float figure;
    public float timer;
    public Barrier(float figure, float timer)
    {
        this.figure = figure;
        this.timer = timer;
    }
}