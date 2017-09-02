using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    // Controle de posicionamento de camera
    public int virtualminX, virtualminY;         // Valores minimos da matrix de controle de camera
    public int virtualMaxX, virtualMaxY;         // Valores máximos da matrix de controle de camera
    public int alturaDaCamera;                   // Valor que controla a altura da camera

    private float cameraHeight;                   // Valor de controle da altura da camera em relação ao plano de jogo
    public ponto2D posicaoDaCamera;               // Posição da camera na matriz.

    // Controle de movimentação da camera
    private Vector3 fixedPosition, newPosition;   // Valores usados para controle de posicao da camera. FixedPosition é uma posição que a camera não pode sair, Nova posição acumula a posição para onde a camera deve se mover.
    private Quaternion fixedRotation;             // Valor fixo de rotação da camera.
    private Vector3 startPosition;                // Posição inicial da camera.
    public static CameraState estadoDaCamera;     // Estado de controle do movimento da camera.
    private CameraTarget alvoDaCamera;            // Alvo da camera a ser usado para seu controle de movimento.
    private float moveTimeElapsed;                // Tempo que se passou desde o inicio do movimento.

    // Singleton da Camera
    public static GameObject mainCamera;          // Acesso ao singleton da camera do jogo.

    //DEBUG
    public CameraState estadoAtualDaCamera;       // DEBUG ONLY;

    // Use this for initialization
    void Start () {

        CameraControl.mainCamera = this.gameObject;

        //Retangulo virtual inicial
        this.virtualminX = 4;
        this.virtualMaxX = 7;

        this.virtualminY = 2;
        this.virtualMaxY = 4;

        this.posicaoDaCamera = new ponto2D(5, 0); 
        Vector3 posicaoV3NoMundo = GameController.wargridToPosition(posicaoDaCamera, this.gameObject);

        // Posição inicial da camera, usada para calibragem
        this.transform.position = (new Vector3 (posicaoV3NoMundo.x, alturaDaCamera, posicaoV3NoMundo.z));

        updateTransformStandards(this.transform.position, this.transform.rotation);
    }
	
	// Update is called once per frame
	void Update () {

        //DEBUG
        this.estadoAtualDaCamera = CameraControl.estadoDaCamera;

        if (CameraControl.estadoDaCamera == CameraState.changeTarget)
        {
            // Caso seja o turno do jogador
            if (GameController.controller.jogadorAtivo.Equals(PlayersTags.Jogador.ToString()))
            {
                if (this.alvoDaCamera == null)
                {
                    alvoDaCamera = CursorReactor.cursor.GetComponent<CameraTarget>();
                    this.setCameraToPosition(alvoDaCamera);
                }
                else if (!this.alvoDaCamera.isCursor())
                {
                    alvoDaCamera = CursorReactor.cursor.GetComponent<CameraTarget>();
                    this.setCameraToPosition(alvoDaCamera);
                }
                CameraControl.estadoDaCamera = CameraState.idle;
            } // </ turno do jogador>

            // Caso seja o turno do inimigo:
            else
            {
                //Garante que o alvo da camera será atualizado somente se o personagem tiver sido escolhido pelo inimigo.
                if (GameController.controller.personagemEscolhido != null)
                {
                    CameraTarget alvo = GameController.controller.personagemEscolhido.GetComponent<CameraTarget>();

                    if (this.alvoDaCamera == null)
                    {
                        if (alvo.isTargetActive)
                        {
                            alvoDaCamera = alvo;
                            this.setCameraToPosition(alvoDaCamera);
                            CameraControl.estadoDaCamera = CameraState.idle;
                            updateTransformStandards(this.transform.position, this.transform.rotation);
                        }
                    }
                    else if (!this.alvoDaCamera.isEqual(GameController.controller.personagemEscolhido.GetComponent<CameraTarget>()))
                    {
                        if (alvo.isTargetActive)
                        {
                            alvoDaCamera = alvo;
                            this.setCameraToPosition(alvoDaCamera);
                            CameraControl.estadoDaCamera = CameraState.idle;
                            updateTransformStandards(this.transform.position, this.transform.rotation);
                        }
                    }
                }
            } // </ turno do inimigo>
        } //</change camera target>

        
        if (this.alvoDaCamera != null)
        {
            
            if (alvoDaCamera.getEstado() == CameraTargetState.movendo)
            {
                if (alvoDaCamera.getPosition().x >= this.virtualMaxX)
                {
                    if (alvoDaCamera.getPosition().y >= this.virtualMaxY)
                    {
                        //Direita Cima
                        MovecameraRIGHT_UP();
                    }
                    else if (alvoDaCamera.getPosition().y < this.virtualminY)
                    {
                        //Direita Baixo
                        MovecameraRIGHT_DOWN();
                    }
                    else
                    {
                        //Direita
                        MovecameraRIGHT();
                    }
                }
                else if (alvoDaCamera.getPosition().x < this.virtualminX)
                {
                    if (alvoDaCamera.getPosition().y >= this.virtualMaxY)
                    {
                        // Esquerda Cima
                        MovecameraLEFT_UP();
                    }
                    else if (alvoDaCamera.getPosition().y < this.virtualminY)
                    {
                        // Esquerda Baixo
                        MovecameraLEFT_DOWN();
                    }
                    else
                    {
                        // Esquerda
                        MovecameraLEFT();
                    }
                }

                else
                {
                    if (alvoDaCamera.getPosition().y >= this.virtualMaxY)
                    {
                        //cima
                        MovecameraUP();
                    }
                    else if (alvoDaCamera.getPosition().y < this.virtualminY)
                    {
                        //baixo
                        MovecameraDOWN();
                    }
                    else
                    {
                        // Dentro co retangulo
                        fixedTransform();
                    }
                }
            } // </ alvo.estado = movendo>

        } // </ alvo != null>

        // -------------- move a camera -------------------
        if (estadoDaCamera == CameraState.mover)
        {
            this.startPosition = this.transform.position;
            estadoDaCamera = CameraState.movendo;
            this.moveTimeElapsed = 0.0001f;
        } //</ Camera mover>

        if (estadoDaCamera == CameraState.movendo)
        {
            // Enquanto a posição do cursor for diferente da posição alvo, o lerp age
            if (!(this.transform.position == this.newPosition))
            {
                // Calcula o passo a ser aplicado.
                this.moveTimeElapsed += Time.deltaTime;
                float lerpFractionStep = (this.moveTimeElapsed / this.alvoDaCamera.getVelocity());

                // Aplica o passo do lerp.
                this.transform.position = Vector3.Lerp(this.startPosition, this.newPosition, lerpFractionStep);

                // Atualiza os valores padrões de rotação e posição da camera.
                updateTransformStandards(this.transform.position, this.transform.rotation);
            }

            // Ao terminar o movimento o Cursor volta ao estado Idle e atualiza a posição e rotação
            else
            {
                // Ao final do movimento, retorna o cursor ao estado Idle
                estadoDaCamera = CameraState.idle;
            }

        } //</ Camera movendo>
        // -------------- /move a camera -------------------
    }

    /**
     /* @name: addToVirtualXY
     /* @version: 1.0
     /* @Description: Modifica o valor do quadro virtual usado para controle de movimento da camera.
    */
    private void addToVirtualXY(int x, int y)
    {
        this.virtualMaxX += x;
        this.virtualminX += x;

        this.virtualMaxY += y;
        this.virtualminY += y;

        this.posicaoDaCamera.x += x;
        this.posicaoDaCamera.y += y;
        

        Vector3 wargridPositioning = GameController.wargridToPosition(posicaoDaCamera, this.gameObject);

        // Nova posição deve manter a autura da camera em relação ao chão
        this.newPosition = (new Vector3(wargridPositioning.x, this.fixedPosition.y, wargridPositioning.z));
        estadoDaCamera = CameraState.mover;
    }

    /**
     /* @name: setCameraToPosition
     /* @version: 1.0
     /* @Description: Modifica o valor do ponto2D onde a camera se encontra e a reposiciona a partir de um novo alvo
    */
    public void setCameraToPosition(CameraTarget novoAlvo)
    {
        // Modifica o alvo atual da camera .
        this.alvoDaCamera = novoAlvo;

        // Atualiza a posição da camera, de acordo com a posição do novo alvo.
        this.posicaoDaCamera = new ponto2D(this.alvoDaCamera.getPosition().x,
                                           this.alvoDaCamera.getPosition().y-2);
        
        this.virtualMaxX = alvoDaCamera.getPosition().x+2;
        this.virtualminX = alvoDaCamera.getPosition().x-1;

        this.virtualMaxY = alvoDaCamera.getPosition().y+2;
        this.virtualminY = alvoDaCamera.getPosition().y;
        
        // Seta o valor para posicionamento da camera no level.
        Vector3 posicaoV3NoMundo = GameController.wargridToPosition(posicaoDaCamera, this.gameObject);

        // Posiciona o objeto da camera no local calculado.
        this.transform.position = (new Vector3(posicaoV3NoMundo.x, alturaDaCamera, posicaoV3NoMundo.z));

        // Atualiza os valores de fixamento da camera para o novo local.
        updateTransformStandards(this.transform.position, this.transform.rotation);
    }

    private void MovecameraLEFT()
    {
        this.addToVirtualXY(-1,0);
    }

    private void MovecameraLEFT_UP()
    {
        this.addToVirtualXY(-1, 1);
    }

    private void MovecameraLEFT_DOWN()
    {
        this.addToVirtualXY(-1, -1);
    }

    private void MovecameraRIGHT()
    {
        this.addToVirtualXY(1, 0);
    }

    private void MovecameraRIGHT_UP()
    {
        this.addToVirtualXY(1, 1);
    }

    private void MovecameraRIGHT_DOWN()
    {
        this.addToVirtualXY(1, -1);
    }

    private void MovecameraUP()
    {
        this.addToVirtualXY(0, 1);
    }

    private void MovecameraDOWN()
    {
        this.addToVirtualXY(0, -1);
    }

    private void updateTransformStandards(Vector3 novaPosicao, Quaternion novaRotacao)
    {
        this.fixedPosition = novaPosicao;
        this.fixedRotation = novaRotacao;
    }

    private void fixedTransform()
    {
        this.transform.position = this.fixedPosition;
        this.transform.rotation = this.fixedRotation;
    }
}
