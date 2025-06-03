
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 30000f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
