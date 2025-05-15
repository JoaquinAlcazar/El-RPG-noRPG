using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;         
    public Vector3 thirdPersonOffset = new Vector3(0.5f, -4f, -1.5f);
    public Vector3 firstPersonOffset = new Vector3(0f, 0f, 0.15f); 
    public float transitionSpeed = 5f;

    private Vector3 currentOffset;

    public float rotationSpeed = 5f;
    public float minY = -35f;        
    public float maxY = 90f;  

    private float currentX = 0f;
    private float currentY = 0f;

    private bool aiming = false; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentOffset = thirdPersonOffset;
    }

    void Update()
    {
        bool newAiming = Input.GetMouseButton(1);
        if (newAiming != aiming)
        {
            aiming = newAiming;

            if (aiming)
            {
                // Al entrar a primera persona
                currentX = 0f;
                currentY = 0f;
            }
            else
            {
                // Al salir de primera persona, resetear cámara
                currentX = 0f;
                currentY = 10f; // Puedes ajustar el valor inicial que quieras
                currentOffset = thirdPersonOffset; // Reset inmediato
            }
        }

        // Siempre actualizar rotación con el mouse
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, minY, maxY);
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
            transform.rotation = rotation;  
        }
        else
        {
            transform.LookAt(target.position + Vector3.up * 1.5f); 
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
