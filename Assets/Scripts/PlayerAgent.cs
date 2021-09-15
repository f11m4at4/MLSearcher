using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// 1. ������� �Է¿� ���� ȸ���ϰ� �ʹ�.
// �ʿ�Ӽ� : ȸ���ӵ�
// 2. ������� �Է¿� ���� (������)�̵��ϰ� �ʹ�.
// �ʿ�Ӽ� : �̵��ӵ�
public class PlayerAgent : Agent
{
    // �ʿ�Ӽ� : ȸ���ӵ�
    public float rotSpeed = 100;
    // �ʿ�Ӽ� : �̵��ӵ�
    public float moveSpeed = 5;
    Rigidbody rb;

    public GameManager gm;

    // Ray ��� �浹�� �����ؼ� �����͸� Policy �� ������ �ʹ�.
    // �ʿ�Ӽ� : tag ��, Ư�� ��� ������ LayerMask, ������ ����
    public string[] detectableTags;
    public LayerMask rayLayerMask;
    [Range(0f, 100f)]
    public float rayLength = 20;
    // ��ü�� ũ��
    [Range(0, 10)]
    public float sphereCastRadius = 0.5f;
    // ����� Ray �� �??
    [Range(0, 50)]
    public int raysPerDirection = 2;
    // �ִ� ����
    [Range(0, 180)]
    public int maxRayDegrees = 60;

    // ���������� ���� �Ѱ��ִ� �Լ�
    public override void CollectObservations(VectorSensor sensor)
    {
        // Ray �� ���� �ε��� ������ ������ҷ� ���� �����ְ� �ʹ�.
        // 1. Ray �� ����� ������ �ʿ�
        // -> �??
        int totalRayCount = raysPerDirection * 2 + 1;
        // -> �� Ray �� ����
        float delta = maxRayDegrees / raysPerDirection;
        // -> �� Ray �� ���� ����
        float[] angles = new float[totalRayCount];
        // -> �� �߾� Ray �� ������ 0
        angles[0] = 0;
        // ������ Ray ���� ���� ���ϱ�
        for (int i = 0; i < raysPerDirection; i++)
        {
            // ���� ����
            angles[2 * i + 1] = -(i + 1) * delta;
            // ������ ����
            angles[2 * i + 2] = (i + 1) * delta;
        }
        foreach (float angle in angles)
        {
            // ���� ���� ���� ������ ����
            float []data = { 0, 0, 0, 1, 0 };
            // 2. Ray �� �����ϱ�
            Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hitInfo;
            bool bHit = Physics.SphereCast(ray, sphereCastRadius, out hitInfo, rayLength, rayLayerMask.value);
            // 3. �ε��� �༮�� ������ �ʿ�
            // 4. ������Ҹ� ����� �����ְ� �ʹ�.
            // 4-1 ���������͸� �����
            if (bHit)
            {
                // Tag ������ �Ҵ�
                for (int i = 0; i < detectableTags.Length; i++)
                {
                    string tag = detectableTags[i];
                    // ���� �ε��� ��ü�� �ױװ� ���� �ױ׿� ���ٸ�
                    if (hitInfo.transform.CompareTag(tag))
                    {
                        // �� �ױ��� �����͸� 1�� Ȱ��ȭ ��������
                        data[i] = 1;
                        break;
                    }
                }
                // �ε����ٰ� ���� 0����
                data[3] = 0;
                // �ε��� ��ü���� �Ÿ� -> Normalize 
                data[4] = hitInfo.distance / rayLength;
            }
            // 4-2 ������ ������
            sensor.AddObservation(data);

            // Ray �׸���
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
    }

    private void OnDrawGizmosSelected()
    {
        int totalRayCount = raysPerDirection * 2 + 1;
        // -> �� Ray �� ����
        float delta = maxRayDegrees / raysPerDirection;
        // -> �� Ray �� ���� ����
        float[] angles = new float[totalRayCount];
        // -> �� �߾� Ray �� ������ 0
        angles[0] = 0;
        // ������ Ray ���� ���� ���ϱ�
        for (int i = 0; i < raysPerDirection; i++)
        {
            // ���� ����
            angles[2 * i + 1] = -(i + 1) * delta;
            // ������ ����
            angles[2 * i + 2] = (i + 1) * delta;
        }
        foreach (float angle in angles)
        {
            // ���� ���� ���� ������ ����
            float[] data = { 0, 0, 0, 1, 0 };
            // 2. Ray �� �����ϱ�
            Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hitInfo;
            bool bHit = Physics.SphereCast(ray, sphereCastRadius, out hitInfo, rayLength, rayLayerMask.value);

            if (bHit)
            {
                Gizmos.color = Color.red;
                // Ray �׸���
                Gizmos.DrawLine(ray.origin, hitInfo.point);
                Gizmos.DrawWireSphere(hitInfo.point, sphereCastRadius);
            }
        }
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    private void Reset()
    {
        rb.velocity = Vector3.zero;
        gm.Reset();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 2. ������ �ʿ�
        float dir = actions.DiscreteActions.Array[0] - 1;
        float move = actions.DiscreteActions.Array[1];
        // 3. ȸ���ϰ� �ʹ�.
        transform.Rotate(Vector3.up * dir * rotSpeed * Time.fixedDeltaTime);
        // ������� �Է¿� ���� (������)�̵��ϰ� �ʹ�.
        // 1. ����� �Է¿� ����
        // 2. ������ �ʿ�
        // 3. �̵��ϰ� �ʹ�.
        rb.velocity = transform.forward * move * moveSpeed;
    }

    // Test ���� -> ����� ���� ���� �Է����ֱ�
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 1. ����� �Է¿�����
        float value = 1;
        if (Input.GetKey(KeyCode.A))
        {
            value = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            value = 2;
        }

        float move = 0;
        if (Input.GetKey(KeyCode.W))
        {
            move = 1;
        }

        actionsOut.DiscreteActions.Array[0] = (int)value;
        actionsOut.DiscreteActions.Array[1] = (int)move;
    }

    
    // Player �� �ٸ� ��ü�� �ε����� �� ó��
    private void OnCollisionEnter(Collision other)
    {
        // Item �ϰ� �ε�����
        if (other.gameObject.CompareTag("Item"))
        {
            // -> �� ���߾��~~~
            EndEpisode();
        }
        // ��ֹ��ϰ� �ε�����
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // -> ����!
            EndEpisode();
        }
    }
}
