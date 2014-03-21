using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TeamFin.Tests
{
    public static class Create
    {
        public static TfsPredicateVisitor<TfsWorkItem> Sut()
        {
            return new TfsPredicateVisitor<TfsWorkItem>(FieldMappings.Map<TfsWorkItem>());
        }

    }
}
