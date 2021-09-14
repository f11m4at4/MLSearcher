using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 사용자의 입력에 따라 회전하고 싶다.
// 필요속성 : 회전속도
// 2. 사용자의 입력에 따라 (앞으로)이동하고 싶다.
// 필요속성 : 이동속도
public class PlayerAgent : MonoBehaviour
{
    // 필요속성 : 회전속도
    public float rotSpeed = 100;
    // 필요속성 : 이동속도
    public float moveSpeed = 5;
    Rigidbody rb;

    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gm.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        // 사용자의 입력에 따라 회전하고 싶다.
        // 1. 사용자 입력에따라
        float value = 1;
        if(Input.GetKey(KeyCode.A))
        {
            value = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            value = 2;
        }

        float move = 0;
        if(Input.GetKey(KeyCode.W))
        {
            move = 1;
        }

        // 2. 방향이 필요
        float dir = value - 1;
        // 3. 회전하고 싶다.
        transform.Rotate(Vector3.up * dir * rotSpeed * Time.fixedDeltaTime);
        // 사용자의 입력에 따라 (앞으로)이동하고 싶다.
        // 1. 사용자 입력에 따라
        // 2. 방향이 필요
        // 3. 이동하고 싶다.
        rb.velocity = transform.forward * move * moveSpeed;
    }
}
