using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. ������� �Է¿� ���� ȸ���ϰ� �ʹ�.
// �ʿ�Ӽ� : ȸ���ӵ�
// 2. ������� �Է¿� ���� (������)�̵��ϰ� �ʹ�.
// �ʿ�Ӽ� : �̵��ӵ�
public class PlayerAgent : MonoBehaviour
{
    // �ʿ�Ӽ� : ȸ���ӵ�
    public float rotSpeed = 100;
    // �ʿ�Ӽ� : �̵��ӵ�
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
        // ������� �Է¿� ���� ȸ���ϰ� �ʹ�.
        // 1. ����� �Է¿�����
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

        // 2. ������ �ʿ�
        float dir = value - 1;
        // 3. ȸ���ϰ� �ʹ�.
        transform.Rotate(Vector3.up * dir * rotSpeed * Time.fixedDeltaTime);
        // ������� �Է¿� ���� (������)�̵��ϰ� �ʹ�.
        // 1. ����� �Է¿� ����
        // 2. ������ �ʿ�
        // 3. �̵��ϰ� �ʹ�.
        rb.velocity = transform.forward * move * moveSpeed;
    }
}
