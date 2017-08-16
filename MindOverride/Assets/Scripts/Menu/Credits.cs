using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {

    public Animator creditsText;             // Texto animado dos Creditos.
    public float creditsDelay;               // Quantidade de tempo antes dos créditos aparecem.
    public float musicTime;                  // Quantidade de tempo antes dos créditos aparecem.
    private bool fadeActivated;              // True caso a transição já tenha sido ativada.

    // Use this for initialization
    void Start()
    {
        AudioController.activate_CreditsFix();
        this.fadeActivated = false;
    }


	// Update is called once per frame
	void Update () {
        // Quando não ouver mais uma cutscene
        if (Cutscene_Handler.controller.tipoDaCutscene == CutsceneType.idle)
        {
            if (this.musicTime < this.creditsDelay)
            {
                // Caso os créditos não tenham sido ativados
                if (!creditsText.GetBool("onCredits"))
                {
                    // Ativa os creditos
                    creditsText.SetBool("onCredits", true);
                }
            }

            if (this.musicTime <= 0)
            {
                if (!this.fadeActivated)
                {
                    this.fadeActivated = true;

                    // Retorna ao menu
                    backToMenu();
                }
            }
            else
            {
                this.musicTime -= Time.deltaTime;
            }

            // Se detectar o botao cancel
            if (Input.GetButtonDown("Cancel"))
            {
                if (!this.fadeActivated)
                {
                    this.fadeActivated = true;

                    // Retorna ao menu
                    backToMenu();
                }
            }

            
        }
    }

    /**
    /* @name: backToMenu
    /* @version: 1.0
    /* @Description: Retorna ao menu
    */
    public void backToMenu()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Menu);
    }
}
