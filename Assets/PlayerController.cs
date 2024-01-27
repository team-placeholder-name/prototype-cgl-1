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
        bool overlaps = CheckOverlap(instance, nextPosition);

        if(overlaps)
        {
            nextPosition = instance.position;
        }

        instance.position = nextPosition;
    
        


        DrawCharacters();
    }
    [SerializeField]
    private List<GameObject> playerMesh;
    private void DrawCharacters()
    {
        for(int i = 0; i < instances.Count; i++)
        {
            PlayerInstance player = instances[i];
            playerMesh[i].transform.position = player.position;
            playerMesh[i].SetActive(true);
            Renderer playerRenderer = playerMesh[i].GetComponentInChildren<Renderer>();
            if (player.Cyan == true && player.Magenta == true && player.Yellow == true)
            {
                playerRenderer.material.color =Color.white;
               
            }
            if (player.Cyan == false && player.Magenta == false && player.Yellow == false)
            {
                playerRenderer.material.color = Color.black;
                
            }
            if (player.Cyan == true && player.Magenta == false && player.Yellow == false)
            {
                playerRenderer.material.color = Color.cyan;
               
            }
            if (player.Cyan == false && player.Magenta == true && player.Yellow == false)
            {
                playerRenderer.material.color = Color.magenta;
            }
            if (player.Cyan == false && player.Magenta == false && player.Yellow == true)
            {
                playerRenderer.material.color = Color.yellow;
            }
            if (player.Cyan == true && player.Magenta == false && player.Yellow == true)
            {
                playerRenderer.material.color = Color.green;
            }
            if (player.Cyan == false && player.Magenta == true && player.Yellow == true)
            {
                playerRenderer.material.color = Color.red;
            }
            if (player.Cyan == true && player.Magenta == true && player.Yellow == false)
            {
                playerRenderer.material.color = Color.blue;
            }
        }
        }
    private bool CheckOverlap(PlayerInstance instance, Vector3 position)
    {
        bool hueOverlap = false;


        //LayerMask mask = LayerMask.GetMask("ColorRegion");
        Vector3 sphereBottom = position + Vector3.up * radius;
        Vector3 sphereTop = position + Vector3.up * height + Vector3.down * radius;
        Collider[] collisions = Physics.OverlapCapsule(sphereBottom, sphereTop, radius);

        foreach (Collider collision in collisions)
        {
            if (collision.TryGetComponent(out ColorComponent colorComponent))
            {
                if (!(instance.Cyan == colorComponent.Cyan && instance.Magenta == colorComponent.Magenta && instance.Yellow == colorComponent.Yellow))
                {
                    hueOverlap = true;
                }
            }
            else
            {
                //Load next level when colliding with anything that doesn't have the proper component
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        //Debug.Log("Overlaps { Cyan: " + cyanOverlap + ", Magenta: " + magentaOverlap+ ", Yellow: "+ yellowOverlap+" }");
        return hueOverlap;
    }

    public void OnMove(InputValue value)
    {
        moveInputDirection = value.Get<Vector2>();
    }
    public void OnInteract(InputValue value)
    {
        float mergeDistance=1;
        int closestIndex = -1;
        float closestDistance =100;
        for(int i =0; i<instances.Count; i++)
        {
            if (instances[i] == instances[controlledIndex])
            {
                continue;
            }
            float checkDistance = Vector3.Distance(instances[i].position, instances[controlledIndex].position);
            if (checkDistance < mergeDistance)
            {
                closestIndex = i;
                closestDistance = checkDistance;
            }
        }
        if(closestIndex != -1&& closestDistance<mergeDistance)
        {
            PlayerInstance merge = instances[closestIndex];
            PlayerInstance playerControlled = instances[controlledIndex];
            playerControlled.Cyan |= merge.Cyan;
            playerControlled.Magenta |= merge.Magenta;
            playerControlled.Yellow |= merge.Yellow;
            instances.RemoveAt(closestIndex);
            playerMesh[closestIndex].SetActive(false);
            playerMesh[controlledIndex].SetActive(false);
        }

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
    public void OnSplitR()
    {
        PlayerInstance controlledInstance = instances[controlledIndex];
        if (controlledInstance.Magenta == true&& (controlledInstance.Yellow||controlledInstance.Cyan))
        {
            PlayerInstance newInstance = new PlayerInstance(instances[controlledIndex].position, false, true, false);
            instances.Add(newInstance);
            controlledInstance.Magenta = false;
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == newInstance)
                {
                    controlledIndex = i;
                    break;
                }
            }
        }
    }
    public void OnSplitY()
    {
        PlayerInstance controlledInstance = instances[controlledIndex];
        if (controlledInstance.Yellow == true && (controlledInstance.Magenta||controlledInstance.Cyan))
        {
            PlayerInstance newInstance = new PlayerInstance(instances[controlledIndex].position, false, false, true);
            instances.Add(newInstance);
            controlledInstance.Yellow = false;
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == newInstance)
                {
                    controlledIndex = i;
                    break;
                }
            }
        }
    }
    public void OnSplitB()
    {
        PlayerInstance controlledInstance = instances[controlledIndex];
        if (controlledInstance.Cyan == true&& (controlledInstance.Magenta||controlledInstance.Yellow))
        {
            PlayerInstance newInstance = new PlayerInstance(instances[controlledIndex].position, true, false, false);
            instances.Add(newInstance);
            controlledInstance.Cyan = false;
            for(int i = 0; i<instances.Count; i++)
            {
                if (instances[i] == newInstance)
                {
                    controlledIndex = i; 
                    break;
                }
            }
        }
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
