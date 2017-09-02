using UnityEngine;
using System.Collections;

public class PlayerOptions : MonoBehaviour {

    

    public static PlayerOptions controller;

    void Awake()
    {
        if (PlayerOptions.controller == null)
        {
            // Atualiza referencia a objeto estático
            PlayerOptions.controller = this;

            // Torna objeto unico no jogo e imortal
            DontDestroyOnLoad(this.gameObject);

        }
        else if (PlayerOptions.controller != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
