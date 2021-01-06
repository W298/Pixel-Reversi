using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Grid parentGrid = null;
    
    public Sprite BlackSprite;
    public Sprite WhiteSprite;

    public Sprite[] FlipSprites;
    
    private SpriteRenderer SR;
    private float flipInterval = 0.03f;    // Higher is Slower

    public void SetColor(Grid.Status color)
    {
        Start();

        if (color == Grid.Status.Black)
        {
            SR.sprite = BlackSprite;
        }
        else if (color == Grid.Status.White)
        {
            SR.sprite = WhiteSprite;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetParentGrid(Grid parent)
    {
        parentGrid = parent;
    }

    public void FlipAnim(Grid.Status to)
    {
        if (to == Grid.Status.Black)
        {
            var curIndex = -1;
            
            StartCoroutine(Next());
            IEnumerator Next()
            {
                yield return new WaitForSeconds(flipInterval);

                curIndex++;
                SR.sprite = FlipSprites[curIndex];

                if (curIndex < 8)
                {
                    StartCoroutine(Next());
                }
            }
        }
        else
        {
            var curIndex = 9;
            
            StartCoroutine(Next());
            IEnumerator Next()
            {
                yield return new WaitForSeconds(flipInterval);

                curIndex--;
                SR.sprite = FlipSprites[curIndex];

                if (curIndex > 0)
                {
                    StartCoroutine(Next());
                }
            }
        }
    }
    
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        
    }
}
