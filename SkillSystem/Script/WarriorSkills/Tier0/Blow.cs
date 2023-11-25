using System;
using System.Collections;
using UnityEngine;

namespace WarriorSkills
{
    [System.Serializable]
    public class Blow : Skill
    {
        public override float coefficient { get { return 20 + level * 10 + player.stat.AD * 0.6f; } }
        public override string detail { get { return "20 + level * 10 + AD * 0.6"; } }
        public override string expaln { get { return description.Replace("?", coefficient.ToString()); } }
        public override float CoolTime { get { return cooltime - level; } }
        private float skilltimer = 3;

        public override void Init(Unit unit)
        {
            base.Init(unit);
            type = SKILL_TYPE.ACTIVE;
        }

        public override void Updating()
        {
            base.Updating();
        }

        //��ų ����ñ�ٴ� ��
        protected override void Install()
        {
            //SkillList �� Impact�� �ִ´ٴ� �� ȣ���ϸ鼭 Ű�� ���� ��쿡 �ߵ��Ѵٴ� ��
            //�ߺ�üũ�� �ϴ� �κ��̿��µ� ���ֱ�� ��
            //if (!player.IsActionAlreadyRegistered(InputKey))
            //{
                player.SkillList += InputKey;
            //}
        }
        //��ų ���ٴ� ��
        protected override void Uninstall()
        {
            //�ߺ�üũ�� �ϴ� �κ��̿��µ� ���ֱ�� ��
            //if (player.IsActionAlreadyRegistered(InputKey))
            //{
                player.SkillList -= InputKey;
                //��ų�� �������µ�(level�� 0�̵Ǽ� ������) �̹� ȿ���� �ߵ����� �� ����
                //�׷���� �ߵ����� ȿ���� ���ֱ�
                if (player.stat.AlreadyNomalBefore(Impact))
                {
                    player.stat.RemoveNomalAttackBefore(Impact);
                }
            //}
        }
        //��ų Ű�� ������ �ߵ��ϴ� ȿ����� ��
        private void InputKey()
        {
            //Ű�� ��������
            if (Input.GetKeyDown(_keyCode))
            {
                if (cooltimer > 0)
                    return;
                //�⺻������ ��ȭ�Ǵ� ��ų�ε� �̹� ��ȭ���϶� �� ��ȭ�ϸ� �ȵű⿡ �ߺ�üũ�� �ؾ���
                if(!player.stat.AlreadyNomalBefore(Impact))
                {
                    player.stat.AddNomalAttackBefore(Impact);
                }
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = StartCoroutine(skillTimer(skilltimer, RemoveSkill));
                StartCoroutine(CooltimeCoroutine(CoolTime));
            }
        }
        //��ų�� ����
        private float Impact(Stat per, Stat victim, float figure)
        {
            figure += coefficient;
            //��ų ȿ���� �ߵ������ϱ� ���ֱ�
            player.stat.RemoveNomalAttackBefore(Impact);
            //��ų�� �ѹ� ��������� ���ӽð��� �������� ������ Ÿ�̸�
            StopCoroutine(coroutine);
            coroutine = null;
            return figure;
        }

        //Ÿ�̸� �������� ����� ȿ�� ���ֱ�
        private void RemoveSkill()
        {
            player.stat.RemoveNomalAttackBefore(Impact);
        }

        
    }
}
