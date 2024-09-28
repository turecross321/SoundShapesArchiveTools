using DatabaseGenerator.Common.Database;
using DatabaseGenerator.Common.Database.Types;
using Microsoft.EntityFrameworkCore;

namespace DatabaseGenerator.Common.Extensions;

public static class DbSetExtensions
{
    public static (int, int) MergeAddRange<TArchiveDb>(this ArchiveDatabaseContext database, DbSet<TArchiveDb> dbItems, 
        IEnumerable<TArchiveDb?> newItems) where TArchiveDb : ArchiveDbItem<TArchiveDb>
    {
        List<TArchiveDb> addedNow = [];
        int merged = 0;
        
        foreach (TArchiveDb? newItem in newItems)
        {
            if (newItem == null)
                continue;
            
            TArchiveDb? alreadyAdded = dbItems.AsEnumerable().FirstOrDefault(i => i.SamePrimaryKey(newItem)) ?? addedNow.FirstOrDefault(i => i.SamePrimaryKey(newItem));
            if (alreadyAdded != null)
            {
                alreadyAdded.Merge(newItem);
                merged++;
            }
            else
            {
                dbItems.Add(newItem);
                addedNow.Add(newItem);
            }
            
        }
        database.SaveChanges();
        
        return (addedNow.Count, merged);
    }
}