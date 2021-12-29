using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public event PlayerState playerState;

    public void Initialize(PlayerState startState)
    {
        playerState = startState;
    }

    public void UpdateStateMachine()
    {
        try
        {
            playerState();
        }
        catch
        {
            Debug.LogError("State Machine not initialized with a state, please initialize State Machine on the Awake or Start method with a state.");
        }
    }

    public void ChangeState(PlayerState NextState)
    {
        gameObject.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        playerState = NextState;
        gameObject.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
    }

    public void ChangeState(GameObject Caller, PlayerState NextState)
    {
        gameObject.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        Caller.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        playerState = NextState;
        gameObject.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
        Caller.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
    }

    public delegate void PlayerState();
}
