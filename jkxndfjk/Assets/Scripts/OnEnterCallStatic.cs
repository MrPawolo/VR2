using UnityEngine;

public class OnEnterCallStatic : MonoBehaviour
{
    public enum Call
    {
        FinishLevel,
        EnterSpikes
    }
    public Call onTriggerEnterCall = Call.EnterSpikes;
    private void OnTriggerEnter(Collider other)
    {
        switch (onTriggerEnterCall)
        {
            case Call.EnterSpikes:
                Debug.Log("EnterSpikes");
                StaticGameController.Instance.OnSpikesPlayerEnterCall();
                break;
            case Call.FinishLevel:
                Debug.Log("EnterFinish");
                StaticGameController.Instance.OnLevelFinishedCall();
                break;
            
        }
        
    }
}
