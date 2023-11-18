

using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public static class AngleCalculate
{
    //������ ��ġ���
    public static Vector3 SphericalCoordinate(float angle, float distance, bool collision)
    {
        //������ ���� ��ȯ �� cos���� ���
        float cosY = Mathf.Cos(angle * Mathf.Deg2Rad);
        //... sin���� ���
        float sinY = Mathf.Sin(angle * Mathf.Deg2Rad);

        //...
        float cosX = Mathf.Cos(angle * Mathf.Deg2Rad);
        //...
        float sinX = Mathf.Sin(angle * Mathf.Deg2Rad);

        //���� cos sin��� ��ġ�� ��� �� �Ÿ��� ���� ��ġ�� ����
        return new Vector3(sinX, sinY, cosX * cosY) * distance;
    }

    //��ġ�� ���� ���
    public static float Angle_Calculation_X(Transform criteria, Transform target)
    {
        //x�� ���� (������ ������ x ���� -x���� �ٶ󺸸鼭 ���������� ���� +)
        //�ڱ� �����ʿ� ������ 0�� ���� ������ 90�� �Ʒ��� ������ -90�� (������ ������ �����ʿ� �ִٴ°��� �տ� �ִٴ� ��) 
        float x = target.position.z - criteria.position.z;
        float y = target.position.y - criteria.position.y;
        float angleInR = Mathf.Atan2(y, x);
        return angleInR * (180 / Mathf.PI);
    }
    public static float Angle_Calculation_Y(Transform criteria, Transform target)
    {
        //Y�� ���� (y ���� -y�� �ٶ󺸸鼭 ���������� ���� +)
        float x = target.position.x - criteria.position.x;
        float y = target.position.z - criteria.transform.position.z;
        float angleInR = Mathf.Atan2(y, x);
        return angleInR * (180 / Mathf.PI);
    }
    public static float Angle_Calculation_Z(Transform criteria, Transform target)
    {
        //z�� ���� (-z ���� z�� �ٶ󺸸鼭 ���������� ���� +)
        float x = target.position.x - criteria.transform.position.x;
        float y = target.position.y - criteria.transform.position.y;
        float angleInR = Mathf.Atan2(y, x);
        return angleInR * (180 / Mathf.PI);
        //�� z�ุ -z���� z�� ���� �ϴ°��� ������ �𸣰ڳ�
    }

    //270���� -90���� ���������� ���ϱ� ����⿡
    //������ �׻� -180 ~ 180�� �̳��� ����� ���� ���ϱ⿡ ���ϰ� ����
    public static float NormalizeAngle(float angle)
    {
        angle %= 360; // ������ 360�� ���� ���� ����

        if (angle < -180)
        {
            angle += 360; // -180�� �̸��� ���, 180���� ���� 180�� ���� ���� �̵�
        }
        else if (angle > 180)
        {
            angle -= 360; // 180�� �ʰ��� ���, 180���� �� 180�� ���� ���� �̵�
        }

        return angle;
    }

    //�Ի纤�Ϳ� �븻���͸� ������ �ݻ纤�͸� ��������
    //���������� vector2��°� vecotr3�� �׳� ������ xy���� ��ƾ�� z���� �ȵ��ͼ� �̻��� ���� ���� �� ����
    public static Vector2 ReflexAngle(Vector2 dir, Vector2 normal)
    {
        return Vector2.Reflect(dir, normal);
    }
}