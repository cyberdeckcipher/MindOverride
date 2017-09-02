using UnityEngine;

public class GroundOverlayReactor : MonoBehaviour
{
    //Posição fixa
    private Vector3 fixedPosition;            // Controle de posição do tile para posição fixa.
    private Quaternion fixedRotation;         // Controle de rotação do tile para posição fixa.
    private Vector3 startPosition;            // Posição inicial do movimento do tile.
    private Vector3 newPosition;              // Posição final do movimento do tile.

    //Controle do Tile
    public ponto2D wargrid;                   // Posição no wargrid.
    public ponto2D poolPosition;              // Posição original na fila de Tiles.
    public float tileVelocity;                // Controle de velocidade utilizado no Lerp do tile.

    //Propriedades do Tile
    public TileStates estado;                 // Estado de controle de posição.
    private bool chanceColor;                 // Variavel que garante que a cor de um tile será trocada apenas uma vez, por troca.
    private float alpha;                      // valor entre 0 e 1 que controla transparencia do tile

    //Colisão com unidades de um mesmo jogador
    public GameObject aliadoColidido;         // Caso o tile esteja em contato com uma unidade do jogador atual, o objeto estará referenciado nesta variavel.
    public GameObject inimigoColidido;        // Caso o tile esteja em contato com uma unidade que não é do jogador, o objeto estará referenciado nesta variavel.
    public GameObject interactColidido;       // Caso o tile esteja em contato com um objeto interagivel, o objeto estará referenciado nesta variavel.

    //DEBUG
    public int x;                             // x da posição atual
    public int y;                             // y da posição atual

    // Use this for initialization
    void Start()
    {
        this.estado = TileStates.idle;
        this.tileVelocity = 3; // 600;
        this.alpha = 0.5f;

        // fixa posição inicial
        updateFixedPosition();

        //desabilita detecção de colisões
        this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    }

    // Update is called once per frame
    void Update()
    {

        // Ativa o movimento de um Tile, capturando as flags descer e subir.
        switch (this.estado)
        {
            case TileStates.descer:
                this.startPosition = this.gameObject.transform.position;
                this.removeColisionRef();
                this.estado = TileStates.descendo;
                break;

            case TileStates.subir:
                this.startPosition = this.gameObject.transform.position;
                this.atualizaColisionRefComUnidades();
                this.estado = TileStates.subindo;
                break;

            case TileStates.inativo:
                this.setObjectColor(TileColors.invisivel);
                this.fixedPosition = tilePoolCoord(this.poolPosition);
                this.estado = TileStates.idle;
                break;

            case TileStates.ativo:
                this.x = this.wargrid.x;
                this.y = this.wargrid.y;
                fixPosition();
                break;
        }

        //Movimenta o Tile caso esteja em um estado de movimento.
        if (this.estado == TileStates.subindo 
         || this.estado == TileStates.descendo)
        {
            if (this.chanceColor)
            {
                this.chanceColor = false;
                if (GameController.isObstacle(this.wargrid, ActionsTypes.Comum) 
                    || GameController.controller.acaoEscolhida == playersActions.Atacar)
                {
                    this.setObjectColor(TileColors.obstaculo);
                }
            }
            
            //Detecta final do movimento e atualiza estado do tile
            if (this.transform.position == this.newPosition)
            {
                // Caso esteja subindo
                if (this.estado == TileStates.subindo)
                {
                    this.estado = TileStates.ativo;

                    updateFixedPosition();
                }

                // Caso não esteja subindo
                else
                {
                    this.estado = TileStates.inativo;

                    this.setObjectColor(TileColors.invisivel);
                    this.chanceColor = true;
                }
            }
            else
            {
                // Movimento usando lerp
                float lerpVelocity = 1 / (this.tileVelocity);
                this.transform.position = Vector3.Lerp(startPosition, newPosition, lerpVelocity);
                this.startPosition = this.transform.position;
                
            }
        }

    }

    /**
    /* @name: setObjectColor
    /* @version: 2.5
    /* @Description: seta uma nova cor para o color lerp interpolar.
    */
    public void setObjectColor(TileColors cor)
    {
        switch (cor)
        {
            case TileColors.andavel:
                this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, alpha); //Color.blue;
                break;
            case TileColors.interagir:
                this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, alpha); //Color.green;
                break;
            case TileColors.invisivel:
                this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, alpha); //Color.clear;
                break;
            case TileColors.obstaculo:
                this.gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, alpha); //Color.red;
                break;
        }
    }

    /**
    /* @name: levantar
    /* @version: 1.0
    /* @Description: levanta um tile em uma determinada posição, ativando a animação.
    */
    public void levantar(ponto2D ponto, Vector3 position, Vector3 levantarStartPosition, TileColors cor)
    {
        this.wargrid = ponto;
        this.newPosition = position;
        this.gameObject.transform.position = levantarStartPosition;

        this.setObjectColor(cor);

        this.estado = TileStates.subir;

    }

    /**
    /* @name: abaixar
    /* @version: 1.1
    /* @Description: Abaixa um determinado Tile, ativando a animação.
    */
    public void abaixar(ponto2D ponto, Vector3 position)
    {
        this.newPosition = position;

        this.estado = TileStates.descer;
    }

    /**
    /* @name: tileEstaAcima
    /* @version: 1.0
    /* @Description: True caso o Tile esteja subindo ou ativo
    */
    public bool tileEstaAcima()
    {
        return this.estado == TileStates.subindo 
            || this.estado == TileStates.subir 
            || this.estado == TileStates.ativo 
            || this.estado == TileStates.obstaculo;
    }

    /**
    /* @name: atualizaColisionRefComUnidades
    /* @version: 1.7
    /* @Description: recupera objetos do wargrid e atualiza valores de aliado e inimigo
    */
    private void atualizaColisionRefComUnidades()
    {
        if (GameController.getObjectfromGrid(this.wargrid) != null)
        {
            GameObject objNaMesmaPosicao = GameController.getObjectfromGrid(this.wargrid);

            if (GameController.isPlayerTag(objNaMesmaPosicao.tag))
            {
                if (objNaMesmaPosicao.CompareTag(PlayersTags.Jogador.ToString()))
                {
                    this.aliadoColidido = objNaMesmaPosicao;
                    this.aliadoColidido.GetComponent<GenericCharacter>().colidingTile = this;
                    setObjectColor(TileColors.obstaculo);
                }
                else
                {
                    this.inimigoColidido = objNaMesmaPosicao;
                    this.inimigoColidido.GetComponent<GenericCharacter>().colidingTile = this;
                    setObjectColor(TileColors.obstaculo);
                }
            }

            if (objNaMesmaPosicao.GetComponent<InteractActor>() != null
                && GameController.controller.acaoEscolhida == playersActions.Interagir)
            {
                objNaMesmaPosicao.GetComponent<InteractActor>().colidingTile = this;
                this.interactColidido = objNaMesmaPosicao;
                setObjectColor(TileColors.interagir);
            }
        } // </ object from grid != null>

    }

    /**
    /* @name: removeColisionRef
    /* @version: 1.1
    /* @Description: Remove referencia ao tile dos personagens colididos
    */
    private void removeColisionRef()
    {
        if (this.aliadoColidido != null)
        {
            this.aliadoColidido.GetComponent<GenericCharacter>().colidingTile = null;
        }
        else if (this.inimigoColidido != null)
        {
            this.inimigoColidido.GetComponent<GenericCharacter>().colidingTile = null;
        }
        else if(this.interactColidido != null)
        {
            this.interactColidido.GetComponent<InteractActor>().colidingTile = null;
        }

        this.aliadoColidido = null;
        this.inimigoColidido = null;
        this.interactColidido = null;
    }

    /**
    /* @name: tilePoolCoord
    /* @version: 1.0
    /* @Description: calcula a posição espacial de um objeto dado sua posição no Tilepool, mas com altura abaixo do solo, usado somente para abaixar o tile
    */
    private Vector3 tilePoolCoord(ponto2D position)
    {
        return new Vector3(((this.transform.localScale.x) * position.x),
                            GameController.getyValueForTilePool(),
                          -((this.transform.localScale.z) * position.y)
                          );
    }

    /**
    /* @name: updateFixedPosition
    /* @version: 1.0
    /* @Description: atualiza valores de posição fixa deste objeto.
    */
    private void updateFixedPosition()
    {
        this.fixedPosition = this.transform.position;
        this.fixedRotation = this.transform.rotation;
    }

    /**
    /* @name: fixPosition
    /* @version: 1.0
    /* @Description: fixa valores de posição e rotação.
    */
    private void fixPosition()
    {
        this.transform.position = this.fixedPosition;
        this.transform.rotation = this.fixedRotation;
    }

    /**
    /* @name: temAliado
    /* @version: 1.0
    /* @Description: True caso exista um aliado neste tile
    */
    public bool temAliado()
    {
        return (this.aliadoColidido != null);
    }

    /**
    /* @name: getAliado
    /* @version: 1.0
    /* @Description: Retorna o aliado colidido com este tile
    */
    public GameObject getAliado()
    {
        return this.aliadoColidido;
    }

    /**
    /* @name: temInimigo
    /* @version: 1.0
    /* @Description: True caso exista um inimigo neste tile
    */
    public bool temInimigo()
    {
        return (this.inimigoColidido != null);
    }

    /**
    /* @name: getInimigo
    /* @version: 1.0
    /* @Description: Retorna o inimigo colidido com o Tile
    */
    public GameObject getInimigo()
    {
        return this.inimigoColidido;
    }

    /**
    /* @name: temInteractActor
    /* @version: 1.0
    /* @Description: True caso exista uma interação neste tile
    */
    public bool temInteracao()
    {
        return (this.interactColidido != null);
    }

    /**
    /* @name: getInteracao
    /* @version: 1.0
    /* @Description: Retorna a interação colidido com o Tile
    */
    public GameObject getInteracao()
    {
        return this.interactColidido;
    }
}
