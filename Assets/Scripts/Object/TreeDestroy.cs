using UnityEngine;
using System.Collections;

public class TreeDestroy : MonoBehaviour {

    public GameObject[] stumps;
	
    public void TakeDamage()
    {
        GameObject stumpClone = Instantiate(stumps[Random.Range(0, stumps.Length)], transform.position, transform.rotation, gameObject.transform.parent) as GameObject;
        Destroy(gameObject);
    }
	
}
