using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cutscene_Handler : MonoBehaviour {

    // controla qual mensagem aparece na tela.
    public GameObject proximoJogadorHumano;       // Animação para mudança de jogador: Humano.
    public GameObject proximoJogadorMaquina;      // Animação para mudança de jogador: Maquina.
    public GameObject missaoCumprida;             // Animação de missão cumprida.
    public GameObject missaoFalhou;               // Animação de missão falha.
    
    public GameObject cutsceneLevel1;             // Texto da cutscene 1.
    public GameObject cutsceneLevel2;             // Texto da cutscene 2.
    public GameObject cutsceneLevel3;             // Texto da cutscene 3.

    // Controle de tempo da cutscene
    public float tempoRestante;                   // Contagem regressiva para temianr uma cutscene
    public float tempoAteProximoNivel;            // Tempo restante até passar pra proxima fase, permite terminar as animações nescessárias
    public bool contandoAteDesativarCutscene;     // True caso contagem de tempo restante tenha começado a ser contado.
    public bool contandoAteProximoNivel;          // True caso a cutscene de proximo nivel tenha terminado, ativa a contagem regressiva para carregar a proxima fase.
    public const float minTempoNextPlayer = 1.2f; // Tempo minimo que a cutscene de aviso de mudança de player deve ficar ativo. Este tempo é contado sempre que termina a cutscene
    public int nextScene;                         // Proxima scena a ser carregada pelo jogo.

    //Controle do tipo de cutscene
    public CutsceneType tipoDaCutscene;           // Tipo da cutscene
    public static bool initMissionUpdate;               // true caso tenha passado pela cutscene init

    public static Cutscene_Handler controller;

    void Awake()
    {
        if (Cutscene_Handler.controller == null)
        {
            Cutscene_Handler.controller = this;
        }
        else if (Cutscene_Handler.controller != this)
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start()
    {
        tipoDaCutscene = CutsceneType.idle;
        contandoAteProximoNivel = false;
        contandoAteDesativarCutscene = false;
        this.nextScene = -1;
        initMissionUpdate = false;
    }

	// Update is called once per frame
	void Update () {

        // Em caso de FadeIN, ao final da cutscene carreta a proxima cena, Ou fecha o jogo, dependendo do parametro passado
        if (this.tipoDaCutscene == CutsceneType.idle && this.nextScene > -1)
        {
            if (this.nextScene == 999)
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(this.nextScene, LoadSceneMode.Single);
            }
            
        }

        //Se o tempo restante estiver ativo, começa a contar o tempo
        if (tempoRestante > 0)
        {
            tempoRestante -= Time.deltaTime;
        }
        else if (this.contandoAteDesativarCutscene)
        {
            contandoAteDesativarCutscene = false;
            endCutscene();
        }

        // Contagem regressiva até a passagem de nivel
        if (tempoAteProximoNivel > 0)
        {
            tempoAteProximoNivel -= Time.deltaTime;
        }
        else if (contandoAteProximoNivel)
        {
            this.contandoAteProximoNivel = false;

            // Carrega proxima fase
            GameController.controller.loadNextLevel();
        }
    }

    /**
    /* @name: endCutscene
    /* @version: 1.8
    /* @Description: Desativa telas e animações de cutscene para retornar o jogo a posição de idle
    */
    public void endCutscene()
    {
        switch (tipoDaCutscene)
        {
            case CutsceneType.levelInit:
                CanvasController.controller.hideCutsceneBG();
                CanvasController.controller.hidePanelInitLevel();

                this.start_NextPlayer(PlayersTags.Demonios.ToString());
                Cutscene_Handler.initMissionUpdate = true;
                break;

            case CutsceneType.nextPlayer:
                CanvasController.controller.hideNextPlayerPanel();
                this.tipoDaCutscene = CutsceneType.idle;
                GameController.controller.estadoDoJogo = Gamestates.playing;

                if (GameController.controller.getCurrentTag().Equals(PlayersTags.Jogador.ToString()) && Cutscene_Handler.initMissionUpdate)
                {
                    // Atualiza imagens da UI de missões.
                    MissionMenu_Handler.controller.updateMissionOnUI();
                }

                break;

            case CutsceneType.battle:
                GameController.controller.alvoDaAcao.GetComponent<GenericCharacter>().atacado(GameController.controller.personagemEscolhido.dano);
                break;

            case CutsceneType.interact:
                GameController.controller.interagirCom();
                break;

            case CutsceneType.levelClear:
                this.proximoJogadorHumano.SetActive(false);
                this.proximoJogadorMaquina.SetActive(false);
                this.missaoCumprida.SetActive(false);
                this.missaoFalhou.SetActive(false);

                CanvasController.controller.hideNextPlayerPanel();
                CanvasController.controller.hidePauseMenu();
                CanvasController.controller.showCutsceneBG();

                this.tempoAteProximoNivel = Cutscene_Handler.minTempoNextPlayer;
                contandoAteProximoNivel = true;
                this.tipoDaCutscene = CutsceneType.idle;
                break;

            case CutsceneType.gameOver:
                this.proximoJogadorHumano.SetActive(false);
                this.proximoJogadorMaquina.SetActive(false);
                this.missaoCumprida.SetActive(false);
                this.missaoFalhou.SetActive(false);

                CanvasController.controller.hideNextPlayerPanel();
                CanvasController.controller.hidePauseMenu();
                CanvasController.controller.showCutsceneBG();

                this.tipoDaCutscene = CutsceneType.idle;
                this.fadeInTo(Scene.GameOver);
                break;

            case CutsceneType.fadeIN:
                this.tipoDaCutscene = CutsceneType.idle;
                break;
            
            case CutsceneType.fadeOUT:
                this.tipoDaCutscene = CutsceneType.idle;
                break;
        }
    }

    /**
    /* @name: start_LevelCutscene
    /* @version: 1.6
    /* @Description: Inicia a cutscene de novo nivel. pausando todas as ações até o fim da animação.
   */
    public void start_LevelCutscene()
    {
        this.tipoDaCutscene = CutsceneType.levelInit;

        CanvasController.controller.showInitCutsceneBG();
        CanvasController.controller.showPanelInitLevel();

        AudioController.sfx_Play_Cutscene();

        switch (GameController.nivelCarregadoAtual)
        {
            case 1:
                this.cutsceneLevel1.SetActive(true);
                this.cutsceneLevel2.SetActive(false);
                this.cutsceneLevel3.SetActive(false);
                break;

            case 2:
                this.cutsceneLevel1.SetActive(false);
                this.cutsceneLevel2.SetActive(true);
                this.cutsceneLevel3.SetActive(false);
                break;

            case 3:
                this.cutsceneLevel1.SetActive(false);
                this.cutsceneLevel2.SetActive(false);
                this.cutsceneLevel3.SetActive(true);
                break;
        }

        GameController.controller.estadoDoJogo = Gamestates.cutscene;

        //Inicia contagem para desligar a cutscene
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = 3;
    }

    /**
    /* @name: start_NextPlayer
    /* @version: 1.8
    /* @Description: Inicia a cutscene de nextplayer. pausando todas as ações até o fim da animação.
   */
    public void start_NextPlayer(string tagDoJogador)
    {
        //Inicia as animações
        this.tipoDaCutscene = CutsceneType.nextPlayer;
        CanvasController.controller.showNextPlayerPanel();

        this.missaoCumprida.SetActive(false);
        this.missaoFalhou.SetActive(false);

        //Ativa qual mensagem deve aparecer na animação
        if (tagDoJogador == PlayersTags.Jogador.ToString())
        {
            this.proximoJogadorMaquina.SetActive(false);
            this.proximoJogadorHumano.SetActive(true);
            AudioController.flipMusic(AudioContexts.playerTurn, minTempoNextPlayer);
        }
        else if (tagDoJogador == PlayersTags.Demonios.ToString())
        {
            this.proximoJogadorHumano.SetActive(false);
            this.proximoJogadorMaquina.SetActive(true);
            AudioController.flipMusic(AudioContexts.enemyTurn, minTempoNextPlayer);
        }

        //Inicia contagem para desligar a cutscene
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = minTempoNextPlayer;

        //Trava o jogador para agir somente ao final da cutscene
        GameController.controller.estadoDoJogo = Gamestates.cutscene;
    }

    /**
    /* @name: start_MissionComplete
    /* @version: 1.2
    /* @Description: Inicia a cutscene de termino de nivel. pausando todas as ações.
   */
    public void start_MissionComplete()
    {
        //trava o jogo para outras ações.
        GameController.controller.estadoDoJogo = Gamestates.cutscene;

        //Inicia as animações
        CanvasController.controller.showPauseBG();
        this.tipoDaCutscene = CutsceneType.levelClear;
        CanvasController.controller.showNextPlayerPanel();

        // ativa mensagem correta
        this.proximoJogadorHumano.SetActive(false);
        this.proximoJogadorMaquina.SetActive(false);
        this.missaoCumprida.SetActive(true);

        //Inicia contagem para desligar a cutscene
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = minTempoNextPlayer * 3;
    }

    /**
    /* @name: start_MissionFail
    /* @version: 1.1
    /* @Description: Inicia a cutscene de game over. pausando todas as ações.
   */
    public void start_MissionFail()
    {
        //trava o jogo para outras ações.
        GameController.controller.estadoDoJogo = Gamestates.cutscene;

        //Inicia as animações
        CanvasController.controller.showPauseBG();
        this.tipoDaCutscene = CutsceneType.gameOver;
        CanvasController.controller.showNextPlayerPanel();

        // ativa mensagem correta
        this.proximoJogadorHumano.SetActive(false);
        this.proximoJogadorMaquina.SetActive(false);
        this.missaoCumprida.SetActive(false);
        this.missaoFalhou.SetActive(true);

        //Inicia contagem para desligar a cutscene
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = minTempoNextPlayer * 2;
    }

    /**
    /* @name: start_Interaction
    /* @version: 1.2
    /* @Description: Toca animação do personagem e ao fim, abre a tela com as informações.
    */
    public void start_Interaction()
    {
        // Atualiza valores de controle para travar movimentos.
        this.tipoDaCutscene = CutsceneType.interact;
        GameController.controller.estadoDoJogo = Gamestates.cutscene;

        // Gira o personagem em direção ao alvo.
        GameController.controller.personagemEscolhido.giraPersonagem(GameController.controller.personagemEscolhido.gridPosition,
                                                                        GameController.controller.alvoDaAcao.GetComponent<InteractActor>().gridposition);

        // Inicia a animação do personagem.
        GameController.controller.personagemEscolhido.GetComponent<Animator>().SetTrigger("Interact");

        //Inicia contagem de tempo para fim da cutscene de animação.
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = minTempoNextPlayer * 2;
    }

    /**
    /* @name: start_Attack
    /* @version: 1.1
    /* @Description: Toca animação de ataque e ao fim, diminui life das unidades com as informações.
   */
    public void start_Attack()
    {
        this.tipoDaCutscene = CutsceneType.battle;

        GameController.controller.personagemEscolhido.giraPersonagem(GameController.controller.personagemEscolhido.gridPosition, 
                                                                        GameController.controller.alvoDaAcao.GetComponent<GenericCharacter>().gridPosition);
        GameController.controller.alvoDaAcao.GetComponent<GenericCharacter>().giraPersonagem(GameController.controller.alvoDaAcao.GetComponent<GenericCharacter>().gridPosition, 
                                                                                                GameController.controller.personagemEscolhido.gridPosition);

        if (GameController.controller.personagemEscolhido.tipoDeAtaque == AttackType.melee)
        {
            GameController.controller.personagemEscolhido.GetComponent<Animator>().SetTrigger("Melee");
        }
        else
        {
            GameController.controller.personagemEscolhido.GetComponent<Animator>().SetTrigger("Ranged");
        }

        //Inicia contagem para desligar a cutscene
        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = minTempoNextPlayer;
    }

    /**
    /* @name: fadeIn
    /* @version: 1.0
    /* @Description: Inicia cutscene de FadeIn
   */
    public void fadeInTo(int proximaCena)
    {
        this.nextScene = proximaCena;
        CanvasController.controller.fadeIn();
        this.tipoDaCutscene = CutsceneType.fadeIN;

        //Avisa ao controle de audio da mudança na cena
        AudioController.changeContextFlag(proximaCena);

        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = 0.6f;
    }

    /**
    /* @name: fadeOut
    /* @version: 1.0
    /* @Description: Inicia Cutscene de FadeOut
   */
    public void fadeOut()
    {
        CanvasController.controller.fadeOut();
        this.tipoDaCutscene = CutsceneType.fadeOUT;

        this.contandoAteDesativarCutscene = true;
        this.tempoRestante = 0.6f; 
    }
}
