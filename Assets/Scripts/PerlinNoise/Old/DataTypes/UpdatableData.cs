using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR)
public class UpdatableData : ScriptableObject
{
    public System.Action OnValuesUpdated;
    public bool AutoUpdate;

    protected virtual void OnValidate()
    {
        if (AutoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValue;
        }
    }

    public void NotifyOfUpdatedValue()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValue;
        if (OnValuesUpdated!=null)
        {
            OnValuesUpdated();
        }
    }

}
#endif
