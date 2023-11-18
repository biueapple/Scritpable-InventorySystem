
//ui���Ե��� ���� ��Ʈ���Ϳ��� �巡�� ������� �־��ָ� �װ��� ����ϴ� Ŭ����
//edit ��Ȳ���� ���� ������ ���ڴٴ°�
public class StaticInterface : ContainerInterface
{
    //StaticInterface�� ������ �ڵ����� ��������� �ʰ� ���� �����ͻ󿡼� ����� �ν����ͷ� �־���� ��
    public Slot_UI[] slots;

    public override void CreateSlotsUI()
    {
        for(int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            if(i >= slots.Length)
            {
                slotsInterface.Add(containerObject.storage.slots[i], null);
            }
            else
            {
                slots[i].Slot = containerObject.storage.slots[i];
                slots[i].ContainerInterface = this;
                slots[i].name = "slot_" + i;

                slotsInterface.Add(containerObject.storage.slots[i], slots[i]);
            }
        }
    }
}
