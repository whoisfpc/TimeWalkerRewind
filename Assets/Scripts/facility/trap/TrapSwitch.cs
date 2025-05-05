using UnityEngine;
using System.Collections;

public class TrapSwitch : MonoBehaviour
{
    public GameObject trapChest;
    private TrapChest trapChestCtrl;

    // Use this for initialization
    void Start()
    {
        trapChestCtrl = trapChest.GetComponent<TrapChest>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.gameObject.tag == "Player") {
			trapChestCtrl.TriggerTrap ();
		}
    }
}
