using System.Collections.Generic;

public class QuirkStack
{
    Stack<int> green = new Stack<int>();
    Stack<int> blue = new Stack<int>();
    bool greenIsActive = true;
    public QuirkStack(int totalCount)
    {
        for (int i = 0; i < totalCount; i++)
        {
            green.Push(i);
        }
    }
    public void quirkPop()
    {
        if (greenIsActive)
        {
            int x = green.Pop();
            blue.Push(x);
            if (green.Count == 0) { greenIsActive = false; }
        }
        else
        {
            int x = blue.Pop();
            green.Push(x);
            if (blue.Count == 0) { greenIsActive = true; }
        }
    }
    public int quirkPeek()
    {
        if (greenIsActive)
        {
            return green.Peek();
        }
        else
        {
            return blue.Peek();
        }
    }
}
