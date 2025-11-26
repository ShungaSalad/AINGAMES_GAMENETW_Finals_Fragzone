public abstract class Node
{
    // Getter for readability outside the class
    public NodeState NodeState => _nodeState;
    protected NodeState _nodeState;

    // Define a delegate that will return a a type of NodeState and accept 0 parameters
    // The function that will return the status of the node state
    public delegate NodeState NodeReturnDelegate();

    // Constructor
    public Node()
    {
    }

    // Function that the derived classes will implement for returning the node state
    // Contains the logic whether the node evaluates as success, failure or running
    public abstract NodeState Evaluate();
}
