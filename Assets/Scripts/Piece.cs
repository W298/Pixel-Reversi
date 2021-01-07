using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GridBox ParentGridBox = null;
    
    public Sprite BlackSprite;
    public Sprite WhiteSprite;

    public Sprite[] FlipSprites;
    
    private SpriteRenderer SR;
    private float flipInterval = 0.03f;    // Higher is Slower

    public bool isAnimating = false;

    public void SetColor(GridBox.Status color)
    {
        Start();

        if (color == GridBox.Status.Black)
        {
            SR.sprite = BlackSprite;
        }
        else if (color == GridBox.Status.White)
        {
            SR.sprite = WhiteSprite;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetParentGrid(GridBox parent)
    {
        ParentGridBox = parent;
    }

    public void FlipAnim(GridBox.Status to)
    {
        if (to == GridBox.Status.Black)
        {
            var curIndex = -1;

            isAnimating = true;
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

                isAnimating = false;
            }
        }
        else
        {
            var curIndex = 9;
            
            isAnimating = true;
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

                isAnimating = false;
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
