using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         // El personaje o el punto de enfoque
    public Vector3 thirdPersonOffset = new Vector3(0.5f, -4f, -1.5f);
    public Vector3 firstPersonOffset = new Vector3(0f, 0f, 0.15f); // solo ligeramente delante del CameraTarget
    public float transitionSpeed = 5f;

    private Vector3 currentOffset;

    public float rotationSpeed = 5f;
    public float minY = -35f;        // Límite inferior de rotación vertical
    public float maxY = 90f;         // Límite superior de rotación vertical

    private float currentX = 0f;
    private float currentY = 0f;

    private bool aiming = false;  // Para verificar si estamos en modo primera persona

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentOffset = thirdPersonOffset;
    }

    void Update()
    {
        // Detectar el cambio de cámara (clic derecho)
        bool newAiming = Input.GetMouseButton(1);
        if (newAiming != aiming)
        {
            aiming = newAiming;

            // Si cambiamos de vista, reiniciamos la rotación de la cámara
            if (aiming)
            {
                currentX = 0f;  // Reiniciar rotación X (horizontal)
                currentY = 0f;  // Reiniciar rotación Y (vertical)
            }
        }

        // Actualizamos las rotaciones basadas en el movimiento del ratón
        if (!aiming) // Solo actualizamos la rotación cuando estamos en tercera persona
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    void LateUpdate()
    {
        Vector3 targetOffset = aiming ? firstPersonOffset : thirdPersonOffset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * transitionSpeed);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * currentOffset;
        transform.position = desiredPosition;

        if (aiming)
        {
            transform.rotation = rotation;  // En primera persona, aplicamos la rotación de la cámara
        }
        else
        {
            transform.LookAt(target.position + Vector3.up * 1.5f); // En tercera persona, miramos al personaje
        }
    }

    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.position, 0.05f);
        }
    }
}
