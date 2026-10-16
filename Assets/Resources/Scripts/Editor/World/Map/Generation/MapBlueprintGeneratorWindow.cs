using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlameOfTheLost.Editor
{
    public class MapBlueprintGeneratorWindow : EditorWindow
    {
        // --- Runtime ---
        private MapBlueprintSettings _settings = new MapBlueprintSettings();

        private MapBlueprint _generatedBlueprint;

        private Vector2 _scrollPosition;

        private int _overviewScale = 2;

        private string _outputDirectory;

        // --- Menu ---
        [MenuItem("FlameOfTheLost/Tools/World/Map Blueprint Generator")]
        public static void OpenWindow()
        {
            var window = MapBlueprintGeneratorWindow.GetWindow<MapBlueprintGeneratorWindow>();

            window.titleContent = new GUIContent("Map Blueprint Generator");

            window.minSize = new Vector2(460, 560);

            window.Show();
        }

        // --- Unity Callbacks ---
        private void OnEnable()
        {
            _outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "../Documents/GameContent/World/Maps/" + "GreenvaleCrossroads/Generated"));
        }

        // --- GUI ---
        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            this.DrawHeader();
            this.DrawGenerationSettings();
            this.DrawExportSettings();
            this.DrawActions();
            this.DrawGenerationSummary();

            EditorGUILayout.EndScrollView();
        }

        // --- Helpers ---
        //  1. Draw
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("FlameOfTheLost", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Map Blueprint Generator", EditorStyles.largeLabel);

            EditorGUILayout.HelpBox("Generate ASCII map blueprints, region layouts, terrain classifications, placement suggestions and map statistics.\n\nThis tool does not modify Unity scenes.", MessageType.Info);

            EditorGUILayout.Space(10);
        }
        private void DrawGenerationSettings()
        {
            EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);

            _settings.MapName = EditorGUILayout.TextField("Map Name", _settings.MapName);

            _settings.Seed = EditorGUILayout.IntField("Seed", _settings.Seed);

            _settings.Width     = EditorGUILayout.IntField("Width (Cells)",     _settings.Width);
            _settings.Height    = EditorGUILayout.IntField("Height (Cells)",    _settings.Height);

            _settings.CellSize = EditorGUILayout.FloatField("Cell Size (Unity Units)", _settings.CellSize);

            _settings.Width     = Mathf.Max(120, _settings.Width);
            _settings.Height    = Mathf.Max(110, _settings.Height);

            _settings.CellSize = Mathf.Max(0.1f, _settings.CellSize);

            _settings.RegionPositionVariation = EditorGUILayout.IntSlider("Region Variation", _settings.RegionPositionVariation, 0, 4);

            _settings.RoadWidth = EditorGUILayout.IntSlider("Road Width", _settings.RoadWidth, 1, 7);

            _settings.EnvironmentDensity = EditorGUILayout.Slider("Environment Density", _settings.EnvironmentDensity, 0f, 1f);

            _settings.GenerateEnvironmentPlacements = EditorGUILayout.Toggle("Generate Object Placements", _settings.GenerateEnvironmentPlacements);

            EditorGUILayout.Space(10);
        }
        private void DrawExportSettings()
        {
            EditorGUILayout.LabelField("Export Settings", EditorStyles.boldLabel);

            _overviewScale = EditorGUILayout.IntSlider("Region Map Scale", _overviewScale, 1, 5);

            EditorGUILayout.LabelField("Output Directory");

            EditorGUILayout.SelectableLabel(_outputDirectory, EditorStyles.textField, GUILayout.Height(40));

            if (GUILayout.Button("Select Output Directory"))
            {
                string selected = EditorUtility.OpenFolderPanel("Select Blueprint Output Directory", _outputDirectory, "");

                if (!string.IsNullOrEmpty(selected))
                {
                    _outputDirectory = selected;
                }
            }

            EditorGUILayout.Space(10);
        }
        private void DrawActions()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Generate Blueprint", GUILayout.Height(35)))
            {
                this.GenerateBlueprint();
            }

            using (new EditorGUI.DisabledScope(_generatedBlueprint == null))
            {
                if (GUILayout.Button("Export Generated Blueprint", GUILayout.Height(35)))
                {
                    this.ExportBlueprint();
                }
            }

            if (GUILayout.Button("Open Output Directory"))
            {
                if (!Directory.Exists(_outputDirectory))
                {
                    Directory.CreateDirectory(_outputDirectory);
                }

                EditorUtility.RevealInFinder(_outputDirectory);
            }

            EditorGUILayout.Space(10);
        }
        private void DrawGenerationSummary()
        {
            if (_generatedBlueprint == null)
            {
                EditorGUILayout.HelpBox("No blueprint has been generated.", MessageType.None);
                return;
            }

            EditorGUILayout.LabelField("Generated Blueprint", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Map",               _generatedBlueprint.MapName);
            EditorGUILayout.LabelField("Seed",              _generatedBlueprint.Seed.ToString());
            EditorGUILayout.LabelField("Dimensions",     $"{_generatedBlueprint.Width} × {_generatedBlueprint.Height}");
            EditorGUILayout.LabelField("Regions",           _generatedBlueprint.Regions.Count.ToString());
            EditorGUILayout.LabelField("Connections",       _generatedBlueprint.Connections.Count.ToString());
            EditorGUILayout.LabelField("Object Placements", _generatedBlueprint.Placements.Count.ToString());

            int warningCount = _generatedBlueprint.ValidationWarnings.Count;

            EditorGUILayout.LabelField("Validation Warnings", warningCount.ToString());

            if (warningCount > 0)
            {
                EditorGUILayout.HelpBox("The generated blueprint contains validation warnings. Review GenerationReport.md.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Generation completed without validation warnings.", MessageType.Info);
            }
        }

        //  2. 
        private void GenerateBlueprint()
        {
            try
            {
                _generatedBlueprint = MapBlueprintGenerator.Generate(_settings);

                Debug.Log($"[MapBlueprintGenerator] Generated {_generatedBlueprint.MapName} with seed {_generatedBlueprint.Seed}.");

                this.Repaint();
            }
            catch (Exception exception)
            {
                _generatedBlueprint = null;

                Debug.LogException(exception);

                EditorUtility.DisplayDialog("Map Generation Failed", exception.Message, "OK");
            }
        }

        private void ExportBlueprint()
        {
            if (_generatedBlueprint == null) return;

            try
            {
                MapBlueprintExporter.Export(_generatedBlueprint, _outputDirectory, _overviewScale);

                Debug.Log($"[{nameof(MapBlueprintGenerator)}] Blueprint exported to: " + _outputDirectory);

                EditorUtility.DisplayDialog("Export Completed", $"Blueprint successfully exported to:\n" + _outputDirectory, "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                EditorUtility.DisplayDialog("Export Failed", exception.Message, "OK");
            }
        }
    }
}