﻿using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
    public class ListenEventTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public string EventType { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ListenEventTask(tree, parentTask, id, this);
        }
    }

    [ProtoContract]
    class ListenEventTaskProps : ITaskProps
    {
        [ProtoMember(1)]
        public bool IsEventTriggered { get; set; }
    }

    class ListenEventTask : DecoratorTask
    {
        private ListenEventTaskDesc description;
        private ListenEventTaskProps props;

        public ListenEventTask(BehaviorTree tree, Task parentTask, uint id, ListenEventTaskDesc description)
            : base(tree, parentTask, id, description, new ListenEventTaskProps())
        {
            this.description = description;
            props = (ListenEventTaskProps)Props;
        }

        protected override void OnStart()
        {
            base.OnStart();

            props.IsEventTriggered = false;

            Tree.EventBus.Subscribe<SimpleEventTriggeredEvent>(this);

            ChildTask.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            if (props.IsEventTriggered)
            {
                ChildTask.EnqueueForAbort();

                return TaskResult.Success;
            }

            return ChildTask.LastResult;
        }

        protected override void OnEnd()
        {
            if (!props.IsEventTriggered)
                Tree.EventBus.Unsubscribe<SimpleEventTriggeredEvent>(this);

            base.OnEnd();
        }

        protected override void OnEventTriggered(IEvent @event)
		{
            base.OnEventTriggered( @event );

            if (!props.IsEventTriggered)
            {
                var e = @event as SimpleEventTriggeredEvent;
                if (e != null && e.EventType == description.EventType)
                {
                    props.IsEventTriggered = true;

                    Tree.EventBus.Unsubscribe<SimpleEventTriggeredEvent>(this);

                    EnqueueForUpdate();
                }
            }
        }
    }
}