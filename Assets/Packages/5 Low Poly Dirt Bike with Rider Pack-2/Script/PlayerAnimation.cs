using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class PlayerAnimation : MonoBehaviour {

    public Text txtAnimationName;
	public GameObject goBikePlayer;
	Animator animBikePlayer;
	// Use this for initialization
	void Start () {
		animBikePlayer = goBikePlayer.GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

	}


	void OnGUI(){
		//if(GUI.Button(new Rect(0,0,100,50),"Left Turn")){
		//	animBikePlayer.CrossFade ("turnleft",1f);
		//} else if(GUI.Button(new Rect(Screen.width - 100,0,100,50),"Right Turn")){
		//	animBikePlayer.CrossFade ("turnright",1f);
		//} else if (GUI.Button(new Rect((Screen.width/2 - 50),0,100,50),"Win")){
		//	animBikePlayer.CrossFade ("win",1f);
		//} else if (GUI.Button(new Rect((Screen.width/2 - 50),150,100,50),"Ride")){
		//	animBikePlayer.CrossFade ("ride",1f);
		//} else if (GUI.Button(new Rect((Screen.width / 2 + 250), 0, 100, 50), "Right Leg Kick")) {
  //          animBikePlayer.CrossFade("rightkick", 1f);
  //      } else if (GUI.Button(new Rect((Screen.width / 2 - 350), 0, 100, 50), "Left Leg Kick")) {
  //          animBikePlayer.CrossFade("leftkick", 1f);
  //      }
    }

    public void Ride()
    {
        txtAnimationName.text = "Ride";
        animBikePlayer.CrossFade("ride", 1f);
    }

    public void LeftTurn()
    {
       // txtAnimationName.text = "Turn Left";
        animBikePlayer.CrossFade("turnleft", 1f);
    }

    public void RightTurn()
    {
       // txtAnimationName.text = "Turn Right";
        animBikePlayer.CrossFade("turnright", 1f);
    }

    public void Win()
    {
     //   txtAnimationName.text = "Win";
        animBikePlayer.CrossFade("win", 1f);
    }

    public void LeftKick()
    {
      //  txtAnimationName.text = "Left Kick";
        animBikePlayer.CrossFade("leftkick", 1f);
    }

    public void RightKick()
    {
       // txtAnimationName.text = "Right Kick";
        animBikePlayer.CrossFade("rightkick", 1f);
    }

    public void LeftHandHit()
    {
       // txtAnimationName.text = "Left Hand Hit";
        animBikePlayer.CrossFade("lefthandhit", 1f);
    }

    public void RightHandHit()
    {
       // txtAnimationName.text = "Right Hand Hit";
        animBikePlayer.CrossFade("righthandhit", 1f);
    }

    public void LeftTurn2()
    {
       // txtAnimationName.text = "Left Turn 2";
        animBikePlayer.CrossFade("leftturn", 1f);
    }

    public void RightTurn2()
    {
        //txtAnimationName.text = "Right Turn 2";
        animBikePlayer.CrossFade("rightturn", 1f);
    }

    public void FirstWheelUp()
    {
       // txtAnimationName.text = "Wheel Up";
        animBikePlayer.CrossFade("firstwheelup", 1f);
    }

    public void BackWheelUp()
    {
      //  txtAnimationName.text = "Wheel Down";
        animBikePlayer.CrossFade("backwheelup", 1f);
    }

    public void StartBike()
    {
      //  txtAnimationName.text = "Start Bike";
        animBikePlayer.CrossFade("startbike", 1f);
    }

    public void Win2()
    {
       // txtAnimationName.text = "Win 2";
        animBikePlayer.CrossFade("win2", 1f);
    }

   
}
