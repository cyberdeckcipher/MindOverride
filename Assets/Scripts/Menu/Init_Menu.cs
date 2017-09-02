using UnityEngine;

public class Init_Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Input.ResetInputAxes();
    }

    /**
    /* @name: menu_startGame
    /* @version: 1.0
    /* @Description: Inicia um novo jogo.
    */
    public void menu_startGame()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Gameplay);
    }

    /**
    /* @name: startGame
    /* @version: 1.0
    /* @Description: Abre a tela de creditos do jogo.
    */
    public void menu_credits()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Credits);
    }

    /**
    /* @name: menu_exit
    /* @version: 1.0
    /* @Description: Fecha o jogo.
    */
    public void menu_exit()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Quit);
    }

    /**
    /* @name: menu_GameOver
    /* @version: 1.0
    /* @Description: Inicia um novo jogo.
    */
    public void menu_GameOver()
    {
        Cutscene_Handler.controller.fadeInTo(Scene.Menu);
    }
}
