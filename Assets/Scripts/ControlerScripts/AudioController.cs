using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    //Controle de transição entre cenas
    public bool isMusicPlaying;                                // true caso já tenha alguma musica tocando.
    public AudioContState contextChangeState;                  // Estados de controle da mudança de contexto de audio.
    public AudioContexts currentContext;                       // Contexto de audio que está sendo executado no momento.
    public AudioContexts nextContext;                          // Proximo contexto para transição.
    public float maxDuracaoDaTransicao;                        // Duração máxima total da transição de audio.
    public float transition_time;                              // Duração total da transição de audio.
    public float transition_Timelapsed;                        // Valor atual da transição de audio.

    // Referencia de AudioSources
    public AudioSource musicSourceA;                            // AudioSource responsavel por tocar a musica de cena.
    public AudioSource musicSourceB;                            // AudioSource responsavel por tocar a musica de cena.
    private AudioSource currentMusicSource;                     // Referencia a fonte de musica ativa.
    private AudioSource secundaryMusicSource;                   // Referencia a fonre de musica secundaria, usada para transicao.

    public AudioSource sfxSourceA;                              // AudioSource responsavel por tocar a musica de cena.
    public AudioSource sfxSourceB;                              // AudioSource responsavel por tocar a musica de cena.

    public AudioSource AmbienceSourceA;                         // AudioSource responsavel por tocar a ambiencia de cena.
    public AudioSource AmbienceSourceB;                         // AudioSource responsavel por tocar a ambiencia de cena.
    private AudioSource currentAmbienceSource;                  // Referencia a fonte de ambiencia ativa.
    private AudioSource secundaryAmbienceSource;                // Referencia a fonre de ambiencia secundaria, usada para transicao.

    // Referencia de snapshots do Audio Mixer.
    public AudioMixerSnapshot standard;                         // Snapshot com todos os efeitos desativados.
    public AudioMixerSnapshot pausedGameplay;                   // Snapshot com efeito lowpass e de volume para tela pausada.
    public AudioMixerSnapshot creditsFix;                       // Snapshot com efeito de glitch.
    public AudioMixerSnapshot steps;                            // Snapshot com efeito de volume para passos.
    public AudioMixerSnapshot AMB;                              // Snapshot com efeito para ambiencias.

    // Musicas do jogo
    public AudioClip theme_menu;                                // Musica tema do menu.
    public AudioClip theme_Gameplay1;                           // Musica do gameplay 1.
    public AudioClip theme_Gameplay2;                           // Musica do gameplay 2.
    public AudioClip theme_Credits;                             // Musica dos créditos.
    public AudioClip theme_GameOver;                            // Musica da tela de Game Over.
    public AudioClip theme_Loading;                             // Musica da tela de Loading.

    // Ambiencia de fases

    public AudioClip amb_street;                                // Ambiencia da rua.
    public AudioClip amb_building;                              // Ambiencia do escritorio.
    public AudioClip amb_lab;                                   // Ambiencia do laboratorio.

    // Efeitos Sonoros
    public AudioClip sfx_LevelCutscene;                         // Efeito sonoro de Cutscene no começo da fase.
    public AudioClip sfx_MenuChange;                            // Efeito sonoro de mudança de item do menu.
    public AudioClip sfx_MenuClick;                             // Efeito sonoro de escolha de item do menu.

    // Efeitos sonoros de personagem
    public AudioClip sfx_Step01;                                // Efeito sonoro de passo 1.
    public AudioClip sfx_Step02;                                // Efeito sonoro de passo 2.
    public AudioClip sfx_Punch;                                 // Efeito sonoro de soco.
    public AudioClip sfx_Shot;                                  // Efeito sonoro de tiro.
    public AudioClip sfx_Damage;                                // Efeito sonoro de dano.
    public AudioClip sfx_Death;                                 // Efeito sonoro de morte.

    // Efeitos sonoros de interação
    public AudioClip sfx_FlipPaper;                             // Efeito sonoro de papel.
    public AudioClip sfx_TypeComputer;                          // Efeito sonoro de computador.
    public AudioClip sfx_Door;                                  // Efeito sonoro de abrir Porta.


    //Rerefencia estática
    public static AudioController controller;                   // Referencia estática ao controlador.

    // Garante que o objeto não irá ser destruido e configura sua referencia estática.
    void Awake()
    {
        if (AudioController.controller == null)
        {
            // Atualiza referencia a objeto estático
            AudioController.controller = this;

            // Torna objeto unico no jogo e imortal
            DontDestroyOnLoad(this.gameObject);
            
            // Inicia variaveis de controle de musica
            this.currentMusicSource = musicSourceA;
            this.secundaryMusicSource = musicSourceB;

            this.currentMusicSource.volume = 0;
            this.secundaryMusicSource.volume = 0;

            // Inicia variaveis de controle de ambiencia
            this.currentAmbienceSource = AmbienceSourceA;
            this.secundaryAmbienceSource = AmbienceSourceB;

            this.currentAmbienceSource.volume = 0;
            this.secundaryAmbienceSource.volume = 0;

            // Inicializa o valor do contexto de audio atual do objeto
            transition_time = maxDuracaoDaTransicao/2;
            changeContextFlag(SceneManager.GetActiveScene().buildIndex);
        }
        else if (AudioController.controller != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        if (!this.isMusicPlaying)
        {
            this.start_Music();
        }
    }

    void Update()
    {

        if (contextChangeState != AudioContState.idle)
        {
            switch (contextChangeState)
            {
                case AudioContState.change:
                    //Atualiza variaveis iniciais antes da transição
                    this.transition_Timelapsed = 0.0001f;
                    transition_time = maxDuracaoDaTransicao;

                    // Garante que ambas as fontes de musica estão tocando para executar a transição
                    this.currentMusicSource.Play();
                    this.secundaryMusicSource.Play();
                    
                    // Inicia a transição
                    contextChangeState = AudioContState.changing;

                    break;

                case AudioContState.changing:

                    // Anda um passo no lerp
                    musicTransitionLerpStep();

                    // Caso o tempo passado desde o inicio da transição seja igual ao tempo da transição total, a transição termina
                    if (this.transition_Timelapsed == this.transition_time)
                    {
                        // Ao completar a transição, Garante que os volumes estão maximizados para os extremos
                        this.secundaryMusicSource.volume = 1;
                        this.currentMusicSource.volume = 0;

                        // A fonte secundária agora torna-se a principal e a antiga principal é colocada na posicao secundaria através de um swap simples.
                        swapAudioSources();
                        
                        // Atualiza marcador de contexto
                        this.currentContext = this.nextContext;

                        // Conclui o fim da transicao ao retornar o estado da mudança para idle.
                        this.contextChangeState = AudioContState.idle;
                    }
                    break;
            }
        }
    }

    /**
    /* @name: musicTransitionLerpStep
    /* @version: 1.0
    /* @Description: Dá um passo na transição de lerp usando função de tempo
    */
    private void musicTransitionLerpStep()
    {
        float lerpFragmentFromTime = (transition_Timelapsed / transition_time);

        //lerp de volume é executado
        // CurrentMusic SEMPRE vai diminuir até 0.
        if (this.currentMusicSource.volume > 0)
        {
            this.currentMusicSource.volume = Mathf.Lerp(this.currentMusicSource.volume, 0, lerpFragmentFromTime);    // Lerp de 1 a 0;
        }
        // SecondaryMusic SEMPRE vai aumentar até 1.
        if (this.secundaryMusicSource.volume < 1)
        {

            this.secundaryMusicSource.volume = Mathf.Lerp(0, 1, lerpFragmentFromTime); // Lerp de 0 a 1;
        }

        // Atualiza a quantidade de tempo que já passou
        this.transition_Timelapsed += Time.deltaTime;
        //S e passar do valor máximo, corrige o excesso
        if (this.transition_Timelapsed > this.transition_time)
        {
            this.transition_Timelapsed = this.transition_time;
        }
    }

    /**
    /* @name: swapAudioSources
    /* @version: 1.0
    /* @Description: Troca a posição do current e secundary audio sorce para controle de transição.
    */
    private void swapAudioSources()
    {
        AudioSource fonteAntiga = this.currentMusicSource;
        this.currentMusicSource = this.secundaryMusicSource;

        this.secundaryMusicSource = fonteAntiga;
    }

    /**
    /* @name: start_Music
    /* @version: 1.1
    /* @Description: Inicia uma musica dependendo do contexto.
    */
    private void start_Music()
    {
        
        switch (this.currentContext)
        {
            case AudioContexts.premenu:
                this.currentMusicSource.clip = this.theme_menu;
                this.currentMusicSource.Play();
                this.secundaryMusicSource.Play();
                break;

            case AudioContexts.menu:
                this.currentMusicSource.clip = this.theme_menu;
                this.currentMusicSource.Play();
                this.secundaryMusicSource.Play();
                break;

            case AudioContexts.loadingGameplay:
                this.currentMusicSource.clip = this.theme_Loading;
                this.currentMusicSource.Play();
                this.secundaryMusicSource.Play();
                break;

            case AudioContexts.gameover:
                this.currentMusicSource.clip = this.theme_GameOver;
                this.currentMusicSource.Play();
                this.secundaryMusicSource.Play();
                break;
        }
    }

    /**
    /* @name: changeContextFlag
    /* @version: 1.1
    /* @Description: Atualiza o proximo contexto e marca variavel de controle para transição
    */
    public static void changeContextFlag(int proximaCena)
    {
        AudioController control = AudioController.controller;
        
        control.contextChangeState = AudioContState.change;
        AudioController.activate_StandardAudioEffect(control.maxDuracaoDaTransicao);

        switch (proximaCena)
        {
            case Scene.Require:
                control.nextContext = AudioContexts.premenu;
                control.secundaryMusicSource.clip = control.theme_menu;
                break;

            case Scene.Menu:
                control.nextContext = AudioContexts.menu;
                control.secundaryMusicSource.clip = control.theme_menu;
                break;

            case Scene.Credits:
                control.nextContext = AudioContexts.credits;
                control.secundaryMusicSource.clip = control.theme_Credits;
                break;

            case Scene.Gameplay:
                control.nextContext = AudioContexts.loadingGameplay;
                control.secundaryMusicSource.clip = control.theme_Loading;
                AudioController.activate_PausedEffect();
                break;

            case Scene.GameOver:
                control.nextContext = AudioContexts.gameover;
                control.secundaryMusicSource.clip = control.theme_GameOver;
                AudioController.activate_PausedEffect();
                break;
        }

        // Caso a transição seja entre as cenas de menu e pre-menu, não deverá haver mudança de musica.
        if (control.currentContext == AudioContexts.premenu &&
            control.nextContext == AudioContexts.menu)
        {
            control.contextChangeState = AudioContState.idle;
        }
    }

    /**
    /* @name: changeContextFlag
    /* @version: 1.2
    /* @Description: Atualiza o contexto para troca de musica sem troca de cena.
    */
    private void flipMusicFix( AudioContexts newNextContext)
    {
        AudioController control = AudioController.controller;

        control.nextContext = newNextContext;

        switch (this.nextContext)
        {
            case AudioContexts.playerTurn:
                
                if (control.secundaryMusicSource.clip != control.theme_Gameplay1)
                {
                    control.secundaryMusicSource.clip = control.theme_Gameplay1;
                }
                break;

            case AudioContexts.enemyTurn:
                if (control.secundaryMusicSource.clip != control.theme_Gameplay2)
                {
                    control.secundaryMusicSource.clip = control.theme_Gameplay2;
                }
                break;
        }

        // Caso a transição seja entre as cenas de menu e pre-menu, não deverá haver mudança de musica.
        if (control.currentContext == control.nextContext)
        {
            control.contextChangeState = AudioContState.idle;
        }
    }

    /**
    /* @name: flipMusic
    /* @version: 1.2
    /* @Description: Muda a musica da cena para a seu segundo tema.
    */
    public static void flipMusic(AudioContexts newNextContext, float time = .1f)
    {
        AudioController.controller.contextChangeState = AudioContState.change;
        AudioController.controller.flipMusicFix(newNextContext);
    }

    // --------------------------------------------------------- SFX PLAY ---------------------------------------------------------

    /**
    /* @name: activate_PausedEffect
    /* @version: 1.0
    /* @Description: Ativa o efeito de pause através de transição para o snapshot de pause no audio mixer.
    */
    public static void activate_PausedEffect(float time = .2f)
    {
        AudioController.controller.pausedGameplay.TransitionTo(0.2f);
    }

    /**
    /* @name: activate_StandardAudioEffect
    /* @version: 1.0
    /* @Description: Retorna para condição normal de audio no audio mixer.
    */
    public static void activate_StandardAudioEffect(float time = .2f)
    {
        AudioController.controller.standard.TransitionTo(0.2f);
    }

    /**
    /* @name: activate_CreditsFix
    /* @version: 1.0
    /* @Description: Ativa o concerto de volume da musica de créditos através de transição para o snapshot de pause no audio mixer.
    */
    public static void activate_CreditsFix(float time = .1f)
    {
        AudioController.controller.creditsFix.TransitionTo(0.1f);
    }

    /**
    /* @name: sfx_Play_Cutscene
    /* @version: 1.0
    /* @Description: Toca o efeito de som de cutscene no sfx.
    */
    public static void sfx_Play_Cutscene()
    {
        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_LevelCutscene;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_MenuChange
    /* @version: 1.0
    /* @Description: Toca o efeito de som de mudança de menu no sfx.
    */
    public static void sfx_Play_MenuChange()
    {
        // Não muda o snapshot de audio caso o jogo estiver em pause
        if((GameController.controller != null))
        {
            if (!(GameController.controller.estadoDoJogo == Gamestates.paused))
            {
                AudioController.controller.standard.TransitionTo(0.001f);
            }
        }
        
        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_MenuChange;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_MenuClick
    /* @version: 1.0
    /* @Description: Toca o efeito de som de click de menu no sfx.
    */
    public static void sfx_Play_MenuClick()
    {
        // Não muda o snapshot de audio caso o jogo estiver em pause
        if ((GameController.controller != null))
        {
            if (!(GameController.controller.estadoDoJogo == Gamestates.paused))
            {
                AudioController.controller.standard.TransitionTo(0.001f);
            }
        }

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_MenuClick;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Passo1
    /* @version: 1.0
    /* @Description: Toca o efeito de som de passo1 no sfx.
    */
    public static void sfx_Play_Passo1()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_Step01;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Passo2
    /* @version: 1.0
    /* @Description: Toca o efeito de som de passo2 no sfx.
    */
    public static void sfx_Play_Passo2()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_Step02;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Punch
    /* @version: 1.0
    /* @Description: Toca o efeito de som de soco no sfx.
    */
    public static void sfx_Play_Punch()
    {
        AudioController.controller.standard.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_Punch;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Shot
    /* @version: 1.0
    /* @Description: Toca o efeito de som de soco no sfx.
    */
    public static void sfx_Play_Shot()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_Shot;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Damage
    /* @version: 1.0
    /* @Description: Toca o efeito de som de dano no sfx.
    */
    public static void sfx_Play_Damage()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceB.clip = AudioController.controller.sfx_Damage;
        AudioController.controller.sfxSourceB.volume = 1;
        AudioController.controller.sfxSourceB.Play();
    }

    /**
    /* @name: sfx_Play_Death
    /* @version: 1.0
    /* @Description: Toca o efeito de som de morte no sfx.
    */
    public static void sfx_Play_Death()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceB.clip = AudioController.controller.sfx_Death;
        AudioController.controller.sfxSourceB.volume = 1;
        AudioController.controller.sfxSourceB.Play();
    }

    /**
    /* @name: sfx_Play_Door
    /* @version: 1.0
    /* @Description: Toca o efeito de som de porta no sfx.
    */
    public static void sfx_Play_Door()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceB.clip = AudioController.controller.sfx_Door;
        AudioController.controller.sfxSourceB.volume = 1;
        AudioController.controller.sfxSourceB.Play();
    }

    /**
    /* @name: sfx_Play_NewsPaper
    /* @version: 1.0
    /* @Description: Toca o efeito de som de jornal no sfx.
    */
    public static void sfx_Play_NewsPaper()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_FlipPaper;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }

    /**
    /* @name: sfx_Play_Computer
    /* @version: 1.0
    /* @Description: Toca o efeito de som de computador no sfx.
    */
    public static void sfx_Play_Computer()
    {
        AudioController.controller.steps.TransitionTo(0.001f);

        AudioController.controller.sfxSourceA.clip = AudioController.controller.sfx_TypeComputer;
        AudioController.controller.sfxSourceA.volume = 1;
        AudioController.controller.sfxSourceA.Play();
    }
}
