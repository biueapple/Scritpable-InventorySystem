public static class ItemEffect
{
    /// <summary>
    /// 70의 물리피해를 추가로 주는 아이템 효과
    /// </summary>
    /// <param name="owner"> 아이템을 가지고 있는 유닛 </param>
    public static void ChargingAttack(Unit owner, Unit victim)
    {
        owner.stat.Attacked_AD(70, 0, 0, victim);
    }

    /// <summary>
    /// 매 공격마다 10의 물리피해를 주는 아이템 효과
    /// </summary>
    /// <param name="owner">아이템을 가지고 있는 유닛</param>
    public static void AdditionalAttacks(Unit owner, Unit victim)
    {
        owner.stat.Attacked_AD(10, 0, 0, victim);
    }
}
