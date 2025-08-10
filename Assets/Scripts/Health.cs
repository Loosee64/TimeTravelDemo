using UnityEngine;

public class Health : MonoBehaviour
{
    public TMPro.TextMeshProUGUI healthText;

    public void setValue(int t_value) { healthText.text = "Health: " + t_value.ToString(); }
}
