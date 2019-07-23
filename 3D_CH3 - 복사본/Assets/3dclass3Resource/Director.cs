using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour {

    GameObject timerTxt;
    GameObject scoreTxt;
    float timer = 30.0f;
    int score = 0;
    GameObject generation;

    public void GetScore() {
        this.score += 100;
    }

    public void loseScore() {
        this.score /= 2;
    }
	// Use this for initialization
	void Start () {
        this.generation = GameObject.Find("dropGenerator");
        this.timerTxt = GameObject.Find("Timer");
        this.scoreTxt = GameObject.Find("Score");
    }
	
	// Update is called once per frame
	void Update () {

        this.timer -= Time.deltaTime;
        this.timerTxt.GetComponent<Text>().text = this.timer.ToString("F1");
        this.scoreTxt.GetComponent<Text>().text = this.score.ToString() + "Score";

        if(this.timer < 0) {
            this.timer = 0;

            this.generation.GetComponent<dropGenerator>().SetParameter(10000.0f, 0, 0);
        } else if (0 <= this.timer && this.timer < 5) { 
            this.generation.GetComponent<dropGenerator>().SetParameter(0.7f, -0.04f, 4);
        } else if (0 <= this.timer && this.timer < 14) {
            this.generation.GetComponent<dropGenerator>().SetParameter(0.4f, -0.06f, 8);
        } else if (0 <= this.timer && this.timer < 22) {
            this.generation.GetComponent<dropGenerator>().SetParameter(0.8f, -0.05f, 6);
        } else if (0 <= this.timer && this.timer < 30) {
            this.generation.GetComponent<dropGenerator>().SetParameter(1.0f, -0.03f, 2);
        }	
	}
}
