using Unity.Netcode;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [field:Header("Components"), SerializeField] public CharacterController Controller {  get; private set; }

    [Header("Movement Variables")]
    [field: SerializeField] public float BaseMovementSpeed { get; private set; } = 5f;
    [SerializeField] float slideSpeed = 8;

    [Header("GroundCheckVariables")]
    [SerializeField] float groundCheckRadius = 1f;
    [SerializeField] float groundCheckOffset = 0f;
    [SerializeField] LayerMask groundLayer;
    public bool IsGrounded { get; private set; }
    protected Vector3 hitNormal;

    [Header("Forces Variables")]
    [SerializeField] Vector3 externalForceVelocity;
    [SerializeField] protected float gravity = -9.8f;
    [SerializeField] float friction = 10;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    public void Move(Vector3 direction)
    {
        Controller.Move(((direction * BaseMovementSpeed) + CalculateForces()) * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        Controller.enabled = false;
        transform.position = position;
        Controller.enabled = true;
    }

    public Vector3 CalculateForces()
    {
        Vector3 slideDirection = Vector3.zero;

        float angle = Vector3.Angle(Vector3.up, hitNormal);

        if (angle > Controller.slopeLimit)
        {
            slideDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z) * slideSpeed;
        }

        if (IsGrounded && externalForceVelocity.y < 0)
        {
            externalForceVelocity.y = -0.5f;
            externalForceVelocity.x = Mathf.Lerp(externalForceVelocity.x, 0, friction) * Time.deltaTime;
            externalForceVelocity.z = Mathf.Lerp(externalForceVelocity.z, 0, friction) * Time.deltaTime;
        }
        else
        {
            externalForceVelocity.y += gravity * Time.deltaTime;
        }

        return externalForceVelocity + slideDirection;
    }

    Vector3 SphereCastPoint => (transform.position + Controller.center) - new Vector3(0, (Controller.height / 2 - Controller.radius), 0);

    bool GroundCheck()
    {
        return Physics.CheckSphere(SphereCastPoint - new Vector3(0, groundCheckOffset, 0), groundCheckRadius, groundLayer);
    }
}
