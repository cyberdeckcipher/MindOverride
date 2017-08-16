using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour{

    //Singleton Reference
    public static CanvasController controller;    // Variavel estática de acesso ao singleton do CanvasController

    // Slots para paineis
    public Animator panelAcao;                    // Painel que controla menu de ações do jogo.
    public Animator panelConfirmacao;             // Painel que controla o menu de confirmação.
    public Animator panelPause;                   // Painel que controla o fundo preto do menu do pause.
    public Animator panelPauseMenu;               // Painel que controle o menu de pause.
    public Animator panelMissions;                // Painel que controla a lista de missões.
    public Animator panelInteract;                // Painel que controla a tela de interação com texto.
    public Animator fadePanel;                    // Painel que controla efeito de fadein e fadeout para transição entre cenas.

    // Slots para cutscenes
    public Animator cutsceneBG;                   // Animação que controla a animação do BG de cutscene.
    public Animator panelNextJogador;             // Painel que controla a mudança de jogador.
    public Animator panelInitLevel;               // Painel que controla a mensagem da cutscene inicial.

    // Slots para objetos animados na tela.
    public Animator jornalInteract;               // Jornal animado que aparece na tela de interação de texto.

    // Slot para controle da tela de loading
    public GameObject loadingCanvas;              // Tela de loading.

    // Controle de propriedades
    public MenuCursorReactor pauseMenuControler;  // Controle do menu de pausa.
    public MenuCursorReactor ActionMenuControler; // Controle do menu de ação.

    void Awake()
    {
        if (CanvasController.controller == null)
        {
            CanvasController.controller = this;
        }
        else if (CanvasController.controller != this)
        {
            Destroy(this);
        }
    }

    /**
    /* @name: showActionPanel
    /* @version: 1.0
    /* @Description: Ativa animação do painel de ação, mostrando o menu na tela.
    */
    public void showActionPanel()
    {
        panelAcao.SetBool("onAction",true);
        this.showMissions();
        ActionMenuControler.isMenuSet = true;
    }

    /**
    /* @name: hideActionPanel
    /* @version: 1.1
    /* @Description: Ativa animação do painel de ação, retirando o menu da tela.
    */
    public void hideActionPanel()
    {
        panelAcao.SetBool("onAction", false);
        ActionMenuControler.isMenuSet = false;
        this.hideMissions();
        ActionMenuControler.resetMenu();
    }

    /**
    /* @name: showConfirmationPanel
    /* @version: 1.0
    /* @Description: Ativa animação do painel de confirmação, mostrando o menu da tela.
    */
    public void showConfirmationPanel()
    {
        panelConfirmacao.SetBool("onConfirmation", true);
    }

    /**
    /* @name: hideConfirmationPanel
    /* @version: 1.0
    /* @Description: Ativa animação do painel de confirmação, mostrando o menu da tela.
    */
    public void hideConfirmationPanel()
    {
        panelConfirmacao.SetBool("onConfirmation", false);
    }

    /**
    /* @name: showPauseMenu
    /* @version: 1.0
    /* @Description: Ativa animação do painel de pause, mostrando o menu da tela.
    */
    public void showPauseMenu()
    {
        panelPause.SetBool("onPause", true);
        panelPauseMenu.SetBool("onPause", true);

        if (pauseMenuControler.isActiveAndEnabled)
        {
            pauseMenuControler.isMenuSet = true;
        }
        
        if (GameController.controller.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
        {
            if (GameController.controller.estadoDoJogo == Gamestates.cutscene)
            {
                pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().forceInactive = true;
                pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().esteBotao.interactable = false;
            }
            else
            {
                pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().forceInactive = false;
                pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().esteBotao.interactable = true;
            }
        }
        else
        {
            pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().forceInactive = true;
            pauseMenuControler.itensDoMenu[1].GetComponent<MenuButtonAnimator>().esteBotao.interactable = false;
        }
    }

    /**
    /* @name: showPauseBG
    /* @version: 1.0
    /* @Description: Ativa animação do BG de pause, mostrando o menu da tela.
    */
    public void showPauseBG()
    {
        panelPause.SetBool("onPause", true);
    }

    /**
    /* @name: hidePauseMenu
    /* @version: 1.1
    /* @Description: Ativa animação do painel de pause, mostrando o menu da tela.
    */
    public void hidePauseMenu()
    {
        panelPause.SetBool("onPause", false);
        panelPauseMenu.SetBool("onPause", false);
        pauseMenuControler.isMenuSet = false;
        this.pauseMenuControler.resetMenu();
    }

    /**
    /* @name: showMissions
    /* @version: 1.1
    /* @Description: Ativa tela de missões.
    */
    public void showMissions()
    {
        panelMissions.SetBool("onMission", true);
    }

    /**
    /* @name: hideMissions
    /* @version: 1.1
    /* @Description: desativa tela de missões.
    */
    public void hideMissions()
    {
        panelMissions.SetBool("onMission", false);
    }

    /**
    /* @name: showLoadingScreen
    /* @version: 1.0
    /* @Description: Ativa tela de loading.
    */
    public void showLoadingScreen()
    {
        this.loadingCanvas.SetActive(true);
    }

    /**
    /* @name: hideLoadingScreen
    /* @version: 1.0
    /* @Description: desativa tela de loading.
    */
    public void hideLoadingScreen()
    {
        this.loadingCanvas.SetActive(false);
    }

    /**
    /* @name: showCutsceneBG
    /* @version: 1.0
    /* @Description: Ativa background da cutscene.
    */
    public void showCutsceneBG()
    {
        this.cutsceneBG.SetBool("startCutscene", true);
    }

    /**
    /* @name: showInitCutsceneBG
    /* @version: 1.0
    /* @Description: inicia background da cutscene.
    */
    public void showInitCutsceneBG()
    {
        this.cutsceneBG.SetBool("initCutscene", true);
    }

    /**
    /* @name: hideCutsceneBG
    /* @version: 1.3
    /* @Description: desativa background da cutscene e o painel de cutscene inicial ao mesmo tempo.
    */
    public void hideCutsceneBG()
    {
        this.cutsceneBG.SetBool("startCutscene", false);
        this.cutsceneBG.SetBool("initCutscene", false);
    }

    /**
   /* @name: showPanelInitLevel
   /* @version: 1.0
   /* @Description: Ativa animação do painel da cutscene de inicio de fase.
   */
    public void showPanelInitLevel()
    {
        this.panelInitLevel.SetBool("onInitLevel", true);
    }

    /**
    /* @name: hideCutsceneBG
    /* @version: 1.0
    /* @Description: Desativa animação do painel da cutscene de inicio de fase.
    */
    public void hidePanelInitLevel()
    {
        this.panelInitLevel.SetBool("onInitLevel", false);
    }

    /**
    /* @name: showNextPlayerPanel
    /* @version: 1.0
    /* @Description: Ativa animação do painel de Next Player.
    */
    public void showNextPlayerPanel()
    {
        this.panelNextJogador.SetBool("onNextPlayer", true);
    }

    /**
    /* @name: hideNextPlayerPanel
    /* @version: 1.0
    /* @Description: Desativa animação do painel de Next Player.
    */
    public void hideNextPlayerPanel()
    {
        this.panelNextJogador.SetBool("onNextPlayer", false);
    }

    /**
    /* @name: showInteractTextPanel
    /* @version: 1.2
    /* @Description: Ativa animação do painel de interacao.
    */
    public void showInteractTextPanel()
    {
        this.showPauseBG();
        this.panelInteract.SetBool("onTextInteract", true);
        this.jornalInteract.SetBool("onInteract", true);
        GameController.controller.estadoDoJogo = Gamestates.interactText;
    }

    /**
    /* @name: showMissionTextPanel
    /* @version: 1.0
    /* @Description: Ativa animação do painel de interacao, sem o jornal
    */
    public void showMissionTextPanel()
    {
        this.showPauseBG();
        this.panelInteract.SetBool("onTextInteract", true);
        GameController.controller.estadoDoJogo = Gamestates.interactText;
    }

    /**
    /* @name: hideInteractTextPanel
    /* @version: 1.2
    /* @Description: Desativa animação do painel de interacao.
    */
    public void hideInteractTextPanel()
    {
        panelPause.SetBool("onPause", false);
        this.panelInteract.SetBool("onTextInteract", false);
        this.jornalInteract.SetBool("onInteract", false);

        // Somente ao final da interação, a ação do personagem acaba
        if (GameController.controller.personagemEscolhido != null)
        {
            GameController.controller.personagemEscolhido.force_terminarAcao();
        }
        GameController.controller.estadoDoJogo = Gamestates.playing;
    }

    // ---------------------------------- Transitions Effects ----------------------------------

    /**
    /* @name: fadeIn
    /* @version: 1.0
    /* @Description: Ativa efeito de fade in
    */
    public void fadeIn()
    {
        this.fadePanel.SetBool("onFade", true);
    }

    /**
    /* @name: fadeOut
    /* @version: 1.0
    /* @Description: Ativa efeito de fadeOut
    */
    public void fadeOut()
    {
        this.fadePanel.SetBool("onFade", false);
    }
}
