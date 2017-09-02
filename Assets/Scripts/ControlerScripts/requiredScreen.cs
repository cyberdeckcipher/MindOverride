using UnityEngine;

public class requiredScreen : MonoBehaviour {
    void Start()
    {

        Cutscene_Handler.controller.fadeInTo(Scene.Menu);
    }
	// Update is called once per frame
	void Update () {
        /*if (Input.GetButtonDown("Accept"))
        {
            Input.ResetInputAxes();
            // Vai para o menu
            Cutscene_Handler.controller.fadeInTo(Scene.Menu);
        }*/

    }
}
