using UnityEngine;

public class LetterModel
{
    public Vector2 EnvelopeSize { get; set; }
    public Color EnvelopeColor { get; set; }
    public string Recipient { get; set; }
    public string Sender { get; set; }
    public StampModel Stamp { get; set; }
    
    public LetterModel(Vector2 envelopeSize, Color envelopeColor, string recipient, string sender, StampModel stamp) => (EnvelopeSize, EnvelopeColor, Recipient, Sender, Stamp) = (envelopeSize, envelopeColor, recipient, sender, stamp);
}