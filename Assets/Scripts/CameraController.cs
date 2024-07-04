using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Tao object player
    public GameObject player;

    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z - 10);
    }
}
