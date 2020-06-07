using UnityEngine;

public class CellDoor : MonoBehaviour
{
    public bool connected = false;
    public BoxCollider2D backWall;
    public string Dir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BoxCollider2D>() != backWall)
        {
            if (collision.gameObject.tag == "BackWall")
                connected = true;
        }
    }
}