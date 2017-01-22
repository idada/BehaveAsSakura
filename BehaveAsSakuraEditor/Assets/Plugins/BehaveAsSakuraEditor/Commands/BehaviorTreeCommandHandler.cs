﻿using BehaveAsSakura.Tasks;
using System;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeCommandHandler : EditorCommandHandler
    {
        public override void ProcessCommand(EditorCommand command)
        {
            base.ProcessCommand(command);

            if (command is CreateTaskCommand)
            {
                OnCreateTaskCommand((CreateTaskCommand)command);
            }
            else if (command is RemoveTaskCommand)
            {
                OnRemoveTaskCommand((RemoveTaskCommand)command);
            }
            else if (command is ChangeTaskSummaryCommand)
            {
                OnChangeTaskSummaryCommand((ChangeTaskSummaryCommand)command);
            }
            else if (command is ChangeTaskDescCommand)
            {
                OnChangeTaskPropertyCommand((ChangeTaskDescCommand)command);
            }
        }

        private void OnCreateTaskCommand(CreateTaskCommand command)
        {
            var parent = Repository.States[command.Id];
            var parentTaskId = 0u;
            if (parent is BehaviorTreeState)
            {
                var tree = (BehaviorTreeState)parent;
                if (tree.RootTaskId > 0)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Behavior tree already has root task" });
                    return;
                }
            }
            else if (parent is TaskState)
            {
                var task = (TaskState)parent;
                if (task.Desc is LeafTaskDescWrapper)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Leaf task cannot have child task" });
                    return;
                }
                else if (task.Desc is DecoratorTaskDescWrapper)
                {
                    var desc = (DecoratorTaskDescWrapper)task.Desc;
                    if (desc.ChildTaskId != 0)
                    {
                        parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Decorator task can only has one child task" });
                        return;
                    }
                }

                parentTaskId = task.Desc.Id;
            }
            else
                throw new NotSupportedException(command.Id);

            {
                var tree = (BehaviorTreeState)Repository.States[BehaviorTreeState.GetId()];
                TaskDescWrapper taskDescWrapper;
                if (typeof(ILeafTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new LeafTaskDescWrapper();
                else if (typeof(IDecoratorTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new DecoratorTaskDescWrapper();
                else if (typeof(ICompositeTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new CompositeTaskDescWrapper();
                else
                    throw new NotSupportedException(command.TaskType.ToString());
                taskDescWrapper.Id = tree.NextTaskId;
                taskDescWrapper.CustomDesc = (ITaskDesc)Activator.CreateInstance(command.TaskType);
                var taskState = EditorState.CreateInstance<TaskState>(Domain, TaskState.GetId(tree.NextTaskId));
                taskState.ParentTaskId = parentTaskId;
                taskState.Desc = taskDescWrapper;

                parent.ApplyEvent(new TaskCreatedEvent(command.Id) { NewTask = taskState });
            }
        }

        private void OnRemoveTaskCommand(RemoveTaskCommand command)
        {
            var task = (TaskState)Repository.States[command.Id];

            task.ApplyEvent(new TaskRemovedEvent(command.Id));
        }

        private void OnChangeTaskSummaryCommand(ChangeTaskSummaryCommand command)
        {
            var task = Repository.States[command.Id];

            task.ApplyEvent(new TaskSummaryChangedEvent(command.Id)
            {
                Name = command.Name,
                Comment = command.Comment,
            });
        }

        private void OnChangeTaskPropertyCommand(ChangeTaskDescCommand command)
        {
            var task = Repository.States[command.Id];

            task.ApplyEvent(new TaskPropertyDescEvent(command.Id)
            {
                CustomDesc = command.CustomDesc,
            });
        }
    }
}
