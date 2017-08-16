using UnityEngine;
using System.Collections;

public class MenuCursorActor : MonoBehaviour {

    public Vector3 originalPosition;                  // Posição original do cursor.
    public Vector3 posiçãoAtual;                      // Posição atual do cursor, depende do controlador.
    public MenuCursorReactor controlador;             // Pai do cursor, controlador das animações.
    public int passo;                                 // Quantidade de pixels que o cursor deve pular para apontar para o item correto


	// Use this for initialization
	void Start () {
        this.originalPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        if (controlador != null)
        {
            posiçãoAtual = this.originalPosition;
            posiçãoAtual.y = originalPosition.y - (passo) * controlador.currentItemID;
        }

        this.transform.localPosition = this.posiçãoAtual;
	}
}
