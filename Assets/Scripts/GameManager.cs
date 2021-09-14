using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ִ� ��ü���� ��� ���ġ���ش�.
// �ʿ�Ӽ� : ��ü��, �ݰ�, ��ü���� �Ÿ�
public class GameManager : MonoBehaviour
{
    // �ʿ�Ӽ� : ��ü��, �ݰ�, ��ü���� �Ÿ�
    public Transform[] objects;
    public float range = 5;
    public float distanceBetweenObj = 2;

    private void Update()
    {
        //if(Input.GetButtonDown("Fire1"))
        //{
        //    Reset();
        //}
    }

    // ���� �ִ� ��ü���� ��� ���ġ���ش�.
    public void Reset()
    {
        // ���� �ִ� ��ü���� ��� ���ġ���ش�.
        // 1. �������� �ݺ�������
        for (int i = 0; i < objects.Length; i++)
        {
            // 2. ��ġ�� ��ġ�� �ʿ�
            Vector3 pos = Random.insideUnitSphere * range;
            pos.y = 0;
            // 3. ���ġ ���ش�.
            objects[i].localPosition = pos;
            
            for(int j=0;j<i;j++)
            {
                // ���� ����� �ǳʶڴ�.
                //if(i == j)
                //{
                //    continue;
                //}
                // ���� �ٸ� ��ü����� �Ÿ��� ���������� ���� ���� ������
                float distance = Vector3.Distance(pos, objects[j].localPosition);
                if (distance < distanceBetweenObj)
                {
                    // �ٽ� ���� ��ġ ������
                    i--;
                    break;
                }
            }
        }
    }
}
