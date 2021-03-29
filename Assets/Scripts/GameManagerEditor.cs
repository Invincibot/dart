using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private static int minMeteors = 0;
    private static int maxMeteors = 25;
    private static int minMeteorSize = 1;
    private static int maxMeteorSize = 10;

    private GameManager _gameManager;

    private void OnEnable()
    {
        _gameManager = (GameManager) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        (_gameManager.minMeteors, _gameManager.maxMeteors) = IntMinMaxSlider(
            "Meteors",
            _gameManager.minMeteors,
            _gameManager.maxMeteors,
            minMeteors,
            maxMeteors
            );
        
        (_gameManager.minMeteorSize, _gameManager.maxMeteorSize) = IntMinMaxSlider(
            "Meteor Size",
            _gameManager.minMeteorSize,
            _gameManager.maxMeteorSize,
            minMeteorSize,
            maxMeteorSize
        );
    }

    private (int, int) IntMinMaxSlider(string label, int minValue, int maxValue, int minLimit, int maxLimit)
    {
        float minValueFloat = minValue;
        float maxValueFloat = maxValue;
        EditorGUILayout.LabelField("Min " + label + ":", minValue.ToString());
        EditorGUILayout.LabelField("Max "+ label + ":", maxValue.ToString());
        EditorGUILayout.MinMaxSlider(
            label + "Range",
            ref minValueFloat,
            ref maxValueFloat,
            minLimit, 
            maxLimit
        );
        return ((int) minValueFloat, (int) maxValueFloat);
    }
}