using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TeamFin
{
    /// <summary>
    /// WorkItemQueryStore is a wrapper around WorkItemStore.  A TfsWorkItem cannot be created directly - only through 
    /// this class as a factory.  WorkItemQueryStore also 
    /// </summary>
    /// <typeparam name="TWorkItemType"></typeparam>
    public class WorkItemQueryStore<TWorkItemType> : IWorkItemQueryStore<TWorkItemType>
       where TWorkItemType : TfsWorkItem
    {
        private readonly WorkItemStore _workItemStore;

        public WorkItemQueryStore(string projectCollectionUri)
            : this(new Uri(projectCollectionUri))
        {
        }

        public WorkItemQueryStore(Uri projectCollectionUri)
        {
            _workItemStore = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(projectCollectionUri).GetService<WorkItemStore>();
        }

        internal WorkItemQueryStore(WorkItemStore workItemStore)
        {
            _workItemStore = workItemStore;
        }

        /// <summary>
        /// Retrieves work items from the store based on the provided query and returns them as TWorkItemType
        /// </summary>
        /// <param name="wiql"></param>
        /// <returns></returns>
        public virtual IEnumerable<TWorkItemType> Query(string wiql)
        {
            return this._workItemStore.Query(wiql)
                                      .Cast<WorkItem>()
                                      .To<TWorkItemType>(_workItemStore);                                                                       

        }

        /// <summary>
        /// Creates a TfsWorkItem using the provided TfsWorkItemType and projectName and with all other fields
        /// set to defaults.  Throws ArgumentException if the TfsWorkItemType does not correspond to such a type
        /// in TFS.  
        /// </summary>
        /// <param name="workItemType"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public virtual TWorkItemType CreateNew(TfsWorkItemType workItemType, string projectName)
        {
            var actualWorkItemType = _workItemStore.Projects[projectName].WorkItemTypes[workItemType.ToString()];

            if (actualWorkItemType == null)
                throw new ArgumentException(string.Format("Unable to find work item type '{0}' in store", workItemType.ToString()));
            
            return CreateWorkItem.From<TWorkItemType>(actualWorkItemType, _workItemStore);
        }

        /// <summary>
        /// Links the parent and child item together using a Hierarchy-Forward relationship.  Saves 
        /// both the parent and child items before adding the link, in that order, if the item Id equals 0. 
        /// Saves the parent item after the link is added to save the item link.  
        /// </summary>
        /// <param name="parentItem"></param>
        /// <param name="childItem"></param>
        public virtual void LinkAsChild(TWorkItemType parentItem, TWorkItemType childItem)
        {
            // If either work item has not yet been saved, must save them first in order to
            // obtain an ID to use for linking them together.  
            if (parentItem.Id == 0)
                parentItem.Save();

            if (childItem.Id == 0)
                childItem.Save();

            // Link Type Id 2 is Child.  The Forward End refers to the child with the Reverse being the parent.
            // We'll link parent to child using this link type, but the child is technically the source with the
            // parent being the target.  That's important in setting up the Add() call to ensure the link is the 
            // right direction.  
            var linkType = _workItemStore.WorkItemLinkTypes.Single(a => a.ForwardEnd.Id == 2);
            var linkTypeEnd = _workItemStore.WorkItemLinkTypes.LinkTypeEnds[linkType.ForwardEnd.Name];

            parentItem.WorkItem.WorkItemLinks.Add(new WorkItemLink(linkTypeEnd, parentItem.Id, childItem.Id));

            parentItem.Save();      

        }
    }

    internal static class CreateWorkItem
    {
        public static IEnumerable<TWorkItemType> To<TWorkItemType>(this IEnumerable<WorkItem> workItems, WorkItemStore workItemStore)
        {
            return workItems.Select(a => a.WorkItem<TWorkItemType>(workItemStore));
        }


        public static TWorkItemType WorkItem<TWorkItemType>(this WorkItem workItem, WorkItemStore workItemStore)
        {
            var f = workItem.Fields.Cast<Field>().Count();

            return (TWorkItemType)Activator.CreateInstance(typeof(TWorkItemType), new object[] { workItem, workItemStore });     
        }


        public static TWorkItemType From<TWorkItemType>(this WorkItemType workItemType, WorkItemStore workItemStore)
        {
            var wi = new WorkItem(workItemType);

            var f = wi.Fields.Cast<Field>().Count();

            return (TWorkItemType)Activator.CreateInstance(typeof(TWorkItemType), new object[] { wi, workItemStore });
        }     

    }
}
