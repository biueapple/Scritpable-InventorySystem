

namespace WarriorSkills
{
    public class Thorn : Skill
    {
        public override float coefficient { get { return 10 + level * 5 + player.stat.AP * 0.6f; } }
        public override string detail { get { return "10 + level * 5 + AP * 0.6f"; } }
        public override string expaln { get { return description.Replace("?", coefficient.ToString()); } }

        new protected void Start()
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
            player.stat.AddHitNomalAfter(Impact);
        }
        protected override void Uninstall()
        {
            player.stat.RemoveHitNomalAfter(Impact);
        }

        //스킬의 내용
        private void Impact(Stat per, Stat victim, float figure)
        {
            per.Be_Attacked(player.stat, coefficient, ATTACKTYPE.NONE, DAMAGETYPE.AD);
        }
    }
}

