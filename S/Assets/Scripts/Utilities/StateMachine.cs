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
        if(playerState != null)
        {
            playerState();
        }
        else
        {
;            Debug.LogError("State Machine not initialized with a state, please initialize State Machine on the Awake or Start method with a state.");
        }
        //try
        //{
        //    playerState();
        //}
        //catch (System.Exception e)
        //{
        //    Debug.LogError("State Machine not initialized with a state, please initialize State Machine on the Awake or Start method with a state.");
        //    throw new System.Exception($"state machine error? --> {e}");
        //}
    }

    public void ChangeState(PlayerState NextState)
    {
        if(NextState == null)
        {
            throw new System.Exception("kkjaddfkjslksdjf");
        }
        //Debug.Log($"NextPS: {NextState.Method.Name}");
        gameObject.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        playerState = NextState;
        gameObject.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
    }

    public void ChangeState(GameObject Caller, PlayerState NextState)
    {
        if (NextState == null)
        {
            throw new System.Exception("kkjaddfkjslksdjf");
        }
        //Debug.Log($"NextPS: {NextState.Method.Name}");
        gameObject.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        Caller.SendMessage(playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
        playerState = NextState;
        gameObject.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
        Caller.SendMessage(playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
    }

    public delegate void PlayerState();
}
