using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInterface : MonoBehaviour
{
    public Unit player;
    //��ų��
    public Canvas canvas;
    public Image linePre;
    private List<Image> lineList = new List<Image>();
    public Skill[] skills;
    public Text description;
    private Coroutine follow = null;
    //��ų�� tree�� �������� ���ϴ� ������ ������ ��ų�� ���� �� �ֱ� ������
    //��ų�� ���ܰ��� ��ų�� ������ �ʿ��� ��찡 �ִ�
    //��ų�� ���������� ��ü ��ų�� ������ Ȯ���ؾ���

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].Init(player);
            skills[i].triggerUpdate += Updating;
            skills[i].skillInterface = this;
        }
    }

    // Update is called once per frame
    void Update() 
    {
        
    }

    public void Updating()
    {
        for(int i = 0; i < skills.Length; i++)
        {
            skills[i].Updating();
        }
    }

    public void Description(Skill s)
    {
        //��ų�� �̸��� ����
        description.text = s.S_Name + '\n';
        description.text += s.expaln;

        //������ ���콺�� ����ٴϵ��� (���߿� �ٲ�� �ҵ���)
        if (follow == null)
            follow = StartCoroutine(FollowMouse());
        //���� ������Ʈ�� ���̵���
        description.gameObject.SetActive(true);

        //�� ���� ������ ���� ��ũ��Ʈ�⿡ ������ ���� ��ų�� ���⼭ ��
        if (s.trigger.triggers.Length == 0)
            return;

        //���ǿ� ���� ����
        description.text += "\n\n";
        description.text += "�ʿ� ���� : "; 
        switch(s.trigger.type)
        {
            case TRIGGER_TYPE.ALL:
                description.text += "�Ʒ� ��ų���� ���� " + s.trigger.needLevel + " �̻� �ʿ�";
                break;
            case TRIGGER_TYPE.ONE:
                description.text += "�Ʒ� ��ų���� �ϳ��� " + s.trigger.needLevel + " �̻� �ʿ�";
                break;
            case TRIGGER_TYPE.SUM:
                description.text += "�Ʒ� ��ų���� ������ ���� " + s.trigger.needLevel + " �̻� �ʿ�";
                break;
        }

        //������ �̸��� ��ġ�� �˷��� ���� (������ƮǮ�� ����)
        description.text += "\n";
        for (int i = 0; i < s.trigger.triggers.Length; i++)
        {
            //������ �̸�
            description.text += s.trigger.triggers[i].S_Name + '\n';
            //���� ������ �θ�ä
            lineList.Add(ObjectPooling.CreateObject(linePre.gameObject, canvas.transform,
                //������ ��ġ
                s.transform.position + (s.trigger.triggers[i].transform.position - s.transform.position) / 2,
                //������ ����
                Quaternion.Euler(0, 0, AngleCalculate.Angle_Calculation_Z(s.transform, s.trigger.triggers[i].transform) + 90)).GetComponent<Image>());
            //���� ����
            lineList[lineList.Count - 1].GetComponent<RectTransform>().sizeDelta = new Vector2(10, Vector2.Distance(s.transform.position, s.trigger.triggers[i].transform.position)) ;
            //���̶�Ű�� �ε��� ���� (������ interface�� ��������Ʈ�� �������µ� ���� �����ؼ� �����Ƽ� ���� �Ⱥ��̸� interface�� ��������Ʈ�� �������ؼ��� �� ����)
            lineList[lineList.Count - 1].transform.SetSiblingIndex(0);
        }
    }

    private IEnumerator FollowMouse()
    {
        while (true)
        {
            description.transform.position = Input.mousePosition;
            yield return null;
        }
    }

    public void EndDescription()
    {
        if(follow != null)
            StopCoroutine(follow);
        follow = null;
        description.gameObject.SetActive(false);
        for(int i = 0; i < lineList.Count; i++)
        {
            ObjectPooling.DestroyObject(lineList[i].gameObject);
        }
        lineList.Clear();
    }
}
