using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDamager : MonoBehaviour
{
    private bool attackCD = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !attackCD)
        {
            Debug.Log("Player Damaged");
            attackCD = true;
            other.GetComponent<PlayerControllerInput>().HP -= 50;
            StartCoroutine(AttackCooldown());
        }
    }

    public IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(3f);
        attackCD = false;

    }
}
