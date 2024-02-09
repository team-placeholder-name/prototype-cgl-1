using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    List<PlayerInstance> instances = new List<PlayerInstance>() { new PlayerInstance(Vector3.zero,true,true, true) };

    Vector3 moveInputDirection = Vector3.zero;
    float moveSpeed = 5f;

    [SerializeField]
    float radius;
    [SerializeField]
    float height;

    int controlledIndex = 0;

    public Vector3 GetPlayerPosition()
    {
        return instances[controlledIndex].position;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        forward = forward.normalized;
        forward *= moveInputDirection.z;
        right *= moveInputDirection.x;


        PlayerInstance instance = instances[controlledIndex];
 
        Vector3 moveOffset = (forward+right) * moveSpeed * Time.deltaTime;
        Vector3 nextPosition = Vector3.zero;        
        nextPosition.x = instance.position.x + moveOffset.x;
        nextPosition.y = instance.position.y + moveOffset.y;
        nextPosition.z = instance.position.z + moveOffset.z;

        //Check that next spot is valid;
        
        bool walkable = false;
        if (Physics.Raycast(nextPosition+Vector3.up, Vector3.down, out RaycastHit hit, 2f, 1 << LayerMask.NameToLayer("Default")))
        {
            nextPosition.y = hit.point.y+0.2f;
            walkable = true;
        }
        bool overlaps = CheckOverlap(instance, nextPosition);
        if (overlaps||!walkable)
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
                if(collision.tag== "Finish")
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                hueOverlap = true;
            }
        }
        //Debug.Log("Overlaps { Cyan: " + cyanOverlap + ", Magenta: " + magentaOverlap+ ", Yellow: "+ yellowOverlap+" }");
        return hueOverlap;
    }

    public float yrot =0;
    public void OnLook(InputValue value)
    {
        yrot += value.Get<Vector2>().x;
    }
    public void OnMove(InputValue value)
    {



        moveInputDirection = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);

    }
    public void OnProject(InputValue value)
    {
        float closestAngle=180f;
        int closestIndex = -1;
        
        Vector3  direction = Camera.main.transform.forward;
        direction.y = 0;
        direction = direction.normalized;

        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] == instances[controlledIndex])
            {
                continue;
            }
            Vector3 offset = instances[i].position - instances[controlledIndex].position;
            float angle = Vector3.Angle(offset, direction);
            
            
            if(!Physics.Raycast(instances[controlledIndex].position, offset, offset.magnitude, 1 << LayerMask.NameToLayer("Default")))
            {
                if (angle < closestAngle)
                {
                    closestIndex = i;
                    closestAngle = angle;
                }
            }


        }
        if (closestIndex != -1)
        {
            PlayerInstance merge = instances[closestIndex];
            PlayerInstance playerControlled = instances[controlledIndex];
            merge.Cyan |= playerControlled.Cyan;
            merge.Magenta |= playerControlled.Magenta;
            merge.Yellow |= playerControlled.Yellow;
            instances.RemoveAt(controlledIndex);
            playerMesh[controlledIndex].SetActive(false);
            playerMesh[closestIndex].SetActive(false);

            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == merge)
                {
                    controlledIndex = i;
                    break;
                }
            }
        }
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
