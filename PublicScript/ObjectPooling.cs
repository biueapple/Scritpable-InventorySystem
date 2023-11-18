
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectPooling
{
    static List<GameObject> activeObject = new List<GameObject>();
    static public List<GameObject> Active { get { return activeObject; } }
    static List<GameObject> deactiveObject = new List<GameObject>();
    static GameObject parent;

    //CreateObject �� �Լ��� ������ȭ�� obj�� �ް� �װ����� ���� ������Ʈ�� �ִ���
    //��Ȱ��ȭ ����Ʈ���� ã���� �װ��� Ȱ��ȭ �� ��Ȱ��ȭ ����Ʈ���� ������ Ȱ��ȭ ����Ʈ�� �߰� �� ����
    //���ٸ� �ϳ� ����� Ȱ��ȭ ����Ʈ�� �߰�

    public static GameObject CreateObject(GameObject obj)
    {
        //�ϴ� �θ�ü�� ���� �׾ȿ� �ְڴٴ°ǵ� ��� ������
        if (parent == null)
            parent = new GameObject("������ƮǮ��");
        //���� ������Ʈ�� �ִ��� Ȯ�� GetInstanceID() �� ���� ���������� ����� �������� Ȯ�� ���ึ�� ���� �޶����⿡ �÷����߿��� �񱳰���
        GameObject g = deactiveObject.FirstOrDefault(obj => obj.GetInstanceID() == obj.GetInstanceID());
        if (g == null)
        {
            //�� ������Ʈ �����
            GameObject go = GameObject.Instantiate(obj, parent.transform);
            activeObject.Add(go);
            return go;
        }
        else
        {
            //�̹� �ִ� ������Ʈ ����
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
