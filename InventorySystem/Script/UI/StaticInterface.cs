
//ui슬롯들을 직접 인트펙터에서 드래그 드랍으로 넣어주면 그것을 사용하는 클래스
//edit 상황에서 만든 슬롯을 쓰겠다는것
public class StaticInterface : ContainerInterface
{
    //StaticInterface은 슬롯을 자동으로 만들어주지 않고 직접 에디터상에서 만들고 인스펙터로 넣어줘야 함
    public Slot_UI[] slots;

    public override void CreateSlotsUI()
    {
        for(int i = 0; i < containerObject.storage.slots.Length; i++)
        {
            //ui의 숫자보다 저장공간이 더 많다면 남는 칸은 ui상으로는 존재하지 않음
            if(i >= slots.Length)
            {
                slotsInterface.Add(containerObject.storage.slots[i], null);
            }
            //ui와 저장공간 둘다 있을때
            else
            {
                slots[i].Slot = containerObject.storage.slots[i];
                slots[i].ContainerInterface = this;
                slots[i].name = "slot_" + i;

                slotsInterface.Add(containerObject.storage.slots[i], slots[i]);
            }
            //ui가 더 많은 경우엔 에러가 나옴
        }
    }
}
