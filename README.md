인벤토리 시스템 완성

//Inventory System //

scriptable database 만들어서 아이템 저장할 곳 만들고 (edit) (아이템을 만들면 자동으로 만들어진 database에 저장되도록 만들었음 모든 아이템에게 고유의 id를 부여하기 위해)

(itemObject에 [System.Serializable]를 달지 않으면 만든 itemObject의 이름을 바꾼다던가 할때 database에게 부여받은 id가 초기화 되는 문제가 발생)

scriptable container 만들고 보유한 아이템 저장할 곳 만들고 (play)

scriptable item 만들어서 아이템의 원본데이터를 만들고

gound item에다가 scriptable item넣고 아이템을 획득해야 할때 create item하면

scriptable item이 가지고 있는 데이터를 하나 만들어서 리턴해줌

scriptable container에 save load도 있음 C:\Users\사용자명\AppData\LocalLow\DefaultCompany\My InventorySystem + savePath에 저장됨

// ItemEffect //

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

다음에 play할때 그 이름으로 원하는 ItemEffect를 리턴받아 사용하도록 만듬

뭔가 더 좋은 방법이 있을거 같은데 모르겠음 문자열로 비교하는건 불안함

// Rune //

아이템에 추가로 장비에 장착 가능한 rune이라는걸 만들어봄

rune도 결국 아이템이니까

rune이라는 item을 상속받은 자식 클래스를 만들어서 구현하려고 했는데

scriptable은 자식클래스를 저장하진 못해서 다른 방식으로 만들어봄

item type에 rune을 추가하고 모든 아이템에 rune[] 을 만들고

장비 아이템과 rune아이템에만 공간을 할당함

장비 아이템은 3~5개의 빈 룬공간을 갖고

룬 아이템은 하나의 룬공간에 룬이 가져야할 스탯과 문자를 가지고 있음

룬에도 장비 아이템처럼 효과도 가질 수 있는지 아직 정하진 못했음

이런식으로 만드는게 좋은건지 모르겠는데 다른방법을 모르겠음

// Google Spreadsheets //

google spreadsheets를 이용한 level,exp,stat에 대한 정보들을 입력해놓고

불러오는 방식을 만들어 놓음

아직 단순하게 방법만 만들어 놓고 구현은 하지 않음

//Skill //

warrior의 스킬 4개를 만들었는데 만들다보니 느껴지는게

cc기나 다른 스킬들을 만들려고 생각해보니 unit에 많이 추가될듯함

publicScript에 있는 스크립트는 static이나 공용으로 쓰일 스크립트

//UI //

ui쪽에서 mouseDown 과 mouseUp 을 통해서 슬롯끼리의 상호작용을 했는데

mouseClick으로 변경하고 interface간의 공유할 corsorSlot을 하나 만들어서 공용으로 쓰는걸로 바꿈

원래는 corsorSlot이라는건 없었고 downUI와 upUI를 서로 swap하기만 하는식이였음

//Crafting //

마인크래프트에서 보던 조합법을 만들어봄

원래는 3x3크기의 행렬을 하나하나 비교하는걸로 만들었는데

조합법이 10000개면 비교를 90000만번하는게 좀 오래걸릴거 같아서

행렬을 잘라서 저장하는 방법으로 (-1은 빈칸이라는 뜻)

table 1

1 1 -1

1 1 -1

-1 -1 -1

를

table 2

1 1

1 1

로 변환해서

다시 hashtable처럼

모든 크기의 테이블을 위한 리스트를 만들어서 저장하는 식으로 만들어봄

행렬을 자르는 방법을 생각해 내는것이 가장 힘들었고 오래걸림

처음 생각한것은 모든 칸을 일렬로 만들어 보는것이었는데 table1 을 일렬로 하면 1 1 -1 1 1 -1 -1 -1 -1 이 되는 느낌

음수가 들어가기에 하나의 long타입으로 저장이 불가능하고 string으로 저장해야 하고 비교도 string으로 하는게 마음에 들지 않음

추가로 앞뒤에 -1이 아닌 숫자를 만날때 까지 자르는 방법을 사용하면 더 짧아지지만

1 1 -1 1 1 와 11 -1 11 두개를 구분 못하는 문제가 생김

결국 지금의 방식으로 변경 만드는데 거의 1주일 걸림

모든 테이블 종류 추가

조합법을 적용 시키는데 resultBox에 Action을 추가하는 방식으로 구현하려고 했는데 무한 반복하는 문제가 생김

Action으로 box의 내용이 바뀌면 다시 resultBox의 Action을 호출하는 방식으로

따라서 아이템을 넣고 뺄때마다 Action을 추가하고 빼는 과정이 추가됨 

//

아직 만드는 중이라 [SerializeField]가 많이 붙어있음 확인용으로
