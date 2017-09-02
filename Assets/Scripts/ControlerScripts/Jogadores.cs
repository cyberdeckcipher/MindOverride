using UnityEngine;
using System.Collections;

// Esta classe controla qual jogador tem a vez em cada dado turno
class JogadorDaVez
{

    private Jogador rootJogadores;

    public JogadorDaVez()
    {

        // Cria uma lista circular com os jogadores possiveis. Cada jogador tem uma lista de personagens
        this.rootJogadores = new Jogador(PlayersTags.Demonios);
        this.rootJogadores.next = new Jogador(PlayersTags.Inimigos);
        this.rootJogadores.next.next = new Jogador(PlayersTags.Aliados);
        this.rootJogadores.next.next.next = new Jogador(PlayersTags.Jogador);
        this.rootJogadores.next.next.next.next = this.rootJogadores;
    }

    public string getTag()
    {
        return this.rootJogadores.ToString();
    }

    public Jogador getCurrent()
    {
        return this.rootJogadores;
    }

    public Jogador getNext()
    {
        this.rootJogadores = this.rootJogadores.next;
        return this.rootJogadores;
    }
}