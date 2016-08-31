﻿using BehaveAsSakura.Variables;
using ProtoBuf;
using System;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public sealed class LogTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public string Message { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public VariableDesc[] MessageParameters { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new LogTask(tree, parentTask, id, this);
        }
    }

    class LogTask : LeafTask
    {
        private LogTaskDesc description;
        private Variable[] variables;

        public LogTask(BehaviorTree tree, Task parentTask, uint id, LogTaskDesc description)
            : base(tree, parentTask, id, description)
        {
            this.description = description;

            if (description.MessageParameters != null)
                variables = Array.ConvertAll(description.MessageParameters, desc => new Variable(desc));
        }

        protected override TaskResult OnUpdate()
        {
            if (variables != null)
                LogInfo(description.Message, Array.ConvertAll(variables, v => v.GetValue(this)));
            else
                LogInfo(description.Message);

            return TaskResult.Success;
        }
    }
}
