using UnityEngine;
using System.Collections;

public class GlitchAnimatorController : MonoBehaviour {

    // Static controll
    public static bool isWorldGlitched;                     // Variavel global que marca se os assets devem estar em glitch.
    public static bool ForceGlitched;                       // Variavel global que força o algoritmo a ativar o efeito de glitch.
    public static GlitchAnimatorController controller;      // Variavel de acesso estático ao singleton do controlador de animação de glitch.

    // Controle de tempo
    public float glitchedTime;                              // Quantidade de tempo que os assets passam com glitch.
    public float normalTime;                                // Quantidade de tempo que os assets passam sem glitch.

    // Estado interno do controlador
    private GlitchState estadoDoGlitch;                     // Estado interno do controlador de glitch global.

    void Awake()
    {
        if (GlitchAnimatorController.controller == null)
        {
            GlitchAnimatorController.controller = this;
        }
        else if (GlitchAnimatorController.controller != this)
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {
        GlitchAnimatorController.isWorldGlitched = false;
    }
	
	// Update is called once per frame
	void Update () {

        switch(this.estadoDoGlitch)
        {
            case GlitchState.normal:
                if (GlitchAnimatorController.isWorldGlitched)
                {
                    GlitchAnimatorController.isWorldGlitched = false;
                }
                if (this.normalTime > 0)
                {
                    this.normalTime -= Time.deltaTime;
                }
                else
                {
                    this.estadoDoGlitch = GlitchState.glitch;
                    this.nextGlitchedTime();
                }
                break;

            case GlitchState.glitch:
                if (!GlitchAnimatorController.isWorldGlitched)
                {
                    GlitchAnimatorController.isWorldGlitched = true;
                }
                if (this.glitchedTime > 0)
                {
                    this.glitchedTime -= Time.deltaTime;
                }
                else
                {
                    this.estadoDoGlitch = GlitchState.normal;
                    this.nextNormalTime();
                }
                break;
        }

        if (ForceGlitched)
        {
            GlitchAnimatorController.isWorldGlitched = true;
            Debug.Log("Force_Glitched_ON");
        }
    }

    /**
    /* @name: nextGlitchedTime
    /* @version: 1.0
    /* @Description: Marca quanto tempo o asset vai ficar no estado de glitch.
    */
    private void nextGlitchedTime()
    {
        int diceValue = Random.Range(0, 3);

        switch (diceValue)
        {
            case 0:
                this.glitchedTime = 0.2f;
                break;
            case 1:
                this.glitchedTime = 0.5f;
                break;
            case 2:
                this.glitchedTime = 1f;
                break;
            default:
                this.glitchedTime = 5f;
                break;
        }
    }

    /**
    /* @name: nextNormalTime
    /* @version: 1.0
    /* @Description: Marca quanto tempo o asset vai ficar no estado normal.
    */
    private void nextNormalTime()
    {
        int diceValue = Random.Range(0, 4);

        switch (diceValue)
        {
            case 0:
                this.normalTime = 0.5f;
                break;
            case 1:
                this.normalTime = 1f;
                break;
            case 2:
                this.normalTime = 2f;
                break;
            case 3:
                this.normalTime = 3f;
                break;
            default:
                this.normalTime = 1f;
                break;
        }
    }
}
