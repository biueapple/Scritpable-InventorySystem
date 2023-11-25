
namespace WarriorSkills
{
    public class Armor : Skill
    {
        public override float coefficient { get { return (1 - (0.01f * (5 + level * 3))); } }
        public override string detail { get { return "(1 - (0.01f * (5 + level * 3)))"; } }
        public override string expaln { get { return description.Replace("?", (5 + level * 3).ToString()); } }
        public override float CoolTime { get { return cooltime - level; } }

        public override void Init(Unit unit)
        {
            base.Init(unit);
            type = SKILL_TYPE.PASSIVE;
        }

        public override void Updating()
        {
            base.Updating();
        }

        protected override void Install()
        {
            player.stat.AddHitNomalBefore(Impact);
        }

        protected override void Uninstall()
        {
            player.stat.RemoveHitNomalBefore(Impact);
        }

        private float Impact(Stat per, Stat vic, float f)
        {
            return f * coefficient;
        }
    }
}
