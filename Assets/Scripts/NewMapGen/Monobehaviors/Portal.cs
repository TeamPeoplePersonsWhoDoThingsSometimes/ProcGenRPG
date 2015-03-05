using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {

    public Direction dir;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            MasterDriver.Instance.moveArea(dir);
        }
    }

}
