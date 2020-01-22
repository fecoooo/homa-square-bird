using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QualityShower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<TextMeshProUGUI>().text = QualitySettings.GetQualityLevel().ToString();

	}
}
