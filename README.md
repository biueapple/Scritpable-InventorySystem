# Scritpable-InventorySystem

인벤토리 시스템 완성

//Inventory System
//



scriptable database 만들어서 아이템 저장할 곳 만들고 (edit) (아이템을 만들면 자동으로 만들어진 database에 저장되도록 만들었음 모든 아이템에게 고유의 id를 부여하기 위해)

(itemObject에 [System.Serializable]를 달지 않으면 만든 itemObject의 이름을 바꾼다던가 할때 database에게 부여받은 id가 초기화 되는 문제가 발생)

scriptable container 만들고 보유한 아이템 저장할 곳 만들고 (play)

scriptable item 만들어서 아이템의 원본데이터를 만들고

gound item에다가 scriptable item넣고 아이템을 획득해야 할때 create item하면

scriptable item이 가지고 있는 데이터를 하나 만들어서 리턴해줌

scriptable container에 save load도 있음 C:\Users\사용자명\AppData\LocalLow\DefaultCompany\My InventorySystem + savePath에 저장됨

// ItemEffect
//

아이템에 아이템 효과(ItemEffect)를 넣어봄

아이템을 저장할때 자식클래스까지는 저장이 안됨

ex) 

class A {}

case B : A {}

A a = new B();

scriptable에 a 저장하면 A가 저장되고 B는 저장이 안됨

A에 virtual로 만들고 B에서 override해도 A의 virtual이 호출됨

그래서 ItemEffect에 이름을 넣어서 이름으로 구분하도록 만듬

플레이중에 만든 ItemEffect는 제대로 B로 만들어지고 A안에 있는 변수도

제대로 만들어지고 정해지니까

edit상태로 돌아왔을때 저장된 A안에도 B로 만들때 정해준 변수들이 그대로 저장됨

다음에 play할때 그가

// 

아직 만드는 중이라 [SerializeField]가 많이 붙어있음 확인용으로
