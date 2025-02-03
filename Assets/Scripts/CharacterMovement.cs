using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [field:Header("Components"), SerializeField] public CharacterController Controller {  get; private set; }

    [Header("Movement Variables")]
    [field: SerializeField] public float BaseMovementSpeed { get; private set; } = 5f;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    public void Move(Vector3 direction)
    {
        Controller.Move((direction * BaseMovementSpeed) * Time.deltaTime);
    }

    public void Teleport(Vector3 position)
    {
        Controller.enabled = false;
        transform.position = position;
        Controller.enabled = true;
    }
}
