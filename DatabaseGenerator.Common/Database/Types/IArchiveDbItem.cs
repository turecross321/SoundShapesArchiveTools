namespace DatabaseGenerator.Common.Database.Types;

public abstract record ArchiveDbItem<TSelf> where TSelf : ArchiveDbItem<TSelf>
{
    public virtual void Merge(TSelf other)
    {
        if (other.Sources != this.Sources)
        {
            this.Sources += "|" + other.Sources;
        }
    }
    public required string Sources { get; set; }
    public abstract bool SamePrimaryKey(TSelf other);
}