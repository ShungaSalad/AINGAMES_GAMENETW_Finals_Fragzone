using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : Node
{
    // A selector can contain one or more child nodes
    private readonly List<Node> _nodes = new();

    // Constructor
    public SequenceNode(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public override NodeState Evaluate()
    {
        bool isAnyChildRunning = false;
        foreach (Node node in _nodes)
        {
            switch (node.Evaluate())
            {
                // if a child node returns FAILURE, we exit immediately
                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;
                // if a child node returns SUCCESS, continue until all children is evaluated
                case NodeState.SUCCESS:
                    continue;
                // if a child is running, we need to wait until it completes
                case NodeState.RUNNING:
                    isAnyChildRunning = true;
                    continue;
            }
        }
        // This part of the code will only run when no child node fails
        _nodeState = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return _nodeState;
    }
}
