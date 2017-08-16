using UnityEngine;
using System.Collections;

// Esta classe implementa uma fila para guardar Tiles do wargrid, para serem usados sob demanda.
public class TilePool
{
    // variaveis de controle 
    private int quantidade;               // Quantidade de objetos na fila
    private int head;                     // Apontador do inicio da fila
    private int tail;                     // Apontador do fim da fila
    private int poolSize;                 // Tamanho total do armazenamento
    private GroundOverlayReactor[] fila;  // Pool de tiles

    public TilePool(int tamanho)
    {
        this.poolSize = tamanho;
        this.fila = new GroundOverlayReactor[tamanho];
        this.head = 0;
        this.tail = -1;
        this.quantidade = 0;
    }

    public GroundOverlayReactor withdraw()
    {
        if (this.isEmpty())
        {
            return null;
        }

        GroundOverlayReactor saida = fila[head];
        fila[head] = null;
        this.moveHead();
        this.quantidade--;

        return saida;
    }

    public bool deposit(GroundOverlayReactor deposito)
    {
        if (this.isFull())
        {
            return false;
        }

        this.moveTail();
        this.fila[this.tail] = deposito;
        this.quantidade++;

        return true;
    }

    private void moveHead()
    {
        this.head++;

        if (this.head >= poolSize)
        {
            this.head = 0;
        }
    }

    private void moveTail()
    {
        this.tail++;

        if (this.tail >= poolSize)
        {
            this.tail = 0;
        }
    }

    private bool isEmpty()
    {
        return quantidade <= 0;
    }

    private bool isFull()
    {
        return quantidade >= poolSize;
    }
}