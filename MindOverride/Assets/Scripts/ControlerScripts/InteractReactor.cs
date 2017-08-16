using UnityEngine;
using System.Collections;

public class InteractReactor : MonoBehaviour {

    private InteractActor thisActor;                              // Referencia ao ator de interação.
    public reactorMissions hidenMissionType;                      // tipo da escondida.
    private bool missionTrigged;                                  // True caso o gatilho tenha sido executado.

	// Use this for initialization
	void Start () {
        this.missionTrigged = false;
        this.thisActor = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.thisActor == null)
        {
            thisActor = this.gameObject.GetComponent<InteractActor>();
        }

        if (GameController.controller.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()) && !this.missionTrigged)
        {
            switch (this.hidenMissionType)
            {
                case reactorMissions.selectTutorial:
                    if (GameController.controller.personagemEscolhido != null)
                    {
                        this.concluiMissao();
                    }
                    break;

                case reactorMissions.moveTutorial:
                    if (GameController.controller.acaoEscolhida == playersActions.Andar &&
                        GameController.controller.actionflag == Actionflag.acting)
                    {
                        this.concluiMissao();
                    }
                    break;

                case reactorMissions.attackTutorial:
                    if (GameController.controller.acaoEscolhida == playersActions.Atacar &&
                        GameController.controller.actionflag == Actionflag.acting)
                    {
                        this.concluiMissao();
                    }
                    break;

                case reactorMissions.interactTutorial:
                    if (GameController.controller.acaoEscolhida == playersActions.Interagir &&
                        GameController.controller.actionflag == Actionflag.acting)
                    {
                        this.concluiMissao();
                    }
                    break;
            }
        }
        if ((GameController.controller.actionflag == Actionflag.actionEnd ||
            GameController.controller.actionflag == Actionflag.idle) && this.missionTrigged)
        {
            // Ao final da utilização da missão, atualiza-se a tela de missões.
            MissionMenu_Handler.controller.updateMissionOnUI();
            GameObject.Destroy(this);
        }
	}

    private void concluiMissao()
    {
        if (this.gameObject.GetComponent<InteractActor>() != null)
        {
            this.gameObject.GetComponent<InteractActor>().missionCleard = true;

            GameController.controller.chegarMissoes = true;
            this.missionTrigged = true;

        }
    }
}
