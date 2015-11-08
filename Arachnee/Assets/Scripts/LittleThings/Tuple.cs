
public class Tuple 
{
    public int Left { get; set; }
    public int Right { get; set; }

    public Tuple(int left, int right)
    {
        this.Left = left;
        this.Right = right;
    }

    public override int GetHashCode()
    {
        return (this.Left + this.Right).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is Tuple)
        {
            return this.Left == ((Tuple)obj).Left && this.Right == ((Tuple)obj).Right;
        }
        return false;
    }
}
