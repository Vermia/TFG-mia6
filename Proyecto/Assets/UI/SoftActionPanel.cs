using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoftActionPanel {
    BehUIEdit bigPanel;

    Dropdown typeDropdown;
    Dropdown variableDropdown;
    Dropdown numberDropdown;

    SoftAction action;

    public SoftActionPanel(Dropdown ptype, Dropdown pvar, Dropdown pnum){
        typeDropdown = ptype; variableDropdown = pvar; numberDropdown = pnum;
        action = new SoftAction(SoftActions.setVariable, Variables.A, 0);
        bigPanel = GameObject.Find("UICanvasImageEdit").GetComponent<BehUIEdit>();

        typeDropdown.onValueChanged.AddListener( changeType );
        variableDropdown.onValueChanged.AddListener( changeVar );
        numberDropdown.onValueChanged.AddListener( changeNum );
    }

    void changeType(int pvalue){
        action.softAction = (SoftActions) pvalue;
        bigPanel.refreshRule(pvalue);
    }

    void changeVar(int pvalue){
        action.affectedVariable = (Variables) pvalue;
        bigPanel.refreshRule(pvalue);
    }

    void changeNum(int pvalue){
        action.affectedNumber = pvalue;
        bigPanel.refreshRule(pvalue);
    }

}