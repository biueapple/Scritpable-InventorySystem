using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SKILL_TYPE
{
    PASSIVE,
    ACTIVE,
}

public enum TRIGGER_TYPE
{
    ALL,    //�Ѵ� level��ŭ �־�� �Ѵ�
    ONE,    //�ϳ��� level��ŭ ������ �ȴ�
    SUM     //���ļ� level��ŭ ������ �ȴ�
}


//�����Ҷ� üũ�ؾ� �Ұ�
//��� �� �ִ� ��ų�ΰ� (��� �� �ִٸ� deactivation_Image = false)
//��ų������ 0�ΰ� (0�̸� minus false max�� plus false)
//Ű�� ������ �ߵ��ϴ� ��Ƽ�� ��ų�� ��� player.SkillList�� �߰��ؾ��ϰ�
//��� �ߵ����� �нú� ��ų�� ��� player.stat �� �ٷ� �־������
//�ߺ�üũ�� �߰��Ҷ��� ���� �ؾ��ϴµ� 
//��Ƽ�� ��ų�� �ߺ�üũ�� player.IsActionAlreadyRegistered���� false�� ������ �߰��ص� �ȴٴ°�
//�нú� ��ų�� �ߺ�üũ�� player.stat.Already �� ���� �Լ��� �̿��ϸ� ��
//�ٵ� ��ų�� �ߺ�üũ�� �ؾ��ϳ� �ͳ� 
//��ų�� �ߺ�üũ�� ���ϴ°ɷ�
[System.Serializable]
public abstract class Skill : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public Unit player;
    public SkillInterface skillInterface;
    [SerializeField]
    protected string s_name;
    public string S_Name { get { return s_name; } }
    //����
    [TextArea(5, 10)]
    public string description;
    /// <summary>
    /// ��ų �� (�����������)
    /// </summary>
    public virtual float coefficient { get; }
    /// <summary>
    /// ��ų ���� ��� ���������� string���� ����
    /// </summary>
    public virtual string detail { get; }
    /// <summary>
    /// ��� ������ ����
    /// </summary>
    public virtual string expaln { get; }

    [SerializeField]
    protected Sprite sprite;
    public Sprite S_Sprite { get { return sprite;} }
    [SerializeField]
    protected int level;
    public int Level { get { return level; } }
    [SerializeField]
    protected int maxLevel;
    [SerializeField]
    protected SKILL_TYPE type;
    public SKILL_TYPE TYPE { get { return type; } }

    [SerializeField]
    protected Text levelText;
    [SerializeField]
    protected Button minusButton;
    [SerializeField]
    protected Button plusButton;
    [SerializeField]
    protected Image deactivation_Image;
    [SerializeField]
    protected KeyCode _keyCode;
    public KeyCode Code { get { return _keyCode; } set { _keyCode = value; } }

    public SkillTrigger trigger;

    //Ÿ�̸Ӹ� �����ϴ� �ڷ�ƾ
    protected Coroutine coroutine = null;

    //��ų�� ������ ��ų�� �����ϴ� �Լ�
    //install�� skill�� level�� 0���� �ö󰥶� �ѹ� ȣ����
    protected abstract void Install();
    //��ų�� ����ϸ� ��ų�� ���ٴ� �Լ�
    //install�� skill�� level�� 0�ɶ� �ѹ� ȣ����
    protected abstract void Uninstall();

    public Action triggerUpdate = null;

    protected void Start()
    {
        GetComponent<Image>().sprite = sprite;
        Updating();

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(LevelDown);
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(LevelUp);
    }



    public void LevelUp()
    {
        if (player.SkillPoint > 0 && level < maxLevel)
        {
            if(level == 0)
            {
                Install();
            }

            player.SkillPoint = -1;
            level++;

            if (triggerUpdate != null)
                triggerUpdate();
        }
    }

    public void LevelDown()
    {
        if(level > 0)
        {
            player.SkillPoint = 1;
            level--;

            if(triggerUpdate != null)
                triggerUpdate();

            if (level == 0)
            {
                Uninstall();
            }
        }
    }

    public virtual void Updating()
    {
        //�� ��ų�� ���� �� �ִ� ����
        if(trigger.TriggerCheck())
        {
            deactivation_Image.enabled = false;
            levelText.text = level == 0 ? "" : level.ToString();
            if (level <= 0)
            {
                minusButton.gameObject.SetActive(false);
                plusButton.gameObject.SetActive(true);
            }
            else if (level >= maxLevel)
            {
                minusButton.gameObject.SetActive(true);
                plusButton.gameObject.SetActive(false);
            }
            else
            {
                minusButton.gameObject.SetActive(true);
                plusButton.gameObject.SetActive(true);
            }
        }
        //�� ��ų�� ���� �� ���� ����
        else
        {
            deactivation_Image.enabled = true;
            levelText.text = "";
            minusButton.gameObject.SetActive(false);
            plusButton.gameObject.SetActive(false);
            player.SkillPoint = level;
            level = 0;
        }
    }

    //Ÿ�̸�
    protected IEnumerator skillTimer(float f, Action action)
    {
        float timer = f;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        if (action != null)
        {
            action();
        }
        coroutine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillInterface.Description(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillInterface.EndDescription();
    }
}

//�� ��ų�� ������ �����̿��߸� �Ѵ�
[System.Serializable]
public class SkillTrigger
{
    //���� ��ų��
    public Skill[] triggers;
    //������ Ÿ��
    public TRIGGER_TYPE type;
    //���� ����
    public int needLevel;

    public bool TriggerCheck()
    {
        switch(type)
        {
            case TRIGGER_TYPE.ALL:
                return TriggerAll();
            case TRIGGER_TYPE.ONE:
                return TriggerOne();
            case TRIGGER_TYPE.SUM:
                return TriggerSum();
            default:
                return true;
        }
    }

    public bool TriggerAll()
    {
        for(int i = 0; i < triggers.Length; i++)
        {
            if(triggers[i].Level < needLevel)
                return false;
        }
        return true;
    }

    public bool TriggerOne()
    {
        for (int i = 0; i < triggers.Length; i++)
        {
            if (triggers[i].Level >= needLevel)
                return true;
        }
        return false;
    }

    public bool TriggerSum()
    {
        int l = 0;
        for (int i = 0; i < triggers.Length; i++)
        {
            l += triggers[i].Level;
            if (l >= needLevel)
                return true;
        }
        return false;
    }
}