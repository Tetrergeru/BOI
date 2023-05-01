using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    public TMPro.TextMeshPro Text;

    void Start()
    {
        StartCoroutine(Destroy());
    }

    void FixedUpdate()
    {
        var t = this.transform.position;
        this.transform.position = new Vector3(t.x, t.y + 0.03f, t.z);
        var s = this.transform.localScale.x - 0.003f;
        this.transform.localScale = new Vector3(s, s, s);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(this.gameObject);
    }
}
