using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TeamScoreUI : MonoBehaviour {
    public static List<TeamScoreUI> scoreFields = new List<TeamScoreUI>();

    public string teamName = "defaultteam";
    [HideInInspector] Text text;

    void Awake () {
        text = GetComponent<Text>();
        scoreFields.Add(this);
    }


	public static void SetScore(string team, int score){
		foreach(TeamScoreUI ui in scoreFields){
			if(ui.teamName == team){
                ui.text.text = "" + score;
            }
		}
	}
}
