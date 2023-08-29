using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Stat : MonoBehaviour
{
    private Unit user;

    //체력
    [SerializeField]
    private float hp;
    public float HP { get { return  hp; } }
    private float maxHp;
    public float MAXHP { get { return maxHp; } set { maxHp += value; hp += value; } }
    //체력 자연 회복량
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
    /// 맞을때 발동하는 unit = 누가 공격했는지 float = 대미지, 0번째는 항상 ui에 대미지 표시
    /// </summary>
    public Action<Unit, string, Color, bool> hitTextCallback = null;
    protected Action<Unit, float> hitDelegate = null;
    // 누굴 때릴때 발동하는 함수들 누굴 공겨했는지 float = 대미지
    protected Action<Unit, float> attackDelegate = null;
    //맞을때 호출하는 함수 (화면에 대미지를 표시하기 위해)
    //protected Action<Unit, string> hitView = null;

    //color값
    private Color red = new Color(0.5660378f, 0, 0);
    private Color green = new Color(0, 0.5647059f, 0);
    private Color blue = new Color(0, 0.2810156f, 1);

    //상태이상을 받는지 안받는지
    public bool poison = true;
    public bool burn = true;
    public bool shock = true;
    public bool bleeding = true;


    //도트뎀이나 자연 회복이 몇초에 한번 일어날지
    public float dotTime = 2;
    public float naturalTime = 2;

    public string SetText()
    {
        string str = "체력 = " + Mathf.Floor(hp * 100f) / 100f + " / " + Mathf.Floor(maxHp * 100f) / 100f + "\n" +
                    "마나 = " + Mathf.Floor(mp * 100f) / 100f + " / " + Mathf.Floor(maxMp * 100f) / 100f + "\n" +
                    "체력재생 = " + naturalHP + "\n" +
                    "마나재생 = " + naturalMP + "\n" +
                    "방어력 = " + defence + "\n" +
                    "저항력 = " + resistance + "\n" +
                    "공격력 = " + ad + "\n" +
                    "주문력 = " + ap + "\n" +
                    "속도 = " + speed;
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
    /// ad 대미지를 방어력에 맞게 조정해줌
    /// </summary>
    /// <param name="figure">대미지</param>
    /// <param name="penetration">방어구 관통력 (고정) </param>
    /// <param name="per">방어구 관통력 (퍼센트)</param>
    /// <returns>방어력과 관통력을 거쳐간 대미지</returns>
    public float Halved_AD(float figure, float penetration, float per)
    {
        float defence = this.defence - penetration;
        defence -= defence * per * 0.01f;
        return (100 / (100 + defence) * figure);
    }
    /// <summary>
    /// ap 대미지를 방어력에 맞게 조정해줌
    /// </summary>
    /// <param name="figure">대미지</param>
    /// <param name="penetration">저항력 관통력 (고정) </param>
    /// <param name="per">저항력 관통력 (퍼센트)</param>
    /// <returns>저항력과 관통력을 거쳐간 대미지</returns>
    public float Halved_AP(float figure, float penetration, float per)
    {
        float resistance = this.resistance - penetration;
        resistance -= resistance * per * 0.01f;
        return (100 / (100 + resistance) * figure);
    }

    /// <summary>
    /// 누군가에게 공격 받았을 때
    /// </summary>
    /// <param name="figure">대매지</param>
    /// <param name="penetration">관통력</param>
    /// <param name="per">관통력 (퍼센트)</param>
    /// <param name="perpetrator">누가 공격한건지</param>
    public float Be_Attacked_AD(float figure, float penetration, float per, Unit perpetrator)
    {
        float damage = Halved_AD(figure, penetration, per); 
        MinusHp(damage);
        HitInvocation(perpetrator, damage);
        return damage;
    }
    /// <summary>
    /// 누군가에게 공격 받았을 때
    /// </summary>
    /// <param name="figure">대매지</param>
    /// <param name="penetration">관통력</param>
    /// <param name="per">관통력 (퍼센트)</param>
    /// <param name="perpetrator">누가 공격한건지</param>
    public float Be_Attacked_AP(float figure, float penetration, float per, Unit perpetrator)
    {
        float damage = Halved_AP(figure, penetration, per);
        MinusHp(damage);
        HitInvocation(perpetrator, damage);

        return damage;
    }
    /// <summary>
    /// 누군가에게 공격 받았을 때
    /// </summary>
    /// <param name="figure">대매지</param>
    /// <param name="perpetrator">누가 공격한건지</param>
    public float Be_Attacked_TRUE(float figure, Unit perpetrator)
    {
        MinusHp(figure);
        HitInvocation(perpetrator, figure);

        return figure;
    }

    //자연 회복, max보다 많아지면 끝남
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
    /// 내가 누군갈 공격할때, attackDelegate발동
    /// </summary>
    /// <param name="figure">대미지</param>
    /// <param name="penetration">방어구 관통력 + </param>
    /// <param name="per">방어구 관통력 % </param>
    /// <param name="perpetrator">공격받는 유닛</param>
    public void Attacked_AD(float figure, float penetration, float per, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_AD(figure, penetration, per, user);
        AttackInvocation(perpetrator, figure);
    }

    /// <summary>
    /// 내가 누군갈 공격할때
    /// </summary>
    /// <param name="figure">대미지</param>
    /// <param name="penetration">방어구 관통력 + </param>
    /// <param name="per">방어구 관통력 % </param>
    /// <param name="perpetrator">공격받는 유닛</param>
    public void Attacked_AP(float figure, float penetration, float per, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_AP(figure, penetration, per, user);
    }

    /// <summary>
    /// 내가 누군갈 공격할때
    /// </summary>
    /// <param name="figure">대미지</param>
    /// <param name="perpetrator">공격받는 유닛</param>
    public void Attacked_TRUE(float figure, Unit perpetrator)
    {
        perpetrator.stat.Be_Attacked_TRUE(figure, user);
    }

    //도트대미지는 받을때 이미 발동중인 도트대미지를 없앤 후 발동함
    //  독 : 방어력이나 저항력 판정을 받지 않음 attackDelegate 판정을 받음
    /// <summary>
    /// 독 도트뎀 1초마다 대미지가 들어감
    /// </summary>
    /// <param name="duration">발동 횟수</param>
    /// <param name="figure">1회당 대미지</param>
    /// <param name="attack">공격시 발동하는 delegate</param>
    /// <param name="perpetrator">누가 했는지</param>
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
    //화상 : 방어력이나 저항력 판정을 받지 않음 hitDelegate판정을 받음
    /// <summary>
    /// 화상 도트뎀 1초마다 대미지가 들어감
    /// </summary>
    /// <param name="duration">발동 횟수</param>
    /// <param name="figure">1화당 대미지</param>
    /// <param name="perpetrator">누가 했는지</param>
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
    //감전 : ap딜 저항력 판정을 받음 hitDelegate판정을 받음
    /// <summary>
    /// 감전 도트뎀 1초마다 대미지가 들어감
    /// </summary>
    /// <param name="duration">발동 횟수</param>
    /// <param name="figure">1회당 대미지</param>
    /// <param name="penetration">방어구 관통력(+)</param>
    /// <param name="per">방어구 관통력(%)</param>
    /// <param name="perpetrator">누가 했는지</param>
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
    //출혈 : ad딜 방어력 판정을 받음 attackDelegate 판정을 받음
    /// <summary>
    /// 출혈 도트뎀 1초마다 대미지가 들어감
    /// </summary>
    /// <param name="duration">발동 횟수</param>
    /// <param name="figure">1회당 대미지</param>
    /// <param name="penetration">방어구 관통력(+)</param>
    /// <param name="per">방어구 관통력(%)</param>
    /// <param name="attack">공격시 발동하는 delegate</param>
    /// <param name="perpetrator">누가 했는지</param>
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
    /// hitDelegate 전부 발동
    /// </summary>
    /// <param name="perpetrator">누가 날 공격했는지</param>
    /// <param name="figure">대미지</param>
    public void HitInvocation(Unit perpetrator, float figure)
    {
        hitDelegate(perpetrator, figure);
    }

    /// <summary>
    /// 누굴 공격했을때 attackDelegate 전부 발동
    /// </summary>
    /// <param name="perpetrator">누굴 공격했는지</param>
    /// <param name="figure">대미지</param>
    public void AttackInvocation(Unit perpetrator, float figure)
    {
        attackDelegate(perpetrator, figure);
    }
}
