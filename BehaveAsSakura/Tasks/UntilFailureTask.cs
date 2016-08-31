﻿using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class UntilFailureDesc : RepeaterTaskDesc, ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new UntilFailureTask(tree, parentTask, id, this);
        }
    }

    class UntilFailureTask : RepeaterTask
    {
        public UntilFailureTask(BehaviorTree tree, Task parentTask, uint id, UntilFailureDesc description)
            : base(tree, parentTask, id, description)
        {
        }

        protected override bool IsRepeaterCompleted(TaskResult result)
        {
            return result == TaskResult.Failure;
        }
    }
}
