using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [field:Header("Components"), SerializeField] public CharacterController Controller {  get; private set; }

    [Header("Movement Variables")]
    [field: SerializeField] public float BaseMovementSpeed { get; private set; } = 5f;
    [field: SerializeField] public float AirMovementMultiplier { get; private set; } = 0.01f;
    [field: SerializeField] public Vector2 JumpVelocity = new(2, 1.5f);
    [SerializeField] float slideSpeed = 8;
    public float CurrentVelocity { get; private set; }

    [Header("Ground and Slope Variables")]
    [SerializeField] float groundCheckRadius = 1f;
    [SerializeField] float groundCheckOffset = 0f;
    [SerializeField] LayerMask groundLayer;
    protected Vector3 hitNormal;
    public bool IsGrounded { get; private set; }

    [Header("Forces Variables")]
    [SerializeField] Vector3 forcesVelocity;
    [SerializeField] protected float gravity = -9.8f;
    [field: SerializeField] public float Weight { get; private set; } = 5f;

    [SerializeField] float friction = 10;
    [SerializeField] float airResistance = 5;

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
        Vector3 prevPosition = transform.position;

        float angle = Vector3.Angle(Vector3.up, hitNormal);
        if (angle < Controller.slopeLimit)
        {
            direction = AdjustDirectionToSlope(direction);
        }

        forcesVelocity += (IsGrounded ? 1 : AirMovementMultiplier) * BaseMovementSpeed * Time.deltaTime * movementMultiplier * direction;
        Controller.Move(CalculateForces() * Time.deltaTime);

        CurrentVelocity = Vector3.Distance(prevPosition, transform.position) / Time.deltaTime;
    }

    public void Teleport(Vector3 position)
    {
        Controller.enabled = false;

        if (TryGetComponent(out NetworkTransform networkTransform))
        {
            networkTransform.Teleport(position, transform.rotation, transform.lossyScale);
        }
        else
        {
            transform.position = position;
        }

        Controller.enabled = true;
    }

    public void Jump(Vector3 direction)
    {
        if (IsOnSlope || !GroundCheck()) return;
        forcesVelocity += new Vector3((direction.x * JumpVelocity.x), Mathf.Sqrt(JumpVelocity.y * -2f * gravity), (direction.z * JumpVelocity.x));
    }

    public Vector3 CalculateForces()
    {
        Vector3 slideDirection = Vector3.zero;

        float angle = Vector3.Angle(Vector3.up, hitNormal);

        if (angle > Controller.slopeLimit)
        {
            slideDirection = new Vector3(hitNormal.x, -1, hitNormal.z) * slideSpeed * Time.deltaTime;
        }

        if (IsGrounded && forcesVelocity.y < 0)
        {
            forcesVelocity.y = -0.5f;
            forcesVelocity.x = Mathf.Lerp(forcesVelocity.x, 0, friction * Time.deltaTime);
            forcesVelocity.z = Mathf.Lerp(forcesVelocity.z, 0, friction * Time.deltaTime);
        }
        else
        {
            forcesVelocity.y += gravity * Time.deltaTime;
        }

        forcesVelocity.x = Mathf.Lerp(forcesVelocity.x, 0, airResistance * Time.deltaTime);
        forcesVelocity.z = Mathf.Lerp(forcesVelocity.z, 0, airResistance * Time.deltaTime);

        return forcesVelocity + slideDirection;
    }

    public void Push(Vector3 direction)
    {
        forcesVelocity += (direction / Weight);
    }

    Vector3 SphereCastPoint => (transform.position + Controller.center) - new Vector3(0, (Controller.height / 2 - Controller.radius), 0);

    #region Slope & Ground Check

    bool GroundCheck()
    {
        return Physics.CheckSphere(SphereCastPoint - new Vector3(0, groundCheckOffset, 0), groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
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
