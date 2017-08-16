using UnityEngine;
using System.Collections.Generic;

// Esta classe controla o jogo, executando comandos do jogador e controlando movimento das outras peças
public class GameController : MonoBehaviour {

    // Singleton Reference.
    public static GameController controller;    // Variavel estática de acesso ao singleton do GameController

    // Dados internos de controle do jogo e de turno.
    private JogadorDaVez currentplayer;          // Armazena o nome do jogador da vez.
    public bool checarJogador;                   // Variavel que controla se é hora de checar se o jogador da vez tem unidades com ações possiveis.
    public GenericCharacter personagemEscolhido; // Personagem escolhido pelo jogador para executar uma determinada ação.
    public playersActions acaoEscolhida;         // Ação escolhida pelo jogador para ser executada pelo personagem escolhido em um alvo.
    public playersActions novaAcaoEscolhida;     // Nova ação que foi escolhida mas ainda não modificou a ação anterior. (setado por botão)
    public PlayersState estadoJogador;           // Estado atual do jogador atual. Controla o que o jogador já fez e o que deve fazer para executar uma ação.
    public Actionflag actionflag;                // Sistema de flag utilizada por outros objetos para sinalizar que uma determinada ação terminou.
    public Gamestates estadoDoJogo;              // Estado do jogo para controle do fluxo de ações de objetos e controle de menu de pausa e level loading.
    public GameObject alvoDaAcao;                // Alvo de uma ação.

    // IA e movimentação.
    private Aestrela pathfinder;                 // Pathfinder usado para encontrar caminhos na matrix do mapa do jogo.
    private IA_Pathfinder ia;                    // Instancia da inteligencia artifical.
    private CharacterQueue enemieQueue;          // Pilha contendo todas as unidades de um inimigo. Usado para controlar unidades via IA.
    private bool enemieQueueCreated;             // Variavel de controle de criação da pilha de inimigos. //TODO: pilha de inimigos deve ser criada pela função AutoPlayerPass quando carregar um jogador inimigo.
    private GameObject closestEnemy;             // Apontador que escolhe o inimigo mais proximo para ser atacado pela IA.

    // Variaveis de controle para geração de mapa e manuntenção do jogo.
    public GameObject levelMap;                  // Mapa com artes e onde os personagens se movem.
    public int mapSizeX;                         // Tamanho X da matriz usada como mapa do jogo.
    public int mapSizeY;                         // Tamanho Y da matriz usada como mapa do jogo.
    public int tilePoolSize;                     // Tamanho do reservatório de tiles.
    public int tileSeparation;                   // Espaço de separação entre os tiles do mapa.
    public int yValueForTilePool;                // Valor da posição y para tiles que estão na Tile Pool

    // Objetos a serem instanciados pelo criador do mapa.
    public GameObject groundOverlayTile;         // Objeto que deve ser instanciado para se criar os tiles do jogo.

    // Mapa do jogo.
    private TilePool tilePool;                   // Fila que armazena tiles para serem utilizados nas areas de ação do jogo.
    private GameObject[][] warGrid;              // Matriz de controle de posições do jogo.

    // Controle de fim de jogo.
    public bool chegarMissoes;                   // Variavel que controla se é hora de chegar se o jogador completou todas as missões do nivel atual.
    public bool jogadorPerdeSemUnidades;         // Variavel de controle que avisa ao teste de gameover quando começar a testar pelo numero de unidades do jogador.

    // Controle de ativação de cutscenes.
    public bool onNextPlayer;                    // Variavel de controle de aplicação de animação de troca de jogador.

    // Variavel para visibilidade de informações.
    public string jogadorAtivo;                  // Armazena string da tag do jogador ativo.
    public static int nivelCarregadoAtual;       // Exibe qual é o nivel carregado atualmente no jogo.

    // Variavel de controle de transições por tempo.
    public float velo_interact_anim;             // Velocidade da animação de interação para controlar aplicação de efeito de som pelo audio controller.
    public float velo_nextPlayer_anim;           // Velocidade da animação da UI de mudança de jogador.

    void Awake()
    {
        if (GameController.controller == null)
        {
            GameController.controller = this;
        }
        else if (GameController.controller != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        this.jogadorPerdeSemUnidades = false;
        this.currentplayer = new JogadorDaVez();
        this.checarJogador = false;
        this.chegarMissoes = false;
        this.onNextPlayer = false;
        this.enemieQueueCreated = false;
        this.estadoJogador = PlayersState.idle;
        this.estadoDoJogo = Gamestates.loadNewLevel;
        this.actionflag = Actionflag.idle;
        this.yValueForTilePool = 3000;

        //Inicia pathfinder
        this.pathfinder = new Aestrela(this.mapSizeX, this.mapSizeY); // Inicia pathfinder do jogador
        this.ia = new IA_Pathfinder(this.mapSizeX, this.mapSizeY);    // Inicia pathfinder da inteligencia artifical.
        
        CanvasController.controller.showLoadingScreen();

        CanvasController.controller.hideConfirmationPanel();
        CanvasController.controller.hideActionPanel();
        CanvasController.controller.hidePauseMenu();

        AudioController.activate_PausedEffect();

        //TODO: carregar via XML map
        this.generateTilePool(tilePoolSize);
        this.generateMap();
    }

    // Update is called once per frame
    void Update() {

        //Atualiza a tag de jogador por motivos de controle
        this.jogadorAtivo = currentplayer.getTag();


        switch (this.estadoDoJogo)
        {

            case Gamestates.levelLoaded: // desativa UI de load, lerp de transparencia desativa loading cover.

                // Reinicia variaveis de controle de jogo para os valores iniciais de fase
                this.currentplayer = new JogadorDaVez();
                this.checarJogador = false;
                this.chegarMissoes = false;
                this.enemieQueueCreated = false;
                this.estadoJogador = PlayersState.idle;
                this.actionflag = Actionflag.idle;
                CameraControl.estadoDaCamera = CameraState.changeTarget;

                // Inicia cutscene de inicio de fase.
                Cutscene_Handler.controller.start_LevelCutscene();

                // Desativa a tela de loading e efeito de audio de load
                CanvasController.controller.hideLoadingScreen();
                AudioController.activate_StandardAudioEffect();

                this.jogadorPerdeSemUnidades = true;

                break;


            case Gamestates.paused:
                if (Input.GetButtonDown("Cancel"))
                {
                    this.joystickStartbutton();
                }
                break;

            case Gamestates.interactText:
                if (Input.GetButtonDown("Accept"))
                {
                    // Executa ação do botão "Accept" do joystick e reseta o input para não ocorrer multipla entrada de dados.
                    joystickAcceptButtonAction();
                }
                break;

            case Gamestates.playing:

                // ------------- Ação do Jogador -------------

                // Jogador escolha um personagem e age com ele.

                //Caso jogador ativo seja o player
                if (this.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
                {
                    // E a flag de ação não esteja ativa
                    if (this.actionflag == Actionflag.idle)
                    {

                        // Se o jogador não estiver agindo
                        if (this.estadoJogador != PlayersState.agindo)
                        {

                            // Caso o jogador aperte o botão Accept (escolhe personagem, escolhe alvo, aceita ação)
                            if (Input.GetButtonDown("Accept"))
                            {
                                // Executa ação do botão "Accept" do joystick e reseta o input para não ocorrer multipla entrada de dados.
                                joystickAcceptButtonAction();

                            } // </ botão Accept apertado>

                            if (Input.GetButtonDown("Cancel"))
                            {
                                switch (this.estadoJogador)
                                {
                                    case PlayersState.personagem:
                                        this.button_setActionBack();
                                        break;

                                    case PlayersState.acao:
                                        this.button_CancelAction();
                                        break;

                                    default:
                                        this.button_setActionBack();
                                        break;
                                }
                            }

                        }// </ jogador não estiver agindo>
                        
                    } // </ bloqueio de ação caso actionflag não seja idle>

                    if (this.actionflag == Actionflag.acting &&
                            this.acaoEscolhida == playersActions.Interagir &&
                            Cutscene_Handler.controller.tipoDaCutscene == CutsceneType.interact)
                    {
                        if (Input.GetButtonDown("Accept"))
                        {
                            // Executa ação do botão "Accept" do joystick e reseta o input para não ocorrer multipla entrada de dados.
                            joystickAcceptButtonAction();

                        }
                    }// </ jogador está agindo com uma interação>

                } // </ Ação do JOGADOR>
                // ------------- /Ação do Jogador -------------



                // ------------- Ação da IA -------------
                //Caso JOGADOR não seja o player atual, inteligencia artificial é usada para controlar as unidades.
                //<Controle da IA>
                else
                {
                    /** IA Pseudo code:
                     1- carrega unidades em uma pilha
                        1a- cria pilha, carrega unidades
                        1b- desempilha uma unidade
                     2- age com cada unidade até ela ficar sem ações
                        2a- se está perto do jogador, ataca
                        2b- caso contrário, anda.
                     3- checa jogador
                        3a - checa jogador, volta a idle
                        3b - go to 1b
                    */

                    // Enquanto o estado do jogador estiver Idle, a IA irá tentar criar uma lista de unidades até conseguir ou passar a vez por não encontrar unidades.
                    if (this.estadoJogador == PlayersState.idle)
                    {
                        //Caso a lista não tenha sido criada
                        if (!enemieQueueCreated)
                        {
                            // Tenta criar a lista
                            this.enemieQueue = new CharacterQueue();
                            this.enemieQueueCreated = true;

                            // Se a lista estiver vazia, marca para checar unidades do jogador (pra ver e se ele tem alguma ação) e tentar criar a lista novamente
                            if (enemieQueue.getSize() == 0)
                            {
                                this.enemieQueueCreated = false;
                                this.checarJogador = true;
                            }
                        } // </ criação da lista de unidades para a IA controlar>


                        // Caso a lista tenha sido criada com sucesso, IA pega um personagem pra controlar
                        else
                        {
                            // Caso não exista um personagem escolhido
                            if (this.personagemEscolhido == null)
                            {
                                // Pega um novo personagem
                                GenericCharacter proximaUnidade = this.enemieQueue.nextCharacter();

                                // Faz a camera procurar pelo alvo (personagem escolhido)
                                CameraControl.estadoDaCamera = CameraState.changeTarget;

                                //Caso este novo personagem não seja null
                                if (proximaUnidade != null)
                                {
                                    this.personagemEscolhido = proximaUnidade; // escolhe a unidade para agir
                                    this.estadoJogador = PlayersState.personagem; // Com o personagem escolhido, passa para a etapa de ação.
                                }

                                //Caso não exista mais unidades na pilha
                                else
                                {
                                    this.enemieQueue = null;
                                    this.enemieQueueCreated = false;
                                    this.AutoPlayerPass();
                                }
                            } // </ IA escolhe um personagem da lista para controlar>
                        }
                    } //</> Playerstate = idle


                    // IA Controla personagem escolhido
                    if (this.estadoJogador == PlayersState.personagem)
                    {
                        // A IA passa para a etapa de escolher seu alvo e ação
                        this.estadoJogador = PlayersState.acao;

                        // Encontra o inimigo mais proximo e atualiza a variavel de alvo.
                        findClosestEnemy();

                    } //</ Playerstate = personagem>

                    // IA escolhe uma ação baseada na distancia até o alvo.
                    if (this.estadoJogador == PlayersState.acao)
                    {
                        //Caso o alvo da ação tenha sido eliminado, encontra um novo alvo.
                        if (this.alvoDaAcao == null)
                        {
                            findClosestEnemy();
                        }

                        // A IA só escolha uma ação se houver um alvo para atacar.
                        else
                        {
                            // Calcula a distancia entre o inimigo e a unidade, levando em consideração os obstáculos para a IA
                            int distanciaEntreInimigos = this.ia.findDistanceTo(this.personagemEscolhido.gridPosition,
                                                                                this.alvoDaAcao.GetComponent<GenericCharacter>().gridPosition);
                            
                            // Caso a IA seja do tipo 2 a unidade só deve agir se alguma unidade do jogador se aproximar.
                            if (this.personagemEscolhido.iaType == 2)
                            {
                                // Caso tenha um inimigo dentro da distancia e a unidade não tenha notado ainda
                                if ((distanciaEntreInimigos <= ((this.personagemEscolhido.MAXwalk * 2)+1))
                                    && !this.personagemEscolhido.enemySpoted)
                                {
                                    this.personagemEscolhido.enemySpoted = true;
                                    Overhead_Spawner.spawnOverhead(5, OverHeads.IAExclamation);

                                    // Muda o tipo da IA para tipo 1.
                                    this.personagemEscolhido.iaType = 1;
                                }

                                // Caso não a IA tipo 2 não tenha detectado ninguem, termina sua ação.
                                if (!this.personagemEscolhido.enemySpoted)
                                {
                                    this.personagemEscolhido.force_terminarAcao();
                                }
                                else
                                {
                                    // Caso a distancia esteja dentro da area de ataque, a IA decide atacar
                                    if (distanciaEntreInimigos > this.personagemEscolhido.minalcance
                                     && distanciaEntreInimigos <= this.personagemEscolhido.alcance)
                                    {
                                        this.acaoEscolhida = playersActions.Atacar;
                                    }

                                    //Caso contrário a IA deverá se mover até a posição de ataque
                                    else
                                    {
                                        // Apesar da ação de Andar ser padrão, decidi reforçar a escolha, caso exista algum outro trexo reescrevendo a escolha sem querer.
                                        this.acaoEscolhida = playersActions.Andar;
                                    }

                                    // Depois de escolher a ação, a IA levanta a flag para execução da ação.
                                    this.estadoJogador = PlayersState.agindo;
                                }
                            }

                            // Caso a IA seja de tipo 1, a unidade apenas busca o inimigo mais próximo e se move para atacar.
                            if (this.personagemEscolhido.iaType == 1)
                            {
                                
                                // Caso a distancia esteja dentro da area de ataque, a IA decide atacar
                                if (distanciaEntreInimigos > this.personagemEscolhido.minalcance
                                 && distanciaEntreInimigos <= this.personagemEscolhido.alcance)
                                {
                                    this.acaoEscolhida = playersActions.Atacar;
                                }

                                //Caso contrário a IA deverá se mover até a posição de ataque
                                else
                                {
                                    // Apesar da ação de Andar ser padrão, decidi reforçar a escolha, caso exista algum outro trexo reescrevendo a escolha sem querer.
                                    this.acaoEscolhida = playersActions.Andar;
                                }

                                // Depois de escolher a ação, a IA levanta a flag para execução da ação.
                                this.estadoJogador = PlayersState.agindo;
                            }
                        }

                    }

                    // IA executa a ação escolhida
                    if (this.estadoJogador == PlayersState.agindo)
                    {
                        // Garante que a execução da ação da IA será executada apenas uma vez
                        if (this.actionflag == Actionflag.idle)
                        {
                            if (this.alvoDaAcao.GetComponent<GenericCharacter>().estadoAtual == CharacterState.dead)
                            {
                                this.estadoJogador = PlayersState.acao;
                                this.alvoDaAcao = null;
                            }
                            else
                            {
                                // IA levanta flag de ação para a unidade escolhida, travando o numero de execuções desta função
                                this.actionflag = Actionflag.acting;
                                //Executa a ação
                                executaAcaoIA();
                            }
                            
                        }
                    }
                    
                    // Quando a unidade sinalizar a finalização da ação, a IA analiza se pode agir novamente ou se deve escolher um novo personagem
                    if (this.actionflag == Actionflag.actionEnd)
                    {
                        this.actionflag = Actionflag.idle;

                        // Caso jogador possa agir, IA retorna a posição de escolha de ação
                        if (this.personagemEscolhido.aindaPodeAgir())
                        {
                            this.estadoJogador = PlayersState.acao;
                        }

                        // Caso contrário retorna a posição de escolha de personagem
                        else
                        {
                            resetChosen();
                        }
                        
                    } //</ unidade termina uma ação>

                } //</ Controle da IA>
                  // ------------- /Ação da IA -------------

                break; //</ Game State: Playing>
                
        } // </ switch case do gamestate>

        // ------------- checa Jogador -------------
        // Testa se o jogador perdeu todas as suas unidades, caso positivo, jogador é levado a cena de Game Over.
        if (this.jogadorPerdeSemUnidades)
        {
            // Se a quantidade de unidades do jogador for igual o menor que zero
            if (GameObject.FindGameObjectsWithTag("Jogador").Length <= 0)
            {
                this.loadGameOverScreen();
                jogadorPerdeSemUnidades = false;
            }
        }

        // Sempre que um jogador oqualquer agir, o jogo testa se este ainda tem ações válidas a realizar.
        // Caso contrario o turno passa para o proximo jogador.
        if (this.personagemEscolhido == null && this.checarJogador 
            && this.estadoDoJogo == Gamestates.playing)
        {
            this.checarJogador = false;
            this.AutoPlayerPass();
        }

        // Caso uma missão tenha sido concluida, checa missões
        if (this.chegarMissoes && this.estadoJogador != PlayersState.agindo)
        {
            // Caso todas as missões tenham sido concluidas
            if (MissionMenu_Handler.controller.isMissionClear())
            {
                Cutscene_Handler.controller.start_MissionComplete();
                this.chegarMissoes = false;
            }
        }

        if (this.actionflag == Actionflag.actionEnd)
        {
            if (this.personagemEscolhido != null)
            {
                if (!this.personagemEscolhido.aindaPodeAgir())
                {
                    this.resetChosen();
                }
                else
                {
                    escolhaDePersonagem(this.personagemEscolhido.gameObject);
                }
            }

            this.checarJogador = true;
            this.actionflag = Actionflag.idle;
        }
        // ------------- /checa Jogador ------------- 


        // Detecta se o jogador apertou um botão de PAUSE
        if (Input.GetButtonDown("Pause"))
        {
            this.joystickStartbutton();
        }

    }// </On Update>

    // ---------------------------------------------------------------- Public functions ----------------------------------------------------------------

    // ---------------------------------button functions ---------------------------------
    /**
    /* @name: button_setActionAttack
    /* @version: 1.0
    /* @Description: Muda a ação escolhida pelo jogador para a ação de ataque
    */
    public void button_setActionAttack()
    {
        if (this.personagemEscolhido != null)
        {
            this.personagemEscolhido.estadoArea = AreaOfActionState.changeAction;
            this.novaAcaoEscolhida = playersActions.Atacar;
            this.fazAreaDeAcao();
        }
    }

    /**
    /* @name: button_setActionRun
    /* @version: 1.0
    /* @Description: Muda a ação escolhida pelo jogador para a ação de corrida
    */
    public void button_setActionRun()
    {
        if (this.personagemEscolhido != null)
        {
            this.personagemEscolhido.estadoArea = AreaOfActionState.changeAction;
            this.novaAcaoEscolhida = playersActions.Andar;
            this.fazAreaDeAcao();
        }
    }

    /**
    /* @name: button_setActionInteract
    /* @version: 1.0
    /* @Description: Muda a ação escolhida pelo jogador para a ação de interagir
    */
    public void button_setActionInteract()
    {
        if (this.personagemEscolhido != null)
        {
            this.personagemEscolhido.estadoArea = AreaOfActionState.changeAction;
            this.novaAcaoEscolhida = playersActions.Interagir;
            this.fazAreaDeAcao();
        }
    }

    /**
    /* @name: button_setActionWait
    /* @version: 1.0
    /* @Description: Executa uma ação para fazer o personagem terminar uma ação parado.
    */
    public void button_setActionWait()
    {
        
        if (this.personagemEscolhido != null)
        {
            this.acaoEscolhida = playersActions.Andar;
            this.escolhaDoAlvoDaAcao(this.personagemEscolhido.gameObject);
        }
    }

    /**
    /* @name: button_setActionPassTurn
    /* @version: 1.0
    /* @Description: Executa uma ação para passar de turno.
    */
    public void button_setActionPassTurn()
    {
        if (this.estadoDoJogo == Gamestates.paused)
        {
            this.joystickStartbutton();
        }
        
        this.acaoEscolhida = playersActions.PassarTurno;

        if (this.personagemEscolhido != null)
        {
            this.escolhaDoAlvoDaAcao(this.personagemEscolhido.gameObject);
        }
        else
        {
            // Atualiza estado do jogador
            this.estadoJogador = PlayersState.acao;

            // Ativa canvas de confirmação de ação.
            CanvasController.controller.hideActionPanel();
            CanvasController.controller.showConfirmationPanel();
        }
    }

    /**
    /* @name: button_setActionReloadLevel
    /* @version: 1.0
    /* @Description: Recarrega o nivel atual, iniciando do zero.
    */
    public void button_setActionReloadLevel()
    {
        this.joystickStartbutton();
        
        if (this.personagemEscolhido != null)
        {
            this.acaoEscolhida = playersActions.RecarregarNivel;
            this.escolhaDoAlvoDaAcao(this.personagemEscolhido.gameObject);
        }
    }

    /**
    /* @name: button_ActionBack
    /* @version: 1.5
    /* @Description: desseleciona o personagem, retornando o jogador para a seleção de personagem
    */
    public void button_setActionBack()
    {
        resetChosen();
        CanvasController.controller.hideConfirmationPanel();
        CanvasController.controller.hideActionPanel();

        AudioController.sfx_Play_MenuChange();
    }

    /**
    /* @name: button_CancelAction
    /* @version: 1.1
    /* @Description: Retorna o jogador ao estado de escolha de ação
    */
    public void button_CancelAction()
    {
        this.estadoJogador = PlayersState.personagem;
        this.alvoDaAcao = null;
        
        CanvasController.controller.hideConfirmationPanel();
        CanvasController.controller.showActionPanel();
        
        AudioController.sfx_Play_MenuChange();
    }

    /**
    /* @name: button_ConfirmAction
    /* @version: 1.2
    /* @Description: Confirma ação do jogador. sensivel ao contexto do tipo de ação escolhida
    */
    public void button_ConfirmAction()
    {
        // Atualiza flags de controle do estado do jogador.
        this.estadoJogador = PlayersState.agindo;
        this.actionflag = Actionflag.acting;

        // Desativa interface de usuário.
        CanvasController.controller.hideConfirmationPanel();
        CanvasController.controller.hideActionPanel();

        // executa a ação escolhida.
        this.executaAcao();
    }

    /**
    /* @name: button_Quit
    /* @version: 1.2
    /* @Description: Envia o jogador para a tela de menu principal.
    */
    public void button_QuitToMenu()
    {
        this.quitToMenu();
    }

    // --------------------------------- /button functions ---------------------------------

    /**
    /* @name: getCurrentTag
    /* @version: 1.0
    /* @Description: retorna a tag do jogador atual;
    */
    public string getCurrentTag()
    {
        return this.currentplayer.getTag();
    }

    /**
    /* @name: isTileAtivado
    /* @version: 2.0
    /* @Description: true caso um cubo já esteja subindo neste ponto.
    */
    public bool isTileAtivado(ponto2D ponto)
    {
        if (isValidPoint(ponto))
        {
            if (warGrid[ponto.x][ponto.y] != null)
            {
                if (this.warGrid[ponto.x][ponto.y].CompareTag("GroundOverlayTile"))
                {
                    return this.warGrid[ponto.x][ponto.y].GetComponent<GroundOverlayReactor>().tileEstaAcima();
                }
            }
        }
        

        return false;
    }

    /**
    /* @name: getCurrentPlayer
    /* @version: 2.0
    /* @Description: retorna a tag do jogador atual;
    */
    public static string getCurrentPlayer()
    {
        return GameController.controller.currentplayer.getTag();
    }

    /**
    /* @name: getAcaoEscolhida
    /* @version: 2.0
    /* @Description: retorna ação escolhida atual do jogador
    */
    public static playersActions getAcaoEscolhida()
    {
        return GameController.controller.acaoEscolhida;
    }

    /**
    /* @name: wargridToPosition
    /* @version: 2.1
    /* @Description: calcula a posição espacial (Vector3) de um objeto dado sua posição no Wargrid.
    */
    public static Vector3 wargridToPosition(ponto2D wargridPoint, GameObject objeto)
    {
        Vector3 TileBoundSize = GameController.controller.groundOverlayTile.GetComponent<MeshRenderer>().bounds.size;
        Vector3 TileLocalScale = GameController.controller.groundOverlayTile.transform.localScale;

        return new Vector3(((TileBoundSize.x/ TileLocalScale.x) * wargridPoint.x),
                            (objeto.transform.localScale.y),
                            ((TileBoundSize.z/ TileLocalScale.z) * wargridPoint.y)
                          );
    }

    /**
    /* @name: isPlayerTag
    /* @version: 1.0
    /* @Description: Itera sobre uma lista de Players e compara a tag fornecida com a lista de jogadores, retornando TRUE caso a tag fornecida pertença a lista de Players.
    */
    public static bool isPlayerTag(string OneTag)
    {
        // transforma o enum Players em um array de Players, cada elemento contem um elemento do enum
        PlayersTags[] ArrayDeJogadores = (PlayersTags[])System.Enum.GetValues(typeof(PlayersTags));

        //Caso a tag fornecida seja igual a algum jogador, retorna true
        foreach (PlayersTags jogador in ArrayDeJogadores)
        {
            if (OneTag == jogador.ToString())
            {
                return true;
            }
        }

        //caso contrário retorna false.
        return false;
    }

    /**
    /* @name: isObstacle
    /* @version: 3.3
    /* @Description: true caso a posição passada corresponda a posição de um inimigo ou obstáculo.
    */
    public static bool isObstacle(ponto2D position, ActionsTypes tipoDeAcaoDaIA)
    {
        // Ponto invalido
        if (!GameController.controller.isValidPoint(position))
        {
            return true;
        }

        if (GameController.getObjectfromGrid(position) != null)
        {
            switch (tipoDeAcaoDaIA)
            {
                case ActionsTypes.Comum:
                    switch (GameController.controller.acaoEscolhida)
                    {
                        case playersActions.Andar:

                            if (GameController.getObjectfromGrid(position).CompareTag("interativos"))
                            {
                                if (GameController.controller.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
                                {
                                    return true;
                                }
                                return GameController.getObjectfromGrid(position).GetComponent<InteractActor>().tipoDeInteragir != Interactibles.door;
                            }

                            return GameController.getObjectfromGrid(position).CompareTag("Obstacle") ||
                                GameController.getObjectfromGrid(position).CompareTag("semiObstacle");

                        case playersActions.Atacar:
                            return GameController.getObjectfromGrid(position).CompareTag("Obstacle");

                        case playersActions.Interagir:
                            return false;
                    }
                    break;

                case ActionsTypes.Distance:
                    if (GameController.getObjectfromGrid(position).CompareTag("semiObstacle") || GameController.getObjectfromGrid(position).CompareTag("interativos"))
                    {
                        return false;
                    }
                    return GameController.getObjectfromGrid(position).CompareTag("Obstacle");
            }

        }
        
        return false;
    }

    /**
    /* @name: tileHasEnemy
    /* @version: 3.0
    /* @Description true caso exista um personagem inimigo no tile.
    */
    public static bool tileHasEnemy(ponto2D position)
    {
        // Se a posição atual for igual a posição do personagem escolhido, então não existe inimigos.
        if (position.isEqual(GameController.controller.personagemEscolhido.gridPosition))
        {
            return false;
        }

        // Se o ponto for valido
        if (GameController.controller.isValidPoint(position))
        {
            // E ouver um objeto nesta posição
            if (GameController.getObjectfromGrid(position) != null)
            {
                GameObject objetoDaGrid = GameController.getObjectfromGrid(position);

                if (GameController.isPlayerTag(objetoDaGrid.tag))
                {
                    // Caso o personagem seja do jogador, os inimigos são os personagens do inimigos:
                    if (GameController.controller.personagemEscolhido.CompareTag(PlayersTags.Jogador.ToString()))
                    {
                        return !objetoDaGrid.CompareTag(PlayersTags.Jogador.ToString());
                    }
                    // Caso o personagem não seja do jogador, os inimigos são os personagens do jogador
                    else
                    {
                        return !objetoDaGrid.CompareTag(PlayersTags.Demonios.ToString());
                    }
                }
            }
        }

        return false;

    }

    /**
    /* @name: actionFlagEnd
    /* @version: 2.0
    /* @Description: muda a flag de ação para ação terminada, assim o gameController é sinalizado que todos os objetos terminaram suas ações e estão prontos para receber novas instruções.
    */
    public static void actionFlagEnd()
    {
        GameController.controller.actionflag = Actionflag.actionEnd;
    }

    /**
    /* @name: estadoDoJogo
    /* @version: 2.0
    /* @Description: retorna o estado atual do jogo
    */
    public static Gamestates getEstadoDoJogo()
    {
        return GameController.controller.estadoDoJogo;
    }

    /**
    /* @name: getEstadoDoJogador
    /* @version: 2.0
    /* @Description: retorna o estado atual do jogador
    */
    public static PlayersState getEstadoDoJogador()
    {
        return GameController.controller.estadoJogador;
    }

    /**
    /* @name: levelLoadedFlag
    /* @version: 2.0
    /* @Description: Detecta termino do algoritmo de carregamento de level
    */
    public static void levelLoadedFlag()
    {
        GameController.controller.estadoDoJogo = Gamestates.levelLoaded;
    }

    /**
    /* @name: loadNewLevelFlag
    /* @version: 2.1
    /* @Description: muda estado de jogo para carregando um novo nivel.
    */
    public static void loadNewLevelFlag()
    {

        AudioController.changeContextFlag(Scene.Gameplay);
        GameController.controller.estadoDoJogo = Gamestates.loadNewLevel;
    }

    /**
    /* @name: LoadingLevelFlag
    /* @version: 2.0
    /* @Description: Inicia o carregamento de uma nova fase
    */
    public static void LoadingLevelFlag()
    {
        GameController.controller.estadoDoJogo = Gamestates.loadingLevel;
    }

    /**
    /* @name: getyValueForTilePool
    /* @version: 2.0
    /* @Description: retorna valor de y para distancia entre o piso/mapa e a pilha de tiles
    */
    public static int getyValueForTilePool()
    {
        return GameController.controller.yValueForTilePool;
    }

    /**
    /* @name: updateMaxSize
    /* @version: 2.0
    /* @Description: Atualiza os valores máximos de X e Y e gera novamente o wargrid baseado nestes valores.
    */
    public static void updateMaxSize(int newX, int newY)
    {
        
        //Caso os valores sejam ilegais, logar o erro.
        if (newX <= 0 || newY <= 0)
        {
            Debug.Log("[updateMaxSize] VALORES ILEGAIS");
        }
        else
        {
            
            //Atualiza valores máximos de X e Y.
            GameController.controller.mapSizeX = newX;
            GameController.controller.mapSizeY = newY;

            //Recria o wargrid baseado nos novos valores.
            GameController.controller.generateMap();

            //Reinicia pathfinder baseado nos novos valores de X e Y
            GameController.controller.pathfinder = new Aestrela(GameController.controller.mapSizeX, GameController.controller.mapSizeY); // Inicia pathfinder do jogador
            GameController.controller.ia = new IA_Pathfinder(GameController.controller.mapSizeX, GameController.controller.mapSizeY);    // Inicia pathfinder da inteligencia artifical.
        }
        
    }

    /**
    /* @name: getMaincamera
    /* @version: 2.0
    /* @Description: Retorna a camera principal da cena
    */
    public static void setCursorPosition(ponto2D ponto)
    {
        CursorReactor.cursor.setCursorPosition(ponto);
    }

    /**
    /* @name: atualizaObjetoWargrid
    /* @version: 1.0
    /* @Description: Muda a posição de um objeto dentro do wargrid.
    */
    public static void atualizaObjetoWargrid(ponto2D oldPosition, ponto2D newPosition)
    {
        if (GameController.controller.isValidPoint(oldPosition) && 
            GameController.controller.isValidPoint(newPosition))
        {
            GameController.controller.warGrid[newPosition.x][newPosition.y] = GameController.controller.warGrid[oldPosition.x][oldPosition.y];
            GameController.controller.warGrid[oldPosition.x][oldPosition.y] = null;
        }
    }

    /**
    /* @name: setObjectatGrid
    /* @version: 1.0
    /* @Description: Adiciona um objeto a grid do mapa.
    */
    public static void setObjectatGrid(GameObject objeto, ponto2D ponto)
    {
        if (GameController.controller.isValidPoint(ponto))
        {
            GameController.controller.warGrid[ponto.x][ponto.y] = objeto;
        }
    }

    /**
    /* @name: delObjectatGrid
    /* @version: 1.0
    /* @Description: Remove um objeto da grid do mapa.
    */
    public static void delObjectatGrid(ponto2D ponto)
    {
        GameController.controller.warGrid[ponto.x][ponto.y] = null;
    }

    /**
    /* @name: getObjectfromGrid
    /* @version: 1.0
    /* @Description: retorna um objeto da grid do mapa.
    */
    public static GameObject getObjectfromGrid(ponto2D ponto)
    {
        try
        {
            return GameController.controller.warGrid[ponto.x][ponto.y];
        }
        catch (System.NullReferenceException e)
        {
            Debug.Log("pontoPassado: " + ponto.toString());
        }

        return GameController.controller.warGrid[ponto.x][ponto.y];
    }

    // ---------------------------------------------------------------- /Public functions ----------------------------------------------------------------


    // ---------------------------------------------------------------- Private functions ----------------------------------------------------------------


    // ---------------------------------Controller functions ---------------------------------

    /**
    /* @name: joystickStartbutton
    /* @version: 1.0
    /* @Description: Controla o funionamento do botão de pause do joystick.
    */
    public void joystickStartbutton()
    {
        // game state > pause > playing
        if (this.estadoDoJogo == Gamestates.playing)
        {
            this.estadoDoJogo = Gamestates.paused;
            CanvasController.controller.showPauseMenu();
            AudioController.activate_PausedEffect();
        }
        else if (this.estadoDoJogo == Gamestates.paused)
        {
            this.estadoDoJogo = Gamestates.playing;
            CanvasController.controller.hidePauseMenu();
            AudioController.activate_StandardAudioEffect();
        }

    }

    /**
    /* @name: joystickAcceptButtonAction
    /* @version: 2.7
    /* @Description: Ação do botão "Accept" do joystick
    */
    private void joystickAcceptButtonAction()
    {

        AudioController.sfx_Play_MenuClick();

        // Jogador escolhe um personagem usando o cursor do jogo.
        if (this.estadoJogador == PlayersState.idle)
        {

            // Se tiver um objeto na posição do cursor
            if (CursorReactor.cursor.objInWargridPosition != null)
            {
                // E esse objeto for um personagem
                if (CursorReactor.cursor.objInWargridPosition.GetComponent<GenericCharacter>() != null)
                {
                    // Escolha esse personagem
                    escolhaDePersonagem(CursorReactor.cursor.objInWargridPosition);
                    Input.ResetInputAxes();
                }
            }

        } // </ estado do jogador = idle>

        // Caso o personagem já tenha sido escolhido, um proximo acionamento do "Accept" significa a escolha de um alvo para a ação.
        else
        {
            // Caso jogador já tenha escolhido um personagem
            if (this.estadoJogador == PlayersState.personagem)
            {
                // E ele for uma unidade do jogador
                if (this.personagemEscolhido.CompareTag(PlayersTags.Jogador.ToString()))
                {
                    // E puder agir:
                    if (this.personagemEscolhido.GetComponent<GenericCharacter>().aindaPodeAgir())
                    {
                        GroundOverlayReactor tileFromCursor = null;

                        if (CursorReactor.cursor.getTileinPosition() != null)
                        {
                            if (CursorReactor.cursor.getTileinPosition().GetComponent<GroundOverlayReactor>() != null)
                            {
                                tileFromCursor = CursorReactor.cursor.getTileinPosition().GetComponent<GroundOverlayReactor>();
                            }
                        }
                        
                        switch (this.acaoEscolhida)
                        {
                            case playersActions.Andar:
                                if (!GameController.isObstacle(CursorReactor.cursor.getPosition(), ActionsTypes.Comum) &&
                                        !GameController.tileHasEnemy(CursorReactor.cursor.getPosition()))
                                {
                                    if (tileFromCursor != null)
                                    {
                                        if (!tileFromCursor.temAliado())
                                        {
                                            escolhaDoAlvoDaAcao(tileFromCursor.gameObject);
                                            Input.ResetInputAxes();
                                        }
                                    }
                                }

                                break;

                            case playersActions.Atacar:
                                if (tileFromCursor != null)
                                {
                                    if (tileFromCursor.temInimigo())
                                    {
                                        escolhaDoAlvoDaAcao(tileFromCursor.getInimigo());
                                        Input.ResetInputAxes();
                                    }
                                }


                                break;

                            case playersActions.Interagir:
                                if (tileFromCursor != null)
                                {
                                    if (tileFromCursor.temInteracao())
                                    {
                                        escolhaDoAlvoDaAcao(tileFromCursor.getInteracao());
                                        Input.ResetInputAxes();
                                    }
                                }

                                break;
                        }

                    }// </ pode agir>
                } // </ é unidade do jogador>


            } // </ estado do jogador = personagem>
            
            else if (this.estadoJogador == PlayersState.acao)
            {
                this.button_ConfirmAction();
                Input.ResetInputAxes();
            } // </ estado do jogador = acao>

        } // </ estado do jogador != idle>

        if (this.actionflag == Actionflag.acting &&
                this.acaoEscolhida == playersActions.Interagir &&
                Cutscene_Handler.controller.tipoDaCutscene == CutsceneType.interact &&
                this.estadoDoJogo == Gamestates.playing ||
                this.estadoDoJogo == Gamestates.interactText)
        {
            CanvasController.controller.hideInteractTextPanel();
            Cutscene_Handler.controller.tipoDaCutscene = CutsceneType.idle;
            AudioController.activate_StandardAudioEffect(this.velo_interact_anim);
            Input.ResetInputAxes();
        }
    }


    // ---------------------------------Controller functions ---------------------------------

    /**
    /* @name: AutoPlayerPass
    /* @version: 1.4
    /* @Description: Itera sobre os personagens do jogador atual, passa a vez do jogador caso o jogador atual não possa agir. false caso contrário.
    /* Se não houver unidades que poassam agir em nenhum time e esta função for chamada, ela entrará em loop infinito.
    */
    private bool AutoPlayerPass() {
        GenericCharacter algumPersonagem;

        // tenta encontrar algum personagem daquele jogador que ainda possa agir
        foreach (GameObject unidade in GameObject.FindGameObjectsWithTag(this.currentplayer.getTag()))
        {
            algumPersonagem = unidade.GetComponent<GenericCharacter>();

            if (algumPersonagem.aindaPodeAgir()) {

                // Caso seja nescessario avisar mudança de jogador:
                if (onNextPlayer)
                {
                    if (this.currentplayer.getTag() == PlayersTags.Jogador.ToString() ||
                    this.currentplayer.getTag() == PlayersTags.Demonios.ToString())
                    {
                        Cutscene_Handler.controller.start_NextPlayer(this.currentplayer.getTag());
                        this.onNextPlayer = false;
                    }
                }
                // termina ação da função
                return false;
            }
        }

        // Reseta todos os personagens para o proximo turno.
        foreach (GameObject unidade in GameObject.FindGameObjectsWithTag(this.currentplayer.getTag()))
        {
            unidade.GetComponent<GenericCharacter>().restPodeAgir();
        }

        // passa a vez para o proximo jogador e executa AutoPlayerPass novamente.
        estadoJogador = PlayersState.idle;
        this.currentplayer.getNext();
        this.enemieQueueCreated = false;
        this.onNextPlayer = true;

        CameraControl.estadoDaCamera = CameraState.changeTarget;

        return AutoPlayerPass();
    }

    /**
    /* @name: escolhaDePersonagem
    /* @version: 2.0
    /* @Description: responsavel por atualizar a variavel personagemEscolhido e por aplicar um highlight usando shader.
    */
    private void escolhaDePersonagem(GameObject novoPersonagem) {
        //Se o jogador já escolheu um personagem, seu shader é retornado ao normal
        if (this.personagemEscolhido != null)
        {
            CanvasController.controller.hideActionPanel();

            desfazAreaDeAcao();
            this.personagemEscolhido.estadoAtual = CharacterState.idle;

            AudioController.sfx_Play_MenuClick();
        }

        if (novoPersonagem != null)
        {
            //Então a variavel de personagem escolhido é atualizada
            this.personagemEscolhido = novoPersonagem.GetComponent<GenericCharacter>();
            this.personagemEscolhido.estadoAtual = CharacterState.selecionado;
            this.estadoJogador = PlayersState.personagem;

            // Cria area de ação e UI de personagem
            fazAreaDeAcao();
            
            CanvasController.controller.showActionPanel();
        }
        else
        {
            Debug.Log("Objeto null foi passado para escolha de personagem.");
        }

    }

    /**
    /* @name: escolhaDoAlvoDaAcao
    /* @version: 1.0
    /* @Description: Atualiza a variavel alvoDaAcao e ativa a interface de confirmação de ação.
    */
    private void escolhaDoAlvoDaAcao(GameObject tileAlvo)
    {
        if (tileAlvo != null)
        {
            this.alvoDaAcao = tileAlvo;
            this.estadoJogador = PlayersState.acao;

            AudioController.sfx_Play_MenuClick();

            // ativa canvas de confirmação de ação.
            CanvasController.controller.hideActionPanel();
            CanvasController.controller.showConfirmationPanel();
        }
        else
        {
            Debug.Log("Objeto null foi passado como alvo de ação.");
        }
        
    }

    /**
    /* @name: interagirCom
    /* @version: 2.2
    /* @Description: Função que executa a sequencia de passos de uma interação de acordo com o tipo de objeto que o personagem está interagindo.
    */
    public void interagirCom()
    {
        InteractActor objetoInteragido = GameController.controller.alvoDaAcao.GetComponent<InteractActor>();

        // Caso o jogador ativo seja o player
        if (this.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
        {
            // Caso o actor não tenha sido usado
            if (!objetoInteragido.used)
            {
                switch (objetoInteragido.tipoDeInteragir)
                {
                    case Interactibles.jornal:
                        AudioController.sfx_Play_NewsPaper();
                        break;

                    case Interactibles.computer:
                        AudioController.sfx_Play_Computer();
                        break;

                    case Interactibles.door:
                        AudioController.sfx_Play_Door();
                        break;
                    case Interactibles.healthBox:
                        this.personagemEscolhido.life = this.personagemEscolhido.MAXlife;
                        break;
                }

                if (objetoInteragido.isMission())
                {
                    objetoInteragido.missionCleard = true;
                    this.chegarMissoes = true;
                    objetoInteragido.used = true;

                    GameController.controller.estadoDoJogo = Gamestates.playing;
                    Cutscene_Handler.controller.tipoDaCutscene = CutsceneType.idle;

                    // Ao final da utilização da missão, atualiza-se a tela de missões.
                    MissionMenu_Handler.controller.updateMissionOnUI();

                    GameController.controller.personagemEscolhido.force_terminarAcao();
                }

                else if (objetoInteragido.isReusable)
                {
                    CanvasController.controller.showInteractTextPanel();

                    AudioController.activate_PausedEffect(this.velo_interact_anim);
                }
                else
                {
                    objetoInteragido.used = true;
                    // Somente ao final da interação, a ação do personagem acaba
                }
            }

            if (objetoInteragido.isDestructable)
            {
                GameObject.Destroy(objetoInteragido.gameObject);

                // Somente ao final da interação, a ação do personagem acaba
                this.personagemEscolhido.force_terminarAcao();

                this.estadoDoJogo = Gamestates.playing;
                Cutscene_Handler.controller.tipoDaCutscene = CutsceneType.idle;
            }
        }

        // Caso o jogador ativo NÃO seja o player
        else
        {
            AudioController.sfx_Play_Door();

            GameObject.Destroy(objetoInteragido.gameObject);
            this.personagemEscolhido.force_terminarAcao();
            this.estadoDoJogo = Gamestates.playing;
            Cutscene_Handler.controller.tipoDaCutscene = CutsceneType.idle;
        }
    }

    /**
    /* @name: resetChosen
    /* @version: 1.5
    /* @Description: Reinicia os valores de personagemEscolhido e Objetivo de movimetação para que não exista lixo entre ações. E desfaz a area de ação anterior.
    */
    private void resetChosen() {
        // Caso personagem não seja nulo, reinicia suas variaveis de controle para o estado de idle.
        if (this.personagemEscolhido != null)
        {
            desfazAreaDeAcao();
            this.personagemEscolhido.estadoAtual = CharacterState.idle;
            this.personagemEscolhido = null;
        }
        
        // reinicia variaveis de controle para o estado de idle.
        this.acaoEscolhida = playersActions.Andar;
        this.estadoJogador = PlayersState.idle;
        this.alvoDaAcao = null;
    }

    /**
    /* @name: generateMap
    /* @version: 3.0
    /* @Description: Cria uma matrix (mapSizeX, mapSizeY) vazio onde Tiles (GroundOverlay) podem ser colocados.
    */
    private void generateMap()
    {
		//inicia uma matriz vazia que será usada como mapa
		this.warGrid = new GameObject [mapSizeX][];
        for (int x = 0; x < mapSizeX; x++)
        {
            this.warGrid[x] = new GameObject[mapSizeY];
        }

    }

    /**
    /* @name: generateTilePool
    /* @version: 1.0
    /* @Description: Cria uma fila de tiles que será usado para criar areas de ação.
    */
    private void generateTilePool(int tileSize)
    {
        // Cria uma fila de tiles para ser usada pela criação de areas;
        this.tilePool = new TilePool(tileSize);
        Vector3 tilePosition;
        int columNumber = 0;
        int rowNumber = 0;

        //Popula a fila com Tiles para que estes sejam usados no jogo.
        for (int i = 0; i < tileSize; i++)
        {
            rowNumber = i % 20;
            if (rowNumber == 0)
            {
                columNumber ++;
            }
            

            tilePosition = GameController.wargridToPosition(new ponto2D(rowNumber, columNumber), this.groundOverlayTile);
            tilePosition.y = yValueForTilePool;

            Quaternion rotacao = new Quaternion();
            rotacao.Set(0, 0, 0, 0); // corrige rotação do blender # DEPRECATED

            GroundOverlayReactor tile = ((GameObject)Instantiate(groundOverlayTile,       // O que instanciar.
                                                                 tilePosition,            // Posição de instanciamento.
                                                                 rotacao)                 // Rotação inicial.
                                        ).GetComponent<GroundOverlayReactor>();           // acessa o script de controle do objeto e armarzena este na variavel;
           
            tile.poolPosition = new ponto2D(rowNumber, columNumber);
            this.tilePool.deposit(tile);
        }

    }

    /**
    /* @name: fazAreaDeAcao
    /* @version: 1.0
    /* @Description: Chama as funções corretas de acordo com a ação do pesonagem
    */
    private void fazAreaDeAcao()
    {
        if (this.personagemEscolhido != null)
        {
            if (!(this.personagemEscolhido.estadoArea == AreaOfActionState.done))
            {
                if (this.personagemEscolhido.estadoArea == AreaOfActionState.undone)
                {

                    switch (this.acaoEscolhida)
                    {
                        case playersActions.Andar:
                            this.generateActionArea(this.personagemEscolhido.gridPosition, this.personagemEscolhido.walk, 0, 0);
                            this.personagemEscolhido.estadoArea = AreaOfActionState.done;
                            break;

                        case playersActions.Atacar:
                            this.generateActionArea(this.personagemEscolhido.gridPosition, 0, this.personagemEscolhido.alcance, this.personagemEscolhido.minalcance);
                            this.personagemEscolhido.estadoArea = AreaOfActionState.done;
                            break;

                        case playersActions.Interagir:
                            this.generateActionArea(this.personagemEscolhido.gridPosition, 0, 1, 0);
                            this.personagemEscolhido.estadoArea = AreaOfActionState.done;
                            break;
                    }
                }

                else
                {
                    desfazAreaDeAcao();
                    this.acaoEscolhida = this.novaAcaoEscolhida;
                    fazAreaDeAcao();
                }
            }
        }
    }

    /**
    /* @name: desfazAreaDeAcao
    /* @version: 1.0
    /* @Description: desfaz a area de ação de acordo com a ação escolhida pelo jogador.
    */
    public void desfazAreaDeAcao()
    {
        if (this.personagemEscolhido != null)
        {
            deGenerateActionArea();
            this.personagemEscolhido.estadoArea = AreaOfActionState.undone;
        }
    }

    /**
    /* @name: generateActionArea
    /* @version: 2.6
    /* @Description: Levanta Tiles no mapa de jogo para marcar a area de ação do jogador.
    */
    private void generateActionArea(ponto2D centro, int walk, int maxRange, int minRange)
    {
        /**
        1- começa no centro.
        2- usa algoritmo de pathfinding para criar uma pilha de tiles para subir
        3- cada posição válida levanta um tile
        */

        Path pilhaDeTiles = pathfinder.findAreaFrom(centro, maxRange + walk, minRange);

        if (pilhaDeTiles == null)
        {
            Debug.Log("[GEN_AREA] TILE_POOL_NULL");
        }
        else
        {
            ponto2D pontoTile = pilhaDeTiles.nextStep();

            while (pontoTile != null)
            {
                switch (acaoEscolhida)
                {

                    case playersActions.Andar:
                        if (!this.personagemEscolhido.gridPosition.isEqual(pontoTile))
                        {
                            if (GameController.getObjectfromGrid(pontoTile) != null)
                            {
                                if (!GameController.getObjectfromGrid(pontoTile).CompareTag("interativos"))
                                {
                                    levantaTile(pontoTile);
                                }
                            } else
                            {
                                levantaTile(pontoTile);
                            }
                            
                        }

                        break;

                    case playersActions.Atacar:
                        if (pontoTile.distancia(centro) > minRange)
                        {
                            if (GameController.getObjectfromGrid(pontoTile) != null)
                            {
                                if (!GameController.getObjectfromGrid(pontoTile).CompareTag("interativos")&&
                                    !GameController.getObjectfromGrid(pontoTile).CompareTag("semiObstacle"))
                                {
                                    levantaTile(pontoTile);
                                }
                            }
                            else
                            {
                                levantaTile(pontoTile);
                            }
                        }

                        break;

                    case playersActions.Interagir:
                        if (!this.personagemEscolhido.gridPosition.isEqual(pontoTile))
                        {
                            levantaTile(pontoTile);
                        }

                        break;
                }
                
                pontoTile = pilhaDeTiles.nextStep();
            }
        }
        
    }

    /**
    /* @name: deGenerateActionArea
    /* @version: 2.5
    /* @Description: Abaixa Tiles no mapa de jogo para marcar a area de ação do jogador.
    */
    private void deGenerateActionArea()
    {
        /**
        1- acessa lista da area de ação do personagem
        2- abaixa todos os tiles
        */

        if (this.personagemEscolhido.areaDeAcao != null)
        {
            foreach (GroundOverlayReactor tile in this.personagemEscolhido.areaDeAcao)
            {
                if (tile != null)
                {
                    // Remove objeto do wargrid apenas se o objeto na posição for um tile 
                    if (GameController.getObjectfromGrid(tile.wargrid) != null)
                    {
                        if (GameController.getObjectfromGrid(tile.wargrid).GetComponent<GroundOverlayReactor>() != null)
                        {
                            GameController.delObjectatGrid(tile.wargrid);
                        }
                    }
                    
                    // Abaixa o tile e retorna ao pool
                    tile.abaixar(tile.wargrid, wargridCoordDOWN(tile.wargrid));
                    this.tilePool.deposit(tile);
                }
            }
        }
        
        this.personagemEscolhido.areaDeAcao = new List<GroundOverlayReactor>();

    }

    /**
    /* @name: levantaTile
    /* @version: 2.0
    /* @Description: Levanta um tile em um ponto especifico.
    */
    private void levantaTile(ponto2D ponto)
    {
        TileColors tileColor = TileColors.andavel;
        switch (this.acaoEscolhida)
        {
            case playersActions.Andar:
                tileColor = TileColors.andavel;
                break;

            case playersActions.Atacar:
                tileColor = TileColors.obstaculo;
                break;

            case playersActions.Interagir:
                tileColor = TileColors.interagir;
                break;
        }

        GroundOverlayReactor TileLevantado = null;

        if (this.warGrid[ponto.x][ponto.y] == null)
        {
            TileLevantado = this.tilePool.withdraw();
            this.warGrid[ponto.x][ponto.y] = TileLevantado.gameObject;
        }
        else
        {
            if (this.warGrid[ponto.x][ponto.y].tag == "GroundOverlayTile")
            {
                TileLevantado = this.warGrid[ponto.x][ponto.y].GetComponent<GroundOverlayReactor>();
            }
            else
            {
                TileLevantado = this.tilePool.withdraw();
            }
        }
        
        TileLevantado.levantar(ponto, GameController.wargridToPosition(ponto, this.groundOverlayTile), this.wargridCoordDOWN(ponto), tileColor);

        if (this.warGrid[ponto.x][ponto.y] == null)
        {
            this.warGrid[ponto.x][ponto.y] = TileLevantado.gameObject;
        }

        this.personagemEscolhido.areaDeAcao.Add(TileLevantado);
    }

    /**
    /* @name: wargridCoordDOWN
    /* @version: 2.0
    /* @Description: calcula a posição espacial de um objeto dado sua posição no Wargrid, mas com altura abaixo do mapa, usado somente para abaixar o tile.
    */
    private Vector3 wargridCoordDOWN(ponto2D wargridPoint)
    {
        Vector3 posicaoCorrigida = GameController.wargridToPosition(wargridPoint, groundOverlayTile);
        posicaoCorrigida.y *= -20;

        return posicaoCorrigida;
    }

    /**
    /* @name: isValid
    /* @version: 1.0
    /* @Description: True caso o ponto esteja contido dentro do range de valores possiveis do wargrid.
    */
    private bool isValidPoint(ponto2D ponto)
    {
        return ponto.x >= 0 && ponto.x < this.mapSizeX &&
               ponto.y >= 0 && ponto.y < this.mapSizeY;
    }

    /**
    /* @name: areaContainsPosition
    /* @version: 1.6
    /* @Description: True caso o ponto posição esteja dentro da area que deve ser levantada
    */
    private bool areaContainsPosition(ponto2D centro, ponto2D posicao, int walk, int maxAlcance, int minAlcance)
    {
        int distancia = centro.distancia(posicao);

        if (isObstacle(posicao,ActionsTypes.Comum))
        {
            return false;
        }

        return ((distancia < walk || distancia > minAlcance)
                && (distancia <= maxAlcance));
    }

    /**
    /* @name: findClosestEnemy
    /* @version: 1.0
    /* @Description: Encontra o inimigo mais próximo.
    */
    private void findClosestEnemy()
    {
        // Inicia variaveis locais para busca do alvo da unidade.
        GenericCharacter algumInimigo = null;                         // Uma unidade da lista de inimigos da IA e Inicia a unidade para evitar lixo.
        int distanciaAnterior = int.MaxValue;                         // Ultimo menor valor de distancia entre um inimigo e a unidade.

        // Encontra um alvo para o inimigo, pesquisando a unidade do jogador mais proxima para atacar.
        // Para cada unidade do jogador...
        foreach (GameObject unidade in GameObject.FindGameObjectsWithTag(PlayersTags.Jogador.ToString()))
        {
            // Calcula-se a distancia entre a unidade do jogador e a escolhida pela IA
            algumInimigo = unidade.GetComponent<GenericCharacter>();
            int distanciaEntreInimigos = this.ia.findDistanceTo(this.personagemEscolhido.gridPosition, algumInimigo.gridPosition);

            // Caso essa distancia seja menor do que a ultima encontrada, substitui as variaveis de saida.
            if (distanciaEntreInimigos < distanciaAnterior)
            {
                distanciaAnterior = distanciaEntreInimigos;
                this.alvoDaAcao = unidade;
            }

            //Em caso de empate, o desempate é feito a partir de uma comparação da quantidade de life
            else if (distanciaEntreInimigos == distanciaAnterior)
            {
                if (this.alvoDaAcao == null)
                {
                    this.alvoDaAcao = unidade;
                }
                else if (alvoDaAcao.GetComponent<GenericCharacter>().life > algumInimigo.life)
                {
                    this.alvoDaAcao = unidade;
                }
            }// </ desempate>

        } // </foreach>

    }

    /**
    /* @name: executaAcao
    /* @version: 2.8
    /* @Description: Executa a ação escolhida pelo jogador
    */
    private void executaAcao()
    {
        switch (this.acaoEscolhida)
        {
            case playersActions.Andar:

                if (this.alvoDaAcao != this.personagemEscolhido.gameObject)
                {
                    this.pathfinder = new Aestrela(this.mapSizeX, this.mapSizeY);
                    Path caminho = pathfinder.findPathTo(this.personagemEscolhido.gridPosition, this.alvoDaAcao.GetComponent<GroundOverlayReactor>().wargrid);

                    if (caminho != null)
                    {
                        Overhead_Spawner.spawnOverhead(1, OverHeads.playerText);
                        this.personagemEscolhido.andarPara(caminho);
                    }
                    // Caso o algoritmo não encontre um caminho, a ação é cancelada.
                    else
                    {
                        this.button_CancelAction();
                        this.actionflag = Actionflag.idle;
                    }
                } // </ Alvo da ação != personagem escolhido>
                else
                {
                    Overhead_Spawner.spawnOverhead(4, OverHeads.playerText);
                    this.personagemEscolhido.force_terminarAcao();
                }
                

                break;

            case playersActions.Atacar:

                Cutscene_Handler.controller.start_Attack();
                
                this.personagemEscolhido.estadoAtual = CharacterState.atacando;

                Overhead_Spawner.spawnOverhead(2, OverHeads.playerText);

                break;

            case playersActions.Interagir:
                Cutscene_Handler.controller.start_Interaction();

                Overhead_Spawner.spawnOverhead(3, OverHeads.playerText);

                break;

            case playersActions.PassarTurno:

                if (this.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
                {
                    foreach (GameObject unidade in GameObject.FindGameObjectsWithTag(this.currentplayer.getTag()))
                    {
                        if (unidade != null)
                        {
                            unidade.GetComponent<GenericCharacter>().acoes = 0;
                            unidade.GetComponent<GenericCharacter>().podeAgir = false;
                        }
                    }

                    if (personagemEscolhido != null)
                    {
                        this.personagemEscolhido.force_terminarAcao();
                    }
                    else
                    {
                        GameController.actionFlagEnd();
                    }
                }

                break;

            case playersActions.RecarregarNivel:
                this.joystickStartbutton();
                this.personagemEscolhido.force_terminarAcao();
                this.reLoadLevel();

                break;

            default:
                Debug.Log("ExecutaAção invocado, mas nenhuma ação executada.");
                break;
        }

        if (this.personagemEscolhido != null)
        {
            this.desfazAreaDeAcao();
        }
    }

    /**
    /* @name: executaAcaoIA
    /* @version: 1.7
    /* @Description: Executa a ação escolhida pela IA
    */
    private void executaAcaoIA()
    {
        switch (this.acaoEscolhida)
        {
            case playersActions.Andar:
                // Encontra caminho usando algoritmo de Pathfinding da IA
                Path caminho = ia.findPathTo(this.personagemEscolhido.gridPosition,
                    this.alvoDaAcao.GetComponent<GenericCharacter>().gridPosition,
                    this.personagemEscolhido.alcance,
                    this.personagemEscolhido.minalcance);
                
                // Executa o caminho encontrado
                if (caminho != null)
                {
                    this.personagemEscolhido.andarParaIA(caminho);
                }
                else
                {
                    Debug.Log("IA_Caminho_Null");
                    this.personagemEscolhido.force_terminarAcao();
                }

                break;

            case playersActions.Atacar:
                Cutscene_Handler.controller.start_Attack();
                this.personagemEscolhido.estadoAtual = CharacterState.atacando;
                
                break;

            default:
                Debug.Log("IA_NAO_AGIU.");
                break;
        }
    }

    /**
    /* @name: loadGameOverScreen
    /* @version: 1.0
    /* @Description: Envia o jogador para a tela de gameover.
    */
    private void loadGameOverScreen()
    {
        MissionMenu_Handler.controller.MissionFail();
    }

    /**
    /* @name: quitToMenu
    /* @version: 1.0
    /* @Description: Retorna o jogador para o menu inicial
    */
    private void quitToMenu()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Menu);
    }

    /**
    /* @name: loadNextLevel
    /* @version: 2.5
    /* @Description: ativa tela de loading, descarrega objetos e inicia loading do novo nivel
    */
    public void loadNextLevel()
    {
        // Destroi objetos da fase anterior
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("GameCursor"))
        {
            GameObject.Destroy(objeto);
        }

        if (GameController.controller.personagemEscolhido != null)
        {
            GameController.controller.desfazAreaDeAcao();
        }

        MissionMenu_Handler.controller.restMissions();

        CanvasController.controller.hidePauseMenu();
        CanvasController.controller.hideActionPanel();
        CanvasController.controller.showLoadingScreen();

        this.jogadorPerdeSemUnidades = false;
        this.chegarMissoes = false;

        // Destroi objetos da fase anterior
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("semiObstacle"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("interativos"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Ground"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Demonios"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Jogador"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Missions"))
        {
            GameObject.Destroy(objeto);
        }

        // Carrega objetos da nova fase
        GameController.loadNewLevelFlag();
        LevelLoader.loadNextLevel();
    }

    /**
    /* @name: reLoadLevel
    /* @version: 1.3
    /* @Description: Reinicia o nivel.
    */
    public void reLoadLevel()
    {
        // Destroi objetos da fase anterior
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("GameCursor"))
        {
            GameObject.Destroy(objeto);
        }

        if (GameController.controller.personagemEscolhido != null)
        {
            GameController.controller.desfazAreaDeAcao();
        }

        MissionMenu_Handler.controller.restMissions();

        CanvasController.controller.hidePauseMenu();
        CanvasController.controller.hideActionPanel();
        CanvasController.controller.showLoadingScreen();

        this.jogadorPerdeSemUnidades = false;
        this.chegarMissoes = false;

        // Destroi objetos da fase anterior
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("semiObstacle"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("interativos"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Ground"))
        {
            GameObject.Destroy(objeto);
        }
        
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Demonios"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Jogador"))
        {
            GameObject.Destroy(objeto);
        }

        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag("Missions"))
        {
            GameObject.Destroy(objeto);
        }

        // Carrega objetos da fase
        GameController.loadNewLevelFlag();
    }
}