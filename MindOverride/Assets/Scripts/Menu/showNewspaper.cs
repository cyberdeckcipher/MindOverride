using UnityEngine;
using System.Collections;

public class showNewspaper : MonoBehaviour {

    public GameObject news;
    private GameObject text;

    public void spawnNewsText()
    {
        if (GameController.controller.alvoDaAcao != null && GameController.controller.acaoEscolhida == playersActions.Interagir)
        {
            if (GameController.controller.alvoDaAcao.GetComponent<InteractActor>() != null)
            {
                if (GameController.controller.alvoDaAcao.GetComponent<InteractActor>().UI_Texto != null)
                {
                    //Spawna a imagem
                    GameObject imagemspawnada = ((GameObject)Instantiate(GameController.controller.alvoDaAcao.GetComponent<InteractActor>().UI_Texto,       // O que instanciar.
                                                                 new Vector3(0, -135, 0),               // Posição de instanciamento.
                                                                 new Quaternion(0, 0, 0, 0))            // Rotação inicial.
                                                );

                    //Coloca imagem como filha do container
                    imagemspawnada.transform.SetParent(this.news.transform, false);
                    this.text = imagemspawnada;
                }
            }
        }
        else
        {
            if (MissionMenu_Handler.controller.missaoAtiva != null)
            {
                if (MissionMenu_Handler.controller.missaoAtiva.UI_Texto != null)
                {
                    //Spawna a imagem
                    GameObject imagemspawnada = ((GameObject)Instantiate(MissionMenu_Handler.controller.missaoAtiva.UI_Texto,       // O que instanciar.
                                                                 new Vector3(0, -135, 0),               // Posição de instanciamento.
                                                                 new Quaternion(0, 0, 0, 0))            // Rotação inicial.
                                                );

                    //Coloca imagem como filha do container
                    imagemspawnada.transform.SetParent(this.news.transform, false);
                    this.text = imagemspawnada;
                }
            }
        }
    }

    public void destroyNewsText()
    {   if (this.text != null)
        {
            GameObject.Destroy(this.text);
            this.text = null;
        }
    }
}
