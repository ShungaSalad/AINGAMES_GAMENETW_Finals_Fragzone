using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : Node
{
    private readonly List<Node> _nodes = new();

    public SelectorNode(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        foreach (Node node in _nodes)
        {
            switch (node.Evaluate())
            {
                // If a child node returns FAILURE, we simply move on to next
                case NodeState.FAILURE:
                    continue;
                // If a child note return SUCCESS, we immediately return success
                case NodeState.SUCCESS:
                    _nodeState = NodeState.SUCCESS;
                    return _nodeState;
                // If a child is Running,
                case NodeState.RUNNING:
                    _nodeState = NodeState.RUNNING;
                    return _nodeState;
            }
        }
        // This part of the code will only run if all child evaluates as FAILURE
        _nodeState = NodeState.FAILURE;
        return _nodeState;
    }
}
