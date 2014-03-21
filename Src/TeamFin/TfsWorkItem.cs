using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TeamFin
{
    /// <summary>
    /// TfsWorkItem is a wrapper around the WorkItem in the TFS API.  The exposed properties correspond to specific
    /// fields in TFS.  Each such property must be marked with a TfsFieldAttribute that specifies the TFS schema's
    /// name for the field.  To accommodate custom fields, derive from this class and follow the same pattern.  
    /// </summary>
    public class TfsWorkItem
    {
        protected readonly WorkItem _workItem;
        protected readonly WorkItemStore _workItemStore;

        protected Action<int, int> _saveChildLink;

        protected internal TfsWorkItem(WorkItem workItem, WorkItemStore workItemStore)
        {
            _workItem = workItem;
            Id = workItem.Id;
            Title = workItem.Title;
            State = TfsWorkItemState.From(workItem.State);
            AreaPath = workItem.AreaPath;
            IterationPath = workItem.IterationPath;
            Project = workItem.Project.Name;
            Description = workItem.Description;
            this._workItemStore = workItemStore;
       
        }

        /// <summary>
        /// Not able to be updated by user. 
        /// </summary>
        [TfsField("System.Id")]
        public int Id { get; private set; }

        [TfsField("System.Title")]
        public string Title { get; set; }

        [TfsField("System.State")]
        public TfsWorkItemState State { get; set; }

        [TfsField("System.AreaPath")]
        public string AreaPath { get; set; }

        [TfsField("System.IterationPath")]
        public string IterationPath { get; set; }

        /// <summary>
        /// Not able to be updated by user.
        /// </summary>
        [TfsField("System.TeamProject")]
        public string Project { get; private set; }

        [TfsField("System.Description")]
        public string Description { get; set; }

        [TfsField("System.WorkItemType")]
        public TfsWorkItemType WorkItemType { get; private set; }

        /// <summary>
        /// Saves the work item back to TFS, then refreshes this work item. 
        /// </summary>
        public virtual void Save()
        {
            _workItem.Title = Title;
            _workItem.State = State.Name;
            _workItem.AreaPath = AreaPath;
            _workItem.IterationPath = IterationPath;
            _workItem.Description = Description;
            _workItem.Save();
            this.Refresh();
        }
        
        internal WorkItem WorkItem { get { return _workItem; } }

        internal WorkItemStore WorkItemStore { get { return _workItemStore; } }

        /// <summary>
        /// Update specific work item properties from the underlying WorkItem.
        /// </summary>
        protected virtual void Refresh()
        {
            Id = _workItem.Id;
            State = TfsWorkItemState.From(_workItem.State);
            AreaPath = _workItem.AreaPath;
            IterationPath = _workItem.IterationPath;
        }

        /// <summary>
        /// Link creates and saves a link between this TfsWorkItem (as the parent) and the provided child item.  Link
        /// saves both items if they are not already saved prior to adding the link.  
        /// </summary>
        /// <param name="childItem"></param>
        public void Link(TfsWorkItem childItem)
        {
            if (this.Id == 0)
                this.Save();

            if (childItem.Id == 0)
                childItem.Save();

            // Link Type Id 2 is Child.  The Forward End refers to the child with the Reverse being the parent.
            // We'll link parent to child using this link type, but the child is technically the source with the
            // parent being the target.  That's important in setting up the Add() call to ensure the link is the 
            // right direction.  
            var linkType = _workItemStore.WorkItemLinkTypes.Single(a => a.ForwardEnd.Id == 2);
            var linkTypeEnd = _workItemStore.WorkItemLinkTypes.LinkTypeEnds[linkType.ForwardEnd.Name];

            this._workItem.WorkItemLinks.Add(new WorkItemLink(linkTypeEnd, this.Id, childItem.Id));
                      
            this.Save();     
        }


      
        /// <summary>
        /// Retrieves all child work items associated with this item. 
        /// </summary>
        public IEnumerable<TfsWorkItem> Children
        {
            get
            {

                var wiQuery = new Query(_workItemStore,GetLinksQuery());
                var info = wiQuery.RunLinkQuery().Where(a => a.LinkTypeId != 0).Select(a => new { CapabilityId = a.SourceId, TaskId = a.TargetId });

                foreach (var id in info.Select(a => a.CapabilityId))
                {                    
                    yield return new TfsQuery<TfsWorkItem>(_workItemStore).Single(a => a.Id == id);
                }

            }
        }

        /// <summary>
        /// Returns the hard-coded WIQL for fetching the children of this TfsWorkItem. 
        /// </summary>
        /// <returns></returns>
        private string GetLinksQuery()
        {
            var query = new StringBuilder();

            query.Append("SELECT * From WorkItemLinks WHERE ([Source].[Sytem.Id]=" + this.Id.ToString());
            query.Append(" AND ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ");
            query.Append(" ([Target].[System.WorkItemType] = 'Task'");
            query.Append("ORDER BY [Source].[System.Id] mode(MustContain)");

            return query.ToString();
        }
    
    }

    
}
