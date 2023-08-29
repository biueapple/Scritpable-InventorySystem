using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Stat : MonoBehaviour
{
    private Unit user;

    //ü��
    [SerializeField]
    private float hp;
    public float HP { get { return  hp; } }
    private float maxHp;
    public float MAXHP { get { return maxHp; } set { maxHp += value; hp += value; } }
    //ü�� �ڿ� ȸ����
    private float naturalHP;
    [SerializeField]
    private float mp;
    public float MP { get { return mp; } }
    private float maxMp;
    public float MAXMP { get { return maxMp; } set { maxMp += value; mp += value; } }
    private float naturalMP;
    [SerializeField]
    private float defence;
    public float DEFENCE { get { return defence; } set { defence += value; } }
    [SerializeField]
    private float resistance;
    public float RESISTANCE { get { return resistance; } set { resistance += value; } }
    [SerializeField]
    private float ad;
    public float AD { get { return ad; } set { ad += value; } }
    [SerializeField]
    private float ap;
    public float AP { get { return ap; } set { ap += value; } }
    private float speed;
    public float SPEED { get { return speed; } }

    //

    //Natural_Recovery_HP
    private Coroutine nrhp = null;
    //Natural_Recovery_MP
    private Coroutine nrmp = null;
    //Dot_Poison
    private Coroutine dot_poison = null;
    private Coroutine dot_burn = null;
    private Coroutine dot_shock = null;
    private Coroutine dot_bleeding = null;

    /// <summary>
    /// ������ �ߵ��ϴ� unit = ���� �����ߴ��� float = �����, 0��°�� �׻� ui�� ����� ǥ��
    /// </summary>
    public Action<Unit, string, Color, bool> hitTextCallback = null;
    protected Action<Unit, float> hitDelegate = null;
    // ���� ������ �ߵ��ϴ� �Լ��� ���� �����ߴ��� float = �����
    protected Action<Unit, float> attackDelegate = null;
    //������ ȣ���ϴ� �Լ� (ȭ�鿡 ������� ǥ���ϱ� ����)
    //protected Action<Unit, string> hitView = null;

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

    public void Init(float _hp, float _mp,float _nh,float _nm, float _ad,float _ap, float _de, float _re, float _speed)
    {
        user = GetComponent<Unit>();
        hp = _hp;
        mp = _mp;
        naturalHP = _nh;
        naturalMP = _nm;
        defence = _de;
        resistance = _re;
        ad = _ad;
        ap = _ap;
        speed = _speed;
    }

    public void MinusHp(float figure)
    {
        hp -= figure;
        if(nrhp == null )
        {
            nrhp = StartCoroutine(Natural_Recovery_HP());
        }
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

    public void RecoveryHP(Unit perpetrator, float figure)
    {
        if (hp + figure > maxHp)
            figure = maxHp - hp;
        hp += figure;
        if (hitTextCallback != null)
            hitTextCallback(perpetrator, figure.ToString(), green, false);
    }

    public void RecoveryMP(Unit perpetrator, float figure)
    {
        if (mp + figure > maxMp)
            figure = maxMp - mp;
        mp += figure;
        if (hitTextCallback != null)
            hitTextCallback(perpetrator, figure.ToString(), blue, false);
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

    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�����</param>
    /// <param name="per">����� (�ۼ�Ʈ)</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    public float Be_Attacked_AD(float figure, float penetration, float per, Unit perpetrator)
    {
        float damage = Halved_AD(figure, penetration, per); 
        MinusHp(damage);
        HitInvocation(perpetrator, damage);
        return damage;
    }
    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�����</param>
    /// <param name="per">����� (�ۼ�Ʈ)</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    public float Be_Attacked_AP(float figure, float penetration, float per, Unit perpetrator)
    {
        float damage = Halved_AP(figure, penetration, per);
        MinusHp(damage);
        HitInvocation(perpetrator, damage);

        return damage;
    }
    /// <summary>
    /// ���������� ���� �޾��� ��
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="perpetrator">���� �����Ѱ���</param>
    public float Be_Attacked_TRUE(float figure, Unit perpetrator)
    {
        MinusHp(figure);
        HitInvocation(perpetrator, figure);

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

                if (hitTextCallback != null)
                    hitTextCallback(user, naturalHP.ToString(), green, false);
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

                if (hitTextCallback != null)
                    hitTextCallback(user, naturalMP.ToString(), blue, false);
            }

            yield return null;
        }
    }

    /// <summary>
    /// ���� ������ �����Ҷ�, attackDelegate�ߵ�
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�� ����� + </param>
    /// <param name="per">�� ����� % </param>
    /// <param name="perpetrator">���ݹ޴� ����</param>
    public void Attacked_AD(float figure, float penetration, float per, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_AD(figure, penetration, per, user);
        AttackInvocation(perpetrator, figure);
    }

    /// <summary>
    /// ���� ������ �����Ҷ�
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="penetration">�� ����� + </param>
    /// <param name="per">�� ����� % </param>
    /// <param name="perpetrator">���ݹ޴� ����</param>
    public void Attacked_AP(float figure, float penetration, float per, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_AP(figure, penetration, per, user);
    }

    /// <summary>
    /// ���� ������ �����Ҷ�
    /// </summary>
    /// <param name="figure">�����</param>
    /// <param name="perpetrator">���ݹ޴� ����</param>
    public void Attacked_TRUE(float figure, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_TRUE(figure, user);
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
    public void Be_Attacked_Poison(int duration, float figure, Action<Unit, float> attack, Unit perpetrator)
    {
        if(poison)
        {
            if (dot_poison != null)
            {
                StopCoroutine(dot_poison);
            }
            if (hitTextCallback != null)
                hitTextCallback(perpetrator, "Poison", red, true);
            dot_poison = StartCoroutine(Dot_Poison(duration, figure, attack));
        }
    }
    private IEnumerator Dot_Poison(int duration, float figure, Action<Unit, float> attack)
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
                attack(user, figure);
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
    public void Be_Attacked_Burn(int duration, float figure, Unit perpetrator)
    {
        if(burn)
        {
            if (dot_burn != null)
            {
                StopCoroutine(dot_burn);
            }
            if (hitTextCallback != null)
                hitTextCallback(perpetrator, "Burn", red, true);
            dot_burn = StartCoroutine(Dot_Burn(duration, figure, perpetrator));
        }
    }
    private IEnumerator Dot_Burn(int duration, float figure, Unit perpetrator)
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
    public void Be_Attacked_Shock(int duration, float figure, float penetration, float per, Unit perpetrator)
    {
        if(shock)
        {
            if (dot_shock != null)
            {
                StopCoroutine(dot_shock);
            }
            if (hitTextCallback != null)
                hitTextCallback(perpetrator, "Shock", red, true);
            dot_shock = StartCoroutine(Dot_Shock(duration, figure, penetration, per, perpetrator));
        }
    }
    private IEnumerator Dot_Shock(int duration, float figure, float penetration, float per, Unit perpetrator)
    {
        int du = 0;
        float t = 0;    
        while (du <= duration)
        {
            yield return null;

            t += Time.deltaTime;

            if(t >= dotTime)
            {
                Be_Attacked_AP(figure, penetration, per, perpetrator);
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
    public void Be_Attacked_Bleeding(int duration, float figure, float penetration, float per, Action<Unit, float> attack, Unit perpetrator)
    {
        if(bleeding)
        {
            if (dot_bleeding != null)
            {
                StopCoroutine(dot_bleeding);
            }
            if (hitTextCallback != null)
                hitTextCallback(perpetrator, "Bleeding", red, true);
            dot_bleeding = StartCoroutine(Dot_Bleeding(duration, figure, penetration, per, attack));
        }
    }
    private IEnumerator Dot_Bleeding(int duration, float figure, float penetration, float per, Action<Unit, float> attack)
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
                attack(user, figure);
                du++;
                t = 0;
            }
        }
        dot_bleeding = null;
    }

    //
    //
    //
    public void AddHit(Action<Unit, float> action)
    {
        hitDelegate += action;
    }
    public void RemoveHit(Action<Unit, float> action)
    {
        hitDelegate -= action;
    }
    /// <summary>
    /// hitDelegate ���� �ߵ�
    /// </summary>
    /// <param name="perpetrator">���� �� �����ߴ���</param>
    /// <param name="figure">�����</param>
    public void HitInvocation(Unit perpetrator, float figure)
    {
        hitDelegate(perpetrator, figure);
    }

    /// <summary>
    /// ���� ���������� attackDelegate ���� �ߵ�
    /// </summary>
    /// <param name="perpetrator">���� �����ߴ���</param>
    /// <param name="figure">�����</param>
    public void AttackInvocation(Unit perpetrator, float figure)
    {
        attackDelegate(perpetrator, figure);
    }
}
