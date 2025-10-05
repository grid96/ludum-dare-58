using System.Linq;

public class FolderModel
{
    public CollectionModel[] Collections { get; } = System.Enum.GetValues(typeof(CollectionModel.Collection)).Cast<CollectionModel.Collection>().Select(c => new CollectionModel(c)).ToArray();
    
    public int TotalScore => Collections.Sum(c => c.TotalScore);
}