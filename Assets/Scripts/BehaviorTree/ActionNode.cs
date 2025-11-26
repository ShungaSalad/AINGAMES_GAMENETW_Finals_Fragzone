using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : Node
{
    private NodeReturnDelegate _nodeFunction;

    // We pass the actual function that will evaluate the node state
    public ActionNode(NodeReturnDelegate nodeFunction)
    {
        _nodeFunction = nodeFunction;
    }
    public override NodeState Evaluate()
    {
       return _nodeFunction.Invoke();
    }
}
