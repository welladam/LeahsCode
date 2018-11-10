using UnityEngine;
using UnityEngine.Events;

public class InteractHealthBox : MonoBehaviour {

    public UnityEvent OnInteract;

    public void OnInteractHealthBox()
    {
        OnInteract.Invoke();
    }      
}
