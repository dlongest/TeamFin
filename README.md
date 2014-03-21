TeamFin
=======

TeamFin (Team Foundation Integration) is designed initially to make querying the TFS API easier by using LINQ syntax and strongly-typed expressions instead of manipulating the string-based Work Item Query Language manually.  TeamFin works against Team Foundation Server 2010. 

The key item in TeamFin is the TfsWorkItem, which is a wrapper around the native TFS WorkItem.  The TfsWorkItem can be inherited to allow for extension due to TFS customizations (e.g. custom fields, etc). 

A `TfsQuery` drives the actual LINQ expressions against the WorkItemQueryStore.  For example:
````var query = new TfsQuery<TfsWorkItem>(store).Where(a => a.Project=="My Project" && a.WorkItemType==TfsWorkItemType.UserStory)````
will create a query that returns all user stories within "My Project". 
