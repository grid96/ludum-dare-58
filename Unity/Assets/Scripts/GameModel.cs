using System.Collections.Generic;

public class GameModel
{
    public FolderModel Folder { get; } = new();
    public List<LetterModel> Letters { get; } = new();
    public List<StampModel> Stamps { get; } = new();
}