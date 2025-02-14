namespace Domain.Model.Helpers;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}