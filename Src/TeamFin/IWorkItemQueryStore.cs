using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamFin
{
    public interface IWorkItemQueryStore<TWorkItemType>
    {
        IEnumerable<TWorkItemType> Query(string wiql);
    }
}
