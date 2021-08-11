using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamExtensionMethods;
using UnityEngine.UI;

public class DestroyableUnit : TeamUnit
{
   
    protected float bHealthPoints;
    
    protected float healthPoints;

    [SerializeField] private Image lifeBar_Img;
    [SerializeField] private Image backLifeBar_Img;
    [SerializeField] private float animationBarSpeed;
    
    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        healthPoints = bHealthPoints;

        transform.position = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y, transform.position.z);
        GameManager.Instance.P_SoundManager.AudioSource = FindObjectOfType<AudioSource>();
      
    }

    public virtual void Restart()
    {
        Init();
    }
    public virtual void GetDamaged(float damage)
    {
        lifeBar_Img.gameObject.SetActive(true);
        backLifeBar_Img.gameObject.SetActive(true);
        healthPoints -= damage; 
        if (healthPoints <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        
    }
  
    public void UpdateLifeBar()
    {
        float actualHealth = healthPoints / bHealthPoints;

        if (backLifeBar_Img.fillAmount > actualHealth)
        {
           lifeBar_Img.fillAmount = actualHealth;
           backLifeBar_Img.color = Color.red;
           backLifeBar_Img.fillAmount  -= animationBarSpeed * Time.deltaTime;

        }
        else
        {
            backLifeBar_Img.fillAmount = actualHealth;
        }

        if (actualHealth > lifeBar_Img.fillAmount)
        {
            backLifeBar_Img.color = Color.green;
            backLifeBar_Img.fillAmount = actualHealth;
            lifeBar_Img.fillAmount  += animationBarSpeed * Time.deltaTime;
         
        }
        else
        {
            lifeBar_Img.fillAmount = actualHealth;
        }

    }
}
