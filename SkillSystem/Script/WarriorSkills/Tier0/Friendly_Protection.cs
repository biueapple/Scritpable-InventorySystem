using UnityEngine;

namespace WarriorSkills
{
    public class Friendly_Protection : Skill
    {
        //���ӽð�
        private float duration = 7;
        public Vector3 size;

        public override float coefficient { get { return 10 + level * 10 + player.stat.MAXHP * 0.1f; } }
        public override string detail { get { return "10 + level * 10 + MAXHP * 0.1f"; } }
        public override string expaln 
        {
            get
            {
                string s = description.Replace("!", duration.ToString());
                return s.Replace("?", coefficient.ToString());
            } 
        }

        new void Start()
        {
            base.Start();
            type = SKILL_TYPE.PASSIVE;
        }

        public override void Updating()
        {
            base.Updating();
        }

        protected override void Install()
        {
            player.SkillList += InputKey;
        }

        protected override void Uninstall()
        {
            player.SkillList -= InputKey;
        }

        public void InputKey()
        {
            //Ű�� ��������
            if (Input.GetKeyDown(_keyCode))
            {
                //������ ������ �����ͼ� ���������� ���� �����
                //������ ������ �簢�������� �����ε� ���� Intersect.IsIsPointInCircleObject(��ġ, ������) �� ����ϸ� �� 
                //GameObject[] obj = Intersect.IsIsPointInCircleObject(player.transform.position, 5);
                GameObject[] obj = Intersect.IsPointInRectObject(player.transform.position, size);
                for(int i = 0; i < obj.Length; i++)
                {
                    if (obj[i].GetComponent<Unit>() != null)
                    {
                        //���� ��ȣ���� �򵵷� �����ؾ���
                        obj[i].GetComponent<Unit>().stat.Barrier = new Barrier(coefficient, duration);
                    }
                }
            }
        }
    }
}

