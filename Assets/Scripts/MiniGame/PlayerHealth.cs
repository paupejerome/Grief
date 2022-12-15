using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private int health;
    [SerializeField] private int nbHearts;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;


    [SerializeField] private float invincibleTime;

    private int nbFlashs = 15;
    private float flashDuration;
    private bool isInvinsible = false;
    public bool isDead { get; private set; }

    Animator animator;

    private Player player;
    [SerializeField] private GameObject meshes;

    void Start()
    {
        animator = GetComponent<Animator>();
        health = nbHearts;
        player = GetComponent<Player>();
        flashDuration = invincibleTime / nbFlashs;
    }

    void Update()
    {
        if (health > nbHearts)
            health = nbHearts;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;


            if (i < nbHearts)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    public void TakeDamage()
    {
        if (!isInvinsible)
        {
            int rand = Random.Range(0, 4);
            MinigameSoundManager.GetInstance().PlaySFX(rand);

            health--;
            StartCoroutine(InvincibleForSeconds());

            if (health <= 0)
            {
                isDead = true;
                player.Death();
            }
            else
            {
                animator.SetTrigger("damage");
            }
        }
    }

    private IEnumerator InvincibleForSeconds()
    {
        if (health > 0)
        {
            isInvinsible = true;
            int temp = 0;

            while (temp < nbFlashs)
            {
                while (GameManager.GetInstance().isPaused)
                    yield return null;

                meshes.SetActive(false);
                yield return new WaitForSeconds(flashDuration);
                meshes.SetActive(true);
                yield return new WaitForSeconds(flashDuration);
                temp++;
            }

            isInvinsible = false;
        }
    }
}
