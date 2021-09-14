using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 씬에 있는 물체들을 섞어서 재배치해준다.
// 필요속성 : 물체들, 반경, 물체간의 거리
public class GameManager : MonoBehaviour
{
    // 필요속성 : 물체들, 반경, 물체간의 거리
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

    // 씬에 있는 물체들을 섞어서 재배치해준다.
    public void Reset()
    {
        // 씬에 있는 물체들을 섞어서 재배치해준다.
        // 1. 여러개를 반복적으로
        for (int i = 0; i < objects.Length; i++)
        {
            // 2. 배치할 위치가 필요
            Vector3 pos = Random.insideUnitSphere * range;
            pos.y = 0;
            // 3. 재배치 해준다.
            objects[i].localPosition = pos;
            
            for(int j=0;j<i;j++)
            {
                // 만약 나라면 건너뛴다.
                //if(i == j)
                //{
                //    continue;
                //}
                // 만약 다른 물체들과의 거리가 일정범위를 유지 하지 않으면
                float distance = Vector3.Distance(pos, objects[j].localPosition);
                if (distance < distanceBetweenObj)
                {
                    // 다시 랜덤 위치 정하자
                    i--;
                    break;
                }
            }
        }
    }
}
