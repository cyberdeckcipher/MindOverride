using UnityEngine;
using System.Collections;

public class Overhead_Spawned : MonoBehaviour {

    public float tempoRestante;

    public OverHeads type;

	// Use this for initialization
	void Start () {
        this.tempoRestante = Overhead_Spawner.controller.timer;
	}
	
	// Update is called once per frame
	void Update () {
        if (tempoRestante >= 0)
        {
            switch (type)
            {
                case OverHeads.IAExclamation:
                    this.gameObject.transform.Rotate(0, Overhead_Spawner.controller.spawnedVelocity*2, 0);
                    break;

                case OverHeads.playerText:
                    this.gameObject.GetComponent<Transform>().position = new Vector3(this.gameObject.GetComponent<Transform>().position.x,
                                                                                this.gameObject.GetComponent<Transform>().position.y + Overhead_Spawner.controller.spawnedVelocity,
                                                                                this.gameObject.GetComponent<Transform>().position.z);
                    break;
            }
            

            tempoRestante -= Time.deltaTime;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
	}
}
