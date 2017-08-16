using UnityEngine;
using System.Collections;

public class GlitchAnimator : MonoBehaviour {

    // Controle de sprites
    public GameObject[] sprites;                  // Lista de sprites relacionados ao glitch.
    public int quantDeSprites;                    // Quantidade de sprites usados na animação.
    
    // Controle de normalização
    private bool isNormalized;                    // Variavel de controle de normalização, trava o algoritmo para que não se repita mais de uma vez.

    // Controle de glitch
    private float nextSprite;                     // Tempo até a proxima troca de sprite.
    private int currentSpriteId;                  // ID do sprite atualmente exibido na tela.

    public GlitchType tipoDoGlitch;               // Variavel de controle do tipo de animação executada pelo animador

    // Use this for initialization
    void Start () {
        // inicialização das variaveis
        this.isNormalized = true;
        this.currentSpriteId = 0;
        this.sprites = new GameObject[quantDeSprites];

        //população da lista de sprites
        int id = 0;
        foreach (Transform child in this.transform)
        {
            if (id < quantDeSprites)
            {
                this.sprites[id] = child.gameObject;
                id++;
            }
            else
            {
                Debug.Log("Quantidade de itens é incompativel com MenuReactor");
                break;
            }
        } // </foreach>
    }
	
	// Update is called once per frame
	void Update () {
        if (GlitchAnimatorController.isWorldGlitched)
        {
            this.glitchSprite();
        }
        else
        {
            normalizeSprite();
        }
	}

    /**
    /* @name: normalizeSprite
    /* @version: 1.5
    /* @Description: Desativa todos os sprites menos o primeiro.
    */
    private void normalizeSprite()
    {
        if (!this.isNormalized)
        {
            switch (this.tipoDoGlitch)
            {
                case GlitchType.multiSprites:
                    for (int i = 0; i < this.quantDeSprites; i++)
                    {
                        if (i == 0)
                        {
                            this.sprites[i].SetActive(true);
                        }
                        else
                        {
                            this.sprites[i].SetActive(false);
                        }
                    }
                    break;

                case GlitchType.rotate180:
                    this.rotateSprite(180);
                    break;
            }

            this.currentSpriteId = 0;
            this.isNormalized = true;

        }// </if not normalized>
    }

    /**
    /* @name: glitchSprite
    /* @version: 1.4
    /* @Description: ativa e desativa sprites randomicos para criar efeito de glitch.
    */
    private void glitchSprite()
    {
        if (this.isNormalized)
        {
            this.isNormalized = false;
        }

        switch (this.tipoDoGlitch)
        {
            case GlitchType.multiSprites:
                if (this.nextSprite <= 0)
                {
                    this.changeSpriteTime();
                    this.changeSprite();
                }
                else
                {
                    this.nextSprite -= Time.deltaTime;
                }
                break;

            case GlitchType.rotate180:
                this.rotateSprite(0);
                break;
        }
    }


    /**
    /* @name: changeSpriteTime
    /* @version: 1.0
    /* @Description: Marca quanto tempo da proxima mudança de sprite.
    */
    private void changeSpriteTime()
    {
        int diceValue = Random.Range(0, 4);

        switch (diceValue)
        {
            case 0:
                this.nextSprite = 0.1f;
                break;

            case 1:
                this.nextSprite = 0.2f;
                break;

            case 2:
                this.nextSprite = 0.5f;
                break;

            case 3:
                this.nextSprite = 0.2f;
                break;

            default:
                this.nextSprite = 0.3f;
                break;
        }
    }

    /**
    /* @name: changeSprite
    /* @version: 1.0
    /* @Description: Muda o sprite ativo atual.
    */
    private void changeSprite()
    {
        // desativa sprite anterior
        this.sprites[this.currentSpriteId].SetActive(false);

        if (this.quantDeSprites > 0)
        {
            // rola novo valor para o sprite ativo
            currentSpriteId = Random.Range(1, this.quantDeSprites);

            // ativa novo sprite
            this.sprites[this.currentSpriteId].SetActive(true);
        }
    }

    /**
    /* @name: rotateSprite
    /* @version: 1.1
    /* @Description: rotaciona o sprite ativo atual.
    */
    private void rotateSprite(int angle)
    {
        this.transform.localRotation = new Quaternion(0, angle, 0,0);
    }
}
