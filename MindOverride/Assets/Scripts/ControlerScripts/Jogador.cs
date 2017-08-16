using UnityEngine;
using System.Collections;

// Esta classe define um jogador de uma partida
public class Jogador
{
    private PlayersTags jogador;
    public Jogador next;

    public Jogador(PlayersTags NovoJogador)
    {

        this.jogador = NovoJogador;
    }

    public PlayersTags getJogador()
    {
        return this.jogador;
    }

    public override string ToString()
    {
        return this.jogador.ToString();
    }

}
