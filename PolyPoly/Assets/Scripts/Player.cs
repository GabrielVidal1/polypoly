using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float steeringSpeed;
    
    private Circuit circuit;

    private Vector3 targetDirection;
    
    private Rigidbody rb;

    public Section CurrentSection = null;


    [SyncVar]
    public int money = 1000;

    private bool stopped;
    
    [SerializeField] private float fuel = 3f;
    
    private void Start()
    {
        GameManager.instance.AddPlayer(this);
        if (isLocalPlayer)
            GameManager.instance.localPlayer = this;

        rb = GetComponent<Rigidbody>();
        
        if (!isLocalPlayer)    
        {
            Destroy(rb);
            tag = "Undefined";
        }
    }

    [Server]
    public void Go(Circuit circuit)
    {
        this.circuit = circuit;
        CurrentSection = circuit.sections[0];
        stopped = false;
    }

    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
            Move(Input.GetKey(KeyCode.LeftArrow), Input.GetKey(KeyCode.RightArrow));
    }
    
    
    [Command]
    private void Move(bool left, bool right)
    {
        Vector3 acceleration = transform.forward * speed;

        float torque = 0f;
        if (left)
        {
            torque = -steeringSpeed;
            acceleration += -transform.right * steeringSpeed;
        }
        if (right)
        {
            torque = steeringSpeed;
            acceleration += transform.right * steeringSpeed;
        }
        rb.AddTorque(transform.up * torque);

        if (fuel >= 0f)
        {
            rb.AddForce(acceleration);
            fuel -= Time.fixedDeltaTime;
        }
        else
        {
            if (!stopped)
                Stop();
        }
    }

    [Command]
    public void Refuel()
    {
        fuel = Random.value * 10f;
        stopped = false;
    }

    [ClientRpc]
    void Stop()
    {
        stopped = true;
        GameManager.instance.mainCanvas.StopScreen.Open();
    }
}
