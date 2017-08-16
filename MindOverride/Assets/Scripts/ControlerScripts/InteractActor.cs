using UnityEngine;
using System.Collections;

public class InteractActor : MonoBehaviour {

    public string mensage;             // Mensagem que o objeto de interação carrega.

    // variaveis de controle de uso:
    public bool missionObjective;                 // True caso este actor seja uma missão.
    public bool missionCleard;                    // True caso esta missão tenha sido concluida.
    public bool isReusable;                       // True caso este actor possa ser executado mais de uma vez.
    public bool isDestructable;                   // True caso este actor deva ser destuido depois de usado.
    public bool used;                             // True caso este actor tenha sido usado.
    public ponto2D gridposition;                  // Posição do item interagivel no grid.

    // variavel de tipo de objeto;
    public Interactibles tipoDeInteragir;         // Tipo do item interagivel.

    //Controle de missões corrente
    public bool isChainedMission;                 // True caso esta seja uma missão chained e, portanto, tenha pré requisito.
    public int missionID;                         // Codigo unico da missao.
    public int chainID;                           // Código da missão de requisito.

    //Controle de UI de Missões
    public MissionUIState UIState;                // Estado que controla quando o texto da missão deve aparecer.
    public GameObject UIText_UNDONE;              // Texto para aparecer na lista de missões POR FAZER.
    public Sprite UIText_DONE;                    // Texto para aparecer na lista de missões FEITAS.

    public GameObject UI_Texture;                 // Referencia de sua textura spawnada na tela.
    public GameObject UI_BG;                      // Referencia do BG da missao.

    public GameObject UI_Texto;                   // Texto que deve ser ativado ao interagir.

    public GroundOverlayReactor colidingTile;     // Ao criar uma area, esta variavel guarda o tile que está na mesma posição do ator.

    // Use this for initialization
    void Start () {
        // Estado padrão inicial deste tipo de objeto deve ser "não usado".
        this.used = false;
        this.colidingTile = null;
    }

    /**
    /* @name: setMensage
    /* @version: 1.0
    /* @Description: seta a mensagem que deve aparecer na tela.
    */
    public void setMensage(string novaMensagem)
    {
        this.mensage = novaMensagem;
    }

    /**
    /* @name: getMensage
    /* @version: 1.0
    /* @Description: seta a mensagem que deve aparecer na tela.
    */
    public string getMensage()
    {
        return this.mensage;
    }

    /**
    /* @name: setMissionObjective
    /* @version: 1.0
    /* @Description: seta se este objeto é uma missão
    */
    public void setMissionObjective(bool newValue)
    {
        this.missionObjective = newValue;
    }

    /**
    /* @name: isMission
    /* @version: 1.0
    /* @Description: retorna true caso este objeto seja uma missao
    */
    public bool isMission()
    {
        return this.missionObjective;
    }

    /**
    /* @name: setReusable
    /* @version: 1.0
    /* @Description: seta valor de reusabilidade do item interagivel
    */
    public void setReusable(bool newValue)
    {
        this.isReusable = newValue;
    }

    /**
    /* @name: getMissionCleared
    /* @version: 1.0
    /* @Description: true caso o objeto interagivel conte como uma missão concluida
    */
    public bool getMissionCleared()
    {
        if (this.isMission())
        {
            return this.missionCleard;
        }
        return true;
    }
}
