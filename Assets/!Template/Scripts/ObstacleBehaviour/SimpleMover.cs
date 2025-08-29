using UnityEngine;

public enum SimpleMoverBehaviour
{
    None,
    Bullet,
    BulletLocal,
    PingPong,
    TranslateBackAndForth,
    Circular,
    Chase
}

public class SimpleMover : MonoBehaviour
{
    public SimpleMoverBehaviour behaviour;
    public Vector3 direction = Vector3.forward;
    public float speed = 1f;
    public float distance = 5f;
    public Transform rotationCenter;
    public float radius = 2f;
    public Transform chaseTarget;

    private Vector3 startPosition;
    private float time;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (GamePause.IsPause)
            return;

        time += Time.deltaTime;

        switch (behaviour)
        {
            case SimpleMoverBehaviour.Bullet:
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
                break;
            case SimpleMoverBehaviour.BulletLocal:
                transform.Translate(direction.normalized * speed * Time.deltaTime, Space.Self);
                break;
            case SimpleMoverBehaviour.PingPong:
                transform.position = startPosition + direction.normalized * Mathf.PingPong(time * speed, distance);
                break;
            case SimpleMoverBehaviour.TranslateBackAndForth:
                float t = Mathf.PingPong(time * speed, 1f);
                transform.Translate(direction.normalized * speed * Time.deltaTime * (t < 0.5f ? 1 : -1), Space.World);
                break;
            case SimpleMoverBehaviour.Circular:
                if (rotationCenter != null)
                {
                    float angle = time * speed;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                    transform.position = rotationCenter.position + offset;
                }
                break;
            case SimpleMoverBehaviour.Chase:
                if (chaseTarget != null)
                {
                    Vector3 dir = (chaseTarget.position - transform.position).normalized;
                    transform.Translate(dir * speed * Time.deltaTime, Space.World);
                }
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        switch (behaviour)
        {
            case SimpleMoverBehaviour.Bullet:
                Gizmos.DrawRay(transform.position, direction.normalized * 2f);
                break;

            case SimpleMoverBehaviour.BulletLocal:
                Gizmos.DrawRay(transform.position, transform.TransformVector(direction.normalized * 2f));
                break;

            case SimpleMoverBehaviour.PingPong:
                Gizmos.DrawLine(transform.position, transform.position + direction.normalized * distance);
                break;

            case SimpleMoverBehaviour.TranslateBackAndForth:
                Gizmos.DrawLine(transform.position, transform.position + direction.normalized * distance);
                Gizmos.DrawLine(transform.position, transform.position - direction.normalized * distance);
                break;

            case SimpleMoverBehaviour.Circular:
                if (rotationCenter != null)
                {
                    Gizmos.DrawWireSphere(rotationCenter.position, radius);
                    Gizmos.DrawLine(transform.position, rotationCenter.position);
                }
                break;

            case SimpleMoverBehaviour.Chase:
                if (chaseTarget != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, chaseTarget.position);
                }
                break;
        }
    }
}