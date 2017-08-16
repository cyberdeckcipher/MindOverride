// Todos os jogadores possiveis
public enum PlayersTags
{
    Demonios,
    Inimigos,
    Aliados,
    Jogador
}

// Ações possiveis para um jogador.
public enum playersActions
{
    Andar,
    Atacar,
    Interagir,
    PassarTurno,
    RecarregarNivel
}

// Estados que controlam o tipo de ação realizada pelo pathfinding, com objetivo de tornar a lista de obstáculos sensivel ao contexto.
public enum ActionsTypes
{
    Comum,
    Distance
}

//Estados que um determinado jogador pode ter enquanto escolhe ações com suas unidades.
public enum PlayersState
{
    idle,
    personagem,
    acao,
    agindo
}

// Tipos de ataque de um personagem.
public enum AttackType
{
    melee,
    ranged
}

// Estados possiveis que um personagem pode se encontrar.
public enum CharacterState
{
    idle,
    selecionado,
    atacando,
    atacado,
    mover,
    movendo,
    dead
}

// Estados possiveis que o cursor pode se encontrar.
public enum CursorState
{
    idle,
    selecionado, // highlighted
    mover,
    movendo
}

// Estados possiveis que o cursor pode se encontrar.
public enum CameraState
{
    idle,
    changeTarget,
    mover,
    movendo
}

// Estados possiveis para um alvo de camera.
public enum CameraTargetState
{
    idle,
    movendo
}

// Estados que controlam a criação de areas de ação em personagens.
public enum AreaOfActionState
{
    done,
    undone,
    changeAction
}

// Estados que controlam movimento e posicionamento de um Tile.
public enum TileStates
{
    idle,
    subir,
    subindo,
    descer,
    descendo,
    ativo,
    obstaculo,
    inativo
}

// Estado que controla cores possiveis para um Tile.
public enum TileColors
{
    obstaculo,
    andavel,
    interagir,
    invisivel
}

// Flags de ações usadas para sinalizar ao GameController quando uma ação termina.
public enum Actionflag
{
    idle,
    acting,
    actionEnd
}

// Estados de controle de fluxo do jogo.
public enum Gamestates
{
    paused,
    loadNewLevel,
    loadingLevel,
    levelLoaded,
    playing,
    cutscene,
    interactText
}

// Estados de controle de animação de glitch.
public enum GlitchState
{ 
    normal,
    glitch
}

// Tipos de glitch usados para animar o glitch.
public enum GlitchType
{
    multiSprites,
    rotate180
}

// Controle de que tipo de cutscene está sendo executada no momento.
public enum CutsceneType
{
    idle,
    levelInit,
    nextPlayer,
    levelClear,
    gameOver,
    battle,
    interact,
    fadeIN,
    fadeOUT,
}

// Tipos de objetos criados acima da cabeça das unidades.
public enum OverHeads
{
    playerText,
    IAExclamation
}

// Contextos de audio para controle de transições e sons.
public enum AudioContexts
{
    awake,
    premenu,
    menu,
    credits,
    loadingGameplay,
    playerTurn,
    enemyTurn,
    gameover
}

// Estados de controle da transição de contexto de audio. 
public enum AudioContState
{
    idle,
    change,
    changing,
}

public enum MissionUIState
{
    showing,
    toBeShown,
    hiden,
    chain
}

public enum Interactibles
{
    mission,
    door,
    jornal,
    computer,
    healthBox
}

public enum reactorMissions
{
    selectTutorial,
    moveTutorial,
    attackTutorial,
    interactTutorial
}