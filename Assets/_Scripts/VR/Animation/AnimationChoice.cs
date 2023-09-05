
public class AnimationChoice
{
    public int index { get; set; }
    public string animationType { get; set; }
    public AnimationChoice(string _animationType, int _index)
    {
        animationType = _animationType;
        index = _index;
    }
    public string ConvertToString()
    {
        return animationType + index.ToString();
    }
}
