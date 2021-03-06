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
    public TargetGroupBehavior targets;
    public GateBehavior gate;
    public NextRoomBehaviour nextRoom;
    public NextRoomDoorBehaviour nextRoomDoor;
    public TrapBehaviour trap;

    public char[] abcTriggers;
   
    public string task = "";
    public string cond = "";
    public int bumpCond;
    public int targetScore;
    public int currentScore;
    public bool taskActive;
    public Text tasktext;
    public int passCond;

	void Start () {
        trig = GameObject.Find("StoryElement").GetComponent<StoryTrigger>();
        triggers = GameObject.FindGameObjectsWithTag("storypad");

        abcTriggerGroup = GameObject.Find("ABCTriggerGroup").GetComponent<TriggerGroupBehaviour>();
        gate = GameObject.Find("Gateway").GetComponent<GateBehavior>();
        targets = GameObject.Find("Targets").GetComponent<TargetGroupBehavior>();
        trap = GameObject.Find("TrapWall").GetComponent<TrapBehaviour>();

        // these are used to refer the index of each trigger
        abcTriggers = new char[] { 'A', 'B', 'C' };

        gate = GameObject.Find("Gateway").GetComponent<GateBehavior>();
        targets = GameObject.Find("Targets").GetComponent<TargetGroupBehavior>();
        nextRoom = GameObject.Find("NextRoom").GetComponent<NextRoomBehaviour>();
        nextRoomDoor = GameObject.Find("NextRoomDoor").GetComponent<NextRoomDoorBehaviour>();


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
            if (task=="main") {
                GameObject.Find("Canvas").GetComponent<PauseMenu>().endScreenUI.SetActive(true);



            }
            if (task == "bumpers") {

                bumpCond = int.Parse(cond);
                tasktext.text = "Task: Raise your score with " + cond + " points."; // set the task text to indicate score challenge
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




            } else if (task == "targets") {
                targets.initTask();
                tasktext.text = "Task: Hit all the wooden targets";
                taskActive = true;
                bll.setTaskActive();



            }
            else if (task == "gate") {
                passCond = int.Parse(cond);
                taskActive = true;
                bll.setTaskActive();
                tasktext.text = "Pass through the gate " + cond + " Times. Times passed: " + gate.getTimesPassed();


            }
            else if (task == "trap") {

                taskActive = true;
                bll.setTaskActive();
                tasktext.text = "Plunge into the trap!";

            }
            else if (task == "enterNextRoom")
            {
                bll.setTaskActive();
                nextRoom.setTaskActive();
                tasktext.text = cond + " by entering the room on top of the board";
                taskActive = true;
            }
            // this task will get next task automatically when completed
            else if (task == "openDoor")
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

            if (task == "bumpers") {
                currentScore = bll.getScore();
                // Check task condition, if met reset task data
                if (currentScore >= targetScore) {
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
            else if (task == "targets") {
                if (targets.isTaskFinished()) {
                    disableTask();

                }

            }
            else if (task == "gate") {
                if (gate.getTimesPassed() >= passCond)
                {
                    disableTask();


                }
                else {
                    tasktext.text = "Pass through the gate " + cond + " Times. Times passed: " + gate.getTimesPassed();

                }

            } else if (task == "trap") {
                if (trap.isCaptured()) {

                    disableTask();

                }


            }
            else if (task == "enterNextRoom")
            {
                if (nextRoom.isTaskCompleted())
                {
                    disableTask();
                    trig.ActivateNextTask();
                    // close the door
                    //nextRoomDoor.SwitchDoor();
                }


            }
            // this task will get next task automatically when completed
            else if (task == "openDoor")
            {
                // get the current progress
                var progress = cond.Substring(0, abcTriggerGroup.GetTaskProgress());
                tasktext.text = "Task: Hit the ABC triggers in the following order: " + cond + ". Progress: " + progress;

                // check if the task is completed
                if (abcTriggerGroup.isTaskCompleted())
                {
                    disableTask();
                    trig.ActivateNextTask();
                    // open the door
                    nextRoomDoor.SwitchDoor();
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
