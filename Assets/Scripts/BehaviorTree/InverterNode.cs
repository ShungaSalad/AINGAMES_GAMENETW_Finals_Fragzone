using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterNode : Node
{
    private Node _node;

    public InverterNode(Node node)
    {
        _node = node;
    }

    public override NodeState Evaluate()
    {
        switch (_node.Evaluate())
        {
            // Reverse the result
            case NodeState.FAILURE:
                _nodeState = NodeState.SUCCESS;
                break;
            case NodeState.SUCCESS:
                _nodeState = NodeState.FAILURE;
                break;
            case NodeState.RUNNING:
                _nodeState = NodeState.RUNNING;
                break;
        }
        return _nodeState;
    }
}
