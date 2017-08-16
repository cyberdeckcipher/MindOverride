using System.Collections.Generic;
using UnityEngine;

// Classe que define um nó para a lista do algorimo A*, contendo os valores 
public class AEstrelaNode
{
    private AEstrelaNode parent;    // filho deste tile.
    private ponto2D gridPosition;   // posição no grid.
    private int euristicValue;      // valor heuristico para ordenação no A*.
    private int passosAteInicio;    // Numero de passos até o inicio. usado para desempate em A*.

    public AEstrelaNode(AEstrelaNode parente, ponto2D posicao, int passos)
    {
        this.parent = parente;
        this.gridPosition = posicao;
        this.euristicValue = 0;
        this.passosAteInicio = passos;
    }

    public void setEuristicValue(ponto2D inicio, ponto2D fim)
    {
        this.euristicValue = (inicio.distancia(this.gridPosition) * 10) + (fim.distancia(this.gridPosition) * 10);
    }

    public int getEuristicValue()
    {
        return this.euristicValue;
    }

    public int getPassosAteInicio()
    {
        return this.passosAteInicio;
    }

    public void setPassosAteInicio(int passos)
    {
        this.passosAteInicio = passos;
    }

    public ponto2D getPosition()
    {
        return this.gridPosition;
    }

    public AEstrelaNode getParent()
    {
        return this.parent;
    }

    public void setParent( AEstrelaNode newParent)
    {
        this.parent = newParent;
    }

    public void destroy()
    {
        this.parent = null;
        this.gridPosition = null;
        this.euristicValue = 0;
        this.passosAteInicio = 0;
    }
}

// Classe que controla uma lista de nos duplamente encadeados e ordenados heuristicamente, com desempate via distancia ao ponto final
class Nodelist
{
    int size;
    ANode head;

    public Nodelist()
    {
        this.size = 0;
        this.head = null;
    }

    /**
    /* @name: add
    /* @version: 1.0
    /* @Description: Adiciona um no a lista, hordenada heuristicamente.
    */
    public void add(AEstrelaNode no, ponto2D fim)
    {
        ANode node = new ANode(no);

        if (this.size == 0)
        {
            this.head = node;
        }
        else
        {
            ANode outroNo = head;
            ANode anterior = outroNo;
            bool getNext = true;

            // Encontra a posição do novo no, usando valor euristico crescente e desempatando com numero de passos até o final
            while (getNext)
            {
                if ((outroNo.node.getEuristicValue() < node.node.getEuristicValue()) // se o valor euristico do no da lista for maior
                    || ((outroNo.node.getEuristicValue() == node.node.getEuristicValue()) // mas caso seja igual
                        &&(outroNo.node.getPosition().distancia(fim) > node.node.getPosition().distancia(fim)) //desempata usando distancia para o fim
                       )
                   )
                {
                    getNext = false;
                }

                else if (outroNo.next == null)
                {
                    getNext = false;
                }

                anterior = outroNo;
                outroNo = outroNo.next;
            }

            node.next = anterior.next;
            node.back = anterior;

            if (anterior.next != null)
            {
                anterior.next.back = node;
            }
            anterior.next = node;
        }

        this.size++;
    }

    /**
    /* @name: remove
    /* @version: 1.0
    /* @Description: remove um no da lista, hordenada heuristicamente.
    */
    public AEstrelaNode remove()
    {
        if (this.head == null)
        {
            return null;
        }

        //Captura AEstrelaNode da cabeça da lista
        AEstrelaNode saida = this.head.node;

        // Corrige apontadores da lista, caso a cabeça não seja o unico elemento;
        if (this.size > 1)
        {
            this.head.next.back = null;
        }
        
        // aponta o objeto cabeça anterior, atualiza cabeça e destroi objeto anterior.
        ANode noRemovido = this.head;
        this.head = this.head.next;
        noRemovido.destroy();

        // Atualiza tamanho da lista
        this.size--;

        return saida;
    }

    /**
    /* @name: contains
    /* @version: 1.0
    /* @Description: true caso já exista um AEstrelaNode na posição.
    */
    public bool contains(ponto2D posicao)
    {
        ANode no = this.head;
        while (no != null)
        {
            if (no.node.getPosition().isEqual(posicao))
            {
                return true;
            }
            no = no.next;
        }

        return false;
    }

    /**
    /* @name: find
    /* @version: 1.0
    /* @Description: Encontra um AEstrelaNode especifico na lista, a partir de sua posição
    */
    public AEstrelaNode find(ponto2D posicao)
    {
        ANode no = this.head;
        while (no != null)
        {
            if (no.node.getPosition().isEqual(posicao))
            {
                return no.node;
            }
            no = no.next;
        }
        return null;
    }

    /**
    /* @name: destroy
    /* @version: 1.0
    /* @Description: destroi todos os objetos da lista, deixando-a limpa.
    */
    public void destroy()
    {
        if (this.size >0)
        {
            ANode no;
            ANode prox = this.head;

            while (prox != null)
            {
                no = prox;
                prox = no.next;
                no.destroy();
            }

            this.size = 0;
        }
    }

    /**
    /* @name: destroy
    /* @version: 1.0
    /* @Description: destroi todos os objetos da lista, deixando-a limpa.
    */
    public int getSize()
    {
        return this.size;
    }

    public string toString()
    {
        return "size:" + getSize();
    }
}

// No da lista Nodelist
class ANode
{
    public AEstrelaNode node;
    public ANode next;
    public ANode back;

    public ANode(AEstrelaNode novoNode)
    {
        this.node = novoNode;
        next = null;
        back = null;
    }

    public void destroy()
    {
        this.back = null;
        this.next = null;
        this.node = null;
    }

}


// Classe A* de pathfindin, modificado para não utilizar movimentos diagonais
public class Aestrela
{
    private Nodelist abertos;       // lista de pontos abertos e ainda não visitados pelo algoritmo de pathfinding
    private Nodelist fechados;      // lista de pontos já visitados e fechados pelo algoritmo de pathfinding
    private ponto2D inicio;         // ponto de inicio para pathfinding
    private ponto2D fim;            // ponto alvo para pathfinding

    private int max_X;              // valor maximo para o x do mapa
    private int max_Y;              // valor maximo para o y do mapa
    private int max_dist;           // distancia maxima que o algoritmo deve considerar, a partir do ponto inicial

    // Debug: controle de numero de processos.
    int visitas;

    public Aestrela(int maxx, int maxy)
    {
        this.abertos = new Nodelist();
        this.fechados = new Nodelist();
        this.max_X = maxx;
        this.max_Y = maxy;
        this.max_dist = -1;
    }

    /**
    /* @name: noExistente
    /* @version: 2.0
    /* @Description: Procura se um AEstrelaNode já existe nas listas abertos ou fechados, retornando o no caso encontrado, null caso contrário
    */
    private AEstrelaNode noExistente(ponto2D posicao)
    {
        AEstrelaNode saida = null;

        if ( this.abertos.contains(posicao) )
        {
           saida = this.abertos.find(posicao);
        }
        else if (this.fechados.contains(posicao))
        {
           saida = this.fechados.find(posicao);
        }

        return saida;
    }

    /**
    /* @name: proximo
    /* @version: 4.0
    /* @Description: recebe um ponto final para ser usado como desempate. retorna o proximo ponto a ser usado algoritmo de pathfinding.
    */
    private AEstrelaNode proximoNoAberto()
    {
        return this.abertos.remove();
    }

    /**
    /* @name: fecharNode
    /* @version: 2.0
    /* @Description: Adiciona um no na lista de nos fechados
    */
    private bool fecharNode(AEstrelaNode no)
    {
        // Caso a lista de nos fechados já contenha o no, retorna falso.
        if (this.fechados.contains(no.getPosition()))
        {
            return false;
        }

        //Caso contrario, adiciona o no e retorna true.
        this.fechados.add(no, this.fim);
        return true;
    }

    /**
    /* @name: vistaNode
    /* @version: 1.6
    /* @Description: recebe um no, atualiza seus vizinhos. retorna true se o ponto atual é o fim. falso caso contrário.
    */
    private bool vistaNode(AEstrelaNode posicaoAtual)
    {
        /**
            se posição atual == fim, return true
            testa se um vizinho já existe (tiles obstáculos são ignorados pelo algoritmo). (quatro lados)
            Se já existe, testa a quantidade de passos que este vizinho tem até chegar no inicio.
            Caso contrário cria um AEstrelaNode, calcula valor euristico e adiciona a lista de abertos.
            ao final, fecha o Node.
        */

        // Caso a distancia seja negativa, retorna true caso a posição atual seja igual ao alvo.
        if (this.max_dist < 0)
        {
            if (posicaoAtual.getPosition().isEqual(this.fim))
            {
                return true;
            }

            this.fecharNode(posicaoAtual);
        }
        else
        {
            this.fecharNode(posicaoAtual);
        }

        if (GameController.controller.acaoEscolhida == playersActions.Atacar)
        {
            this.checaVizinhos(posicaoAtual);
        }
        else
        {
            if (!GameController.tileHasEnemy(posicaoAtual.getPosition()))
            {
                this.checaVizinhos(posicaoAtual);
            }
        }

        // Checa vizinhos apenas se a posição atual não for um obstáculo.
        

        return false;
    }

    private void checaVizinhos(AEstrelaNode posicaoAtual)
    {
        // checa e atualiza visinhos
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x + 1, posicaoAtual.getPosition().y));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x - 1, posicaoAtual.getPosition().y));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x, posicaoAtual.getPosition().y + 1));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x, posicaoAtual.getPosition().y - 1));
    }

    /**
    /* @name: checaVisinho
    /* @version: 1.7
    /* @Description: Checa se um visinho existe, caso afirmativo atualiza, caso contrário cria um no na posição.
    */
    private void checaVisinho(AEstrelaNode parente, ponto2D visinho)
    {
        // Checa inicialmente se a posição do vizinho existe dentro da matriz
        if ((visinho.x < this.max_X)
         && (visinho.y < this.max_Y)
         && (visinho.x >= 0)
         && (visinho.y >= 0))
        {

            //Chega que o visinho já existe
            AEstrelaNode thisnode = this.noExistente(visinho);
            if (thisnode != null)
            {
                //Caso exista, atualiza o visinho.
                if (thisnode.getPassosAteInicio() > (parente.getPassosAteInicio() + 1))
                {
                    thisnode.setParent(parente);
                    thisnode.setPassosAteInicio(parente.getPassosAteInicio() + 1);
                }
            }

            //Caso contrario, cria um visinho.
            else
            {
                
                if (!GameController.isObstacle(visinho, ActionsTypes.Comum))
                {
                    // Se a distancia máxima for negativa, não é levada em consideração.
                    if (this.max_dist < 0)
                    {
                        this.criaNo(parente, visinho);
                    }
                    else
                    {
                        // Apenas se o no a ser criado estiver dentro da distancia maxima
                        if (parente.getPassosAteInicio() <= this.max_dist)
                        {
                            this.criaNo(parente, visinho);
                        }
                    }
                } //</ isObstacle>
            }
        }
    }

    /**
    /* @name: criaNo
    /* @version: 1.0
    /* @Description: Cria um novo no em uma posição no grid
    */
    private void criaNo(AEstrelaNode parente, ponto2D posicao)
    {

        this.abertos.add(new AEstrelaNode(parente, posicao, (parente.getPassosAteInicio() + 1)), this.fim);

    }

    /**
    /* @name: findPathTo
    /* @version: 2.1
    /* @Description: encontra um caminho do ponto de inicio até o ponto final e retorna uma pilha contendo os passos.
    */
    public Path findPathTo(ponto2D pontoInicio, ponto2D pontoFim)
    {
        /**
            ve se o no atual é o fim, se sim, empilha e retorna pilha de movimentos.
            caso contrário atualiza os nós proximos.
                Se o no proximo já existe, tenta atualizar
                    Caso o euristic value for menor, sobrescreve.
                caso contrário, cria no e bota os valores.
                adiciona cada proximo a lista de abertos se puder.
            pega o proximo no, repete.
        */

        //Atualiza variáveis iniciais para a busca do caminho
        this.fim = pontoFim;
        this.inicio = pontoInicio;
        this.max_dist = -1;

        criaNo(new AEstrelaNode(null,this.inicio,0), this.inicio); //inicia a lista de nos abertos.
        AEstrelaNode noAtual = this.proximoNoAberto(); // pega o primeiro no aberto.

        //Busca o fim, desviando dos obstáculos.
        while (!vistaNode(noAtual))
        {
            noAtual = this.proximoNoAberto();

            //Caso noAtual seja nulo (lista de nos abertos vazia) o caminho é impossivel de ser encontrado.
            if (noAtual == null)
            {
                Debug.Log("PATH_NAO_ENCONTRADO");
                return null;
            }
        }

        //Cria pilha de posições para a saida
        Path saida = new Path(noAtual);

        //Limpa as pilhas para uma nova busca
        this.abertos = new Nodelist();
        this.fechados = new Nodelist();

        return saida;
    }

    /**
    /* @name: findAreaFrom
    /* @version: 1.0
    /* @Description: visita e retorna uma lista de pontos contendo todos os pontos a uma determinada distancia do centro
    */
    public Path findAreaFrom(ponto2D centro, int maxDistancia, int minDistancia)
    {

        //Atualiza variáveis iniciais para a busca do caminho
        this.fim = centro;
        this.inicio = centro;
        this.max_dist = maxDistancia;

        criaNo(new AEstrelaNode(null, this.inicio, 0), this.inicio); //inicia a lista de nos abertos.
        AEstrelaNode noAtual = this.proximoNoAberto(); // pega o primeiro no aberto.

        //Busca o fim, desviando dos obstáculos.
        while (!vistaNode(noAtual))
        {
            noAtual = this.proximoNoAberto();

            //Quando alista de nos abertos estiver vazia
            if (noAtual == null)
            {
                // Adiciona um nó a lista de fechados para que este seja deixado para trás na hora de criar a pilha de saida.
                this.fecharNode( new AEstrelaNode(null, new ponto2D(0,0), int.MaxValue));

                // Cria uma lista de pontos para serem levantados
                AEstrelaNode noFechado = this.fechados.remove();        // pega primeiro elemento da lista de fechados
                Path pilhaDeTiles = new Path(fechados.getSize()+1);     // Inicia a pilha usada para armazenar os tiles da area

                while (noFechado != null)
                {
                    pilhaDeTiles.push(noFechado.getPosition());
                    noFechado = this.fechados.remove();
                }

                // retorna a pilha de posições
                return pilhaDeTiles;
            }
        }

        //Limpa as pilhas para uma nova busca
        this.abertos = new Nodelist();
        this.fechados = new Nodelist();

        return null;
    }
}