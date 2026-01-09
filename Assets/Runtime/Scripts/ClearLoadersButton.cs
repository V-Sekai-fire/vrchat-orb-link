using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ClearLoadersButton : UdonSharpBehaviour
{
    [SerializeField] private MultiLoaderManager multiLoaderManager;
    private Button button;
    
    public void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClearClick);
        }
    }
    
    public void OnClearClick()
    {
        if (multiLoaderManager == null)
        {
            Debug.LogError("MultiLoaderManager not assigned to ClearLoadersButton");
            return;
        }
        
        multiLoaderManager.ClearAllLoaders();
    }
}
