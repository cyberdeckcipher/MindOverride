using UnityEngine;
using System.Collections;

public class CursorReactor : MonoBehaviour {

    public static CursorReactor cursor;           // Variavel estática de acesso ao singleton do cursor

    // Caracteristicas do objeto
    private Vector3 fixedPosition;                // Controle de posição do Cursor para uma posição fixa.
    private Quaternion fixedRotation;             // Controle de rotação do Cursor para uma posição fixa.

    // Posicionamento e movimentação
    private ponto2D posicaoAtual;                 // Posição atual do Cursor no Grid de combate.
    public CursorState estadoCursor;              // Estado do cursor.
    public float axisLimit;                       // Valor minimo que um Axis deve ter para ser capturado pelo jogo.
    public float cursorVelocity;                  // Velocidade do Lerp.
    private float moveTimeElapsed;                // Tempo que se passou desde o inicio do movimento.
    private Vector3 startPosition;                // Posição inicial do Lerp.
    private Vector3 newPosition;                  // Posição final do Lerp.

    // Colisão com objetos na cena
    public GameObject objInWargridPosition;       // Objeto que está no wargrid

    //Debug
    public int x;                                 // Valor X da posição do personagem no Wargrid para debug.
    public int y;                                 // Valor Y da posição do personagem no Wargrid para debug.
    

    // Use this for initialization
    void Start () {

        CursorReactor.cursor = this;

        // Localização e cor
        updateTransformStandards(this.transform.position, this.transform.rotation);
        
        // Estado e controle
        this.estadoCursor = CursorState.idle;

        // Movimentação
        this.axisLimit = 0.2f;
        this.cursorVelocity = 0.165f;

    }
	
	// Update is called once per frame
	void Update () {

        this.objInWargridPosition = GameController.getObjectfromGrid(posicaoAtual);
        this.x = posicaoAtual.x;
        this.y = posicaoAtual.y;

        if (GameController.getEstadoDoJogo() == Gamestates.playing && 
            GameController.controller.actionflag == Actionflag.idle)
        {
            
            if (this.estadoCursor == CursorState.idle &&
                (GameController.controller.estadoJogador != PlayersState.acao &&
                GameController.controller.estadoJogador != PlayersState.agindo))
            {
                this.fixedTransform();

                //Captura entrada do axis horizontal
                float axisHORIZONTALInput = Input.GetAxis("Horizontal");
                float axisVERTICALInput = Input.GetAxis("Vertical");
                /*
                float axisHORIZONTAL2Input = Input.GetAxis("Horizontal2");
                float axisVERTICAL2Input = Input.GetAxis("Vertical2");
                */
                ponto2D proximaPosicao;

                // O cursor só se move se a camera estiver parada, para evitar movimentos fantasmas.
                if (CameraControl.estadoDaCamera == CameraState.idle)
                {
                    // Caso o axis Horizontal seja negativo, o cursor deve se mover para a esquerda
                    if (axisHORIZONTALInput < -this.getAxisLimit()/*||
                        axisHORIZONTAL2Input < -this.getAxisLimit()*/)
                    {

                        proximaPosicao = (new ponto2D(this.posicaoAtual.x - 1, this.posicaoAtual.y));
                        moverCursorPara(proximaPosicao);

                    }
                    // Caso o axis Horizontal seja positivo, o cursor deve se mover para a direita
                    else if (axisHORIZONTALInput > this.getAxisLimit()/* ||
                             axisHORIZONTAL2Input > this.getAxisLimit()*/)
                    {

                        proximaPosicao = (new ponto2D(this.posicaoAtual.x + 1, this.posicaoAtual.y));
                        moverCursorPara(proximaPosicao);

                    }
                    else
                    {
                        // caso o axis Vertical seja negativo, o cursor deve se mover para bvaixo
                        if (axisVERTICALInput < -this.getAxisLimit()/* ||
                            axisVERTICAL2Input < -this.getAxisLimit()*/)
                        {
                            proximaPosicao = (new ponto2D(this.posicaoAtual.x, this.posicaoAtual.y - 1));
                            moverCursorPara(proximaPosicao);
                        }

                        // Caso o axis Vertical seja positivo, o cursor deve se mover para cima
                        else if (axisVERTICALInput > this.getAxisLimit()/*||
                                 axisVERTICAL2Input > this.getAxisLimit()*/)
                        {
                            proximaPosicao = (new ponto2D(this.posicaoAtual.x, this.posicaoAtual.y + 1));
                            moverCursorPara(proximaPosicao);
                        }
                    }
                } // </ Camera idle>
                
            } //</ Cursor idle>

            if (this.estadoCursor == CursorState.mover)
            {
                this.startPosition = this.transform.position;
                this.estadoCursor = CursorState.movendo;
                this.moveTimeElapsed = 0.0001f;
            } //</Cursor mover>

            if (this.estadoCursor == CursorState.movendo)
            {
                // Enquanto a posição do cursor for diferente da posição alvo, o lerp age
                if (!(this.transform.position == this.newPosition))
                {
                    // Calcula o passo a ser aplicado.
                    this.moveTimeElapsed += Time.deltaTime;
                    float lerpFractionStep = (this.moveTimeElapsed / this.cursorVelocity);

                    // Aplica o passo do lerp.
                    this.transform.position = Vector3.Lerp(this.startPosition, this.newPosition, lerpFractionStep);
                    
                    // Atualiza os valores padrões de rotação e posição da camera.
                    updateTransformStandards(this.transform.position, this.transform.rotation);
                }

                // Ao terminar o movimento o Cursor volta ao estado Idle e atualiza a posição e rotação
                else
                {
                    // Ao final do movimento, retorna o cursor ao estado Idle
                    this.estadoCursor = CursorState.idle;
                }

            } //</ cursor movendo>

        } // </ is game playins>
    }

    /**
    /* @name: getTileinPosition
    /* @version: 2.5
    /* @Description: Retorna o tile na posição do cursor Ou null caso contrário
    */
    public GameObject getTileinPosition()
    {
        // Se existe um objeto na posicao atual do cursor
        if (this.objInWargridPosition != null)
        {
            // E esse objeto for um Tile
            if (this.objInWargridPosition.CompareTag("GroundOverlayTile"))
            {
                //Retorna o tile
                return this.objInWargridPosition;
            }
            // Caso seja uma unidade de um jogador
            else if (GameController.isPlayerTag(this.objInWargridPosition.tag))
            {
                // E ele tenha um tile armazenado
                if (this.objInWargridPosition.GetComponent<GenericCharacter>().colidingTile != null)
                {
                    // Retorna o tile contido no Generic Character
                    return this.objInWargridPosition.GetComponent<GenericCharacter>().colidingTile.gameObject;
                }
            }
            // Caso seja um objeto de interacao
            else if (this.objInWargridPosition.GetComponent<InteractActor>() != null)
            {
                if (this.objInWargridPosition.GetComponent<InteractActor>().colidingTile != null)
                {
                    // Retorna o tile contido no Interact Actor
                    return this.objInWargridPosition.GetComponent<InteractActor>().colidingTile.gameObject;
                }
            }
        }
        // Caso contrario retorna null
        return null;
    }

    /**
    /* @name: getAliadoinPosition
    /* @version: 1.1
    /* @Description: Retorna o aliado na posição do cursor Ou null caso contrário
    */
    public GameObject getAliadoinPosition()
    {
        if (this.objInWargridPosition != null)
        {
            if (GameController.isPlayerTag(this.objInWargridPosition.tag))
            {
                if (this.objInWargridPosition.CompareTag(PlayersTags.Jogador.ToString()))
                {
                    return this.objInWargridPosition;
                }
            }
        }
        return null;
    }

    /**
    /* @name: getEnemyinPosition
    /* @version: 1.1
    /* @Description: Retorna o Inimigo na posição do cursor Ou null caso contrário
    */
    public GameObject getEnemyinPosition()
    {
        if (this.objInWargridPosition != null)
        {
            if (GameController.isPlayerTag(this.objInWargridPosition.tag))
            {
                if (!this.objInWargridPosition.CompareTag(PlayersTags.Jogador.ToString()))
                {
                    return this.objInWargridPosition;
                }
            }
        }
            
        return null;
    }

    /**
    /* @name: getInteractinPosition
    /* @version: 1.0
    /* @Description: Retorna o Interact Actor na posição do cursor Ou null caso contrário
    */
    public GameObject getInteractinPosition()
    {
        if (this.objInWargridPosition != null)
        {
            if (this.objInWargridPosition.CompareTag("interativos"))
            {
                return this.objInWargridPosition;
            }
        }

        return null;
    }

    /**
    /* @name: setPosition
    /* @version: 1.0
    /* @Description: Seta posição do cursor
    */
    public void setPosition( ponto2D novaPosicao)
    {
        this.posicaoAtual = novaPosicao;
    }

    /**
    /* @name: getPosition
    /* @version: 1.0
    /* @Description: Retorna posição do cursor
    */
    public ponto2D getPosition()
    {
        return this.posicaoAtual;
    }

    /**
    /* @name: updateTransformStandards
    /* @version: 1.0
    /* @Description: Atualiza a posição e rotação padrão do cursor
    */
    private void updateTransformStandards(Vector3 novaPosicao, Quaternion novaRotacao)
    {
        this.fixedPosition = novaPosicao;
        this.fixedRotation = novaRotacao;
    }

    /**
    /* @name: fixedTransform
    /* @version: 1.0
    /* @Description: Aplica posição e rotação padrão ao cursor
    */
    private void fixedTransform()
    {
        this.transform.position = this.fixedPosition;
        this.transform.rotation = this.fixedRotation;
    }

    /**
    /* @name: moverCursorPara
    /* @version: 1.5
    /* @Description: Move o cursor para uma nova posição
    */
    public void moverCursorPara( ponto2D novaPosicao)
    {
        if (   novaPosicao.x < GameController.controller.mapSizeX
            && novaPosicao.x >= 0
            && novaPosicao.y < GameController.controller.mapSizeY
            && novaPosicao.y >= 0)
        {
            setPosition(novaPosicao);
            this.estadoCursor = CursorState.mover;
            this.newPosition = GameController.wargridToPosition(novaPosicao, this.gameObject);
        }
    }

    /**
    /* @name: setCursorPosition
    /* @version: 1.0
    /* @Description: Seta o cursor em uma determinada posição
    */
    public void setCursorPosition( ponto2D posicao)
    {
        this.posicaoAtual = posicao;
        updateTransformStandards(GameController.wargridToPosition(posicao, this.gameObject), transform.rotation);
    }

    /**
    /* @name: getAxisLimit
    /* @version: 1.0
    /* @Description: retorna o minimo que um Axis deve ter para ser capturado pelo jogo.
    */
    public float getAxisLimit()
    {
        return this.axisLimit;
    }
}
