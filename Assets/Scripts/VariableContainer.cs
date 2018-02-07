﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableContainer : MonoBehaviour {
    public StoryTrigger trig;
    public GameObject[] triggers;
    public BallBehaviour bll;
    public TriggerGroupBehaviour abcTriggerGroup;
    public char[] abcTriggers;
   
    public string task = "";
    public string cond = "";
    public int bumpCond;
    public int targetScore;
    public int currentScore;
    public bool taskActive;
    public Text tasktext;

	void Start () {
        trig = GameObject.Find("StoryElement").GetComponent<StoryTrigger>();
        triggers = GameObject.FindGameObjectsWithTag("storypad");
        abcTriggerGroup = GameObject.Find("ABCTriggerGroup").GetComponent<TriggerGroupBehaviour>();
        // these are used to refer the index of each trigger
        abcTriggers = new char[] { 'A', 'B', 'C' };

        bll = GameObject.Find("Ball").GetComponent<BallBehaviour>();

        print("bll " + bll);
        print("abc  " + abcTriggerGroup);

        tasktext.text = "Task: Hit a story panel."; // Set initial task text
       
     
 
	}
    public void SetTaskAndCond(string task, string cond) {
        this.task = task;
        this.cond = cond;
    }

	void Update () {

        // Set the task 
        if (task != "" && cond != "" && !bll.taskStatus()) {
            // checking the task type.
            if (task == "bumpers") {
                
                bumpCond = int.Parse(cond);
                tasktext.text = "Task: Raise your score with " + cond + " points." ; // set the task text to indicate score challenge
                currentScore = bll.getScore();
                targetScore = currentScore + bumpCond;
                taskActive = true;
                bll.setTaskActive();
            }

            else if (task == "ABCtriggers")
            {
                tasktext.text = "Task: Hit the ABC triggers in the following order: " + cond + ". Progress: "; // set the task text to indicate score challenge
                // turn the condition (= string) into a int array
                int[] goal = new int[cond.Length];
                for (var i = 0; i < cond.Length; i++)
                {
                    goal[i] = (Array.IndexOf(abcTriggers, cond.ToCharArray()[i]));
                }
                abcTriggerGroup.SetTaskActive(goal);
                taskActive = true;
                bll.setTaskActive();
            }

            // Disable story triggers
            for (int i = 0; i < triggers.Length; i++)
            {
                Debug.Log("Disabled triggers: " + i);
                triggers[i].GetComponent<StoryTrigger>().disable();
                triggers[i].GetComponent<Renderer>().material.color = Color.black;

            }
        }
        // if task is active in ball, update task
        if (bll.taskStatus()) {

            // check task type

            if (task =="bumpers") {
                currentScore = bll.getScore();
                // Check task condition, if met reset task data
                if (currentScore >= targetScore ) {
                    disableTask();
                }
            }
            else if (task == "ABCtriggers")
            {
                // get the current progress
                var progress = cond.Substring(0, abcTriggerGroup.GetTaskProgress());
                tasktext.text = "Task: Hit the ABC triggers in the following order: " + cond + ". Progress: " + progress;

                // check if the task is completed
                if (abcTriggerGroup.isTaskCompleted())
                {
                    disableTask();
                }
            }
        }
	}

    // used when a task has been completed and a new one is ready to be accepted
    private void disableTask()
    {
        task = "";
        cond = "";
        tasktext.text = "Task: Hit a story panel to continue with the story.";
        taskActive = false;
        bll.disableTask();
        currentScore = 0;
        targetScore = 0;
        // enable story triggers
        for (int i = 0; i < triggers.Length; i++)
        {
            triggers[i].GetComponent<StoryTrigger>().setEnabled();
            triggers[i].GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
