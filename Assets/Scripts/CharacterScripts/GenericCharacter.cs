using UnityEngine;
using System.Collections.Generic;

public class GenericCharacter : MonoBehaviour {

    // Caracteristicas do objeto
    private Vector3 fixedPosition;                // Controle de posição do personagem para uma posição fixa.
    private Quaternion fixedRotation;             // Controle de rotação do personagem para uma posição fixa.
    private Color corOriginal;                    // Cor original do personagem.
    public SkinnedMeshRenderer objectRender;      // Render do objeto do personagem para mudança de cor.
    public Shader silhuete;                       // Shader a ser aplicado quando a unidade estiver selecionado
    public Shader standard;                       // Shader a ser aplicado quando a unidade estiver idle

    // Estado do personagem
    public CharacterState estadoAtual;            // Estado do personagem
    public bool podeAgir;                         // Variavel que controla se o personagem ainda pode executar ações.
    public AreaOfActionState estadoArea;          // Estado de criação da area de ação, controla se a area já foi criada.
    public List<GroundOverlayReactor> areaDeAcao; // Lista de tiles que formam a area de ação do personagem.
    private ponto2D oldgridPosition;              // Container que contem a ultima posição no wargrid, para atualização de posição de objeto no controller.wargrid
    public ponto2D gridPosition;                  // Posição do personagem na matrix de jogo.
    public GroundOverlayReactor colidingTile;     // Ao criar uma area, esta variavel guarda o tile que está na mesma posição do personagem.

    // Variaveis de movimentação
    private float charVelocity;                   // velocidade de movimento do personagem.
    private float moveTimeElapsed;                // Quatidade de tempo que se passou desde o inicio da animação de movimento.
    private Path caminho;                         // Caminho passado para este personagem seguir.
    private Vector3 startPosition;                // Posição inicial do movimento do personagem.
    private Vector3 newPosition;                  // Posição final do movimento do personagem.
    public bool atualizaWargrid;                  // True caso ação de andar tenha sido iniciada.
    private ponto2D ultimoPasso;                  // contem ultimo passo executado pela função de movimento para que o personagem possa girar de acordo.

    // Ficha de personagem
    public PlayersTags jogador;                                 // Dono desta unidade.
    public AttackType tipoDeAtaque;                             // Tipo de ataque que esta unidade executa.
    public int walk, life, dano, alcance, minalcance, acoes;    // Velocidade, Pontos de vida, Força, Alcance de ataque, Alcance minimo, numero de ações por turno.
    public int MAXwalk, MAXlife, MAXacoes;                      // Velicadade máxima, Pontos de vida máximos, numero máximo de ações por turno.

    // controle de Animações
    private bool isCripled;                                     // True caso a unidade tenha sido reduzida a menos da metade de seu life original.
    public Animator thisAnimator;                               // Referencia ao animador desta unidade.
    private int originalIdle;                                   // Valor original do Idle, caso recupere o life.
    
    // IA
    public bool enemySpoted;                                    // True caso algum alvo tenha passado em seu campo de visão.
    public int iaType;                                          // Tipo de IA deste personagem. 0 = jogador. 1= agressive. 2= passive

    //Debug
    public int x;                                               // Valor X da posição do personagem no Wargrid para debug.
    public int y;                                               // Valor Y da posição do personagem no Wargrid para debug.

    void Awake()
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                this.objectRender = child.gameObject.GetComponent<SkinnedMeshRenderer>();
                break;
            }
        }
    }

    // Use this for initialization
    void Start () {
        
        this.areaDeAcao = new List<GroundOverlayReactor>();
        this.estadoArea = AreaOfActionState.undone;
        this.podeAgir = true;
        this.estadoAtual = CharacterState.idle;
        
        this.corOriginal = this.objectRender.material.color;

        this.colidingTile = null;
        this.isCripled = false;

        this.oldgridPosition = this.gridPosition;

        //Iniciação de variaveis de movimento.
        this.charVelocity = 0.8f;
        this.startPosition = this.transform.position;
        this.originalIdle = thisAnimator.GetInteger("idleType");

        //Iniciação de variaveis de IA
        enemySpoted = false;

        //atualiza valores padrões de posição e rotação
        updateTransformStandards(this.transform.position, this.transform.rotation);
    }
	
	// Update is called once per frame
	void Update () {

        if (this.estadoAtual != CharacterState.dead)
        {

            // atualiza animações baseando-se no life:
            if (this.life <= this.MAXlife / 2)
            {
                this.thisAnimator.SetInteger("idleType", 4);
                this.isCripled = true;

            }
            else if (this.isCripled)
            {
                this.isCripled = false;
                this.thisAnimator.SetInteger("idleType", this.originalIdle);
            }

            // Personagens se movem apenas se estado do jogo for playing
            if (GameController.getEstadoDoJogo() == Gamestates.playing)
            {
                //DEBUG
                this.x = this.gridPosition.x;
                this.y = this.gridPosition.y;

                // Se o personagem estiver em idle
                if (this.estadoAtual == CharacterState.idle)
                {
                    // Assegura-se que ele não se mova
                    fixedTransform();
                    atualizaControllerWargrid();
                    //this.objectRender.material.shader = this.standard;
                }
                else
                {

                    //this.objectRender.material.shader = this.silhuete;
                }

                // --------------- detecta se o personagem está se movendo ------------------
                if (this.estadoAtual == CharacterState.movendo)
                {

                    //Para o lerp ao final do movimento, arredondando os valores de comparação para um lerp mais rápido no final
                    if (new Vector3((float)System.Math.Round(this.transform.position.x, 2),
                                     (float)System.Math.Round(this.transform.position.y, 2),
                                     (float)System.Math.Round(this.transform.position.z, 2))
                        == new Vector3((float)System.Math.Round(this.newPosition.x, 2),
                                     (float)System.Math.Round(this.newPosition.y, 2),
                                     (float)System.Math.Round(this.newPosition.z, 2)))
                    {
                        updateTransformStandards(this.transform.position, this.transform.rotation);
                        this.estadoAtual = CharacterState.mover;

                        this.moveTimeElapsed = 0.0001f;
                        this.startPosition = this.gameObject.transform.position;

                        if (this.iaType != 0)
                        {
                            caminhaIA();
                        }
                        else
                        {
                            caminha();
                        }
                    }
                    // Caso o movimento do passo atual não tenha terminado, move o personagem
                    else
                    {
                        this.moveTimeElapsed += Time.deltaTime;

                        float lerpFractionStep = (this.moveTimeElapsed / this.charVelocity);

                        if (moveTimeElapsed > this.charVelocity)
                        {
                            moveTimeElapsed = this.charVelocity;
                        }

                        Vector3 lerpStep = Vector3.Lerp(this.startPosition, this.newPosition, lerpFractionStep);

                        this.gameObject.transform.position = lerpStep;
                    }
                }

                // --------------- /detecta se o personagem está se movendo ------------------

                //pinta a unidade de cinza se não puder mais agir
                if (!this.aindaPodeAgir())
                {
                    this.objectRender.material.color = Color.grey;
                }
                else
                {
                    //this.gameObject.GetComponent<MeshRenderer>().material.color = this.corOriginal;
                }

            } // </ Estado do jogo = Jogando>
        }
        
    } // </ UPDATE>

    // ---------------------------------------------------------------- Public functions ----------------------------------------------------------------

    /**
     /* @name: setUnity
     /* @version: 1.0
     /* @Description: Customiza dados da unidade. 
    */
    public void setUnity(int max_acoes, int max_life, int max_walk, int novo_alcance, int novo_minalcance, int novo_dano, int tipo_IA)
    {
        //criando personagem;
        this.MAXacoes = max_acoes;         // numero de ações máximas que a unidade pode executar
        this.acoes = max_acoes;            // numero de ações de um pesonagem

        this.MAXlife = max_life;           // life máximo que a unidade pode alcançar
        this.life = max_life;              // quantidade de vida do personagem

        this.MAXwalk = max_walk;           // Numero Maximo de passos que uma unidade pode dar
        this.walk = max_walk;              // quantidade de passos que o personagem pode dar

        this.dano = novo_dano;             // dano que o personagem pode causar no inimigo
        this.alcance = novo_alcance;       // alcance do ataque
        this.minalcance = novo_minalcance; // alcance minimo do ataque

        this.iaType = tipo_IA;             // tipo de IA. [0 = jogador]

        this.restPodeAgir();
    }

    /**
    /* @name: andarPara
    /* @version: 1.6
    /* @Description: Recebe um caminho e ativa uma função para andar passo a passo até o fim da pilha de posições.
    */
    public void andarPara(Path novoCaminho)
    {
        this.ultimoPasso = this.gridPosition;

        //Garante que o personagem está num estado de idle antes de andar
        if (this.isIdle())
        {
            //Atualiza valor da posição oritinal para mover objeto no controller.wargrid
            this.oldgridPosition = this.gridPosition;
            this.atualizaWargrid = true;

            // muda o estado do personagem para movendo
            this.caminho = novoCaminho;
            caminha();
        }
        
    }

    public bool isIdle() {
        return this.estadoAtual == CharacterState.idle || this.estadoAtual == CharacterState.selecionado;
    }

    public bool isMoving() {
        return this.estadoAtual == CharacterState.movendo;
    }

    /**
    /* @name: terminaAcao
    /* @version: 1.0
    /* @Description: Ao terminar a ação o numero de ações máximas da unidade é diminuido. Caso esteja sem ações, marca como não podendo mais agir
    */
    public void terminaAcaoDoPersonagem()
    {
        this.acoes--;
        if (this.acoes <= 0)
        {
            this.estadoAtual = CharacterState.idle;
            this.podeAgir = false;
        }
        else
        {
            this.estadoAtual = CharacterState.selecionado;
        }
    }

    /**
    /* @name: terminouAcaoDeMovimento
    /* @version: 2.0
    /* @Description: Ao terminar a ação o numero de ações máximas da unidade é diminuido. Caso esteja sem ações, marca como não podendo mais agir
    */
    private void terminouAcaoDeMovimento()
    {
        this.terminaAcaoDoPersonagem();

        if (this.atualizaWargrid)
        {
            atualizaControllerWargrid();
            this.atualizaWargrid = false;
        }

        GameController.actionFlagEnd(); // TODO: termino de ação em casos de cutscene deve ser realizado pelo cutscene handler.
    }

    /**
    /* @name: force_terminarAcao
    /* @version: 1.0
    /* @Description: Ao terminar a ação o numero de ações máximas da unidade é diminuido. Caso esteja sem ações, marca como não podendo mais agir
    */
    public void force_terminarAcao()
    {
        this.terminouAcaoDeMovimento();
    }

    /**
    /* @name: restPodeAgir
    /* @version: 1.0
    /* @Description: Muda a ação escolhida pelo jogador para a ação de ataque
    */
    public void restPodeAgir()
    {

        //this.gameObject.GetComponent<MeshRenderer>().material.color = this.corOriginal; DEPRECATED
        this.objectRender.material.color = this.corOriginal;
        
        this.areaDeAcao = new List<GroundOverlayReactor>();
        this.podeAgir = true;
        this.walk = this.MAXwalk;
        this.acoes = this.MAXacoes;
    }

    /**
    /* @name: restPodeAgir
    /* @version: 1.0
    /* @Description: Muda a ação escolhida pelo jogador para a ação de ataque
    */
    public bool aindaPodeAgir()
    {
        return this.podeAgir;
    }

    /**
    /* @name: getVelocity
    /* @version: 1.0
    /* @Description: Retorna o valor de velocidade de movimento do personagem.
    */
    public float getVelocity()
    {
        return this.charVelocity;
    }

    /**
    /* @name: atacado
    /* @version: 2.1
    /* @Description: Função que recebe e aplica o dano de um ataque.
    */
    public void atacado(int dano)
    {
        this.life -= dano;
        
        if (this.life <= 0)
        {
            GameController.delObjectatGrid(this.gridPosition);
            this.gameObject.GetComponent<Animator>().SetTrigger("Death");
            this.gameObject.tag = "Ground";
            this.estadoAtual = CharacterState.dead;
        }
        else
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Damage");
        }
    }

    // ---------------------------------------------------------------- Private functions ----------------------------------------------------------------

    /**
    /* @name: caminha
    /* @version: 1.0
    /* @Description: Atualiza o proximo passo do personagem e seta a flag de movimento do objeto.
    */
    private void caminha()
    {
        ponto2D proximoPasso = this.caminho.nextStep();
        
        // Se o proximo passo não for vazio, executa o passo
        if (proximoPasso != null)
        {
            estadoAtual = CharacterState.movendo;
            this.gridPosition = proximoPasso;
            this.newPosition = GameController.wargridToPosition(proximoPasso, this.gameObject);
            this.gameObject.GetComponent<Animator>().SetBool("onWalking", true);

            giraPersonagem(this.ultimoPasso, proximoPasso);
            this.ultimoPasso = proximoPasso;
        }

        // Caso o caminho tenha terminado, a ação termina
        else
        {
            updateTransformStandards(this.transform.position, this.transform.rotation);
            this.caminho = null;
            this.gameObject.GetComponent<Animator>().SetBool("onWalking", false);
            terminouAcaoDeMovimento();
        }
    }

    /**
    /* @name: atualizaControllerWargrid
    /* @version: 1.2
    /* @Description: Atualiza posição do game object no controller.wargrid
    */
    private void atualizaControllerWargrid()
    {
        
        if (this.oldgridPosition != this.gridPosition)
        {
            GameController.setObjectatGrid(this.gameObject, this.gridPosition);
            GameController.setObjectatGrid(null, this.oldgridPosition);
            oldgridPosition = this.gridPosition;
        }
        else
        {
            GameController.setObjectatGrid(this.gameObject, this.gridPosition);
        }
    }

    /**
    /* @name: updateTransformStandards
    /* @version: 1.0
    /* @Description: Atualiza os valores de controle de posição e rotação para serem usados pela função updateTransform.
    */
    private void updateTransformStandards(Vector3 posicao, Quaternion rotacao){
        this.fixedPosition = posicao;
        this.fixedRotation = rotacao;
    }

    /**
    /* @name: fixedTransform
    /* @version: 1.0
    /* @Description: Garante que os valores de posição e rotação permaneçam iguais aos valores das variaveis de controle.
    */
    private void fixedTransform() {
        this.gameObject.transform.position = this.fixedPosition;
        this.gameObject.transform.rotation = this.fixedRotation;
    }

    /**
    /* @name: giraPersonagem
    /* @version: 2.7
    /* @Description: Faz com que um personagem vire em direção a um ponto especifico.
    */
    public void giraPersonagem(ponto2D origem, ponto2D fim)
    {
        if (origem.x == fim.x)
        {
            if (origem.y > fim.y)
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
            else
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
        }
        else if (origem.y == fim.y)
        {
            if (origem.x > fim.x)
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
            else
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
        }
        else if (origem.x > fim.x)
        {
            if (origem.y > fim.y)
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 225, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
            else
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 315, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
        }
        else
        {
            if (origem.y > fim.y)
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 135, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
            else
            {
                this.gameObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 45, transform.eulerAngles.z);
                this.fixedRotation = transform.rotation;
            }
        }
    }

    // ---------------------------------------------------------------- IA functions ----------------------------------------------------------------

    /**
    /* @name: andarParaIA
    /* @version: 1.7
    /* @Description: Recebe um caminho e ativa uma função para andar passo a passo até o máximo de passos que o personagem pode dar.
    */
    public void andarParaIA(Path novoCaminho)
    {
        //Garante que o personagem está num estado de idle antes de andar
        if (this.isIdle())
        {
            this.ultimoPasso = this.gridPosition;

            //Atualiza valor da posição oritinal para mover objeto no controller.wargrid
            this.oldgridPosition = this.gridPosition;
            this.atualizaWargrid = true;

            // muda o estado do personagem para movendo
            this.caminho = novoCaminho;
            caminhaIA();
        }

    }

    /**
    /* @name: IA_caminha
    /* @version: 1.8
    /* @Description: Atualiza o proximo passo do personagem e seta a flag de movimento do objeto.
    */
    private void caminhaIA()
    {
        // TODO: Caso tenha dois ou mais aliados em linha, este trexo vai falhar
        // Ele garante que a unidade não pare caso exista apenas um aliado no final de seu movimento.
        // @EDGE_CASE: Caso existam dois aliados em linha e o PATH passe pelos dois, o ultimo sera evitado, mas o penultimo não.
        // @POSSIVEL_RESOLUCAO: Computar o path completo antes de caminhar, caso não tenha como buscar um novo caminho com X passos amais no maximo
        // A unidade não deve fazer nada.

        ponto2D proximoPasso = this.caminho.nextStep();

        bool mudaAcao = false;

        // Estuda proximo passo para saber se pode andar
        if (proximoPasso != null)
        {
            GameObject ObjectOnGridPosition = GameController.getObjectfromGrid(proximoPasso);

            // E exista um objeto na grid na posição do proximo passo
            if (ObjectOnGridPosition != null)
            {
                // E caso este objeto seja uma unidade de um jogador E esta unidade for parar em cima dela
                if (GameController.isPlayerTag(ObjectOnGridPosition.tag))
                {
                    if ((this.walk <= 1 || this.caminho.lookAtNextStep() == null) && this.acoes <= 1)
                    {
                        Debug.Log("IA Parada por tratamento de colisao. proximoPasso era:"+proximoPasso.toString());
                        proximoPasso = null;
                        this.caminho = null;
                        updateTransformStandards(this.transform.position, this.transform.rotation);
                        this.walk = this.MAXwalk;
                        this.gameObject.GetComponent<Animator>().SetBool("onWalking", false);
                        terminouAcaoDeMovimento();
                    }


                } // </ proximo passo é um personagem>

                // Caso o exista um objeto itnerativo dentro do caminho do personagem esse objeto será destruido //TODO: criar uma diferenciação entre os tipos de objetos interativos: porta, não porta
                if (ObjectOnGridPosition.CompareTag("interativos"))
                {
                    // Para o movimento da caminhada
                    updateTransformStandards(this.transform.position, this.transform.rotation);
                    this.caminho = null;
                    proximoPasso = null;
                    this.walk = this.MAXwalk;
                    this.gameObject.GetComponent<Animator>().SetBool("onWalking", false);

                    GameController.controller.alvoDaAcao = ObjectOnGridPosition;
                    GameController.controller.acaoEscolhida = playersActions.Interagir;
                    GameController.controller.novaAcaoEscolhida = playersActions.Interagir;
                    GameController.controller.estadoJogador = PlayersState.agindo;

                    atualizaControllerWargrid();
                    this.atualizaWargrid = false;

                    mudaAcao = true;

                    Cutscene_Handler.controller.start_Interaction();
                }
            } // </ Objeto na posição do proximo passo != null>

        } // </ proximoPasso não é Null>
        





        if (!mudaAcao)
        {
            // Se puder andar, continua
            // Caso o proximo passo não seja vazio E a unidade ainda possa andar
            if (proximoPasso != null && this.walk > 0)
            {
                this.walk--;
                estadoAtual = CharacterState.movendo;
                this.gridPosition = proximoPasso;
                this.newPosition = GameController.wargridToPosition(proximoPasso, this.gameObject);
                this.gameObject.GetComponent<Animator>().SetBool("onWalking", true);

                giraPersonagem(this.ultimoPasso, proximoPasso);
                this.ultimoPasso = proximoPasso;
            }
            // Caso o proximo passo seja nulo OU a unidade não puder mais andar
            else
            {
                updateTransformStandards(this.transform.position, this.transform.rotation);
                this.caminho = null;
                this.walk = this.MAXwalk;
                this.gameObject.GetComponent<Animator>().SetBool("onWalking", false);
                terminouAcaoDeMovimento();
            } // </ Proximo passo é nulo>

        }// </ Acao nao mudou>
    }

    // -------------------------------------------------------- Eventos de som ----------------------------------------------------------

    public void sfx_Play_Passo1()
    {
        AudioController.sfx_Play_Passo1();
    }

    public void sfx_Play_Passo2()
    {
        AudioController.sfx_Play_Passo2();
    }

    public void sfx_Play_Punch()
    {
        AudioController.sfx_Play_Punch();
    }

    public void sfx_Play_Shot()
    {
        AudioController.sfx_Play_Shot();
    }

    public void sfx_Play_Damage()
    {
        AudioController.sfx_Play_Damage();
    }

    public void sfx_Play_Death()
    {
        AudioController.sfx_Play_Death();
    }

    public void Battle_End()
    {
        GameController.controller.personagemEscolhido.force_terminarAcao();
    }
}
