public static class ItemEffect
{
    /// <summary>
    /// 70�� �������ظ� �߰��� �ִ� ������ ȿ��
    /// </summary>
    /// <param name="owner"> �������� ������ �ִ� ���� </param>
    public static void ChargingAttack(Unit owner, Unit victim)
    {
        owner.stat.Attacked_AD(70, 0, 0, victim);
    }

    /// <summary>
    /// �� ���ݸ��� 10�� �������ظ� �ִ� ������ ȿ��
    /// </summary>
    /// <param name="owner">�������� ������ �ִ� ����</param>
    public static void AdditionalAttacks(Unit owner, Unit victim)
    {
        owner.stat.Attacked_AD(10, 0, 0, victim);
    }
}
