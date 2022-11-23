using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract void Pick();

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Pick();
        }
    }
}