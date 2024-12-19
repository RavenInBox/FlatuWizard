using UnityEngine;

public class CameraContoller : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] KeyBoardMouseController keyC;
    [Space]
    [SerializeField] Transform player;

    bool lockView = true;
    bool lockSpace = true;

    bool topLeft;
    bool topRight;
    bool bottomLeft;
    bool bottomRight;

    [SerializeField] float panSpeed = 20f;
    [SerializeField] Vector2 panLimit;

    public void MyUpdate()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        bool clock = Input.GetKeyDown(keyC.CameraLock);
        bool down_space = Input.GetKeyDown(keyC.CameraTempLock);
        bool up_space = Input.GetKeyUp(keyC.CameraTempLock);

        if (clock)
        {
            if (lockView)
            {
                lockView = false;
                lockSpace = false;
            }
            else
            {
                lockView = true;
                lockSpace = true;
            }
        }

        if (!lockSpace)
        {
            if (down_space) lockView = true;
            if (up_space) lockView = false;
        }

        if (!lockView)
        {
            CameraFree();
            return;
        }

        Vector3 playerPosition = new (player.position.x, transform.position.y, player.position.z);
        transform.position = playerPosition;
    }

    private void CameraFree()
    {
        Vector3 pos = transform.position;

        if (topLeft)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (topRight)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (bottomLeft)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (bottomRight)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, -panLimit.y, panLimit.y);
        pos.z = Mathf.Clamp(pos.z, -panLimit.x, panLimit.x);

        transform.position = pos;
    }

    public void Topleft(bool active)
    {
        topLeft = active;
    }

    public void Topright(bool active) 
    {
        topRight = active;
    }

    public void Bottomleft(bool active)
    {
        bottomLeft = active;
    }

    public void BottomRight(bool active)
    {
        bottomRight = active;
    }
}
