using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class Intersect
{
    /// <summary>
    /// 활성화된 오브젝트중에 사각형 안에 위치한 모든 오브젝트 리턴 (ObjectPooling에 포함된 오브젝트만)
    /// </summary>
    /// <param name="posi">위치</param>
    /// <param name="size">크기</param>
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
    /// 점이 사각형 안에 있는지 확인
    /// </summary>
    /// <param name="posi">사각형 위치</param>
    /// <param name="size">사각형 크기</param>
    /// <param name="point">점의 위치</param>
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
        //거리를 직접 구하는 방법
        ////x 변위량
        //float deltaX = posi.x - point.x;
        //float deltaY = posi.y - point.y;
        //float deltaZ = posi.z - point.z;
        ////거리
        //float length = Mathf.Sqrt(deltaX *  deltaX + deltaY * deltaY + deltaZ * deltaZ);
        
        //거리를 쉽게 구하는 방법 (내부적으론 비슷함)
        float length = Vector3.Distance(posi, point);
        if(length > r)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 두 원의 반지름의 합보다 멀리있으면 충돌 아님
    /// </summary>
    /// <param name="posi">원 1의 위치</param>
    /// <param name="r">원 1의 크기</param>
    /// <param name="circle">원 2의 위치</param>
    /// <param name="circleR">원 2의 크기</param>
    /// <returns></returns>
    static bool IsCircleInCircle(Vector3 posi, float r, Vector3 circle, float circleR)
    {
        float length = Vector3.Distance(posi, circle);
        //두 원의 반지름의 합보다 멀리있으면 충돌 아님
        if(length > (r + circleR))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 두 사각형이 수평충돌과 수직충돌을 만족하면 충돌 삼차원이니까 하나더 필요하네
    /// </summary>
    /// <param name="posi">사각형 1의 위치</param>
    /// <param name="size">사각형 1의 크기</param>
    /// <param name="rectposi">사각형 2의 위치</param>
    /// <param name="rectsize">사각형 2의 크기</param>
    /// <returns></returns>
    static bool IsRectInRect(Vector3 posi, Vector3 size, Vector3 rectposi, Vector3 rectsize )
    {
        //이쪽면이 충돌 했는지 (전부 true여야 충돌한거임)
        bool x = false;
        bool y = false;
        bool z = false;

        //상대 사각형의 왼쪽면이 나의 사각형 안에 있음
        //나의 왼쪽면이 상대의 왼쪽면보다 작으면서 나의 오른쪽면은 더 크다면
        //상대의 왼쪽면은 나의 왼쪽면과 오른쪽면 사이에 있으니 충돌상태 (x축만)
        //상대의 오른쪽면도 똑같이 판단 가능
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
    /// 사각형 안에 원이 있는가
    /// 사각형을 원의 반지름만큼 확장 후 원의 중심과 충돌하는지 판정
    /// 충돌을 안하더라도 사각형의 각 꼭지점에 충돌하는지 한번 더 확인해야 함
    /// </summary>
    /// <returns></returns>
    static bool IsRectInCircle(Vector3 rect, Vector3 size, Vector3 circle, float r)
    {
        //사각형의 크기 확장
        size.x += r;
        size.y += r;
        size.z += r;

        //확장된 크기의 사각형에 원의 중심이 충돌하는지 확인
        if(IsPointInRect(rect, size, circle))
        {
            return true;
        }
        //사각형의 각 꼭지점이 원안에 있는지 확인해야함 정육면체니 8번 비교해야하네
        else
        {
            //사각형의 크기 확장 한걸 되돌리고 비교해야함
            size.x -= r;
            size.y -= r;
            size.z -= r;
            //8개를 언제 다 비교하냐
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
