﻿using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        public TaskState Task { get; private set; }

        protected TaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain
                  , parent
                  , string.Format("{0}-Node", task.Id))
        {
            Task = task;
            Task.OnEventApplied += Task_OnEventApplied;

            if (task.Desc is DecoratorTaskDescWrapper)
            {
                var desc = (DecoratorTaskDescWrapper)task.Desc;

                if (desc.ChildTaskId > 0)
                    CreateChildTaskNode(desc.ChildTaskId);
            }
            else if (task.Desc is CompositeTaskDescWrapper)
            {
                var desc = (CompositeTaskDescWrapper)task.Desc;

                foreach (var id in desc.ChildTaskIds)
                    CreateChildTaskNode(id);
            }
        }

        private void CreateChildTaskNode(uint taskId)
        {
            var task = (TaskState)Repository.States[TaskState.GetId(taskId)];

            Children.Add(Create(this, task));
        }

        public static TaskNode Create(EditorComponent parent, TaskState state)
        {
            if (state.Desc is LeafTaskDescWrapper)
            {
                return new LeafTaskNode(parent.Domain, parent, state);
            }
            else if (state.Desc is DecoratorTaskDescWrapper)
            {
                return new DecoratorTaskNode(parent.Domain, parent, state);
            }
            else if (state.Desc is CompositeTaskDescWrapper)
            {
                return new CompositeTaskNode(parent.Domain, parent, state);
            }
            else
                throw new NotSupportedException(state.Desc.ToString());
        }

        private void Task_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                Children.Add(Create(this, ((TaskCreatedEvent)e).NewTask));
            }
            else if (e is TaskNotCreatedEvent)
            {
                EditorHelper.DisplayDialog("Failed to create task", ((TaskNotCreatedEvent)e).Reason);
            }
            else if (e is TaskRemovedEvent)
            {
                Parent.Children.Remove(this);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            var descType = Task.Desc.CustomDesc.GetType();

            var nodeRect = CalculateGUIRect();
            GUI.Box(nodeRect, string.Empty, EditorConfiguration.BehaviorTreeNodeStyle);

            var iconTexture = Resources.Load(EditorHelper.GetTaskIcon(descType)) as Texture2D;
            if (iconTexture == null)
                iconTexture = (Texture2D)Resources.Load(EditorConfiguration.DefaultTaskIconPath);

            var iconRect = new Rect(nodeRect.position + new Vector2(15, 15), new Vector2(32, 32));
            GUI.Box(iconRect, iconTexture, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                }
            });

            var titleRect = new Rect(nodeRect.position + new Vector2(15 + 32 + 10, 12), new Vector2(50, EditorGUIUtility.singleLineHeight));
            var title = EditorHelper.GetTaskTitle(descType);
            GUI.Box(titleRect, title, new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                }
            });

            if (!string.IsNullOrEmpty(Task.Desc.Title))
            {
                var summaryRect = new Rect(nodeRect.position + new Vector2(15 + 32 + 10, 12 + EditorGUIUtility.singleLineHeight + 5), new Vector2(50, EditorGUIUtility.singleLineHeight));
                GUI.Box(summaryRect, Task.Desc.Title, new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        background = null,
                        textColor = Color.white,
                    }
                });
            }
            DrawConnection();
        }

        protected override Rect CalculateGUIRect()
        {
            return new Rect(RootView.ToWindowPosition(Task.Position - EditorConfiguration.TaskNodeSize / 2), EditorConfiguration.TaskNodeSize);
        }

        private void DrawConnection()
        {
            var fromPoint = RootView.ToWindowPosition(Task.Position + new Vector2(0, -EditorConfiguration.TaskNodeSize.y / 2 + EditorConfiguration.TaskNodeConnectionPadding));
            Vector2 toPoint;
            if (Parent is BehaviorTreeNode)
            {
                var parent = (BehaviorTreeNode)Parent;

                toPoint = EditorConfiguration.BehaviorTreeNodePosition + new Vector2(0, EditorConfiguration.BehaviorTreeNodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }
            else if (Parent is TaskNode)
            {
                var parent = (TaskNode)Parent;

                toPoint = parent.Task.Position + new Vector2(0, EditorConfiguration.TaskNodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }
            else
                throw new NotSupportedException(Parent.ToString());

            toPoint = RootView.ToWindowPosition(toPoint);

            Handles.DrawBezier(fromPoint
                    , toPoint
                    , fromPoint - Vector2.up * EditorConfiguration.TaskNodeConnectionTangent
                    , toPoint - Vector2.down * EditorConfiguration.TaskNodeConnectionTangent
                    , EditorConfiguration.TaskNodeConnectionColor
                    , null
                    , EditorConfiguration.TaskNodeConnectionLineWidth);
        }

        private static void OnTaskNotCreatedEvent(EditorEvent e)
        {
            var title = I18n._("Failed to create task");
            var message = I18n._(((TaskNotCreatedEvent)e).Reason);
            var ok = I18n._("Ok");

            EditorUtility.DisplayDialog(title, message, ok);
        }

        public override void OnSelect(Event e)
        {
            base.OnSelect(e);

            Selection.activeObject = Task.Wrapper;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), (s) => OnContextMenu_NewTask((Type)s));

            menu.AddItem(new GUIContent(I18n._("Remove Task")), false, OnContextMenu_RemoveTask);

            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Task.Id)
            {
                TaskType = taskType,
            });
        }

        private void OnContextMenu_RemoveTask()
        {
            Domain.CommandHandler.ProcessCommand(new RemoveTaskCommand(Task.Id));
        }

        protected abstract bool CanCreateChildTask();
    }
}
