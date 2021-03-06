﻿using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public static class EditorConfiguration
    {
        public static readonly Vector2 MinWindowSize = new Vector2(400, 300);

        public static readonly string BehaviorTreeBackgroundPath = "BehavioreTreeViewBackground.png";

        public static readonly int BehaviorTreeBackgroundDepth = 100;

        public static readonly Vector2 NodeSize = new Vector2(140, 64);

        public static readonly Vector2 BehaviorTreeNodePosition = new Vector2(0, 0);

        public static readonly int NodeDepth = 90;

        public static readonly string BehaviorTreeNodeIconPath = "Icons/Root.png";

        public static readonly GUIStyle NodeBackgroundStyle = new GUIStyle()
        {
            normal = new GUIStyleState() { background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") },
            border = new RectOffset(12, 12, 12, 12),
        };

        public static readonly GUIStyle NodeIconStyle = new GUIStyle()
        {
            normal = new GUIStyleState() { background = null },
        };

        public static readonly Rect NodeIconPosition = new Rect(10, 15, 32, 32);

        public static readonly GUIStyle NodeTitleStyle = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            clipping = TextClipping.Clip,
            normal = new GUIStyleState() { background = null, textColor = Color.white },
        };

        public static readonly Rect NodeTitlePosition = new Rect(NodeIconPosition.position.x + NodeIconPosition.width + 5, 12, 80, 16);

        public static readonly GUIStyle NodeSummaryStyle = new GUIStyle()
        {
            fontSize = 11,
            clipping = TextClipping.Clip,
            normal = new GUIStyleState() { background = null, textColor = Color.white },
        };

        public static readonly Rect NodeSummaryPosition = new Rect(NodeIconPosition.position.x + NodeIconPosition.width + 5, 12 + NodeTitlePosition.height + 5, 80, 16);

        public static readonly Vector2 TaskNodeMinSpace = new Vector2(30, 30);

        public static readonly Logger.Level LoggerLevel = Logger.Level.Debug;

        public static readonly float TaskNodeConnectionPadding = 8;

        public static readonly float TaskNodeConnectionTangent = 30;

        public static readonly Color TaskNodeConnectionColor = Color.white;

        public static readonly float TaskNodeConnectionLineWidth = 3;

        public static readonly string TranslationPath = "BehaveAsSakura/Translations";

        public static readonly string Language = "en";

        public static readonly int PropertyGridViewDepth = 80;

        public static readonly int PropertyGridViewWidth = 400;

        public static readonly string DefaultTaskIconPath = "DefaultTaskIcon.png";
    }
}
