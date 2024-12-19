using UnityEngine;

public class UpdateMyUpdate : MonoBehaviour
{
    public MonoBehaviour[] MyUpdates;
    public bool PauseAll;

    [System.Diagnostics.CodeAnalysis.SuppressMessage
        ("Type Safety", "UNT0016:Unsafe way to get the method name",
        Justification = "<pendiente>")]
    public void Update()
    {
        if (PauseAll) return; 

        foreach (var my_update in MyUpdates)
        {
            if (my_update.gameObject.activeSelf)
            {
                my_update.Invoke("MyUpdate", 0.0f);
            }
        }
    }
}
