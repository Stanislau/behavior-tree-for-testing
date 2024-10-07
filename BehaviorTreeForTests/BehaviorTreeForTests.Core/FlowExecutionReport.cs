using System;
using System.Collections.Generic;
using System.Linq;

namespace Medbullets.Tests.Utils.BehaviorTreeApproach.Core
{
    public record FlowExecutionReport
    {
        public string FlowDescription { get; set; }

        public Exception? Exception { get; set; }

        public List<string> Logs { get; set; } = new();

        public List<string> Suggestions { get; set; } = new();

        public bool Success { get; set; }

        public bool Executed { get; set; } = true;
    }
}