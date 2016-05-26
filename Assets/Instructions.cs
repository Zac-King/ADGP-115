﻿using System;
using Units.Controller;
using UnityEngine;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    [SerializeField]
    private Text m_InstructionsText;

    // Use this for initialization
    void Start()
    {
        m_InstructionsText.text =
            "Instructions:" + "\n" +

            "Keyboard Controls:" + "\n";
        foreach (Key<KeyCode> skillKey in KeyConfiguration.self.userConfigurations[0].skillKeys)
        {
            m_InstructionsText.text +=
                Enum.GetName(typeof(KeyCode), skillKey.keyCode) +
                " - Use Fireball" + "\n";
        }
        m_InstructionsText.text +=
                Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].verticalKeyAxis.positive.keyCode) +
                " - Moves Character Up " + "\n";
        m_InstructionsText.text +=
        Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].verticalKeyAxis.negative.keyCode) +
               " - Move Character Down" + "\n";
        m_InstructionsText.text +=
             Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].horizontalKeyAxis.positive.keyCode) +
                " - Moves Character Right " + "\n";
        m_InstructionsText.text +=
        Enum.GetName(typeof(KeyCode), KeyConfiguration.self.userConfigurations[0].horizontalKeyAxis.negative.keyCode) +
               " - Move Character Left " + "\n";





        m_InstructionsText.text +=

        "Controller Controls:" + "\n";
       foreach (Key<ButtonCode> skillButton in KeyConfiguration.self.userConfigurations[0].skillButtons)
       {
            m_InstructionsText.text +=
                Enum.GetName(typeof(ButtonCode), skillButton.keyCode) +
                " Use Fireball" + "\n";
        }
       m_InstructionsText.text +=
        Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].verticalButtonAxis.positive.keyCode) +
                " - Move Character Up " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].verticalButtonAxis.negative.keyCode) +
               " - Move Character Down " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].horizontalButtonAxis.positive.keyCode) +
               " - Move Character Right " + "\n";
        m_InstructionsText.text +=
       Enum.GetName(typeof(ButtonCode), KeyConfiguration.self.userConfigurations[0].horizontalButtonAxis.negative.keyCode) +
               " - Move Character Left " + "\n";

        m_InstructionsText.text +=
          

            "Upgrade:" + "\n" +
                "\t" + "'+' - Allows upgrade of specific skill" + "\n" +
                "\t" + "(Only available when able to level up)";
    }

    // Update is called once per frame
    void Update()
    {

    }
}