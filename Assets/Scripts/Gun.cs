using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public InputAction m_fireAction;
    public Camera cam;
    public ParticleSystem particlesRef;

    private RaycastHit hit;

    public int damage = 10;
    public float range = 100.0f;
    public float impact = 30.0f;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fire()
    {
        particlesRef.Play();
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null) 
            {
                enemy.damage(damage);
                hit.rigidbody.AddForce(-hit.normal * impact);
            }
        }

    }

    public void fireTargetPlayer(Vector3 t_target, Vector3 t_direction)
    {
        particlesRef.Play();
        if (Physics.Raycast(transform.position, t_direction, out hit ,range))
        {
            Player player = hit.transform.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("player hit");
                player.damage();
            }
        }
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    private void Awake()
    {
    }

}
