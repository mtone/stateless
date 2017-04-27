﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stateless
{
    public partial class StateMachine<TState, TTrigger>
    {
        internal abstract class DeactivateActionBehaviour
        {
            readonly TState _state;
            readonly MethodDescription _actionDescription;

            protected DeactivateActionBehaviour(TState state, MethodDescription actionDescription)
            {
                _state = state;
                _actionDescription = Enforce.ArgumentNotNull(actionDescription, nameof(actionDescription));
            }

            internal MethodDescription Description => _actionDescription;

            public abstract void Execute();
            public abstract Task ExecuteAsync();

            public class Sync : DeactivateActionBehaviour
            {
                readonly Action _action;

                public Sync(TState state, Action action, MethodDescription actionDescription)
                    : base(state, actionDescription)
                {
                    _action = action;
                }

                public override void Execute()
                {
                    _action();
                }

                public override Task ExecuteAsync()
                {
                    Execute();
                    return TaskResult.Done;
                }
            }

            public class Async : DeactivateActionBehaviour
            {
                readonly Func<Task> _action;

                public Async(TState state, Func<Task> action, MethodDescription actionDescription)
                    : base(state, actionDescription)
                {
                    _action = action;
                }

                public override void Execute()
                {
                    throw new InvalidOperationException(
                        $"Cannot execute asynchronous action specified in OnDeactivateAsync for '{_state}' state. " +
                         "Use asynchronous version of Deactivate [DeactivateAsync]");
                }

                public override Task ExecuteAsync()
                {
                    return _action();
                }
            }
        }
    }
}
