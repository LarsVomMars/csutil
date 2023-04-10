using System;

namespace csutil.RowComp
{
    public interface IRow<T> : IEquatable<T>
    {
        bool IsSame(T row);
    }
}