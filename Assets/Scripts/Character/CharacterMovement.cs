using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [field:Header("Components"), SerializeField] public CharacterController Controller {  get; private set; }

    [Header("Movement Variables")]
    [field: SerializeField] public float BaseMovementSpeed { get; private set; } = 5f;
    [field: SerializeField] public Vector2 JumpVelocity = new(2, 1.5f);
    [SerializeField] float slideSpeed = 8;

    [Header("Ground and Slope Variables")]
    [SerializeField] float groundCheckRadius = 1f;
    [SerializeField] float groundCheckOffset = 0f;
    [SerializeField] LayerMask groundLayer;
    protected Vector3 hitNormal;
    public bool IsGrounded { get; private set; }

    [Header("Forces Variables")]
    [SerializeField] Vector3 externalForceVelocity;
    [SerializeField] protected float gravity = -9.8f;
    [field: SerializeField] public float Weight { get; private set; } = 5f;
    [SerializeField] float friction = 10;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        IsGrounded = GroundCheck();
        GetSlopeNormal();
    }

    public void Move(Vector3 direction, float movementMultiplier = 1)
    {
        float angle = Vector3.Angle(Vector3.up, hitNormal);
        if (angle < Controller.slopeLimit)
        {
            direction = AdjustDirectionToSlope(direction);
        }

        Controller.Move(((direction * (BaseMovementSpeed * movementMultiplier)) + CalculateForces()) * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        Controller.enabled = false;
        transform.position = position;
        Controller.enabled = true;
    }

    public void Jump(Vector3 direction)
    {
        if (IsOnSlope || !GroundCheck()) return;
        externalForceVelocity += new Vector3((direction.x * JumpVelocity.x), Mathf.Sqrt(JumpVelocity.y * -2f * gravity), (direction.z * JumpVelocity.x));
    }

    public Vector3 CalculateForces()
    {
        Vector3 slideDirection = Vector3.zero;

        float angle = Vector3.Angle(Vector3.up, hitNormal);

        if (angle > Controller.slopeLimit)
        {
            slideDirection = new Vector3(hitNormal.x, -1, hitNormal.z) * slideSpeed;
        }

        if (IsGrounded && externalForceVelocity.y < 0)
        {
            externalForceVelocity.y = -0.5f;
            externalForceVelocity.x = Mathf.Lerp(externalForceVelocity.x, 0, friction * Time.deltaTime);
            externalForceVelocity.z = Mathf.Lerp(externalForceVelocity.z, 0, friction * Time.deltaTime);
        }
        else
        {
            externalForceVelocity.y += gravity * Time.deltaTime;
        }

        return externalForceVelocity + slideDirection;
    }

    public void Push(Vector3 direction)
    {
        externalForceVelocity += (direction / Weight);
    }

    Vector3 SphereCastPoint => (transform.position + Controller.center) - new Vector3(0, (Controller.height / 2 - Controller.radius), 0);

    #region Slope & Ground Check

    bool GroundCheck()
    {
        return Physics.CheckSphere(SphereCastPoint - new Vector3(0, groundCheckOffset, 0), groundCheckRadius, groundLayer);
    }

    private void GetSlopeNormal()
    {
        RaycastHit hit;
        Debug.DrawRay(SphereCastPoint, -transform.up * (Controller.radius - 0.01f));
        if (Physics.SphereCast(SphereCastPoint, Controller.radius - 0.01f, -transform.up, out hit, 0.3f, groundLayer))
        {
            hitNormal = hit.normal;
        }
        else
            hitNormal = Vector3.zero;
    }

    private Vector3 AdjustDirectionToSlope(Vector3 direction)
    {
        Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, hitNormal);
        Vector3 adjustedDirection = slopeRot * direction;
        if (adjustedDirection.y < 0)
        {
            return adjustedDirection;
        }
        else
        {
            return direction;
        }
    }

    bool IsOnSlope => Vector3.Angle(Vector3.up, hitNormal) > Controller.slopeLimit;

    #endregion

    private void OnDrawGizmos()
    {
        if (IsGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(SphereCastPoint - new Vector3(0, groundCheckOffset, 0), groundCheckRadius);
    }
}
