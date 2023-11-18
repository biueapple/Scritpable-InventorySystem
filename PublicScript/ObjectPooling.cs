
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPooling
{
    static List<GameObject> activeObject = new List<GameObject>();
    static public List<GameObject> Active { get { return activeObject; } }
    static List<GameObject> deactiveObject = new List<GameObject>();
    static GameObject parent;

    //CreateObject 이 함수는 프리팹화된 obj를 받고 그것으로 만든 오브젝트가 있는지
    //비활성화 리스트에서 찾으면 그것을 활성화 후 비활성화 리스트에서 리무브 활성화 리스트에 추가 후 리턴
    //없다면 하나 만들고 활성화 리스트에 추가

    public static GameObject CreateObject(GameObject obj)
    {
        //일단 부모객체를 만들어서 그안에 넣겠다는건데 없어도 괜찮음
        if (parent == null)
            parent = new GameObject("오브젝트풀링");
        //같은 오브젝트가 있는지 확인 GetInstanceID() 는 같은 프리팹으로 만들어 진것인지 확인 실행마다 값이 달라지기에 플레이중에만 비교가능
        GameObject g = deactiveObject.FirstOrDefault(obj => obj.GetInstanceID() == obj.GetInstanceID());
        if (g == null)
        {
            //새 오브젝트 만들기
            GameObject go = GameObject.Instantiate(obj, parent.transform);
            activeObject.Add(go);
            return go;
        }
        else
        {
            //이미 있는 오브젝트 리턴
            g.transform.SetParent(parent.transform);
            g.SetActive(true);
            activeObject.Add(g);
            deactiveObject.Remove(g);
            return g;
        }
    }

    public static GameObject CreateObject(GameObject obj, Transform parent)
    {
        GameObject g = deactiveObject.FirstOrDefault(obj => obj.GetInstanceID() == obj.GetInstanceID());
        if (g == null)
        {
            GameObject go = GameObject.Instantiate(obj, parent);
            activeObject.Add(go);
            return go;
        }
        else
        {
            g.transform.parent.SetParent(parent);
            g.SetActive(true);
            activeObject.Add(g);
            deactiveObject.Remove(g);
            return g;
        }
    }

    public static GameObject CreateObject(GameObject obj, Transform parent, Vector3 position, Quaternion quaternion)
    {
        GameObject g = deactiveObject.FirstOrDefault(obj => obj.GetInstanceID() == obj.GetInstanceID());
        if (g == null)
        {
            GameObject go = GameObject.Instantiate(obj, position, quaternion, parent);
            activeObject.Add(go);
            return go;
        }
        else
        {
            g.transform.parent.SetParent(parent);
            g.transform.position = position;
            g.transform.rotation = quaternion;
            g.SetActive(true) ;
            activeObject.Add(g);
            deactiveObject.Remove(g);
            return g;
        }
    }

    public static void DestroyObject(GameObject obj)
    {
        if (activeObject.Contains(obj))
        {
            activeObject.Remove(obj);
        }

        deactiveObject.Add(obj);
        obj.gameObject.SetActive(false);
    }
}
