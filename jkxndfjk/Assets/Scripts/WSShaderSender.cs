using UnityEngine;

public class WSShaderSender : MonoBehaviour
{
    int HEIGHT = Shader.PropertyToID("_height");
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat(HEIGHT, transform.position.y);
    }
}
