using UnityEngine;
using System.Collections;

public class MissionMenu_Handler : MonoBehaviour {

    // Referencia estática
    public static MissionMenu_Handler controller;   // Referencia estática do controlador.

    // Referencia a posição dos objetos
    public GameObject container;                    // Objeto pai que receberá os objetos de missões.

    // Controle de missões
    public InteractActor[] missions;                // Missões da fase.
    public InteractActor missaoAtiva;               // Missão que está sendo executada.
    public int tail;                                // final da lista.

    public GameObject bGTop;                        // Background da UI de missão para o topo;
    public GameObject bGMid;                        // Background da UI de missão para o meio;
    public GameObject bGBot;                        // Background da UI de missão para baixo;

    public GameObject lastImageBG;                  // Referencia da posição da ultima peça do BG

    public const int maxMissions = 10;


    void Awake()
    {
        if (MissionMenu_Handler.controller == null)
        {
            MissionMenu_Handler.controller = this;
        }
        else if (MissionMenu_Handler.controller != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {
        this.restMissions();
    }
	
	// Update is called once per frame
	void Update () {
    }

    /**
    /* @name: restMissions
    /* @version: 1.0
    /* @Description: Deleta todas as missões da lista e retorna os valores iniciais do controlador.
    */
    public void restMissions()
    {
        this.missions = new InteractActor[10];
        this.tail = 0;
    }

    /**
    /* @name: addMission
    /* @version: 1.0
    /* @Description: Adiciona uma missão a lista de missões do jogo.
    */
    public void addMission( InteractActor newMission)
    {
        this.missions[tail] = newMission;
        this.tail++;
    }

    /**
    /* @name: MissionFail
    /* @version: 1.0
    /* @Description: Inicia cutscene de missão Falha e ao final transporta para o Game Over.
    */
    public void MissionFail()
    {
        Cutscene_Handler.controller.start_MissionFail();
    }

    /**
    /* @name: isMissionClear
    /* @version: 1.0
    /* @Description: true caso todas as missões tenham sido concluidas
    */
    public bool isMissionClear()
    {
        foreach (InteractActor missao in this.missions)
        {
            if (missao != null)
            {
                // Se ouver uma missão na lista que não tenha sido completada
                if (!missao.getMissionCleared())
                {
                    return false;
                }
            }
        }
        return true;
    }

    /**
    /* @name: updateMissionOnUI
    /* @version: 1.0
    /* @Description: Atualiza as imagens na tela.
    */
    public void updateMissionOnUI()
    {
        int imagePosition = 0;  // Variavel que controla a posição de cada imagem na tela.
        GameObject imageBG = null;

        foreach (InteractActor missao in this.missions)
        {
            if ( missao != null)
            {
                switch (missao.UIState)
                {
                    case MissionUIState.toBeShown:

                        imageBG = spawnImageOnUI(missao, imagePosition);

                        if (imagePosition == 0)
                        {
                            imageBG.GetComponent<SpriteRenderer>().sprite = this.bGTop.GetComponent<SpriteRenderer>().sprite;
                        }

                        missao.UIState = MissionUIState.showing;

                        if (missao.UI_Texto != null)
                        {
                            if (this.missaoAtiva == null)
                            {
                                this.missaoAtiva = missao;
                            }
                            else
                            {
                                this.missaoAtiva = missao;
                            }
                            CanvasController.controller.showMissionTextPanel();
                            AudioController.activate_PausedEffect(GameController.controller.velo_interact_anim);
                        }

                        // Atualiza valor de ID
                        imagePosition++;
                        break;

                    case MissionUIState.showing:

                        missao.UI_BG.transform.localPosition = new Vector3(0, calculateUIPosition(imagePosition), 0);
                        missao.UI_Texture.transform.localPosition = new Vector3(0, calculateUIPosition(imagePosition+1) + 25, 0);

                        if (missao.getMissionCleared())
                        {
                            missao.UI_Texture.GetComponent<SpriteRenderer>().sprite = missao.UIText_DONE;
                        }

                        // Atualiza valor de ID
                        imagePosition++;

                        break;

                    case MissionUIState.chain:

                        foreach (InteractActor outraMissao in this.missions)
                        {
                            if (outraMissao != null)
                            {
                                if (outraMissao.missionID == missao.chainID)
                                {
                                    if (outraMissao.missionCleard)
                                    {
                                        imageBG = spawnImageOnUI(missao, imagePosition);

                                        if (imagePosition == 0)
                                        {
                                            imageBG.GetComponent<SpriteRenderer>().sprite = this.bGTop.GetComponent<SpriteRenderer>().sprite;
                                        }

                                        missao.UIState = MissionUIState.showing;

                                        if (missao.UI_Texto != null)
                                        {
                                            if (this.missaoAtiva == null)
                                            {
                                                this.missaoAtiva = missao;
                                            }
                                            else
                                            {
                                                this.missaoAtiva = missao;
                                            }
                                            CanvasController.controller.showMissionTextPanel();
                                            AudioController.activate_PausedEffect(GameController.controller.velo_interact_anim);
                                        }

                                        // Atualiza valor de ID
                                        imagePosition++;
                                    }
                                }
                            }
                            
                        }

                        break;
                }

                if (this.lastImageBG == null)
                {
                    GameObject imagemspawnada;

                    //Spawna o BG da imagem
                    imagemspawnada = ((GameObject)Instantiate(this.bGBot,                       // O que instanciar.
                                                                 new Vector3(0, calculateUIPosition(imagePosition), 0),          // Posição de instanciamento.
                                                                 new Quaternion(0, 0, 0, 0))    // Rotação inicial.
                                      );

                    //Coloca imagem como filha do container
                    imagemspawnada.transform.SetParent(this.container.transform, false);
                    imagemspawnada.tag = "Missions";

                    // Sobrepoe valores de transform
                    imagemspawnada.transform.localScale = new Vector3(80, 80, 0);
                    imagemspawnada.transform.localPosition = new Vector3(0, calculateUIPosition(imagePosition), 0);
                    imagemspawnada.transform.localRotation = new Quaternion();

                    this.lastImageBG = imagemspawnada;
                }
                else
                {
                    this.lastImageBG.transform.localPosition = new Vector3(0, calculateUIPosition(imagePosition), 0);
                }
            }
        } /// </ foreach>
    }

    /**
    /* @name: calculateUIPosition
    /* @version: 1.0
    /* @Description: Atualiza as imagens na tela.
    */
    private float calculateUIPosition(int ID)
    {
        return -55 * ID;
    }

    /**
    /* @name: spawnImageOnUI
    /* @version: 1.0
    /* @Description: Spawna imagem e BG da missão e atualiza sua posição.
    */
    private GameObject spawnImageOnUI(InteractActor missao, int ID)
    {
        //Spawna a imagem
        GameObject imagemspawnada = ((GameObject)Instantiate(missao.UIText_UNDONE,       // O que instanciar.
                                                     new Vector3(0, calculateUIPosition(ID+1) + 25, 0),               // Posição de instanciamento.
                                                     new Quaternion(0,0,0,0))            // Rotação inicial.
                                    );

        //Coloca imagem como filha do container
        imagemspawnada.transform.SetParent(this.container.transform, false);
        imagemspawnada.tag = "Missions";

        // Sobrepoe valores de transform
        imagemspawnada.transform.localScale = new Vector3(30,30,0);
        imagemspawnada.transform.localPosition = new Vector3(0, calculateUIPosition(ID+1) + 25, 0);
        imagemspawnada.transform.localRotation = new Quaternion();


        // Atualiza referencia no objeto de missão.
        missao.UI_Texture = imagemspawnada;

        //Spawna o BG da imagem
        imagemspawnada = ((GameObject)Instantiate(this.bGMid,                       // O que instanciar.
                                                     new Vector3(0, calculateUIPosition(ID), 0),          // Posição de instanciamento.
                                                     new Quaternion(0, 0, 0, 0))    // Rotação inicial.
                          );

        //Coloca imagem como filha do container
        imagemspawnada.transform.SetParent(this.container.transform, false);
        imagemspawnada.tag = "Missions";

        // Sobrepoe valores de transform
        imagemspawnada.transform.localScale = new Vector3(80, 80, 0);
        imagemspawnada.transform.localPosition = new Vector3(0, calculateUIPosition(ID), 0);
        imagemspawnada.transform.localRotation = new Quaternion();

        // Atualiza referencia no objeto de missão.
        missao.UI_BG = imagemspawnada;

        return imagemspawnada;
    }
}
