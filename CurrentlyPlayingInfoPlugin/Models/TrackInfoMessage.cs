namespace AIMP.CurrentlyPlayingInfoPlugin.Models;

public class TrackInfoMessage
{
    public string TrackTitle { get; set; }
    public string Artist { get; set; }
    public bool IsPlaying { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (TrackInfoMessage)obj;
        return TrackTitle == other.TrackTitle &&
               Artist == other.Artist &&
               IsPlaying == other.IsPlaying;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (TrackTitle?.GetHashCode() ?? 0);
            hash = hash * 23 + (Artist?.GetHashCode() ?? 0);
            hash = hash * 23 + IsPlaying.GetHashCode();
            return hash;
        }
    }
}
