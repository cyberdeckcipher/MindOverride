using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

    private ponto2D position;         // Posição do alvo no wargrid.
    public bool isTargetActive;       // True caso o objeto seja um alvo de camera ativo.
    private CameraTargetState estado; // Estado do alvo.
    private bool cursor;              // True caso este objeto seja um cursor (tenha o componente CursorReactor).
    private float targetVelocity;     // Velocidade de movimento do alvo.

    // Use this for initialization
    void Start() {
        this.cursor = this.gameObject.GetComponent<CursorReactor>() != null;

        this.isTargetActive = this.cursor;
    }

    // Update is called once per frame
    void Update() {

        if (this.gameObject.GetComponent<GenericCharacter>() != null)
        {
            this.isTargetActive = this.gameObject.GetComponent<GenericCharacter>().iaType == 1;
        }

        // Atualiza a posição do alvo
        if (this.isCursor())
        {
            this.setPosition(CursorReactor.cursor.getPosition());
            this.setVelocity(CursorReactor.cursor.cursorVelocity);
        }
        else
        {
            this.setPosition(this.gameObject.GetComponent<GenericCharacter>().gridPosition);
            this.setVelocity(this.gameObject.GetComponent<GenericCharacter>().getVelocity());
        }

        //Atualiza o estado do alvo
        if (this.isCursor())
        {
            if (CursorReactor.cursor.estadoCursor == CursorState.movendo &&
                this.estado == CameraTargetState.idle)
            {
                this.estado = CameraTargetState.movendo;
            }
            else if (this.estado == CameraTargetState.movendo)
            {
                this.estado = CameraTargetState.idle;
            }
        }
        else
        {
            GenericCharacter personagemAlvo = this.gameObject.GetComponent<GenericCharacter>();
            if (personagemAlvo != null)
            {
                if (personagemAlvo.estadoAtual == CharacterState.movendo &&
                this.estado == CameraTargetState.idle)
                {
                    this.estado = CameraTargetState.movendo;
                }
                else if (this.estado == CameraTargetState.movendo)
                {
                    this.estado = CameraTargetState.idle;
                }
            }
        }
    }

    /**
     /* @name: isCursor
     /* @version: 1.0
     /* @Description: True caso este objeto seja um cursor.
    */
    public bool isCursor()
    {
        return this.cursor;
    }

    /**
     /* @name: isCharacter
     /* @version: 1.0
     /* @Description: True caso este objeto seja um personagem.
    */
    public bool isCharacter()
    {
        return !(this.cursor);
    }

    /**
     /* @name: isEqual
     /* @version: 1.0
     /* @Description: True caso alvo e o alvo estejam no mesmo ponto.
    */
    public bool isEqual(CameraTarget outroAlvo)
    {
        return this.position.isEqual(outroAlvo.getPosition());
    }

    /**
     /* @name: getEstado
     /* @version: 1.0
     /* @Description: Retorna o estado atual do alvo da camera.
    */
    public CameraTargetState getEstado()
    {
        return this.estado;
    }

    /**
     /* @name: setPosition
     /* @version: 1.0
     /* @Description: Atualiza o valor da posição para o valor do ponto passado.
    */
    public void setPosition(ponto2D novoPonto)
    {
        this.position = novoPonto;
    }

    /**
     /* @name: getPosition
     /* @version: 1.0
     /* @Description: retorna o valor da posição para o valor do ponto passado.
    */
    public ponto2D getPosition()
    {
        return this.position;
    }

    /**
     /* @name: setPosition
     /* @version: 1.0
     /* @Description: Atualiza o valor da posição para o valor do ponto passado.
    */
    public void setVelocity(float novaVelocidade)
    {
        this.targetVelocity = novaVelocidade;
    }

    /**
     /* @name: getPosition
     /* @version: 1.0
     /* @Description: retorna o valor da posição para o valor do ponto passado.
    */
    public float getVelocity()
    {
        return this.targetVelocity;
    }
}
