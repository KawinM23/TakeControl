using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private BoxCollider2D _bounds;

    private void OnEnable()
    {
        StartAnimate();
    }

    private void StartAnimate()
    {
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        var oriPos = transform.position;
        var oriScale = transform.localScale;

        var dx = _bounds.bounds.max.x - _bounds.bounds.min.x;
        var dy = _bounds.bounds.max.y - _bounds.bounds.min.y;
        transform.position = new Vector3(_bounds.bounds.min.x, _bounds.bounds.center.y, oriPos.z);
        transform.localScale = new Vector3(oriScale.x, dy, oriScale.z);

        var di = _speed * Time.fixedDeltaTime;
        for (float i = 0; i < dx; i += di)
        {
            transform.position += new Vector3(di, 0, 0);
            yield return new WaitForFixedUpdate();
        }

        transform.position = oriPos;
        transform.localScale = oriScale;
        gameObject.SetActive(false);
    }
}
