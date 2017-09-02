using UnityEngine; //IA_Pathfinder
using System.Collections.Generic;


// Classe A* de pathfindin, modificado para não utilizar movimentos diagonais
public class IA_Pathfinder
{
    private Nodelist openNodes;       // Lista de nós abertos do algoritmo.
    private Nodelist closedNodes;     // Lista de nós fechados do algoritmo.
    private ponto2D unity_position;   // Posição da unidade que vai se mover.
    private ponto2D enemy_position;   // Posição do inimigo o qual esta unidade quer se aproximar.
    private ActionsTypes tipoDaAcao;  // Tipo da ação para detecção de obstaculos que está sendo executada.
    private int max_X;                // Valor máximo do X.
    private int max_Y;                // Valor máximo do Y.
    private int maxAlcance;           // Valor de distancia que o ponto procurado deve estar da posição do inimigo.
    private int minAlcance;           // Valor de alcance de ataque do personagem. DEPRECATED

    // Debug: controle de numero de processos.
    int visitas;

    public IA_Pathfinder(int maxx, int maxy)
    {
        this.openNodes = new Nodelist();
        this.closedNodes = new Nodelist();
        this.max_X = maxx;
        this.max_Y = maxy;
        this.maxAlcance = 0;
        this.tipoDaAcao = ActionsTypes.Comum;
    }

    /**
    /* @name: noExistente
    /* @version: 2.0
    /* @Description: Procura se um AEstrelaNode já existe nas listas abertos ou fechados, retornando o no caso encontrado, null caso contrário
    */
    private AEstrelaNode noExistente(ponto2D posicao)
    {
        AEstrelaNode saida = null;

        if (this.openNodes.contains(posicao))
        {
            saida = this.openNodes.find(posicao);
        }
        else if (this.closedNodes.contains(posicao))
        {
            saida = this.closedNodes.find(posicao);
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
        return this.openNodes.remove();
    }

    /**
    /* @name: fecharNode
    /* @version: 2.0
    /* @Description: Adiciona um no na lista de nos fechados
    */
    private bool fecharNode(AEstrelaNode no)
    {
        // Caso a lista de nos fechados já contenha o no, retorna falso.
        if (this.closedNodes.contains(no.getPosition()))
        {
            return false;
        }

        //Caso contrario, adiciona o no e retorna true.
        this.closedNodes.add(no, this.enemy_position);
        return true;
    }

    /**
    /* @name: vistaNode
    /* @version: 1.5
    /* @Description: recebe um no, atualiza seus vizinhos. retorna true se o ponto atual é satisfatório. falso caso contrário.
    */
    private bool vistaNode(AEstrelaNode posicaoAtual)
    {
        /**
            se posição atual.distance == distanciaRequerida, return true
            testa se um vizinho já existe (tiles obstáculos são ignorados pelo algoritmo). (quatro lados)
            Se já existe, testa a quantidade de passos que este vizinho tem até chegar no inicio.
            Caso contrário cria um AEstrelaNode, calcula valor euristico e adiciona a lista de abertos.
            ao final, fecha o Node.
        */

        // Caso a distancia passada ao algoritmo seja negativa, o algoritmo encontra path até a posição do inimigo
        if (this.maxAlcance < 0)
        {
            //Caso o ponto atual seja o ponto procurado, retorna true.
            if (posicaoAtual.getPosition().isEqual(this.enemy_position))
            {
                return true;
            }
        }

        // Caso a distancia seja positiva, algoritmo encontra um path para um ponto de distancia calculada.
        else
        {
            //Caso o ponto atual esteja a distancia procurada pela unidade, retorna true.
            if (posicaoAtual.getPosition().distancia(this.enemy_position) == this.maxAlcance)
            {
                return true;
            }
        }
        
        // Caso contrário, checa e atualiza visinhos.
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x, posicaoAtual.getPosition().y + 1));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x, posicaoAtual.getPosition().y - 1));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x + 1, posicaoAtual.getPosition().y));
        this.checaVisinho(posicaoAtual, new ponto2D(posicaoAtual.getPosition().x - 1, posicaoAtual.getPosition().y));

        this.fecharNode(posicaoAtual);

        return false;
    }

    /**
    /* @name: checaVisinho
    /* @version: 1.5
    /* @Description: Checa se um visinho existe, caso afirmativo atualiza, caso contrário cria um no na posição.
    */
    private void checaVisinho(AEstrelaNode parente, ponto2D visinho)
    {
        // Checa inicialmente se a posição do vizinho existe dentro da matrix
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
                //Caso o ponto seja um obstáculo para a IA, não cria o visinho.
                if (!GameController.isObstacle(visinho, this.tipoDaAcao))
                {
                    this.criaNo(parente, visinho);
                }
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
        this.openNodes.add(new AEstrelaNode(parente, posicao, (parente.getPassosAteInicio() + 1)), this.enemy_position);
    }

    /**
    /* @name: findPathTo
    /* @version: 2.5
    /* @Description: encontra um caminho do ponto de inicio até o ponto final e retorna uma pilha contendo os passos.
    */
    public Path findPathTo(ponto2D posicaoDaUnidade, ponto2D posicaoDoInimigo, int distanciaDoInimigo, int distanciaMinimaDoInimigo)
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
        this.enemy_position = posicaoDoInimigo;
        this.unity_position = posicaoDaUnidade;
        this.maxAlcance = distanciaDoInimigo;
        this.tipoDaAcao = ActionsTypes.Comum;

        criaNo(new AEstrelaNode(null, this.unity_position, 0), this.unity_position); //inicia a lista de nos abertos.
        AEstrelaNode noAtual = this.proximoNoAberto(); // pega o primeiro no aberto.

        //Busca o fim, desviando dos obstáculos.
        while (!vistaNode(noAtual))
        {
            noAtual = this.proximoNoAberto();

            

            //Caso noAtual seja nulo (lista de nos abertos vazia) o caminho é impossivel de ser encontrado.
            if (noAtual == null)
            {
                Debug.Log("[IA] PATH_NAO_ENCONTRADO");
                Debug.Log("[IA] alvo: ("+posicaoDoInimigo.x+","+posicaoDoInimigo.y+")- unidade:("+posicaoDaUnidade.x+","+posicaoDaUnidade.y+") - distancia:"+distanciaDoInimigo);
                return null;
            }
            else
            {
                if (noAtual.getPosition().distancia(this.enemy_position) > distanciaMinimaDoInimigo 
                    && noAtual.getPosition().distancia(this.enemy_position) <= this.maxAlcance)
                {
                    break;
                }
            }
        }

        //Cria pilha de posições para a saida
        Path saida = new Path(noAtual);

        //Limpa as pilhas para uma nova busca
        this.openNodes = new Nodelist();
        this.closedNodes = new Nodelist();

        return saida;
    }

    /**
    /* @name: findDistanceTo
    /* @version: 1.6
    /* @Description: versão diferente do pathfinder que retorna apenas a distancia até um determinado ponto, desviando de obstáculos
    */
    public int findDistanceTo(ponto2D posicaoDaUnidade, ponto2D posicaoDoInimigo)
    {
        //Atualiza variáveis iniciais para a busca do caminho
        this.unity_position = posicaoDaUnidade;
        this.enemy_position = posicaoDoInimigo;
        this.maxAlcance = -1;
        this.tipoDaAcao = ActionsTypes.Distance;

        criaNo(new AEstrelaNode(null, this.unity_position, 0), this.unity_position); //inicia a lista de nos abertos.
        AEstrelaNode noAtual = this.proximoNoAberto(); // pega o primeiro no aberto.

        //Busca o fim, desviando dos obstáculos.
        while (!vistaNode(noAtual))
        {
            noAtual = this.proximoNoAberto();

            //Caso noAtual seja nulo (lista de nos abertos vazia) o caminho é impossivel de ser encontrado.
            //Retorna o maior valor possivel
            if (noAtual == null)
            {
                return int.MaxValue;
            }
        }

        //Limpa as pilhas para uma nova busca
        this.openNodes = new Nodelist();
        this.closedNodes = new Nodelist();

        return (noAtual.getPassosAteInicio()-1);
    }
}
