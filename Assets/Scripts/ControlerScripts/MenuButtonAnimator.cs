using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButtonAnimator : MonoBehaviour {
    //Controle de movimento do botão
    public CursorState estadoDoBotao;         // Estado do botão.
    public Button esteBotao;                  // Referencia a este botão.
    private float cursorVelocity;             // Velocidade de movimento do botão.
    private Vector3 startPosition;            // Posição inicial do movimento do botao.
    private Vector3 newPosition;              // Posição final do movimento do botao.
    private int xPixelMove;                   // Quantidade de pixels do movimento.
    public bool forceInactive;                // Caso True, não executa a função deste botão.
    

    // Use this for initialization
    void Start () {
        cursorVelocity = 2;
        this.forceInactive = false;
    }
	
	// Update is called once per frame
	void Update () {
        switch (estadoDoBotao)
        {

            case CursorState.mover:
                this.startPosition = this.gameObject.transform.localPosition;
                this.newPosition = this.gameObject.transform.localPosition;
                this.newPosition.x = this.newPosition.x - xPixelMove;
                this.estadoDoBotao = CursorState.movendo;

                break;

            case CursorState.movendo:

                //Para o lerp ao final do movimento, arredondando os valores de comparação para um lerp mais rápido no final
                if (new Vector3((float)System.Math.Round(this.transform.localPosition.x, 2),
                                 (float)System.Math.Round(this.transform.localPosition.y, 2),
                                 (float)System.Math.Round(this.transform.localPosition.z, 2))
                    == new Vector3((float)System.Math.Round(this.newPosition.x, 2),
                                 (float)System.Math.Round(this.newPosition.y, 2),
                                 (float)System.Math.Round(this.newPosition.z, 2)))
                {
                    this.estadoDoBotao = CursorState.idle;
                }

                // Caso o movimento do passo atual não tenha terminado, move o personagem
                else
                {
                    float lerpFractionStep = (1 / this.cursorVelocity);
                    Vector3 lerpStep = Vector3.Lerp(this.startPosition, this.newPosition, lerpFractionStep);

                    this.gameObject.transform.localPosition = lerpStep;
                    this.startPosition = this.gameObject.transform.localPosition;
                }
                break;
        }
        
	} // </ Update>


    public void Move(int movimento)
    {
        if (movimento != 0 && !this.forceInactive)
        {
            this.xPixelMove = movimento;
            this.estadoDoBotao = CursorState.mover;
        }
    }

    public void executaBotao()
    {
        if (!this.forceInactive)
        {
            esteBotao.onClick.Invoke();
        }
    }
}
