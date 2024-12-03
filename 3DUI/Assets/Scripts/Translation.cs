using UnityEngine;
using UnityEngine.InputSystem;

public class Translation : MonoBehaviour
{
    private NetworkPlayer player;
    // Start is called before the first frame update
    private void Awake()
    {
        player = GetComponent<NetworkPlayer>();
    }

    public void Translate(InputAction.CallbackContext context)
    {
        if (context.started && player != null)
        {
            player.SpaceRecord();
        }
        else if(context.canceled && player != null)
        {
            player.EndRecord();
        }
    }
}
