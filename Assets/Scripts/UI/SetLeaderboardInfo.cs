using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetLeaderboardInfo : MonoBehaviour
{
    public TMP_Text positionText;
    public TMP_Text playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPositionText(string pos)
    {
        positionText.text = pos;
    }

    public void SetPlayerNameText(string pn)
    {
        playerNameText.text = pn;
    }
}
