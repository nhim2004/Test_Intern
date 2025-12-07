using UnityEngine;
using UnityEditor;
using WaterSort.Core;
using WaterSort;
using WaterSort.UI;
using WaterSort.Effects;
using WaterSort.Utilities;

namespace WaterSort.Editor
{
    /// <summary>
    /// Automatic game setup wizard
    /// </summary>
    public class GameSetupWizard : EditorWindow
    {
        private bool createGameManager = true;
        private bool createAudioManager = true;
        private bool createCamera = true;
        private bool createBottlePrefab = true;
        private bool createLevelData = true;
        private bool createUI = true;
        
        [MenuItem("WaterSort/Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<GameSetupWizard>("Water Sort Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Water Sort Game Setup Wizard", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Select components to create:", EditorStyles.label);
            createGameManager = EditorGUILayout.Toggle("Game Manager", createGameManager);
            createAudioManager = EditorGUILayout.Toggle("Audio Manager", createAudioManager);
            createCamera = EditorGUILayout.Toggle("Setup Camera", createCamera);
            createBottlePrefab = EditorGUILayout.Toggle("Bottle Prefab", createBottlePrefab);
            createLevelData = EditorGUILayout.Toggle("Level Data", createLevelData);
            createUI = EditorGUILayout.Toggle("Basic UI", createUI);

            GUILayout.Space(20);

            if (GUILayout.Button("Setup Everything!", GUILayout.Height(40)))
            {
                SetupGame();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Quick Setup (Essential Only)", GUILayout.Height(30)))
            {
                QuickSetup();
            }
        }

        private void QuickSetup()
        {
            createUI = false;
            SetupGame();
        }

        private void SetupGame()
        {
            Debug.Log("Starting Water Sort Setup...");

            // Create folders if not exist
            CreateFolders();

            GameObject bottlePrefab = null;
            LevelData levelData = null;

            if (createBottlePrefab)
            {
                bottlePrefab = CreateBottlePrefab();
            }

            if (createLevelData)
            {
                levelData = CreateLevelDataAsset();
            }

            if (createGameManager)
            {
                CreateGameManagerObject(bottlePrefab, levelData);
            }

            if (createAudioManager)
            {
                CreateAudioManagerObject();
            }

            if (createCamera)
            {
                SetupMainCamera();
            }

            if (createUI)
            {
                CreateBasicUI();
            }

            // Additional helpers
            CreateHelperObjects();

            Debug.Log("✅ Setup Complete! Press Play to test your game!");
            EditorUtility.DisplayDialog("Setup Complete", 
                "Water Sort game setup is complete!\n\n" +
                "Press Play button to test the game.\n\n" +
                "Check the Console for any warnings.", "OK");
        }

        private void CreateFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            
            if (!AssetDatabase.IsValidFolder("Assets/Levels"))
                AssetDatabase.CreateFolder("Assets", "Levels");
            
            if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
                AssetDatabase.CreateFolder("Assets", "Sprites");
        }

        private GameObject CreateBottlePrefab()
        {
            Debug.Log("Creating Bottle Prefab...");

            // Create bottle
            GameObject bottle = new GameObject("Bottle");
            
            // Add Bottle script
            Bottle bottleScript = bottle.AddComponent<Bottle>();
            
            // Add BottleAnimator
            bottle.AddComponent<BottleAnimator>();
            
            // Add BoxCollider2D for clicking
            BoxCollider2D collider = bottle.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 2f);
            
            // Add SpriteRenderer for visual
            SpriteRenderer sr = bottle.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            sr.color = new Color(1f, 1f, 1f, 0.3f);
            
            // Create WaterContainer child
            GameObject waterContainer = new GameObject("WaterContainer");
            waterContainer.transform.SetParent(bottle.transform);
            waterContainer.transform.localPosition = new Vector3(0, -0.5f, 0);

            // Create WaterSegment prefab
            GameObject waterSegment = CreateWaterSegmentPrefab();

            // Assign references using reflection (since fields are private)
            var bottleType = typeof(Bottle);
            var maxCapacityField = bottleType.GetField("maxCapacity", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var waterContainerField = bottleType.GetField("waterContainer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var waterSegmentPrefabField = bottleType.GetField("waterSegmentPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bottleRendererField = bottleType.GetField("bottleRenderer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var waterSegmentHeightField = bottleType.GetField("waterSegmentHeight", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (maxCapacityField != null) maxCapacityField.SetValue(bottleScript, 4);
            if (waterContainerField != null) waterContainerField.SetValue(bottleScript, waterContainer.transform);
            if (waterSegmentPrefabField != null) waterSegmentPrefabField.SetValue(bottleScript, waterSegment);
            if (bottleRendererField != null) bottleRendererField.SetValue(bottleScript, sr);
            if (waterSegmentHeightField != null) waterSegmentHeightField.SetValue(bottleScript, 0.5f);

            // Save as prefab
            string prefabPath = "Assets/Prefabs/Bottle.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(bottle, prefabPath);
            
            DestroyImmediate(bottle);
            
            Debug.Log("✅ Bottle Prefab created at: " + prefabPath);
            return prefab;
        }

        private GameObject CreateWaterSegmentPrefab()
        {
            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Quad);
            segment.name = "WaterSegment";
            
            // Remove collider
            DestroyImmediate(segment.GetComponent<Collider>());
            
            // Setup sprite renderer
            SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                sr.color = Color.white;
            }
            
            // Scale
            segment.transform.localScale = new Vector3(0.6f, 0.4f, 1f);

            // Save as prefab
            string prefabPath = "Assets/Prefabs/WaterSegment.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(segment, prefabPath);
            
            DestroyImmediate(segment);
            
            return prefab;
        }

        private LevelData CreateLevelDataAsset()
        {
            Debug.Log("Creating Level Data...");

            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            
            // Set default values using reflection
            var levelType = typeof(LevelData);
            var levelNumberField = levelType.GetField("levelNumber", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var numberOfBottlesField = levelType.GetField("numberOfBottles", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var numberOfColorsField = levelType.GetField("numberOfColors", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var bottleCapacityField = levelType.GetField("bottleCapacity", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (levelNumberField != null) levelNumberField.SetValue(levelData, 1);
            if (numberOfBottlesField != null) numberOfBottlesField.SetValue(levelData, 8);
            if (numberOfColorsField != null) numberOfColorsField.SetValue(levelData, 5);
            if (bottleCapacityField != null) bottleCapacityField.SetValue(levelData, 4);

            string assetPath = "Assets/Levels/Level_01.asset";
            AssetDatabase.CreateAsset(levelData, assetPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log("✅ Level Data created at: " + assetPath);
            return levelData;
        }

        private void CreateGameManagerObject(GameObject bottlePrefab, LevelData levelData)
        {
            Debug.Log("Creating Game Manager...");

            GameObject gameManagerObj = new GameObject("GameManager");
            GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
            gameManagerObj.AddComponent<InputController>();

            // Create bottle container
            GameObject bottleContainer = new GameObject("BottleContainer");
            bottleContainer.transform.SetParent(gameManagerObj.transform);

            // Assign references using reflection
            var gmType = typeof(GameManager);
            var currentLevelField = gmType.GetField("currentLevel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bottlePrefabField = gmType.GetField("bottlePrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bottleContainerField = gmType.GetField("bottleContainer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bottleSpacingField = gmType.GetField("bottleSpacing", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var bottlesPerRowField = gmType.GetField("bottlesPerRow", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (currentLevelField != null) currentLevelField.SetValue(gameManager, levelData);
            if (bottlePrefabField != null) bottlePrefabField.SetValue(gameManager, bottlePrefab);
            if (bottleContainerField != null) bottleContainerField.SetValue(gameManager, bottleContainer.transform);
            if (bottleSpacingField != null) bottleSpacingField.SetValue(gameManager, 2f);
            if (bottlesPerRowField != null) bottlesPerRowField.SetValue(gameManager, 4);

            Debug.Log("✅ Game Manager created");
        }

        private void CreateAudioManagerObject()
        {
            Debug.Log("Creating Audio Manager...");

            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManagerObj.AddComponent<AudioManager>();

            Debug.Log("✅ Audio Manager created");
        }

        private void SetupMainCamera()
        {
            Debug.Log("Setting up Camera...");

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
            }

            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6f;
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f);
            mainCamera.transform.position = new Vector3(0, 0, -10);

            if (mainCamera.GetComponent<CameraController>() == null)
            {
                mainCamera.gameObject.AddComponent<CameraController>();
            }

            Debug.Log("✅ Camera setup complete");
        }

        private void CreateBasicUI()
        {
            Debug.Log("Creating Basic UI...");

            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Create EventSystem if not exists
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("✅ Basic UI created");
        }

        private void CreateHelperObjects()
        {
            Debug.Log("Creating Helper Objects...");

            // Create SimpleTween helper
            if (GameObject.Find("SimpleTween") == null)
            {
                GameObject tweenObj = new GameObject("SimpleTween");
                tweenObj.AddComponent<SimpleTween>();
            }

            Debug.Log("✅ Helper objects created");
        }
    }

    /// <summary>
    /// Quick menu items for common actions
    /// </summary>
    public static class QuickSetupMenu
    {
        [MenuItem("WaterSort/Quick Setup/Full Setup ⚡")]
        public static void QuickFullSetup()
        {
            var window = EditorWindow.GetWindow<GameSetupWizard>("Water Sort Setup");
            window.Show();
        }

        [MenuItem("WaterSort/Quick Setup/Create Level Data Only")]
        public static void CreateLevelOnly()
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Level Data",
                "NewLevel",
                "asset",
                "Create a new level data file"
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(levelData, path);
                AssetDatabase.SaveAssets();
                Selection.activeObject = levelData;
                Debug.Log("✅ Level Data created at: " + path);
            }
        }

        [MenuItem("WaterSort/Documentation/Open README")]
        public static void OpenReadme()
        {
            var readme = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/../README.md");
            if (readme != null)
            {
                EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(readme));
            }
        }

        [MenuItem("WaterSort/Documentation/Open Setup Guide")]
        public static void OpenSetupGuide()
        {
            var setup = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/../SETUP.md");
            if (setup != null)
            {
                EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(setup));
            }
        }
    }
}
