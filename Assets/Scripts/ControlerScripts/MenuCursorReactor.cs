using UnityEngine;
using System.Collections;

public class MenuCursorReactor : MonoBehaviour {

    //Configuração do menu
    public int quantDeItens;               // Quantidade de itens no menu.
    public GameObject[] itensDoMenu;       // Lista contendo todos os botoes do menu.
    public float AxisLimit;                // Deadzone do controle.

    // Controle do menu
    public bool isTitleMenu;                // Se true, usa direcional para controlar menu, caso contrário usa defesas.
    public bool isMenuSet;                  // True caso o menu esteja habilitado e pronto para ser executado.
    public int currentItemID;               // Indice do atual item escolhido.
    public float maxWait;                   // Valor de tempo que bloqueia mudança do cursor do menu.
    public float waitInSeconds;             // Valor de tempo que bloqueia a mudança do cursor do menu.

    public MenuButtonAnimator currentButton; // Referencia ao botão atual do menu.

    // Controle de movimentação dos botões
    public int highlightedPixelMove;        // Quantidade de pixels que o botão deve mover.

    // Use this for initialization
    void Start () {
        this.itensDoMenu = new GameObject[quantDeItens];
        this.waitInSeconds = maxWait;

        int id = 0;
        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<MenuButtonAnimator>() != null)
            {
                if (id < quantDeItens)
                {
                    this.itensDoMenu[id] = child.gameObject;
                    id++;
                }
                else
                {
                    Debug.Log("Quantidade de itens é incompativel com MenuReactor");
                    break;
                }
            }
            else if (child.GetComponent<MenuCursorActor>() != null)
            {
                child.GetComponent<MenuCursorActor>().controlador = this;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (this.isMenuSet)
        {
            this.currentButton = this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>();

            if (currentButton.estadoDoBotao == CursorState.idle && (waitInSeconds <= 0))
            {
                // Caso esse seja um menu do tipo "titulo", utiliza o direcional pra mover o menu
                if (this.isTitleMenu)
                {
                    float axisVERTICALInput = Input.GetAxis("Vertical");

                    /*float axisVERTICAL2Input = Input.GetAxis("Vertical2");*/

                    // caso o axis Vertical seja negativo, o cursor deve se mover para baixo
                    if (axisVERTICALInput < -this.AxisLimit /*||
                        axisVERTICAL2Input < -this.AxisLimit*/)
                    {
                        AudioController.sfx_Play_MenuChange();
                        this.menuDOWN();
                    }
                    // Caso o axis Vertical seja positivo, o cursor deve se mover para cima
                    else if (axisVERTICALInput > this.AxisLimit /*||
                             axisVERTICAL2Input > this.AxisLimit*/)
                    {
                        AudioController.sfx_Play_MenuChange();
                        this.menuUP();
                    }
                }
                // Caso contrario utiliza botoes
                else
                {
                    // caso o LB seja pressionado, o cursor deve se mover para baixo
                    if (Input.GetButtonDown("shoulderDOWN"))
                    {
                        AudioController.sfx_Play_MenuChange();
                        this.menuDOWN();

                        if (this.currentItemID >= 0 && this.currentItemID < 3)
                        {
                            pressThisButton();
                        }
                        else
                        {
                            GameController.controller.desfazAreaDeAcao();
                        }
                    }
                    // Caso o RB seja pressionado, o cursor deve se mover para cima
                    if (Input.GetButtonDown("shoulderUP"))
                    {
                        AudioController.sfx_Play_MenuChange();
                        this.menuUP();

                        if (this.currentItemID >= 0 && this.currentItemID < 3)
                        {
                            pressThisButton();
                        }
                        else
                        {
                            GameController.controller.desfazAreaDeAcao();
                        }
                    }
                }

                if (Input.GetButtonDown("Accept"))
                {
                    AudioController.sfx_Play_MenuClick();
                    pressThisButton();
                }
            }

            if (waitInSeconds > 0)
            {
                waitInSeconds = waitInSeconds - Time.deltaTime;
            }
        }
    }

    /**
    /* @name: pressThisButton
    /* @version: 1.0
    /* @Description: Ativa o botão atual.
    */
    private void pressThisButton()
    {
        if (this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().enabled)
        {
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().executaBotao();
        }
    }

    /**
    /* @name: menuUP
    /* @version: 1.3
    /* @Description: Escolhe o item acima do atual, se possivel.
    */
    private void menuUP()
    {
        if (this.currentItemID > 0)
        {
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(-this.highlightedPixelMove);

            this.currentItemID--;
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(this.highlightedPixelMove);
            this.waitInSeconds = maxWait;
        }
        else if (!this.isTitleMenu)
        {
            this.currentButton = this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>();

            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(-this.highlightedPixelMove);

            this.currentItemID = (quantDeItens -1);
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(this.highlightedPixelMove);
            this.waitInSeconds = maxWait;
        }
    }

    /**
    /* @name: menuDOWN
    /* @version: 1.3
    /* @Description: Escolhe o item abaixo do atual, se possivel.
    */
    private void menuDOWN()
    {
        if (this.currentItemID < (quantDeItens - 1))
        {
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(-this.highlightedPixelMove);

            this.currentItemID++;
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(this.highlightedPixelMove);
            this.waitInSeconds = maxWait;
        }
        else if (!this.isTitleMenu)
        {
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(-this.highlightedPixelMove);

            this.currentItemID = 0;
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(this.highlightedPixelMove);
            this.waitInSeconds = maxWait;
        }
    }

    /**
    /* @name: resetMenu
    /* @version: 1.0
    /* @Description: Escolhe o item abaixo do atual, se possivel.
    */
    public void resetMenu()
    {
        if (this.currentItemID != 0)
        {
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(-this.highlightedPixelMove);
            this.currentItemID = 0;
            this.itensDoMenu[this.currentItemID].GetComponent<MenuButtonAnimator>().Move(this.highlightedPixelMove);
        }
    }
}
