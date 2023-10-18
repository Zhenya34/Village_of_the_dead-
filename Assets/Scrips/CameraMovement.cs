using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offSet;
    public Transform player;

    private void Update()
    {
        transform.position = offSet + player.position;
    }
}
