using UnityEngine;
using System.Collections;

// Esta classe é uma fila simples que representa o caminho encontrado por A* até um determinado ponto.
public class Path
{
    private ponto2D[] pilha;
    int head;

    /**
    /* @name: Path
    /* @version: 1.0
    /* @Description: Construtor que cria uma pilha automaticamente explorando os parentes dos AEstrelaNodes
    */
    public Path(AEstrelaNode final)
    {
        this.pilha = new ponto2D[final.getPassosAteInicio()];
        this.head = 0;

        AEstrelaNode no = final;
        while ((no != null) && (no.getParent() != null))
        {
            this.push(no.getPosition());
            no = no.getParent();
        }
    }

    /**
    /* @name: Path
    /* @version: 1.0
    /* @Description: Construtor vazio de pontos.
    */
    public Path(int tamanho)
    {
        this.pilha = new ponto2D[tamanho];
        this.head = 0;
    }

    /**
    /* @name: push
    /* @version: 1.0
    /* @Description: Adiciona um ponto na pilha
    */
    public bool push(ponto2D ponto)
    {
        if ((head+1) == pilha.Length)
        {
            return false;
        }
        head++;
        pilha[head] = ponto;

        return true;
    }

    /**
    /* @name: nextStep
    /* @version: 1.0
    /* @Description: Desempilha um ponto da pilha
    */
    public ponto2D nextStep()
    {
        if (head == 0)
        {
            return null;
        }
        head--;

        return pilha[head + 1];
    }

    /**
    /* @name: lookAtNextStep
    /* @version: 1.0
    /* @Description: Retorna proximo passo sem desempilhar
    */
    public ponto2D lookAtNextStep(int steps = 1)
    {
        if (head == 0 || head - steps == 0)
        {
            return null;
        }
        return pilha[head -steps];
    }

    /**
    /* @name: toString
    /* @version: 1.0
    /* @Description: imprime algumas informações sobre o objeto
    */
    public string toString()
    {
        return "size:" + this.pilha.Length+" - head:"+this.head;
    }
}
