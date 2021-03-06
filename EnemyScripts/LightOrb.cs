﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOrb : Enemy {

    // Enemy child specific variables
    public GameObject lancer;
    public float xShift, yShift;
    public int damage;

    // Maybe remove
    private Vector3 oldSpot, newSpot;
	public float amp, wavelen;
 
	// Use this for initialization
	new void Start () {
		base.Start();

        base.SetActiveHit(hurtbox);
        curAtk = null;

        transform.localPosition = new Vector2(transform.localPosition.x, 
			lancer.transform.localPosition.y + yShift);
	}

	// Allow the enemy to act freely
	private void ActFree() {
		if(curHP == 0) {
			curHP = -1; // Don't go into this if statement again
			timer.resetWait();
			ChangeState(States.KO);
		}
		if(isHurt)		base.ApplyHitstun();

		switch(curState){
            case States.Undected:	ActIdle();      		break;
			case States.Grounded:	Agro();        			break;
			case States.Airborne:   Agro();					break;
			case States.Attack:  	Agro(); 	            break;
            case States.Stunned:	base.Stunned();      	break;
			case States.KO:			base.DestroyFoe();		break;
			default: 										break;
		}
	}

	// Handles AI for enemy when player is not detected
	override public void ActIdle() {

        // See if we have come into contact with the player
		HitBox hurt = player.GetComponent<Player>().hurtBox;
		bool isStrike = base.IsHitTarget(hurtbox, gameObject, hurt, player);
		if(isStrike){
            player.SendMessage("Attacked", damage);
		}

        /* ---------- Move in the area above and in front of lancer ---------- */
        // Set relative origin for orb
		if(lancer.transform.localScale.x > 0){
			origin = new Vector2(lancer.transform.localPosition.x + xShift,
									lancer.transform.localPosition.y + yShift);
		}
		else{
			origin = new Vector2(lancer.transform.localPosition.x - xShift,
									lancer.transform.localPosition.y + yShift);
		}

        // Turn the orb around
		if(transform.localPosition.x > origin.x + foe.walkDist/2){
			transform.localScale = new Vector2(flipScale, flipScale);
		}
		else if(transform.localPosition.x < origin.x - foe.walkDist/2) {
			transform.localScale = new Vector2(-flipScale, flipScale);
		}

		// Move to left
		if(transform.localScale.x > 0){
			transform.Translate(-foe.walkSpd * Time.deltaTime, 
				Mathf.Sin(Time.frameCount * wavelen) * Time.deltaTime * amp  , 0);
		}
		// Move to right
		else {
			transform.Translate(foe.walkSpd * Time.deltaTime, 
				Mathf.Sin(Time.frameCount * wavelen) * Time.deltaTime * amp , 0);
		}

		if(transform.localPosition.y > lancer.transform.localPosition.y + yShift + amp) {
			transform.localPosition = new Vector2(transform.localPosition.x, 
				lancer.transform.localPosition.y + yShift + amp);
		}
		else if(transform.localPosition.y < lancer.transform.localPosition.y + yShift - amp) {
			transform.localPosition = new Vector2(transform.localPosition.x, 
				lancer.transform.localPosition.y + yShift - amp);
		}


        /* DO THIS ONCE DARK ORB DIES
		// Player is always detected since it's a boss battle
		ChangeState(States.Grounded);
		timer.resetWait();
        */
	}

	// Handles AI for enemy when player is detected
	override public void Agro() {
        // See if we have come into contact with the player
		HitBox hurt = player.GetComponent<Player>().hurtBox;
		bool isStrike = base.IsHitTarget(hurtbox, gameObject, hurt, player);
		if(isStrike){
            player.SendMessage("Attacked", damage);
		}

		/* ------------------------------ Follow the player ------------------------------ */
        // PLAY DASH ANIMATION
		gameObject.GetComponent<SpriteRenderer>().sprite = foe.foeAnims.dash[0];
		// MOVE HORIZONTALLY TOWARDS PLAYER
		if(transform.localPosition.x > player.transform.localPosition.x){
			transform.localScale = new Vector2(flipScale, flipScale);
			transform.Translate(-foe.dashSpd * Time.deltaTime, 0, 0);
		}
		else {
			transform.localScale = new Vector2(-flipScale, flipScale);
			transform.Translate(foe.dashSpd * Time.deltaTime, 0, 0);
		}
		// MOVE VERTICALLY TOWARDS PLAYER
		if(transform.localPosition.y > player.transform.localPosition.y){
			transform.Translate(0, -foe.dashSpd * Time.deltaTime,  0);
		}
		else if (transform.localPosition.y < player.transform.localPosition.y){
			transform.Translate(0, foe.dashSpd * Time.deltaTime, 0);
		}

		
	}
}