using UnityEngine;
using UnityEngine.UI;
public abstract class SquareBaseState
{
    public bool canAccess;

    public SquareStateObject state;

    public SquareBaseState(SquareStateObject state, bool canAccess)
    {
        this.canAccess = canAccess;
        this.state = state;
    }
    public virtual void EnterState(SquareStateManager square)
    {
        if (square.colorTransition != null)
        {
            square.colorTransition.TransitionToColor(state.color, state.transitionDuration);
        }
        else
        {
            square.placeHolder.color = state.color;
        }

    }

    public abstract void UpdateState(SquareStateManager square);
}

public class SquareEntryPoint : SquareBaseState
{


    public SquareEntryPoint(SquareStateObject state, bool canAccess) : base(state, canAccess) { }



    public override void UpdateState(SquareStateManager square)
    {
        throw new System.NotImplementedException();
    }
}

public class SquareNormal : SquareBaseState
{

    public SquareNormal(SquareStateObject state, bool canAccess) : base(state, canAccess) { }



    public override void UpdateState(SquareStateManager square)
    {
        throw new System.NotImplementedException();
    }
}

public class SquareEndPoint : SquareBaseState
{

    public SquareEndPoint(SquareStateObject state, bool canAccess) : base(state, canAccess) { }


    public override void UpdateState(SquareStateManager square)
    {
        throw new System.NotImplementedException();
    }
}

public class SquareUsed : SquareBaseState
{
    public SquareUsed(SquareStateObject state, bool canAccess) : base(state, canAccess) { }



    public override void UpdateState(SquareStateManager square)
    {
        throw new System.NotImplementedException();
    }
}