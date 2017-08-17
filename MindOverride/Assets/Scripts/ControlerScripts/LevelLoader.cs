using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {

    // Objetos para spawns
    // Misc
    public GameObject gameCursor;
    public GameObject healthBox;

    // Chao
    public GameObject pisoPredio;
    public GameObject pisoAsfaltoPreto;
    public GameObject pisoAsfaltoMeio;
    public GameObject pisoAsfaltoMeioFio;
    public GameObject pisoCalcada;
    public GameObject pisoCalcadaSuja;

    // Paredes
    public GameObject paredePilar;
    public GameObject paredeReta;
    public GameObject paredeCorner;
    public GameObject paredeT;
    public GameObject paredeX;
    public GameObject paredeFim;
    public GameObject paredePorta;
    public GameObject paredePoster;

    // Paredes fundo de Preto
    public GameObject paredeTPreta;
    public GameObject paredeCornerPreta;
    public GameObject paredeRetaPreta;
    public GameObject paredePosterPreta;
    public GameObject paredeJanelaPreta;

    // Moveis
    public GameObject jarro;
    public GameObject mesaFim;
    public GameObject mesaFim2;
    public GameObject mesaMeio;
    public GameObject mesaMeioPC1;
    public GameObject mesaMeioPC2;
    public GameObject mesaCorner;
    public GameObject mesaCentroVazia;
    public GameObject cadeira;
    public GameObject prateleira1;
    public GameObject prateleira2;
    public GameObject caixaGrande1;
    public GameObject caixaGrande2;
    public GameObject caixaPequena2;

    // Interagiveis
    public GameObject porta;
    public GameObject mesaCentro1;
    public GameObject mesaCentro2;
    public GameObject mesaCentro3;
    public GameObject caixaComJornal;

    // Missões
    public GameObject missaoPrincipalFase1;
    public GameObject missaoPrincipalTutorial;
    public GameObject Tutorial01;
    public GameObject Tutorial02;
    public GameObject Tutorial03;
    public GameObject Tutorial04;

    public GameObject humanTube;

    // Personagens
    public GameObject unidade;
    private int[] char1, char2, char3, char4, char5; // ficha depersonagens.

    public int nivelASerCarregado;                   // proximo nivel a ser carregado.

    // Street
    public GameObject hidenObstacle;
    public GameObject lixo1;
    public GameObject lixo2;
    public GameObject poste1;
    public GameObject poste2;
    public GameObject poste3;
    public GameObject predio1;
    public GameObject predio1B;
    public GameObject predio1B2;
    public GameObject predio2;
    public GameObject predio2B;
    public GameObject telhado1;
    public GameObject telhado1B;
    public GameObject telhado2;

    //Variaveis de controle para acesso aos mapas
    private int map_x;
    private int map_y;
    private int mapMax_x;
    private int mapMax_y;

    private int[][] level1, level2, level3, loadingMap;

    // Referencia Singleton
    public static LevelLoader controller;

    void Awake()
    {
        if (LevelLoader.controller == null)
        {
            LevelLoader.controller = this;
        }
        else if (LevelLoader.controller != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {

        this.map_x = 0;
        this.map_y = 0;

        // Ficha dos personagens do level
        // ficha = [acoes, life, walk, alcance, minalcance, dano, IA_Type]
        this.char1 = new int[] { 2, 6, 3, 2, 1, 4, 0 }; // menos dano, de longe 2
        this.char2 = new int[] { 2, 6, 3, 1, 0, 4, 0 }; // mto dano, de perto   3
        this.char3 = new int[] { 2, 2, 3, 1, 0, 2, 1 }; // IA_corre atras       4
        this.char4 = new int[] { 2, 4, 2, 2, 0, 2, 2 }; // IA_fica parado       5
        this.char5 = new int[] { 1, 2, 2, 1, 0, 2, 3 }; // IA_fica nao faz nada 6


        // mapa de posições iniciais do level [30x12]
        this.level1 = new int[][] {  new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 3,86,87},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 4, 3, 4,82,101},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5,78, 4, 3,86,88},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 4,73,86,89},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 3,84,102},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 3, 3,86,90},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 3,74,83,100},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5,72, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5,77, 3, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 4,82,100},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 3, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 3,76,95,102},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5,79, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 3, 3,81,100},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 4, 3, 3,86,-1,},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 4, 3,86,-1,},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 4, 3,83,101},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 3, 4,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 4, 3, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5,77, 4, 3,84,102},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 3, 4,85,102},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 3, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 4, 3,86,-1},
                                     new int []{ 2, 0, 0, 0, 0, 0, 5, 4, 3, 4,82,101},
                                     new int []{ 2, 0, 0, 1, 0, 0, 5, 3, 4, 3,86,-1}};

        // mapa de posições iniciais do level [21x30]
        this.level2 = new int[][] {  new int []{13,26,26,36,26,26,26,26,26,26,17,26,26,17,26,26,26,26,26,26,26,17,26,26,26,26,26,26,26,14 },
                                     new int []{29, 6, 6, 6,91, 6, 6, 6, 6, 6,29, 6, 6,29,49,55,80, 6, 6,25,46,29,49,55, 6, 6, 6,70,46,27 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6, 6,29,53, 6,29,10,76, 6, 6, 6,76, 8,29,10,76, 6, 6, 6,76,95,27 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6, 6,22, 6, 6,29,48, 6, 6, 6, 6,57,51,29,48, 6, 6, 6, 6,57,51,27 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6, 6, 6, 6, 6,20,28,28,21, 6,23,28,28,12,28,28,21, 6,23,28,28,18 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6,75, 6, 6, 6,29,49,55, 6, 6, 6,69,46,29,49,55, 6, 6, 6,68,46,27 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6,59,24, 6, 6,29,10,76, 6, 6, 6,76, 8,29,10,76, 6, 6, 6,76, 8,27 },
                                     new int []{29,63,63,63,63, 6, 6, 6, 6, 6,29, 6, 6,29,48, 6, 6, 6, 6,57,51,29,48, 6, 6, 6, 6,57,51,27 },
                                     new int []{29, 6, 6, 6, 6, 6, 6, 6, 6, 6,29, 6, 6,16,28,28,21, 6,23,28,28,19,28,28,21, 6,23,28,28,18 },
                                     new int []{29,71, 6, 6, 6, 6, 6,49,55, 6,29, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,75,27 },
                                     new int []{31,72, 6, 6, 6, 6, 6,48, 6, 6,29, 6,76,13,28,28,28,30,28,28,17,28,28,28,30,28,14, 6, 6,27 },
                                     new int []{29,71, 6, 6, 6, 6, 6,67,76, 6,29, 6, 6,29,25, 6, 6, 6,68,54,29,48,70,69, 6,25,29, 6, 6,27 },
                                     new int []{29, 6, 6, 6, 6, 6, 6,48, 6, 6,29, 6, 6,29,40,25,59, 6, 6, 6,29,48,54, 6, 6,40,29, 6, 6,27 },
                                     new int []{29, 6, 6, 6, 6, 6, 6,52,44, 6,29, 6, 6,29,68, 6,54,40,68, 6,29,10, 6,76, 6, 6,29, 6, 6,27 },
                                     new int []{29, 6, 6, 6, 6, 6, 6, 6, 6, 6,29, 6, 6,29, 6, 6,80,75,25,54,29,41, 6,94,76, 6,33, 6, 6,27 },
                                     new int []{29,59, 6, 6, 6, 6, 6, 6,59,40,29, 6, 6,29,76, 6, 6, 6, 6,25,29,25, 6, 6, 6, 6,29, 6, 6,27 },
                                     new int []{20,28,28,28,28,30,28,28,28,28,18, 6, 6,29,59,59, 6, 6,25,40,29,93, 6,75, 6, 6,29, 6, 6,27 },
                                     new int []{29, 6, 6,92, 6, 6,76, 6, 6, 6,22, 6, 6,16,28,28,28,30,28,28,19,28,28,28,30,28,15, 6,76,27 },
                                     new int []{29,80,76, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,76,75,27 },
                                     new int []{29, 6, 6, 6, 6, 6, 6, 6, 6,59,24, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,27 },
                                     new int []{16,28,28,28,28,28,28,28,28,28,19,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,28,15 }};

        // mapa de posições iniciais do level [21x30]
        this.level3 = new int[][] {  new int []{13, 26, 26, 26, 26, 26, 26, 26, 26, 26, 17, 26, 26, 26, 26, 26, 26, 26, 26, 26, 14},
                                     new int []{29, 59, 60, 60, 60, 60, 59,  6, 69, 70, 29,103,  6,103, 69,103,  6,103, 70,103, 27},
                                     new int []{29, 63, 80,  6,  6,  6,  6,  6,  6,  6, 29,  6,  6, 25,  6,  6,  6,  6,  6,  6, 27},
                                     new int []{29, 63,  6,  6,  6,  6, 76,  6,  6,  6, 29, 40, 68,  6,  6,103,  6,103,  6,103, 27},
                                     new int []{29, 63,  6, 75,  6, 59,  6,  6,  6,  6, 29,  6, 25,  6, 76,  6, 76,  6,  6,  6, 27},
                                     new int []{29, 63,  6,  6, 76, 11,  6, 76,  6,  6, 31,  6,  6,  6,  6,  6,  6,  6,  6, 95, 27},
                                     new int []{29, 68,  6,  6,  6, 59,  6, 49, 55,  6, 29,  6,  6, 76,  6, 75,  6,  6,  6,  6, 27},
                                     new int []{29, 40,  6,  6,  6, 76,  6, 10,  6,  6, 29,  6,  6,  6, 76,103,  6,103,  6,103, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6, 48, 76,  6, 29,  6,  6, 76,  6,  6,  6,  6, 76,  6, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6, 41,  6,  6, 29,103,  6,103,  6,103,  6,103,  6,103, 27},
                                     new int []{20, 28, 28, 30, 28, 28, 28, 28, 28, 28, 19, 28, 28, 28, 28, 28, 28, 28, 28, 28, 18},
                                     new int []{29,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, 75, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, 76,  6,  6, 76,  6,  6, 27},
                                     new int []{20, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 17, 28, 21,  6,  6,  6, 27},
                                     new int []{29, 59, 60, 60, 60, 60, 60, 70,  6,  6,  6, 76, 69, 75, 29,  6,  6,  6, 76,  6, 27},
                                     new int []{29, 63, 80, 76,  6,  6, 76,  6,  6,  6,  6,  6,  6,  6, 29,  6,  6,  6, 68,  6, 27},
                                     new int []{29, 63, 76, 75,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, 31,  6, 75, 25, 40, 76, 27},
                                     new int []{29, 63,  6,  6,  6, 59,  6,  6,  6,  6, 59,  6,  6,  6, 29,  6,  6,  6, 54, 80, 27},
                                     new int []{29, 63,  6,  6, 53, 11,  6,  6,  6, 23, 28, 28, 28, 17, 19, 28, 28, 28, 28, 28, 18},
                                     new int []{29, 59,  6,  6,  6, 69, 76,  6,  6,  6,  6,  6,  6, 22, 25,  6,  6,  6,  6, 59, 27},
                                     new int []{29,  6,  6,  6,  6,  6, 76,  6,  6,  6,  6,  6,  6,  6,  6,  6, 76,  6,  6,  6, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6, 91,  6,  6,  6,  6,  6, 24,  6,  6,  6,  6,  6,  6, 27},
                                     new int []{20, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 18,  6,  6,  6, 76,  6,  6, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, 31,  6,  6,  6,  6,  6,  6, 27},
                                     new int []{29,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6,  6, 27,  6,  6,  6,  6,  6,  6, 27},
                                     new int []{29,  6,  6, 23, 17, 28, 28, 30, 28, 28, 17, 28, 30, 19, 28, 17, 28, 30, 28, 28, 18},
                                     new int []{29,  6,  6,  6, 29, 91,  6, 75,  6, 59, 29, 92,  6, 75,  6, 29, 53, 76,  6,  6, 27},
                                     new int []{29, 71,  6,  6, 29, 68,  6,  6,  6,  6, 29,  6,  6, 40, 68, 29,  6,  6,  6,  6, 27},
                                     new int []{29, 72, 71,  6, 29, 40, 25,  6,  6,  6, 29,  6,  6,  6, 80, 29,  6,  6,  6,  6, 27},
                                     new int []{16, 28, 28, 28, 19, 28, 28, 28, 28, 28, 19, 28, 28, 28, 28, 19, 28, 28, 28, 28, 15}};

        //Força valores iniciais para primeira fase caso o valor da fase seja negativo
        if (this.nivelASerCarregado < 1)
        {
            this.nivelASerCarregado = 1;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (GameController.getEstadoDoJogo() == Gamestates.loadingLevel)
        {
            /** Loader pseudo code:
             caso x e y == max, muda estado do jogo para jogando.
             lê uma posição no mapa
             spawn objeto
             atualiza valor
            */

            // pega valor de ID na posição atual
            int newMapValue = loadingMap[this.map_x][this.map_y];

            // lê o mapa e cria um objeto na posição atual
            if (newMapValue >= 0) // Caso o ID lido seja um numero valido
            {
                // Spawn Object
                spawnIDObjectAtLocation(newMapValue, new ponto2D(map_x, map_y));
            }


            // Depois de criar objeto da posicao atal, atualiza para a proxima posicao
            if (map_x < (mapMax_x-1))
            {
                map_x++;
            }
            else
            {
                if ( map_y < (mapMax_y-1))
                {
                    map_y++;
                    map_x = 0;
                }
                else
                {
                    GameController.levelLoadedFlag();
                }
            }


        } // </loading level>


        else if (GameController.getEstadoDoJogo() == Gamestates.loadNewLevel)
        {

            // atualiza variaveis de valores X e Y maximos e o mapGrid a ser carregado.
            switch (this.nivelASerCarregado)
            {
                case 1:
                    loadingMap = this.level1;
                    this.mapMax_x = 30;
                    this.mapMax_y = 12;
                    spawGameCursor(new ponto2D(10, 7));
                    break;

                case 2:
                    loadingMap = this.level2;
                    this.mapMax_x = 21;
                    this.mapMax_y = 30;
                    spawGameCursor(new ponto2D(10, 2));
                    break;

                case 3:
                    loadingMap = this.level3;
                    this.mapMax_x = 30;
                    this.mapMax_y = 21;
                    spawGameCursor(new ponto2D(27,2));
                    break;

                case 4:
                    Cutscene_Handler.controller.fadeInTo(Scene.Credits);
                    break;

                default:
                    Debug.Log("[LOADER] NIVEL INVALIDO ="+this.nivelASerCarregado);
                    break;
            }

            // Atualiza variavel para visibilidade de informação
            GameController.nivelCarregadoAtual = this.nivelASerCarregado;

            this.map_x = 0;
            this.map_y = 0;

            // Atualiza valores de X e Y e reinicia o wargrid baseado nestes valores.
            GameController.updateMaxSize(this.mapMax_x, this.mapMax_y);

            // Sinaliza o inicio do carregamento de uma nova fase
            GameController.LoadingLevelFlag(); 

        }// </load new level>

	} //</ UPDATE>


    // -------------------------------------------------------------- public functions ------------------------------------------------------------------------------
    /**
    /* @name: loadNextLevel
    /* @version: 1.0
    /* @Description: carrega o proximo nivel
    */
    public static void loadNextLevel()
    {
        LevelLoader loader = LevelLoader.controller;

        loader.nivelASerCarregado++;
    }

    /**
    /* @ name: isWall
    /* @ version: 1.0
    /* @ Description: true caso posição apassada contenha valor referente a uma parede.
    /* @ DEPRECATED
    */
    public static bool isWall(ponto2D posicao)
    {
        LevelLoader loader = GameObject.FindGameObjectWithTag("gameLoader").GetComponent<LevelLoader>();
        return loader.loadingMap[posicao.x][posicao.y] > 10 &&
               loader.loadingMap[posicao.x][posicao.y] < 60;
    }
    // -------------------------------------------------------------- /public functions ------------------------------------------------------------------------------








    // -------------------------------------------------------------- private functions ------------------------------------------------------------------------------

    /**
    /* @name: spawGameCursor
    /* @version: 1.5
    /* @Description: intancia um cursor de jogo na tela, para ser controlado pelo jogador
    */
    private void spawGameCursor(ponto2D posicaoDoCursor)
    {
        CursorReactor novoCursor = ((GameObject)Instantiate(this.gameCursor,                                                  // O que instanciar.
                                                        GameController.wargridToPosition(posicaoDoCursor, this.gameCursor),   // Posição de instanciamento.
                                                        Quaternion.Euler(new Vector3()))                                      // Rotação inicial.
                                    ).GetComponent<CursorReactor>();

        novoCursor.setPosition(posicaoDoCursor);
        novoCursor.tag = "GameCursor";
    }

    /**
    /* @name: isPar
    /* @version: 1.0
    /* @Description: true caso o numero seja par
    */
    private bool isPar(int x)
    {
        return ((x % 2) == 0);
    }

    /**
    /* @name: isValid
    /* @version: 1.0
    /* @Description: True caso o ponto esteja contido dentro do range de valores possiveis do wargrid.
    */
    private bool isValidPoint(ponto2D ponto)
    {
        return ponto.x >= 0 && ponto.x < this.mapMax_x &&
               ponto.y >= 0 && ponto.y < this.mapMax_y;
    }

    /**
    /* @name: spawnObject
    /* @version: 1.2
    /* @Description: spawna um objeto em um ponto especifico a partir de seu valor de ID
    */
    private void spawnObject(GameObject objeto, ponto2D ponto, int rotation, string novaTag)
    {
        Quaternion transformRotation = new Quaternion();
        Vector3 posicao = GameController.wargridToPosition(ponto, objeto);

        GameObject spawnedObject = ((GameObject)Instantiate(objeto,       // O que instanciar.
                                                     posicao,             // Posição de instanciamento.
                                                     transformRotation)   // Rotação inicial.
                                    );

        spawnedObject.tag = novaTag;

        spawnedObject.transform.Rotate(new Vector3(0,rotation));

        if (novaTag != "Ground")
        {
            GameController.setObjectatGrid(spawnedObject, ponto);
        }
    }

    /**
    /* @overload: #1
    /* @name: spawnObject
    /* @version: 1.6
    /* @Description: spawna um objeto interativo em um ponto especifico a partir de seu valor de ID e configura suas variaveis de controle
    */
    private GameObject spawnObject(GameObject objeto, ponto2D ponto, int rotation, bool newisreusable, bool newisdestructable, bool isMission)
    {
        Quaternion transformRotation = new Quaternion();
        Vector3 posicao = GameController.wargridToPosition(ponto, objeto);

        GameObject spawnedObject = ((GameObject)Instantiate(objeto,       // O que instanciar.
                                                     posicao,             // Posição de instanciamento.
                                                     transformRotation)   // Rotação inicial.
                                    );

        spawnedObject.tag = "interativos";

        spawnedObject.GetComponent<InteractActor>().isReusable = newisreusable;
        spawnedObject.GetComponent<InteractActor>().isDestructable = newisdestructable;
        spawnedObject.GetComponent<InteractActor>().missionObjective = isMission;
        spawnedObject.GetComponent<InteractActor>().gridposition = ponto;

        spawnedObject.transform.Rotate(new Vector3(0, rotation));

        GameController.setObjectatGrid(spawnedObject, ponto);

        //Se for uma missão, adiciona na lista de missões.
        if (isMission)
        {
            MissionMenu_Handler.controller.addMission(spawnedObject.GetComponent<InteractActor>());
        }

        return spawnedObject;
    }

    /* @name: spawCharUnity
    /* @version: 2.7
    /* @Description: inicia uma unidade especifica da lista de personagens em uma posição especifica.
    */
    private void spawCharUnity(GameObject corpo, int charNumber, ponto2D ponto)
    {

        if (nivelASerCarregado == 1)
        {
            spawnObject(pisoCalcada, ponto, 0, "Ground");
        }
        else
        {
            spawnObject(pisoPredio, ponto, 0, "Ground");
        }

        Vector3 posicao = GameController.wargridToPosition(ponto, corpo);

        GenericCharacter unidade = ((GameObject)Instantiate(corpo,                              // O que instanciar.
                                                            posicao,                            // Posição de instanciamento.
                                                            Quaternion.Euler(new Vector3()))    // Rotação inicial.
                                    ).GetComponent<GenericCharacter>();
        /*
        Ficha dos personagens do level
        ficha = [acoes, life, walk, alcance, minalcance, dano, IA_Type]

        this.char1 = new int[] { 2, 4, 3, 2, 1, 1, 0 }; // menos dano, de longe 2
        this.char2 = new int[] { 2, 4, 3, 1, 0, 4, 0 }; // mto dano, de perto   3
        this.char3 = new int[] { 2, 2, 1, 1, 1, 2, 1 }; // IA_corre atras       4
        this.char4 = new int[] { 2, 4, 1, 1, 1, 2, 2 }; // IA_fica parado       5
        */

        int[] ficha = new int[] { 1, 1, 1, 1, 0, 1, 1 }; // char minimo inicial para debug;
        PlayersTags novoJogador = PlayersTags.Demonios;

        //seta tag e dono da unidade
        switch (charNumber)
        {
            case -1:
                ficha = this.char1;
                novoJogador = PlayersTags.Aliados;
                break;
            case 2:
                ficha = this.char1;
                novoJogador = PlayersTags.Jogador;
                break;
            case 3:
                ficha = this.char2;
                novoJogador = PlayersTags.Jogador;
                break;
            case 4:
                ficha = this.char3;
                novoJogador = PlayersTags.Demonios;
                break;
            case 5:
                ficha = this.char4;
                novoJogador = PlayersTags.Demonios;
                break;
            case 6:
                ficha = this.char5;
                novoJogador = PlayersTags.Demonios;
                break;
        }

        //Setando ficha de personagem e informações da unidade
        unidade.setUnity(ficha[0], ficha[1], ficha[2], ficha[3], ficha[4], ficha[5], ficha[6]);
        unidade.gridPosition = ponto;
        unidade.jogador = novoJogador;
        unidade.tag = novoJogador.ToString();

        unidade.gameObject.GetComponent<Animator>().SetInteger("idleType", Random.Range(1, 4));

        //Adiciona a unidade ao wargrid
        GameController.setObjectatGrid(unidade.gameObject, ponto);

        if (unidade.alcance > 1) {
            unidade.tipoDeAtaque = AttackType.ranged;
        }

        if (unidade.jogador == PlayersTags.Demonios)
        {
            if (this.nivelASerCarregado == 1)
            {
                unidade.setUnity(0, 1, 0, 0, 0, 0, 2);
            }
            unidade.objectRender.material.color = Color.red;
            unidade.transform.Rotate(new Vector3(0, 190, 0));
        }
        else if (unidade.jogador == PlayersTags.Jogador)
        {
            unidade.objectRender.material.color = Color.blue;
        }
        else
        {
            unidade.tag = "Ground";
            unidade.setUnity(0, 1, 0, 0, 0, 0, 3);
            unidade.estadoAtual = CharacterState.dead;
            unidade.gameObject.GetComponent<Animator>().SetTrigger("inTube");
        }

    }

    /**
    /* @name: spawnIDObjectAtLocation
    /* @version: 1.6
    /* @Description: spawna um objeto em um ponto especifico a partir de seu valor de ID
    */
    private void spawnIDObjectAtLocation(int idValue, ponto2D posicao)
    {
        if (idValue < 50)
        {
            if (idValue < 25)
            {
                if (idValue < 12)
                {
                    if (idValue < 6)
                    {
                        switch (idValue)
                        {
                            case 0:
                                spawnObject(pisoAsfaltoPreto, posicao, 0, "Ground");
                                break;
                            case 1:
                                spawnObject(pisoAsfaltoMeio, posicao, 0, "Ground");
                                break;
                            case 2:
                                spawnObject(pisoAsfaltoMeioFio, posicao,90, "Ground");
                                break;
                            case 3:
                                spawnObject(pisoCalcada, posicao, 0, "Ground");
                                break;
                            case 4:
                                spawnObject(pisoCalcadaSuja, posicao, 0, "Ground");
                                break;
                            case 5:
                                spawnObject(pisoAsfaltoMeioFio, posicao,270, "Ground");
                                break;
                        }
                    }
                    else // >= 6
                    {
                        switch (idValue)
                        {
                            case 6:
                                spawnObject(pisoPredio, posicao, 0, "Ground");
                                break;
                            case 7:
                                spawnObject(mesaMeioPC2, posicao, 0, "semiObstacle");
                                break;
                            case 8:
                                spawnObject(mesaMeioPC2, posicao, 90, "semiObstacle");
                                break;
                            case 9:
                                spawnObject(mesaMeioPC2, posicao, 180, "semiObstacle");
                                break;
                            case 10:
                                spawnObject(mesaMeioPC2, posicao, 270, "semiObstacle");
                                break;
                            case 11:
                                spawnObject(paredePilar, posicao,0, "Obstacle");
                                break;
                        }
                    }
                }
                else // >= 12
                {
                    if (idValue < 18)
                    {
                        switch (idValue)
                        {
                            case 12:
                                spawnObject(paredeX, posicao, 0, "Obstacle");
                                break;
                            case 13:
                                spawnParedeCorner(posicao, 0);
                                break;
                            case 14:
                                spawnParedeCorner(posicao, 90);
                                break;
                            case 15:
                                spawnParedeCorner(posicao, 180);
                                break;
                            case 16:
                                spawnParedeCorner(posicao, 270);
                                break;
                            case 17:
                                spawnParedeT(posicao, 0);
                                break;
                        }
                    }
                    else // >= 18
                    {
                        switch (idValue)
                        {
                            case 18:
                                spawnParedeT(posicao, 90);
                                break;
                            case 19:
                                spawnParedeT(posicao, 180);
                                break;
                            case 20:
                                spawnParedeT(posicao, 270);
                                break;
                            case 21:
                                spawnObject(paredeFim, posicao, 0, "Obstacle");
                                break;
                            case 22:
                                spawnObject(paredeFim, posicao, 90, "Obstacle");
                                break;
                            case 23:
                                spawnObject(paredeFim, posicao, 180, "Obstacle");
                                break;
                            case 24:
                                spawnObject(paredeFim, posicao, 270, "Obstacle");
                                break;
                        }
                    }
                }
            }
            else // >= 25
            {
                if (idValue < 38)
                {
                    if (idValue < 32)
                    {
                        switch (idValue)
                        {
                            case 25:
                                spawnObject(caixaGrande1, posicao, 0, "semiObstacle");
                                break;
                            case 26:
                                spawnParedeReta(posicao, 0);
                                break;
                            case 27:
                                spawnParedeReta(posicao, 90);
                                break;
                            case 28:
                                spawnParedeReta(posicao, 180);
                                break;
                            case 29:
                                spawnParedeReta(posicao, 270);
                                break;
                            case 30:
                                spawnObject(paredePorta, posicao, 0, "Obstacle");
                                spawnObject(porta, posicao, 0, false, true, false);
                                break;
                            case 31:
                                spawnObject(paredePorta, posicao, 90, "Obstacle");
                                spawnObject(porta, posicao, 90, false, true, false);
                                break;
                        }
                    }
                    else // > 31
                    {
                        switch (idValue)
                        {
                            case 32:
                                spawnParedePoster(posicao, 0);
                                break;
                            case 33:
                                spawnParedePoster(posicao, 90);
                                break;
                            case 34:
                                spawnParedePoster(posicao, 180);
                                break;
                            case 35:
                                spawnParedePoster(posicao, 270);
                                break;
                            case 36:
                                spawnParedeJanela(posicao, 0);
                                break;
                            case 37:
                                spawnParedeJanela(posicao, 90);
                                break;
                        }
                    }
                }
                else // >= 38
                {
                    if (idValue < 44)
                    {
                        switch (idValue)
                        {
                            case 38:
                                spawnParedeJanela(posicao, 180);
                                break;
                            case 39:
                                spawnParedeJanela(posicao, 270);
                                break;
                            case 40:
                                spawnObject(caixaGrande2, posicao, 0, "semiObstacle");
                                break;
                            case 41:
                                spawnObject(mesaFim, posicao, 0, "semiObstacle");
                                break;
                            case 42:
                                spawnObject(mesaFim, posicao, 90, "semiObstacle");
                                break;
                            case 43:
                                spawnObject(mesaFim, posicao, 180, "semiObstacle");
                                break;
                        }
                    }
                    else // > 43
                    {
                        switch (idValue)
                        {
                            case 44:
                                spawnObject(mesaFim, posicao, 270, "semiObstacle");
                                break;
                            case 45:
                                spawnObject(mesaMeio, posicao, 0, "semiObstacle");
                                break;
                            case 46:
                                spawnObject(mesaMeio, posicao, 90, "semiObstacle");
                                break;
                            case 47:
                                spawnObject(mesaMeio, posicao, 180, "semiObstacle");
                                break;
                            case 48:
                                spawnObject(mesaMeio, posicao, 270, "semiObstacle");
                                break;
                            case 49:
                                spawnObject(mesaCorner, posicao, 0, "semiObstacle");
                                break;
                        }
                    }
                }
            }
        }



        else // >= 50
        {
            if (idValue < 75)
            {
                if (idValue < 62)
                {
                    if (idValue < 57)
                    {
                        switch (idValue)
                        {
                            case 50:
                                spawnObject(mesaCorner, posicao, 90, "semiObstacle");
                                break;
                            case 51:
                                spawnObject(mesaCorner, posicao, 180, "semiObstacle");
                                break;
                            case 52:
                                spawnObject(mesaCorner, posicao, 270, "semiObstacle");
                                break;
                            case 53:
                                spawnObject(mesaCentroVazia, posicao, 0, "semiObstacle");
                                break;
                            case 54:
                                spawnObject(caixaGrande2, posicao, 90, "semiObstacle");
                                break;
                            case 55:
                                spawnObject(mesaFim2, posicao, 0, "semiObstacle");
                                break;
                            case 56:
                                spawnObject(mesaFim2, posicao, 90, "semiObstacle");
                                break;
                        }
                    }
                    else // >= 57
                    {
                        switch (idValue)
                        {
                            case 57:
                                spawnObject(mesaFim2, posicao, 180, "semiObstacle");
                                break;
                            case 58:
                                spawnObject(mesaFim2, posicao, 270, "semiObstacle");
                                break;
                            case 59:
                                spawnObject(jarro, posicao, 0, "semiObstacle");
                                break;
                            case 60:
                                spawnObject(cadeira, posicao, 0, "Ground");
                                break;
                            case 61:
                                spawnObject(cadeira, posicao, 90, "Ground");
                                break;
                        }
                    }
                }
                else // >= 62
                {
                    if (idValue < 69)
                    {
                        switch (idValue)
                        {
                            case 62:
                                spawnObject(cadeira, posicao, 180, "Ground");
                                break;
                            case 63:
                                spawnObject(cadeira, posicao, 270, "Ground");
                                break;
                            case 64:
                                spawnObject(mesaMeioPC1, posicao, 0, "semiObstacle");
                                break;
                            case 65:
                                spawnObject(mesaMeioPC1, posicao, 90, "semiObstacle");
                                break;
                            case 66:
                                spawnObject(mesaMeioPC1, posicao, 180, "semiObstacle");
                                break;
                            case 67:
                                spawnObject(mesaMeioPC1, posicao, 270, "semiObstacle");
                                break;
                            case 68:
                                spawnObject(caixaPequena2, posicao, 0, "semiObstacle");
                                break;
                        }
                    }
                    else // >= 69
                    {
                        switch (idValue)
                        {
                            case 69:
                                spawnObject(prateleira1,posicao,0, "Obstacle");
                                break;
                            case 70:
                                spawnObject(prateleira2, posicao, 0, "Obstacle");
                                break;
                            case 71:
                                spawCharUnity(unidade, 2, posicao);
                                break;
                            case 72:
                                spawCharUnity(unidade, 3, posicao);
                                break;
                            case 73:
                                spawnObject(lixo1, posicao, 0, "Obstacle");
                                break;
                            case 74:
                                spawnObject(lixo2, posicao, 0, "Obstacle");
                                break;
                        }
                    }
                }
            }
            else // >= 75
            {
                if (idValue < 87)
                {
                    if (idValue < 81)
                    {
                        switch (idValue)
                        {
                            case 75:
                                spawCharUnity(unidade, 4, posicao);
                                break;
                            case 76:
                                spawCharUnity(unidade, 5, posicao);
                                break;
                            case 77:
                                spawnObject(poste1, posicao, 180, "Obstacle");
                                break;
                            case 78:
                                spawnObject(poste2, posicao, 180, "Obstacle");
                                break;
                            case 79:
                                spawnObject(poste3, posicao, 180, "Obstacle");
                                break;
                            case 80:
                                spawnObject(healthBox, posicao, 0, false, true, false);
                                spawnObject(pisoPredio, posicao, 0, "Ground");
                                break;
                        }
                    }
                    else // >= 81
                    {
                        switch (idValue)
                        {
                            case 81:
                                spawnObject(predio1, posicao, 90, "Obstacle");
                                break;
                            case 82:
                                spawnObject(predio1B, posicao, 90, "Obstacle");
                                break;
                            case 83:
                                spawnObject(predio1B2, posicao, 90, "Obstacle");
                                break;
                            case 84:
                                spawnObject(predio2, posicao, 90, "Obstacle");
                                break;
                            case 85:
                                spawnObject(predio2B, posicao, 90, "Obstacle");
                                break;
                            case 86:
                                spawnObject(hidenObstacle, posicao, 0, "Obstacle");
                                break;
                        }
                    }
                }
                else // >= 87
                {
                    if (idValue < 94)
                    {
                        switch (idValue)
                        {
                            case 87:
                                spawnObject(Tutorial01, posicao, 0, false, false, true);
                                break;
                            case 88:
                                spawnObject(Tutorial02, posicao, 0, false, false, true);
                                break;
                            case 89:
                                spawnObject(Tutorial03, posicao, 0, false, false, true);
                                break;
                            case 90:
                                spawnObject(Tutorial04, posicao, 0, false, false, true);
                                break;
                            case 91:
                                spawnObject(mesaCentro1, posicao, 0, true, false, false);
                                break;
                            case 92:
                                spawnObject(mesaCentro2, posicao, 0, true, false, false);
                                break;
                            case 93:
                                spawnObject(mesaCentro3, posicao, 0, true, false, false);
                                break;
                        }
                    }
                    else // >= 94
                    {
                        switch (idValue)
                        {
                            case 94:
                                spawnObject(caixaComJornal, posicao, 0, true, false, false);
                                break;
                            case 95:
                                spawnMissaoPrincipal(posicao);
                                break;
                            case 96:
                                spawnObject(mesaFim2, posicao, 0, "semiObstacle");
                                break;
                            case 97:
                                spawnObject(mesaFim2, posicao, 90, "semiObstacle");
                                break;
                            case 98:
                                spawnObject(mesaFim2, posicao, 180, "semiObstacle");
                                break;
                            case 99:
                                spawnObject(mesaFim2, posicao, 270, "semiObstacle");
                                break;
                            case 100:
                                spawnObject(telhado1, posicao, 270, "Obstacle");
                                break;
                            case 101:
                                spawnObject(telhado1B, posicao, 270, "Obstacle");
                                break;
                            case 102:
                                spawnObject(telhado2, posicao, 270, "Obstacle");
                                break;
                            case 103:
                                spawnObject(humanTube, posicao, 0, "Obstacle");
                                spawCharUnity(unidade, -1, posicao);
                                break;
                        }
                    }
                }
            }
        }// <= 100

    }// </ spawnIDObjectAtLocation>

    /**
    /* @name: isborda
    /* @version: 1.0
    /* @Description: true caso o ponto persença a borda do mapa.
    */
    private bool isborda(ponto2D ponto)
    {
        return ponto.x == 0 || ponto.x == this.mapMax_x-1 
            || ponto.y == 0 || ponto.y == this.mapMax_y-1;

    }

    /**
    /* @name: spawnParedeReta
    /* @version: 1.0
    /* @Description: cria uma parede na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnParedeReta(ponto2D ponto, int rotation)
    {
        if (isborda(ponto))
        {
            spawnObject(paredeRetaPreta, ponto, rotation, "Obstacle");
        }
        else
        {
            spawnObject(paredeReta, ponto, rotation, "Obstacle");
        }
    }

    /**
    /* @name: spawnParedeT
    /* @version: 1.0
    /* @Description: cria uma parede T na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnParedeT(ponto2D ponto, int rotation)
    {
        if (isborda(ponto))
        {
            spawnObject(paredeTPreta, ponto, rotation, "Obstacle");
        }
        else
        {
            spawnObject(paredeT, ponto, rotation, "Obstacle");
        }
    }

    /**
    /* @name: spawnParedeCorner
    /* @version: 1.0
    /* @Description: cria uma parede Corner na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnParedeCorner(ponto2D ponto, int rotation)
    {
        if (isborda(ponto))
        {
            spawnObject(paredeCornerPreta, ponto, rotation, "Obstacle");
        }
        else
        {
            spawnObject(paredeCorner, ponto, rotation, "Obstacle");
        }
    }

    /**
    /* @name: spawnParedePoster
    /* @version: 1.0
    /* @Description: cria uma parede com poster na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnParedePoster(ponto2D ponto, int rotation)
    {
        if (isborda(ponto))
        {
            spawnObject(paredePosterPreta, ponto, rotation, "Obstacle");
        }
        else
        {
            spawnObject(paredePoster, ponto, rotation, "Obstacle");
        }
    }

    /**
    /* @name: spawnParedeJanela
    /* @version: 1.0
    /* @Description: cria uma parede com Janela na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnParedeJanela(ponto2D ponto, int rotation)
    {
        if (isborda(ponto))
        {
            spawnObject(paredeJanelaPreta, ponto, rotation, "Obstacle");
        }
        else
        {
            spawnObject(paredeJanelaPreta, ponto, rotation, "Obstacle");
        }
    }

    /**
    /* @name: spawnMissaoPrincipal
    /* @version: 1.0
    /* @Description: cria uma parede na posição OU uma parede com as costas escuras se o ponto for a borda do mapa.
    */
    private void spawnMissaoPrincipal(ponto2D ponto)
    {
        if (this.nivelASerCarregado >= 2)
        {   if (this.nivelASerCarregado == 3)
            {
                spawnObject(missaoPrincipalFase1, ponto, 90, false, false, true);
            }
            else
            {
                spawnObject(missaoPrincipalFase1, ponto, 90, false, false, true);
            }
            
        }
        else
        {
            spawnObject(missaoPrincipalTutorial, ponto, 90, "Ground");
            spawnObject(porta, ponto, 90, false, true, false);
        }
    }
}
