using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveTrigger : MonoBehaviour
{
    [SerializeField] DissolveEffect _dissolveEffect;


    // Start is called before the first frame update
    void Start()
    {
        _dissolveEffect = GetComponent<DissolveEffect>();
    }
}
