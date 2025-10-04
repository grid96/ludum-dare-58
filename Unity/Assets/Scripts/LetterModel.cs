public class LetterModel
{
    public StampModel Stamp { get; set; }
    public string Recipient { get; set; }
    public string Sender { get; set; }
    
    public LetterModel(StampModel stamp, string recipient, string sender) => (Stamp, Recipient, Sender) = (stamp, recipient, sender);
}