using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamExtensionMethods;
using UnityEngine.UI;

public class DestroyableUnit : TeamUnit
{
    protected float bHealthPoints;
    public float BHealthPoints
    {
        get => bHealthPoints;
        set => bHealthPoints = value;
    }

    protected float healthPoints;
    public float HealthPoints 
    {
       get => healthPoints;
       set => healthPoints = value;
    }

    [SerializeField] private Image lifeBar_Img;
    public Image LifeBar_Img => lifeBar_Img;
    [SerializeField] private Image backLifeBar_Img;
    public Image BackLifeBar_Img => backLifeBar_Img;
    [SerializeField] private float animationBarSpeed;
    
    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        healthPoints = bHealthPoints;

        transform.position = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y, transform.position.z);
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
      
        if (healthPoints <= 0 && gameObject.activeSelf)
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
