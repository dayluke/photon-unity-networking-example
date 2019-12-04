using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 5; // Used to change the speed at which the player moves.
    [SerializeField] private int mouseSensitivity = 1; // Used to alter the sensitivity of the mouse.
    [SerializeField] private int maxHealth = 100; // Used to initialise the player's health.
    [SerializeField] private int currentHealth = 100; // Used to keep track of the player's health.
    [SerializeField] private int damage = 25; // Used to damage the receiving player.
    [SerializeField] private Image healthSlider = null; // Used to update the health bar above the player.

    private Rigidbody rb; // Used to control the player's movement and apply physics to the player.
    private Camera cameraGO; // Used to control the player's looking and camera movement.

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Retrieves the rigidbody component in the player.
        cameraGO = GetComponentInChildren<Camera>(); // Gets the Camera component in the children of the player.
        currentHealth = maxHealth; // Initialises the player's health to the value in maxHealth.

        if (!photonView.IsMine) // if we are not controlling this player, then disable the camera on them.
        {
            cameraGO.enabled = false;
        }

        // Sets the name of the player's gameobject to the name they set at the beginning.
        gameObject.name = photonView.Owner.NickName;
    }

    private void FixedUpdate()
    {
        // We only process Inputs if we are the local player
        if (photonView.IsMine)
        {
            Move();

            if (Input.GetButtonDown("Fire1"))
            {
                // Ensures that the Attack method is called on all clients.
                photonView.RPC("Attack", RpcTarget.All);
            }
        }
    }

    private void Move()
    {
        // Player movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // TransformDirection() converts from local space into world space as the input is local.
        Vector3 dir = transform.TransformDirection(new Vector3(x, 0, z) * speed);
        rb.velocity = dir; // Add our movement to our rigidbody component.

        // Mouse rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 playerRotation = new Vector3(0, mouseX, 0) * mouseSensitivity;
        transform.Rotate(playerRotation); // Rotate the player when looking left or right.

        Vector3 cameraRotation = new Vector3(-mouseY, 0, 0) * mouseSensitivity;
        cameraGO.transform.Rotate(cameraRotation); // Rotate the camera when looking up or down.
    }

    [PunRPC] // Very important as this is an RPC method.
    private void Attack()
    {
        RaycastHit hit;
        
        // Does the raycast hit anything directing in front of the way we are facing?
        if (Physics.Raycast(transform.position, cameraGO.transform.forward, out hit, 1000))
        {
            // It does, but is the object we hit tagged with the 'Player' tag?
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                // Yes, it is. Therefore, we can deduct health from the player.
                PlayerMovement player = hit.transform.gameObject.GetComponent<PlayerMovement>();
                if (player.currentHealth > damage)
                {
                    player.currentHealth -= damage;
                    player.healthSlider.fillAmount = (float)player.currentHealth / player.maxHealth;
                }
                else
                {
                    // We destroy the receiving player's gameobject as they are now dead.
                    PhotonNetwork.Destroy(player.gameObject);
                }
            }
        }

        // Used to show the player's hit direction in the Unity Editor.
        Debug.DrawRay(transform.position, cameraGO.transform.forward * 100, Color.red);
    }
}
