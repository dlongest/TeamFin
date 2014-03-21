using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamFin
{
    /// <summary>
    /// TfsWorkItemState corresponds to State in TFS.  This list is constrained in the TFS API, but can be
    /// added to as customizations.  TfsWorkItemState includes a number of static states that are either
    /// standard or are commonly added as customizations. 
    /// </summary>
    public class TfsWorkItemState
    {
        private TfsWorkItemState(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public static TfsWorkItemState InProgress { get { return new TfsWorkItemState("In Progress"); } }
        public static TfsWorkItemState Pending { get { return new TfsWorkItemState("Pending"); } }
        public static TfsWorkItemState Discover { get { return new TfsWorkItemState("Discover"); } }
        public static TfsWorkItemState Deliver { get { return new TfsWorkItemState("Deliver"); } }
        public static TfsWorkItemState Deploy { get { return new TfsWorkItemState("Deploy"); } }
        public static TfsWorkItemState Done { get { return new TfsWorkItemState("Done"); } }
        public static TfsWorkItemState New { get { return new TfsWorkItemState("New"); } }
        public static TfsWorkItemState Active { get { return new TfsWorkItemState("Active"); } }
        public static TfsWorkItemState Resolved { get { return new TfsWorkItemState("Resolved"); } }
        public static TfsWorkItemState Closed { get { return new TfsWorkItemState("Closed"); } }
        public static TfsWorkItemState Complete { get { return new TfsWorkItemState("Complete"); } }
        public static TfsWorkItemState Proposed { get { return new TfsWorkItemState("Proposed"); } }
        public static TfsWorkItemState Removed { get { return new TfsWorkItemState("Removed"); } }
        public static TfsWorkItemState Ready { get { return new TfsWorkItemState("Ready"); } }
        public static TfsWorkItemState Backlog { get { return new TfsWorkItemState("Backlog"); } }
        public static TfsWorkItemState NotStarted { get { return new TfsWorkItemState("Not Started"); } }

        public static TfsWorkItemState From(string state)
        {
            return new TfsWorkItemState(state);
        }

        public override bool Equals(object obj)
        {
            var stateObj = obj as TfsWorkItemState;

            if (stateObj == null)
                return false;

            return this.Name == stateObj.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
