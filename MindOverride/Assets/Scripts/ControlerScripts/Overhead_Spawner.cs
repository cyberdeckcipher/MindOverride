using UnityEngine;
using System.Collections;

public class Overhead_Spawner : MonoBehaviour {

    // escala
    public GameObject unidade;                   // Exemplo de humano para escala

    // objetos para spawn
    public GameObject move;                      // Imagem que aparece na unidade do jogador quando recebe uma ordem de mover.
    public GameObject attack;                    // Imagem que aparece na unidade do jogador quando recebe uma ordem de atacar.
    public GameObject interact;                  // Imagem que aparece na unidade do jogador quando recebe uma ordem de interagir.
    public GameObject wait;                      // Imagem que aparece na unidade do jogador quando recebe uma ordem de esperar.
    public GameObject exclamacao;                // Exclamacao que aparece em cima da cabeça de um inimigo quando ele nota um adversario.

    // controle de tempo
    public float timer;                   // Tempo até o objeto se destruir.
    public float spawnedVelocity;         // Velocidade em que a imagem spawnada sobe.

    public static Overhead_Spawner controller;

    void Awake()
    {
        if (Overhead_Spawner.controller == null)
        {
            Overhead_Spawner.controller = this;
        }
        else if (Overhead_Spawner.controller != this)
        {
            Destroy(this);
        }
    }

    /**
    /* @name: spawnOverhead
    /* @version: 1.1
    /* @Description: spawna um texto acima da cabeça de uma unidade
    */
    public static void spawnOverhead(int idDaAcao, OverHeads tipo)
    {
        ponto2D ponto = GameController.controller.personagemEscolhido.gridPosition;
        GameObject objeto = null;

        switch (idDaAcao)
        {
            case 1:
                objeto = Overhead_Spawner.controller.move;
                break;

            case 2:
                objeto = Overhead_Spawner.controller.attack;
                break;

            case 3:
                objeto = Overhead_Spawner.controller.interact;
                break;

            case 4:
                objeto = Overhead_Spawner.controller.wait;
                break;

            case 5:
                objeto = Overhead_Spawner.controller.exclamacao;
                break;
        }

        Quaternion transformRotation = new Quaternion();
        Vector3 posicao = GameController.wargridToPosition(ponto, objeto);
        
        switch (tipo)
        {
            case OverHeads.IAExclamation:
                posicao.y = 55;
                break;

            case OverHeads.playerText:
                posicao.y = 47;
                break;
        }

        GameObject spawnedObject = ((GameObject)Instantiate(objeto,       // O que instanciar.
                                                     posicao,             // Posição de instanciamento.
                                                     transformRotation)   // Rotação inicial.
                                    );
        
        switch (tipo)
        {
            case OverHeads.IAExclamation:
                break;

            case OverHeads.playerText:
                spawnedObject.transform.rotation = CameraControl.mainCamera.transform.rotation;
                break;
        }
        
        spawnedObject.GetComponent<Overhead_Spawned>().type = tipo;
    }

}
