using System.Collections.Generic;
using UnityEngine;

//�߻�Ŭ������ ����� ������ �ν����Ϳ� �Ⱥ��̴� ������ ����
/// <summary>
/// �������� �ߺ��Ǵ� �������� ���Ƽ� ����
/// </summary>
[System.Serializable]
public class ItemEffect
{
    //�̸� ������ �̸��� ������ ���ϱ⿡ ���� �̸��� ���� ȿ���� ������ �ȉ�
    [SerializeField]
    protected string effectName;
    public string Name { get { return effectName; } }

    //ȿ���� ���� ����
    [SerializeField]
    protected string description;
    public string Description { get { return description; } }

    //��ġ
    protected float figure;
    public float Figure { get { return figure; } }

    public virtual void Installation(Stat stat)
    {

    }
    public virtual void Uninstallation(Stat stat)
    {

    }
}

/// <summary>
/// �⺻ ���� ���� �ߵ��Ǵ� ȿ���� ���� ȿ���� �̰��� ��ӹ޾Ƽ� ����
/// </summary>
[System.Serializable]
public abstract class NomalBeforeEffect : ItemEffect
{
    protected abstract float Impact(Stat perpetrator, Stat victim, float figure);
    public override void Installation(Stat stat)
    {
        stat.AddNomalAttackBefore(Impact);
    }
    public override void Uninstallation(Stat stat)
    {
        stat.RemoveNomalAttackBefore(Impact);
    }
}

/// <summary>
/// �⺻ ���� �Ŀ� �ߵ��Ǵ� ȿ���� ���� ȿ���� �̰��� ��ӹ޾Ƽ� ����
/// </summary>
[System.Serializable]
public abstract class NomalAfterEffect : ItemEffect
{
    protected abstract void Impact(Stat perpetrator, Stat victim, float figure);
    public override void Installation(Stat stat)
    {
        stat.AddNomalAttackAfter(Impact);
    }
    public override void Uninstallation(Stat stat)
    {
        stat.RemoveNomalAttackAfter(Impact);
    }
}


//Ŭ������ ��ӹ޾Ƽ� �����ڿ� Impact�� override���ָ� ��

[System.Serializable]
/// <summary>
/// �� �⺻ ���ݸ��� 10�� ������� �߰��ϴ� ȿ��
/// </summary>
public class AdditionalDamage : NomalBeforeEffect
{
    public AdditionalDamage()
    {
        effectName = "�߰������";
        figure = 10;
        description = "�⺻���ݽ� ������� " + figure + "��ŭ �ø���";
    }

    protected override float Impact(Stat perpetrator, Stat victim, float figure)
    {
        return figure + this.figure;
    }
}

[System.Serializable]
/// <summary>
/// �� �⺻ ���ݸ��� 10�� �������ظ� �ִ� ȿ��
/// </summary>
public class AdditionalAttacks : NomalAfterEffect
{
    public AdditionalAttacks()
    {
        effectName = "�߰�����";
        figure = 10;
        description = "�⺻���ݽ�" + figure + "�� �߰� �������ظ� ������";
    }
    protected override void Impact(Stat perpetrator, Stat victim, float figure)
    {
        victim.Be_Attacked(perpetrator, this.figure, ATTACKTYPE.NONE, DAMAGETYPE.AD);
    }
}


//������ ȿ������ �������� ������ �����ؾ���
//������ ȿ���� ������� �з����� ������ �з����� ��������
//���� ȿ���� �ְ� ���� ȿ���� �ְ� ȿ������ ����� �����ٵ�
//ȿ�� ����� Ȯ���� �ϰ� ������ �з��ϴ°�
/// <summary>
/// ��� ������ ȿ���� ������ �������ִ� Ŭ����
/// </summary>
public static class ItemImpactManager
{
    static List<ItemEffect> effects;
    public static void Init()
    {
        if(effects == null)
        {
            effects = new List<ItemEffect>
            {
                new AdditionalAttacks(),
                new AdditionalDamage(),
            };
        }
    }

    public static ItemEffect GetEffect()
    {
        Init();
        return effects[Random.Range(0, effects.Count)];
    }

    public static ItemEffect GetEffect(ItemEffect e)
    {
        Init();
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].Name.Equals(e.Name))
                return effects[i];
        }
        return null;
    }
}

//��¥ ���� ������ �־���
//ScriptableObject�� ����� Ŭ������ ItemEffect i = new AdditionalAttacks();
//�̷� ������ �ص� ���� �÷����� ���� AdditionalAttacks�� �ƴ϶� ItemEffect�μ� ������
//AdditionalAttacks�� NomalAfterEffect���� override �� �Լ��� �ƴ϶� ItemEffect �ȿ� �ִ� virtual �Լ��� ȣ����
//������ ã�� �͵� ���� �ɷȰ� �ذ��ϴ� ����� ���� ��̴�.

//��ų�� ScriptableObject�� ���� �������� ���� �� �ִ� ������ ȿ������ ����ó�� �־ ������� (valueó��)
//�׳� �������� �ؾ�����
//ScriptableObject�� ����� itemó�� id�� ���� string���� ���ϴ� ���� ����������
//�ٵ� ������ �ϳ��ϳ� �� �� �ִ� ȿ������ �־����ڴ� �ʹ� �����Ű�����
//�ٵ� �ϳ��ϳ� �� ȿ���� ��Ʈ�� �� �� �ִ°� �������� ���� �ѵ� 