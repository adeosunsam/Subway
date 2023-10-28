using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Transform lookAt;

    private Vector3 offset = new (0, 1.25f, -5f);

    void Start()
    {
        transform.position = lookAt.position + offset;
    }

    void LateUpdate()
    {
        var position = lookAt.position + offset;
        position.x = 0;
        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime);
    }
}
