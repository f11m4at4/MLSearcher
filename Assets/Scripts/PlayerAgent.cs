using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

// 1. 사용자의 입력에 따라 회전하고 싶다.
// 필요속성 : 회전속도
// 2. 사용자의 입력에 따라 (앞으로)이동하고 싶다.
// 필요속성 : 이동속도
public class PlayerAgent : Agent
{
    // 필요속성 : 회전속도
    public float rotSpeed = 100;
    // 필요속성 : 이동속도
    public float moveSpeed = 5;
    Rigidbody rb;

    public GameManager gm;

    // Ray 들로 충돌을 검출해서 데이터를 Policy 로 보내고 싶다.
    // 필요속성 : tag 들, 특정 대상만 검출할 LayerMask, 광선의 길이
    public string[] detectableTags;
    public LayerMask rayLayerMask;
    [Range(0f, 100f)]
    public float rayLength = 20;
    // 구체의 크기
    [Range(0, 10)]
    public float sphereCastRadius = 0.5f;
    // 방향당 Ray 를 몇개??
    [Range(0, 50)]
    public int raysPerDirection = 2;
    // 최대 각도
    [Range(0, 180)]
    public int maxRayDegrees = 60;

    // 관찰변수를 만들어서 넘겨주는 함수
    public override void CollectObservations(VectorSensor sensor)
    {
        // Ray 를 쏴서 부딪힌 정보를 관찰요소로 만들어서 보내주고 싶다.
        // 1. Ray 를 만드는 정보가 필요
        // -> 몇개??
        int totalRayCount = raysPerDirection * 2 + 1;
        // -> 각 Ray 당 각도
        float delta = maxRayDegrees / raysPerDirection;
        // -> 각 Ray 의 실제 각도
        float[] angles = new float[totalRayCount];
        // -> 정 중앙 Ray 는 각도가 0
        angles[0] = 0;
        // 나머지 Ray 들의 각도 구하기
        for (int i = 0; i < raysPerDirection; i++)
        {
            // 왼쪽 각도
            angles[2 * i + 1] = -(i + 1) * delta;
            // 오른쪽 각도
            angles[2 * i + 2] = (i + 1) * delta;
        }
        foreach (float angle in angles)
        {
            // 최종 보낼 관찰 데이터 선언
            float []data = { 0, 0, 0, 1, 0 };
            // 2. Ray 를 쐈으니까
            Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hitInfo;
            bool bHit = Physics.SphereCast(ray, sphereCastRadius, out hitInfo, rayLength, rayLayerMask.value);
            // 3. 부딪힌 녀석의 정보가 필요
            // 4. 관찰요소를 만들어 보내주고 싶다.
            // 4-1 관찰데이터를 만들기
            if (bHit)
            {
                // Tag 데이터 할당
                for (int i = 0; i < detectableTags.Length; i++)
                {
                    string tag = detectableTags[i];
                    // 만약 부딪힌 물체의 테그가 감지 테그와 같다면
                    if (hitInfo.transform.CompareTag(tag))
                    {
                        // 그 테그쪽 데이터를 1로 활성화 시켜주자
                        data[i] = 1;
                        break;
                    }
                }
                // 부딪혔다고 세팅 0으로
                data[3] = 0;
                // 부딪힌 물체와의 거리 -> Normalize 
                data[4] = hitInfo.distance / rayLength;
            }
            // 4-2 데이터 보내기
            sensor.AddObservation(data);

            // Ray 그리기
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
    }

    private void OnDrawGizmosSelected()
    {
        int totalRayCount = raysPerDirection * 2 + 1;
        // -> 각 Ray 당 각도
        float delta = maxRayDegrees / raysPerDirection;
        // -> 각 Ray 의 실제 각도
        float[] angles = new float[totalRayCount];
        // -> 정 중앙 Ray 는 각도가 0
        angles[0] = 0;
        // 나머지 Ray 들의 각도 구하기
        for (int i = 0; i < raysPerDirection; i++)
        {
            // 왼쪽 각도
            angles[2 * i + 1] = -(i + 1) * delta;
            // 오른쪽 각도
            angles[2 * i + 2] = (i + 1) * delta;
        }
        foreach (float angle in angles)
        {
            // 최종 보낼 관찰 데이터 선언
            float[] data = { 0, 0, 0, 1, 0 };
            // 2. Ray 를 쐈으니까
            Vector3 dir = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hitInfo;
            bool bHit = Physics.SphereCast(ray, sphereCastRadius, out hitInfo, rayLength, rayLayerMask.value);

            if (bHit)
            {
                Gizmos.color = Color.red;
                // Ray 그리기
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
        // 2. 방향이 필요
        float dir = actions.DiscreteActions.Array[0] - 1;
        float move = actions.DiscreteActions.Array[1];
        // 3. 회전하고 싶다.
        transform.Rotate(Vector3.up * dir * rotSpeed * Time.fixedDeltaTime);
        // 사용자의 입력에 따라 (앞으로)이동하고 싶다.
        // 1. 사용자 입력에 따라
        // 2. 방향이 필요
        // 3. 이동하고 싶다.
        rb.velocity = transform.forward * move * moveSpeed;
    }

    // Test 목적 -> 사람이 직접 값을 입력해주기
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 1. 사용자 입력에따라
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

    
    // Player 가 다른 물체와 부딪혔을 때 처리
    private void OnCollisionEnter(Collision other)
    {
        // Item 하고 부딪히면
        if (other.gameObject.CompareTag("Item"))
        {
            // -> 참 잘했어요~~~
            EndEpisode();
        }
        // 장애물하고 부딪히면
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // -> 때끼!
            EndEpisode();
        }
    }
}
