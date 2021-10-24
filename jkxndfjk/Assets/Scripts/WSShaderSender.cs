using UnityEngine;

[ExecuteAlways]
public class WSShaderSender : MonoBehaviour
{
    int HEIGHT = Shader.PropertyToID("_height");
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat(HEIGHT, transform.position.y);
    }
    [ContextMenu("DisableDebug")]
    public void EnableDebug()
    {
        Shader.EnableKeyword("BOOLEAN_FD492A22DC314B0ABA36EF95953F2337_ON");
    }
    [ContextMenu("EnableDebug")]
    public void DisableDebug()
    {
        Shader.DisableKeyword("BOOLEAN_FD492A22DC314B0ABA36EF95953F2337_ON");
    }
}
