using UnityEngine;
using System.Collections;

public class CharacterQueue
{
    private GenericCharacter[] pilhaDeUnidades;
    int head;

    public CharacterQueue()
    {
        
        int tamanho = GameObject.FindGameObjectsWithTag(GameController.controller.getCurrentTag()).Length;

        this.pilhaDeUnidades = new GenericCharacter[tamanho];
        this.head = 0;

        GenericCharacter algumPersonagem;
        foreach (GameObject unidade in GameObject.FindGameObjectsWithTag(GameController.controller.getCurrentTag()))
        {
            algumPersonagem = unidade.GetComponent<GenericCharacter>();

            this.push(algumPersonagem);
        }
    }

    private bool push(GenericCharacter character)
    {
        if ((head + 1) > pilhaDeUnidades.Length)
        {
            return false;
        }

        
        pilhaDeUnidades[head] = character;
        head++;

        return true;
    }

    public GenericCharacter nextCharacter()
    {
        if (head - 1 < 0)
        {
            return null;
        }
        head--;

        return pilhaDeUnidades[head];
    }

    public int getSize()
    {
        return this.head;
    }
}
