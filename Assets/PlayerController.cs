using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    List<PlayerInstance> instances = new List<PlayerInstance>() { new PlayerInstance(Vector3.zero,true,true, true) };

    Vector2 moveInputDirection = Vector2.zero;
    float moveSpeed = 5f;

    [SerializeField]
    float radius;
    [SerializeField]
    float height;

    int controlledIndex = 0;


    // Update is called once per frame
    void Update()
    {
        PlayerInstance instance = instances[controlledIndex];
        Vector2 moveDirection = moveInputDirection;  
        Vector2 moveOffset = moveDirection * moveSpeed * Time.deltaTime;
        Vector3 nextPosition = Vector3.zero;        
        nextPosition.x = instance.position.x + moveOffset.x;
        nextPosition.z = instance.position.z + moveOffset.y;

                //Check that next spot is valid;
                (bool, bool,bool) overlaps = CheckOverlap(instance, nextPosition);

        if(overlaps.Item1 && overlaps.Item2|| overlaps.Item1 && overlaps.Item3||overlaps.Item2&&overlaps.Item3) 
        { 
        
        }
        if(overlaps.Item1 == instance.Cyan&& overlaps.Item2== instance.Magenta&& overlaps.Item3== instance.Yellow)
        {
            nextPosition = instance.position;
        }
        else if ((overlaps.Item1 || overlaps.Item2 || overlaps.Item3)&& (!overlaps.Item1||!overlaps.Item2||!overlaps.Item3 ))
        {
            if (overlaps.Item1 == true)
            {

                instance.Cyan = false;
                PlayerInstance newChar = new PlayerInstance(instance.position, true, false, false);
                instances.Add(newChar);

            }
            if (overlaps.Item2 == true)
            {
                instance.Magenta = false;
                PlayerInstance newChar = new PlayerInstance(instance.position, false, true, false);
                instances.Add(newChar);

            }
            if (overlaps.Item3 == true)
            {
                instance.Yellow = false;
                PlayerInstance newChar = new PlayerInstance(instance.position, false, false, true);
                instances.Add(newChar);
            }
        }
        else 
        {
            //nextPosition = instance.position;
        }
        instance.position = nextPosition;
        return;
        if (instance.Magenta == true && instance.Cyan == true && instance.Yellow == true)
        {
            if (overlaps.Item1 && overlaps.Item2 && overlaps.Item3)
            {
                nextPosition = instance.position;
            }
            else
            {
                if (overlaps.Item1 == true)
                {

                    instance.Cyan = false;
                    PlayerInstance newChar = new PlayerInstance(instance.position, true, false, false);
                    instances.Add(newChar);

                }
                if (overlaps.Item2 == true)
                {
                    instance.Magenta = false;
                    PlayerInstance newChar = new PlayerInstance(instance.position, false, true, false);
                    instances.Add(newChar);

                }
                if (overlaps.Item3 == true)
                {
                    instance.Yellow = false;
                    PlayerInstance newChar = new PlayerInstance(instance.position, false, false, true);
                    instances.Add(newChar);
                }
            }
        }
        else if (overlaps.Item2 == true || overlaps.Item1 == true || overlaps.Item3 == true)
        {
            nextPosition = instance.position;
        }
                instance.position = nextPosition;

            
        
    }
    private (bool,bool,bool) CheckOverlap(PlayerInstance instance, Vector3 position)
    {
        bool cyanOverlap = false;
        bool magentaOverlap = false;
        bool yellowOverlap = false;
        //LayerMask mask = LayerMask.GetMask("ColorRegion");
        Vector3 sphereBottom = position + Vector3.up * radius;
        Vector3 sphereTop = position + Vector3.up * height + Vector3.down * radius;
        Collider[] collisions = Physics.OverlapCapsule(sphereBottom, sphereTop, radius);

        foreach (Collider collision in collisions)
        {
            if (collision.TryGetComponent(out ColorComponent colorComponent))
            {

                if (instance.Cyan && instance.Cyan == colorComponent.Cyan)
                {
                    cyanOverlap = true;

                }

                if (instance.Magenta && instance.Magenta == colorComponent.Magenta)
                {
                    magentaOverlap = true;
                }
                if (instance.Yellow && instance.Yellow == colorComponent.Yellow)
                {
                    yellowOverlap = true;
                }
            }
            else
            {
                //Load next level when colliding with anything that doesn't have the proper component
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        //Debug.Log("Overlaps { Cyan: " + cyanOverlap + ", Magenta: " + magentaOverlap+ ", Yellow: "+ yellowOverlap+" }");
        return (cyanOverlap, magentaOverlap, yellowOverlap);
    }
    public void OnMove(InputValue value)
    {
        moveInputDirection = value.Get<Vector2>();
    }
    public void OnInteract(InputValue value)
    {
        controlledIndex++;
        controlledIndex %= instances.Count;
    }
    private void OnDrawGizmos()
    {

        foreach (PlayerInstance player in instances)
        {
            if (player.Cyan == true && player.Magenta == true&& player.Yellow == true)
            {
                Gizmos.color = Color.white;
            }
            if (player.Cyan == false && player.Magenta == false && player.Yellow == false)
            {
                Gizmos.color = Color.black;
            }
            if (player.Cyan == true && player.Magenta == false && player.Yellow == false)
            {
                Gizmos.color = Color.cyan;
            }
            if (player.Cyan == false && player.Magenta == true && player.Yellow == false)
            {
                Gizmos.color = Color.magenta;
            }
            if (player.Cyan == false && player.Magenta == false && player.Yellow == true)
            {
                Gizmos.color = Color.yellow;
            }
            if (player.Cyan == true && player.Magenta == false && player.Yellow == true)
            {
                Gizmos.color = Color.green;
            }
            if (player.Cyan == false && player.Magenta == true && player.Yellow == true)
            {
                Gizmos.color = Color.red;
            }
            if (player.Cyan == true && player.Magenta == true && player.Yellow == false)
            {
                Gizmos.color = Color.blue;
            }




            Vector3 sphereBottom = player.position + Vector3.up * radius;
            Vector3 sphereTop = player.position + Vector3.up * height + Vector3.down * radius;
            //lowerSphere
            Gizmos.DrawWireSphere(sphereBottom, radius);
            //upperSphere
            Gizmos.DrawWireSphere(sphereTop, radius);
            Gizmos.DrawRay(sphereBottom + Vector3.forward * radius, Vector3.up * (height - 2 * radius));
            Gizmos.DrawRay(sphereBottom + Vector3.back * radius, Vector3.up * (height - 2 * radius));
            Gizmos.DrawRay(sphereBottom + Vector3.left * radius, Vector3.up * (height - 2 * radius));
            Gizmos.DrawRay(sphereBottom + Vector3.right * radius, Vector3.up * (height - 2 * radius));
        }

        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(instances[controlledIndex].position + Vector3.up * height, radius / 3f);
    }
}

[System.Serializable]
public class PlayerInstance
{
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public bool Cyan;
    [SerializeField]
    public bool Magenta;
    [SerializeField]
    public bool Yellow;
    public PlayerInstance(Vector3 position, bool Cyan, bool Magenta, bool Yellow) 
    { 
        this.position= position;
        this.Cyan= Cyan;
        this.Magenta= Magenta;
        this.Yellow = Yellow;
    }
}