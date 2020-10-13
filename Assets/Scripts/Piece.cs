using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite BlackSprite;
    public Sprite WhiteSprite;
    
    private SpriteRenderer SR;

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

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
