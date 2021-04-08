using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSpawner))]
public class GameSpawnerEditor : Editor
{
    private GameSpawner _gameSpawner;

    private void OnEnable()
    {
        _gameSpawner = (GameSpawner) target;
    }

    public override void OnInspectorGUI()
    {
        if (_gameSpawner.boundaryWalls[0] != null) _gameSpawner.bounds.xMax = _gameSpawner.boundaryWalls[0].position.x;
        if (_gameSpawner.boundaryWalls[1] != null)_gameSpawner.bounds.yMax = _gameSpawner.boundaryWalls[1].position.y;
        if (_gameSpawner.boundaryWalls[2] != null)_gameSpawner.bounds.xMin = _gameSpawner.boundaryWalls[2].position.x;
        if (_gameSpawner.boundaryWalls[3] != null)_gameSpawner.bounds.yMin = _gameSpawner.boundaryWalls[3].position.y;
        
        EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Boundary Center", _gameSpawner.bounds.center.ToString());
        EditorGUILayout.LabelField("Boundary Size", _gameSpawner.bounds.size.ToString());

        base.OnInspectorGUI();
    }
}