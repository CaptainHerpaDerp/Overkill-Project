using GaiaElements;
using System.Collections;
using TeamColors;
using UnityEngine;


public class BlueSpecialBehaviour : SpecialBehaviour
{
    [SerializeField] private GameObject smokeGroupPrefab;
    [SerializeField] private float destroyTime;

    public override bool Activate()
    {
        if (onCooldown)
        {
            return false;
        }

        GameObject smokeGroup = Instantiate(smokeGroupPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyEffectGroup(smokeGroup));

        onCooldown = true;
        DoCooldown();

        return true;
    }

    private IEnumerator DestroyEffectGroup(GameObject effectGroup)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(effectGroup);
    }
}
