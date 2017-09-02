using UnityEngine;
using System.Collections;

public class CharacterData_Menu : MonoBehaviour {

    // variaveis container de informações:
    public int ataque;                             // total do ataque do personagem.
    public AttackType tipoDeAtaque;                // tipo de ataque do personagem.
    public int vida;                               // Vida restante do personagem.
    public string tagDoPersonagem;                 // Tag do personagem escolhido.
    private GameObject ultimoPersonagem;           // ultimo personagem escolhido pelo jogador.

    //Slots de controle de coisas para aparecer
    public GameObject[] numeroVida;                // Objetos que marcam a quantidade de vida do personagem.
    public GameObject[] numeroAtaque;              // Objetos que marcam quantidade de passos do personagem.
    public GameObject rangedAttack;                // Tipo do ataque ranged.
    public GameObject meleeAttack;                 // tipo do ataque melee.

    public GameObject canvasJogador;               // Canvas usado para mostrar o menu do personagem do jogador.
    public GameObject canvasInimigo;               // Canvas usado para mostrar dados de um inimigo.
    public GameObject inGameMenu;                  // Menu deve ser mostrado somente se a unidade escolhida for do jogador.
    public GameObject botaoDown;                   // UI do menu.
    public GameObject botaoUp;                     // UI do menu.

    // Use this for initialization
    void Start()
    {
        this.ultimoPersonagem = null;
    }

	// Update is called once per frame
	void Update () {

        if (GameController.controller.personagemEscolhido != null)
        {
            if (this.ultimoPersonagem == null)
            {
                this.ultimoPersonagem = GameController.controller.personagemEscolhido.gameObject;
                this.atualizaValores(GameController.controller.personagemEscolhido);
            }
            else if (this.ultimoPersonagem != GameController.controller.personagemEscolhido.gameObject)
            {
                this.atualizaValores(GameController.controller.personagemEscolhido);
                this.ultimoPersonagem = GameController.controller.personagemEscolhido.gameObject;
            }

            ativaNumeros(numeroVida, 6, this.vida);
            ativaNumeros(numeroAtaque, 6, this.ataque);

            switch (tipoDeAtaque)
            {
                case AttackType.melee:
                    this.rangedAttack.SetActive(false);
                    this.meleeAttack.SetActive(true);
                    break;

                case AttackType.ranged:
                    this.rangedAttack.SetActive(true);
                    this.meleeAttack.SetActive(false);
                    break;
            }

            if (tagDoPersonagem == PlayersTags.Jogador.ToString() &&
                GameController.controller.personagemEscolhido.podeAgir)
            {
                this.canvasInimigo.SetActive(false);
                this.canvasJogador.SetActive(true);
                this.inGameMenu.SetActive(true);
                this.botaoDown.SetActive(true);
                this.botaoUp.SetActive(true);
            }
            else if (tagDoPersonagem == PlayersTags.Demonios.ToString() ||
                    !GameController.controller.personagemEscolhido.podeAgir)
            {
                this.canvasInimigo.SetActive(true);
                this.canvasJogador.SetActive(false);
                this.inGameMenu.SetActive(false);
                this.botaoDown.SetActive(false);
                this.botaoUp.SetActive(false);
            }
        }
    }

    public void atualizaValores(GenericCharacter personagem)
    {
        this.ataque = personagem.dano;
        this.vida = personagem.life;
        this.tipoDeAtaque = personagem.tipoDeAtaque;
        this.tagDoPersonagem = personagem.jogador.ToString();
    }

    private void ativaNumeros(GameObject[] numeros, int total, int idAtivo)
    {
        if (total > 6)
        {
            total = 6;
        }

        int for_i = 0;

        foreach ( GameObject numero in numeros)
        {
            if (for_i == (idAtivo - 1))
            {
                numeros[for_i].SetActive(true);
            }
            else
            {
                numeros[for_i].SetActive(false);
            }
            for_i++;
        }
        
    }
}
