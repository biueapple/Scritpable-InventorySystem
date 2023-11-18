using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class Intersect
{
    /// <summary>
    /// Ȱ��ȭ�� ������Ʈ�߿� �簢�� �ȿ� ��ġ�� ��� ������Ʈ ���� (ObjectPooling�� ���Ե� ������Ʈ��)
    /// </summary>
    /// <param name="posi">��ġ</param>
    /// <param name="size">ũ��</param>
    /// <returns></returns>
    public static GameObject[] IsPointInRectObject(Vector3 posi, Vector3 size)
    {
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < ObjectPooling.Active.Count; i++)
        {
            if (IsPointInRect(posi, size, ObjectPooling.Active[i].transform.position))
                list.Add(ObjectPooling.Active[i]);
        }
        return list.ToArray();
    }

    public static GameObject[] IsIsPointInCircleObject(Vector3 posi, float r)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < ObjectPooling.Active.Count; i++)
        {
            if (IsPointInCircle(posi, r, ObjectPooling.Active[i].transform.position))
                list.Add(ObjectPooling.Active[i]);
        }
        return list.ToArray();
    }

    /// <summary>
    /// ���� �簢�� �ȿ� �ִ��� Ȯ��
    /// </summary>
    /// <param name="posi">�簢�� ��ġ</param>
    /// <param name="size">�簢�� ũ��</param>
    /// <param name="point">���� ��ġ</param>
    /// <returns></returns>
    static bool IsPointInRect(Vector3 posi, Vector3 size,  Vector3 point)
    {
        if ((posi.x - size.x <= point.x && posi.x + size.x >= point.x) &&
           (posi.y - size.y <= point.y && posi.y + size.y >= point.y) &&
           (posi.z - size.z <= point.z && posi.z + size.z >= point.z))
        {
            return true;
        }
        return false;
    }

    static bool IsPointInCircle(Vector3 posi, float r, Vector3 point)
    {
        //�Ÿ��� ���� ���ϴ� ���
        ////x ������
        //float deltaX = posi.x - point.x;
        //float deltaY = posi.y - point.y;
        //float deltaZ = posi.z - point.z;
        ////�Ÿ�
        //float length = Mathf.Sqrt(deltaX *  deltaX + deltaY * deltaY + deltaZ * deltaZ);
        
        //�Ÿ��� ���� ���ϴ� ��� (���������� �����)
        float length = Vector3.Distance(posi, point);
        if(length > r)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// �� ���� �������� �պ��� �ָ������� �浹 �ƴ�
    /// </summary>
    /// <param name="posi">�� 1�� ��ġ</param>
    /// <param name="r">�� 1�� ũ��</param>
    /// <param name="circle">�� 2�� ��ġ</param>
    /// <param name="circleR">�� 2�� ũ��</param>
    /// <returns></returns>
    static bool IsCircleInCircle(Vector3 posi, float r, Vector3 circle, float circleR)
    {
        float length = Vector3.Distance(posi, circle);
        //�� ���� �������� �պ��� �ָ������� �浹 �ƴ�
        if(length > (r + circleR))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// �� �簢���� �����浹�� �����浹�� �����ϸ� �浹 �������̴ϱ� �ϳ��� �ʿ��ϳ�
    /// </summary>
    /// <param name="posi">�簢�� 1�� ��ġ</param>
    /// <param name="size">�簢�� 1�� ũ��</param>
    /// <param name="rectposi">�簢�� 2�� ��ġ</param>
    /// <param name="rectsize">�簢�� 2�� ũ��</param>
    /// <returns></returns>
    static bool IsRectInRect(Vector3 posi, Vector3 size, Vector3 rectposi, Vector3 rectsize )
    {
        //���ʸ��� �浹 �ߴ��� (���� true���� �浹�Ѱ���)
        bool x = false;
        bool y = false;
        bool z = false;

        //��� �簢���� ���ʸ��� ���� �簢�� �ȿ� ����
        //���� ���ʸ��� ����� ���ʸ麸�� �����鼭 ���� �����ʸ��� �� ũ�ٸ�
        //����� ���ʸ��� ���� ���ʸ�� �����ʸ� ���̿� ������ �浹���� (x�ุ)
        //����� �����ʸ鵵 �Ȱ��� �Ǵ� ����
        if((posi.x - size.x < rectposi.x - rectsize.x && posi.x + size.x > rectposi.x - rectsize.x) ||
            (posi.x - size.x < rectposi.x + rectsize.x && posi.x + size.x > rectposi.x + rectsize.x))
        {
            x = true;
        }

        if ((posi.y - size.y < rectposi.y - rectsize.y && posi.y + size.y > rectposi.y - rectsize.y) ||
            (posi.y - size.y < rectposi.y + rectsize.y && posi.y + size.y > rectposi.y + rectsize.y))
        {
            y = true;
        }

        if ((posi.z - size.z < rectposi.z - rectsize.z && posi.z + size.z > rectposi.z - rectsize.z) ||
            (posi.z - size.z < rectposi.z + rectsize.z && posi.z + size.z > rectposi.z + rectsize.z))
        {
            z = true;
        }

        if(x && y && z)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �簢�� �ȿ� ���� �ִ°�
    /// �簢���� ���� ��������ŭ Ȯ�� �� ���� �߽ɰ� �浹�ϴ��� ����
    /// �浹�� ���ϴ��� �簢���� �� �������� �浹�ϴ��� �ѹ� �� Ȯ���ؾ� ��
    /// </summary>
    /// <returns></returns>
    static bool IsRectInCircle(Vector3 rect, Vector3 size, Vector3 circle, float r)
    {
        //�簢���� ũ�� Ȯ��
        size.x += r;
        size.y += r;
        size.z += r;

        //Ȯ��� ũ���� �簢���� ���� �߽��� �浹�ϴ��� Ȯ��
        if(IsPointInRect(rect, size, circle))
        {
            return true;
        }
        //�簢���� �� �������� ���ȿ� �ִ��� Ȯ���ؾ��� ������ü�� 8�� ���ؾ��ϳ�
        else
        {
            //�簢���� ũ�� Ȯ�� �Ѱ� �ǵ����� ���ؾ���
            size.x -= r;
            size.y -= r;
            size.z -= r;
            //8���� ���� �� ���ϳ�
            //Vector3 leftBackDown = new Vector3(rect.x - size.x, rect.y - size.y, rect.z - size.z);
            //Vector3 rightBackDown = new Vector3(rect.x + size.x, rect.y - size.y, rect.z - size.z);
            //Vector3 leftFrontDown = new Vector3(rect.x - size.x, rect.y - size.y, rect.z + size.z);
            //Vector3 rightFrontDown = new Vector3(rect.x + size.x, rect.y - size.y, rect.z + size.z);
            //Vector3 leftBackUp = new Vector3(rect.x - size.x, rect.y + size.y, rect.z - size.z);
            //Vector3 rightBackUp = new Vector3(rect.x + size.x, rect.y + size.y, rect.z - size.z);
            //Vector3 leftFrontUp = new Vector3(rect.x - size.x, rect.y + size.y, rect.z + size.z);
            //Vector3 rightFrontUp = new Vector3(rect.x + size.x, rect.y + size.y, rect.z + size.z);
            Vector3[] vectors = new Vector3[8]
            {
                new Vector3(rect.x - size.x, rect.y - size.y, rect.z - size.z),
                new Vector3(rect.x + size.x, rect.y - size.y, rect.z - size.z),
                new Vector3(rect.x - size.x, rect.y - size.y, rect.z + size.z),
                new Vector3(rect.x + size.x, rect.y - size.y, rect.z + size.z),
                new Vector3(rect.x - size.x, rect.y + size.y, rect.z - size.z),
                new Vector3(rect.x + size.x, rect.y + size.y, rect.z - size.z),
                new Vector3(rect.x - size.x, rect.y + size.y, rect.z + size.z),
                new Vector3(rect.x + size.x, rect.y + size.y, rect.z + size.z)
            };
            for(int i = 0; i < 8; i++)
            {
                if (IsPointInCircle(circle, r, vectors[i]))
                    return true;
            }
        }
        return false;
    }
}
