using UnityEngine;

[System.Serializable]
public struct StateData
{
    public StageState stageState;

    public MonoBehaviour stateComponent;

    public IStageState GetStageState() {
        return stateComponent as IStageState;
    }
}
