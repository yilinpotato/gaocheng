using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;                     // �󶨵� Slider ���
    public string targetTag = "Player";       // �ɸ�Ϊ "Boss" �Ը���
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Ѫ������λ��

    private Entity targetEntity;
    private Camera mainCam;

    void Start()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            if (slider == null)
            {
                Debug.LogError("HealthBarUI δ���ҵ� Slider ���");
                enabled = false;
                return;
            }
        }

        mainCam = Camera.main;
        StartCoroutine(WaitAndBindTarget());
    }

    IEnumerator WaitAndBindTarget()
    {
        while (targetEntity == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target != null && target.activeInHierarchy)
            {
                targetEntity = target.GetComponent<Entity>();
                if (targetEntity != null)
                {
                    slider.maxValue = targetEntity.MaxHP;
                    slider.value = targetEntity.CurrentHP;
                    targetEntity.OnDamageTaken.AddListener(OnDamaged);
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Update()
    {
        if (targetEntity != null && slider != null)
        {
            slider.value = targetEntity.CurrentHP;
            transform.position = mainCam.WorldToScreenPoint(targetEntity.transform.position + offset);
        }
    }

    void OnDamaged(float damage)
    {
        if (targetEntity != null && slider != null)
        {
            slider.value = targetEntity.CurrentHP;
        }
    }
}
