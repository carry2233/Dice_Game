using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class NoBouncePhysics : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 반발력 제거 (단순화된 방식)
        if (collision.relativeVelocity.magnitude > 0.1f)
        {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, collision.contacts[0].normal);
        }
    }
}
